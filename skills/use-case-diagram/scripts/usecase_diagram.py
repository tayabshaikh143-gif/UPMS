#!/usr/bin/env python3
"""Render a clean UML use case diagram from a JSON spec.

Stdlib only. Emits SVG, optional PNG (via headless Chrome/Edge), and optional
PlantUML source.

Usage:
  python usecase_diagram.py spec.json --out diagram.svg
  python usecase_diagram.py spec.json --out diagram.svg --png diagram.png
  python usecase_diagram.py spec.json --out diagram.svg --plantuml diagram.puml
  python usecase_diagram.py --schema

Spec (JSON):
{
  "title": "UPMS - Use Case Diagram",
  "subtitle": "University Point Management System v1.0",
  "system": "UPMS",
  "theme": "light" | "dark",
  "actors": [
    {"id": "passenger", "name": "Passenger", "side": "left"},
    {"id": "gateway", "name": "Payment Gateway", "side": "right", "external": true}
  ],
  "use_cases": [
    {"id": "book", "name": "Book Ticket"},
    {"id": "pay", "name": "Pay Online"}
  ],
  "associations": [["passenger", "book"]],
  "includes": [["book", "check_capacity"]],
  "extends": [["pay", "book"]],
  "generalizations": [["vendor", "staff"]],
  "footnote": "Optional line at the bottom"
}
"""

import argparse
import json
import math
import os
import re
import shutil
import subprocess
import sys
import tempfile
from xml.sax.saxutils import escape

# ---------------------------------------------------------------- constants
FONT = "'Segoe UI', 'Inter', 'Helvetica Neue', Arial, sans-serif"

THEMES = {
    "light": {
        "bg": "#F5F7FB",
        "boundary_fill": "#FFFFFF",
        "boundary_stroke": "#C3CEE4",
        "boundary_label": "#243B63",
        "boundary_band": "#EDF2FB",
        "uc_fill_top": "#EDF4FF",
        "uc_fill_bottom": "#F9FCFF",
        "uc_stroke": "#7E96C9",
        "uc_text": "#182C4A",
        "actor": "#33415C",
        "actor_text": "#111827",
        "assoc": "#65758B",
        "include": "#7C3AED",
        "extend": "#D97706",
        "gen": "#0F766E",
        "muted": "#64748B",
        "title": "#0F2547",
        "label_bg": "#FFFFFF",
        "legend_border": "#D8E0EF",
    },
    "dark": {
        "bg": "#0B1220",
        "boundary_fill": "#111C30",
        "boundary_stroke": "#2B3B57",
        "boundary_label": "#DCE7FA",
        "boundary_band": "#16233A",
        "uc_fill_top": "#172842",
        "uc_fill_bottom": "#122036",
        "uc_stroke": "#4A82C4",
        "uc_text": "#E6EEF9",
        "actor": "#9FB3CE",
        "actor_text": "#E6EEF9",
        "assoc": "#8595AD",
        "include": "#A78BFA",
        "extend": "#FBBF24",
        "gen": "#5EEAD4",
        "muted": "#94A3B8",
        "title": "#F1F6FF",
        "label_bg": "#0F1B2D",
        "legend_border": "#26354E",
    },
}


def die(msg):
    sys.stderr.write("ERROR: " + msg + "\n")
    sys.exit(2)


def load_spec(path):
    if path == "-":
        return json.load(sys.stdin)
    with open(path, "r", encoding="utf-8-sig") as fh:
        return json.load(fh)


# ---------------------------------------------------------------- validation
def validate(spec):
    warnings = []

    actors = spec.get("actors") or []
    ucs = spec.get("use_cases") or []
    if not actors:
        die("spec needs at least one actor")
    if not ucs:
        die("spec needs at least one use case")

    actor_ids = [a.get("id") for a in actors]
    uc_ids = [u.get("id") for u in ucs]

    for label, ids in (("actor", actor_ids), ("use case", uc_ids)):
        seen = set()
        for i in ids:
            if not i:
                die("every %s needs an id" % label)
            if i in seen:
                die("duplicate %s id: %s" % (label, i))
            seen.add(i)

    overlap = set(actor_ids) & set(uc_ids)
    if overlap:
        die("ids used by both an actor and a use case: " + ", ".join(sorted(overlap)))

    def check_pair(kind, pairs, allowed):
        for p in pairs:
            if not isinstance(p, (list, tuple)) or len(p) != 2:
                die("%s entries must be [from, to] pairs" % kind)
            a, b = p
            if a not in allowed or b not in allowed:
                die("%s references unknown id: %s" % (kind, "%s -> %s" % (a, b)))
            if a == b:
                die("%s cannot connect %s to itself" % (kind, a))

    associations = spec.get("associations") or []
    includes = spec.get("includes") or []
    extends = spec.get("extends") or []
    gens = spec.get("generalizations") or []

    check_pair("associations", associations, set(actor_ids) | set(uc_ids))
    check_pair("includes", includes, set(uc_ids))
    check_pair("extends", extends, set(uc_ids))
    check_pair("generalizations", gens, set(actor_ids) | set(uc_ids))

    for a, b in associations:
        if a in uc_ids and b in uc_ids:
            die("associations connect actors to use cases; use includes/extends/generalizations for use case to use case: %s -> %s" % (a, b))

    linked_uc = {b for a, b in associations if a in actor_ids}
    linked_uc |= {a for a, b in associations if b in actor_ids}
    related = set()
    for pairs in (includes, extends, gens):
        for a, b in pairs:
            if a in set(uc_ids):
                related.add(a)
            if b in set(uc_ids):
                related.add(b)
    for u in uc_ids:
        if u not in linked_uc and u not in related:
            warnings.append("use case '%s' has no actor association and no relationship" % u)

    seen_pairs = set()
    for kind, pairs in (("include", includes), ("extend", extends)):
        for p in pairs:
            if tuple(p) in seen_pairs:
                warnings.append("duplicate %s: %s" % (kind, " -> ".join(p)))
            seen_pairs.add(tuple(p))

    if _has_cycle(includes):
        warnings.append("include relationships form a cycle")
    if _has_cycle(extends):
        warnings.append("extend relationships form a cycle")

    for a in actors:
        if a.get("side") not in (None, "left", "right"):
            die("actor '%s' side must be 'left' or 'right'" % a.get("id"))

    for w in warnings:
        sys.stderr.write("WARN: " + w + "\n")

    columns = spec.get("columns")
    if columns:
        if not isinstance(columns, list) or not all(isinstance(c, list) for c in columns):
            die("'columns' must be a list of lists of use case ids")
        seen = [uid for c in columns for uid in c]
        unknown = [uid for uid in seen if uid not in uc_ids]
        if unknown:
            die("'columns' references unknown use case id(s): " + ", ".join(sorted(set(unknown))))
        if len(seen) != len(set(seen)):
            die("'columns' lists a use case more than once")
        missing = [uid for uid in uc_ids if uid not in seen]
        if missing:
            die("'columns' is missing use case id(s): " + ", ".join(missing))

    return warnings


def _has_cycle(pairs):
    graph = {}
    for a, b in pairs:
        graph.setdefault(a, []).append(b)
    state = {}

    def visit(n):
        state[n] = 1
        for m in graph.get(n, []):
            if state.get(m) == 1:
                return True
            if state.get(m) is None and visit(m):
                return True
        state[n] = 2
        return False

    return any(state.get(n) is None and visit(n) for n in list(graph))


# ---------------------------------------------------------------- geometry
class Ellipse:
    __slots__ = ("cx", "cy", "rx", "ry")

    def __init__(self, cx, cy, rx, ry):
        self.cx, self.cy, self.rx, self.ry = cx, cy, rx, ry

    def point_toward(self, x, y):
        dx, dy = x - self.cx, y - self.cy
        denom = math.hypot(dx / self.rx, dy / self.ry)
        if denom == 0:
            return self.cx + self.rx, self.cy
        t = 1.0 / denom
        return self.cx + dx * t, self.cy + dy * t


def _boxes_overlap(a, b, pad=0):
    return not (
        a[2] + pad < b[0] or b[2] + pad < a[0] or a[3] + pad < b[1] or b[3] + pad < a[1]
    )


def text_width(text, size):
    units = 0.0
    for ch in text:
        if ch in " .,:;'|!il":
            units += 0.32
        elif ch in "MW@#%&":
            units += 0.85
        elif ch.isupper():
            units += 0.66
        else:
            units += 0.55
    return units * size


def wrap_text(text, max_width, size):
    words = str(text).split()
    lines, cur = [], ""
    for w in words:
        cand = (cur + " " + w).strip()
        if not cur or text_width(cand, size) <= max_width:
            cur = cand
        else:
            lines.append(cur)
            cur = w
    if cur:
        lines.append(cur)
    return lines or [""]


def choose_grid(n):
    cols = 1 if n <= 4 else 2 if n <= 12 else 3 if n <= 18 else 4
    rows = int(math.ceil(n / cols))
    return rows, cols


# ---------------------------------------------------------------- renderer
class Diagram:
    def __init__(self, spec):
        self.spec = spec
        self.theme = THEMES.get(spec.get("theme", "light"), THEMES["light"])
        self.actors = spec["actors"]
        self.ucs = spec["use_cases"]
        self.assoc = [tuple(p) for p in spec.get("associations") or []]
        self.includes = [tuple(p) for p in spec.get("includes") or []]
        self.extends = [tuple(p) for p in spec.get("extends") or []]
        self.gens = [tuple(p) for p in spec.get("generalizations") or []]

        self.margin = 58
        self.actor_zone = 168
        self.cell_w = 240
        self.cell_h = 116
        self.pad_x = 34
        self.pad_y = 28
        self.header_h = 48
        self.rx = 96
        self.ry = 38
        self.uc_font = 14.5
        self.actor_half_w = 44

        self.left_actors = [a for a in self.actors if a.get("side", "left") != "right"]
        self.right_actors = [a for a in self.actors if a.get("side", "left") == "right"]
        self.uc_by_id = {u["id"]: u for u in self.ucs}
        self.uc_geom = {}
        self.actor_geom = {}
        self.anchor_offset = {}
        self.label_boxes = []

    # ---- layout
    def layout(self):
        s = self.spec
        title_h = 0
        if s.get("title"):
            title_h += 34
        if s.get("subtitle"):
            title_h += 26

        n = len(self.ucs)
        columns = s.get("columns")
        if columns:
            self.rows = max(len(c) for c in columns)
            self.cols = len(columns)
            positions = {}
            for ci, ids in enumerate(columns):
                offset = (self.rows - len(ids)) / 2.0
                for ri, uid in enumerate(ids):
                    positions[uid] = (ci, offset + ri)
            order = [(uid, positions[uid][0], positions[uid][1]) for uid in positions]
        else:
            self.rows, self.cols = choose_grid(n)
            order = [
                (u["id"], i // self.rows, i % self.rows) for i, u in enumerate(self.ucs)
            ]

        self.bx = self.margin + (self.actor_zone if self.left_actors else 0)
        self.by = self.margin + title_h + 8
        self.bw = self.cols * self.cell_w + 2 * self.pad_x
        self.bh = self.rows * self.cell_h + 2 * self.pad_y + self.header_h

        for uid, col, row in order:
            u = self.uc_by_id[uid]
            cx = self.bx + self.pad_x + col * self.cell_w + self.cell_w / 2
            cy = self.by + self.header_h + self.pad_y + row * self.cell_h + self.cell_h / 2
            name = u.get("name", u["id"])
            lines = wrap_text(name, self.rx * 1.42, self.uc_font)
            if len(lines) > 2:
                lines = wrap_text(name, self.rx * 1.55, self.uc_font - 1)
            ry = self.ry if len(lines) <= 2 else self.ry + 6
            self.uc_geom[u["id"]] = {
                "ellipse": Ellipse(cx, cy, self.rx, ry),
                "lines": lines,
                "col": col,
                "row": row,
                "center": (cx, cy),
            }

        self._place_actors(self.left_actors, "left")
        self._place_actors(self.right_actors, "right")
        self._align_actor_generalizations()
        self._spread_anchors()

        legend_space = 48 if s.get("legend", True) else 18
        max_y = self.by + self.bh + legend_space
        for g in self.actor_geom.values():
            max_y = max(max_y, g["y"] + 78)
        if s.get("footnote"):
            max_y += 30
        self.height = max_y + 34
        self.width = int(
            self.bx + self.bw + (self.actor_zone if self.right_actors else 0) + self.margin
        )

        if len(self.left_actors) > 1:
            self.margin_needed = 0
        return self

    def _align_actor_generalizations(self):
        for child, parent in self.gens:
            if child not in self.actor_geom or parent not in self.actor_geom:
                continue
            c, p = self.actor_geom[child], self.actor_geom[parent]
            if c["side"] != p["side"]:
                continue
            if c["y"] < p["y"] + 120:
                c["y"] = p["y"] + 150
                p["y"] = p["y"]

    def _spread_anchors(self):
        for actor_id in list(self.actor_geom):
            partners = []
            for a, b in self.assoc:
                if a == actor_id and b in self.uc_geom:
                    partners.append((self.uc_geom[b]["center"][1], (a, b)))
                elif b == actor_id and a in self.uc_geom:
                    partners.append((self.uc_geom[a]["center"][1], (a, b)))
            partners.sort(key=lambda t: t[0])
            n = len(partners)
            for i, (_, pair) in enumerate(partners):
                if n <= 1:
                    self.anchor_offset[pair] = 0.0
                else:
                    self.anchor_offset[pair] = -17.0 + 34.0 * i / (n - 1)

    def _place_actors(self, actors, side):
        if not actors:
            return
        desired = []
        for a in actors:
            ys = []
            for src, dst in self.assoc:
                if src == a["id"] and dst in self.uc_geom:
                    ys.append(self.uc_geom[dst]["center"][1])
                elif dst == a["id"] and src in self.uc_geom:
                    ys.append(self.uc_geom[src]["center"][1])
            desired.append(sum(ys) / len(ys) if ys else None)

        placed = []
        known = [d for d in desired if d is not None]
        top = self.by + 60
        bottom = self.by + self.bh - 20
        if not known:
            step = (bottom - top) / max(1, len(actors))
            for i, a in enumerate(actors):
                placed.append((a, top + step * (i + 0.5)))
        else:
            for a, d in zip(actors, desired):
                placed.append((a, d if d is not None else sum(known) / len(known)))

        placed.sort(key=lambda t: t[1])
        min_gap = 132
        for i in range(1, len(placed)):
            if placed[i][1] - placed[i - 1][1] < min_gap:
                placed[i] = (placed[i][0], placed[i - 1][1] + min_gap)

        overflow = placed[-1][1] - bottom
        if overflow > 0:
            placed = [(a, y - overflow) for a, y in placed]
            for i in range(len(placed) - 2, -1, -1):
                if placed[i + 1][1] - placed[i][1] < min_gap:
                    placed[i] = (placed[i][0], placed[i + 1][1] - min_gap)

        if side == "left":
            x = self.margin + self.actor_zone / 2 - 18
        else:
            x = self.bx + self.bw + self.actor_zone / 2 - 8

        for a, y in placed:
            self.actor_geom[a["id"]] = {"x": x, "y": y, "side": side}

    # ---- svg pieces
    def actor_svg(self, a, g):
        t = self.theme
        x, y = g["x"], g["y"]
        color = t["actor"]
        parts = [
            '<g class="actor">',
            '<circle cx="%0.1f" cy="%0.1f" r="10" fill="none" stroke="%s" stroke-width="2"/>'
            % (x, y - 33, color),
            '<line x1="%0.1f" y1="%0.1f" x2="%0.1f" y2="%0.1f" stroke="%s" stroke-width="2" stroke-linecap="round"/>'
            % (x, y - 23, x, y + 4, color),
            '<line x1="%0.1f" y1="%0.1f" x2="%0.1f" y2="%0.1f" stroke="%s" stroke-width="2" stroke-linecap="round"/>'
            % (x - 17, y - 13, x + 17, y - 13, color),
            '<line x1="%0.1f" y1="%0.1f" x2="%0.1f" y2="%0.1f" stroke="%s" stroke-width="2" stroke-linecap="round"/>'
            % (x, y + 4, x - 13, y + 24, color),
            '<line x1="%0.1f" y1="%0.1f" x2="%0.1f" y2="%0.1f" stroke="%s" stroke-width="2" stroke-linecap="round"/>'
            % (x, y + 4, x + 13, y + 24, color),
        ]
        if a.get("external"):
            parts.append(
                '<text x="%0.1f" y="%0.1f" text-anchor="middle" font-size="9.5" fill="%s" font-style="italic">external system</text>'
                % (x, y - 48, t["muted"])
            )
        lines = wrap_text(a.get("name", a["id"]), self.actor_zone - 26, 13)
        start = y + 42
        for i, line in enumerate(lines):
            parts.append(
                '<text x="%0.1f" y="%0.1f" text-anchor="middle" font-size="13" font-weight="600" fill="%s">%s</text>'
                % (x, start + i * 16, t["actor_text"], escape(line))
            )
        parts.append("</g>")
        return "\n".join(parts)

    def use_case_svg(self, u, g):
        t = self.theme
        e = g["ellipse"]
        parts = [
            '<g class="usecase">',
            '<ellipse cx="%0.1f" cy="%0.1f" rx="%0.1f" ry="%0.1f" fill="url(#ucGrad)" stroke="%s" stroke-width="1.6" filter="url(#softShadow)"/>'
            % (e.cx, e.cy, e.rx, e.ry, t["uc_stroke"]),
        ]
        line_h = 18 if len(g["lines"]) > 1 else 0
        base = e.cy + 4.6 - (len(g["lines"]) - 1) * line_h / 2
        for i, line in enumerate(g["lines"]):
            parts.append(
                '<text x="%0.1f" y="%0.1f" text-anchor="middle" font-size="%s" fill="%s">%s</text>'
                % (e.cx, base + i * line_h, self.uc_font, t["uc_text"], escape(line))
            )
        parts.append("</g>")
        return "\n".join(parts)

    def assoc_svg(self, actor_id, uc_id):
        t = self.theme
        a = self.actor_geom.get(actor_id)
        u = self.uc_geom.get(uc_id)
        if not a or not u:
            return ""
        e = u["ellipse"]
        direction = 1 if a["side"] == "left" else -1
        ax = a["x"] + direction * 6
        ay = a["y"] - 4 + self.anchor_offset.get((actor_id, uc_id), 0.0)
        ex, ey = e.point_toward(ax, ay)
        c1x = ax + direction * 46
        c2x = ex - direction * 40
        return (
            '<path d="M %.1f %.1f C %.1f %.1f, %.1f %.1f, %.1f %.1f" fill="none" stroke="%s" stroke-width="1.4" opacity="0.9"/>'
            % (ax, ay, c1x, ay, c2x, ey, ex, ey, t["assoc"])
        )

    def _edge_geometry(self, src, dst, kind):
        u1, u2 = self.uc_geom.get(src), self.uc_geom.get(dst)
        if not u1 or not u2:
            return None
        e1, e2 = u1["ellipse"], u2["ellipse"]
        sx, sy = e1.point_toward(e2.cx, e2.cy)
        ex, ey = e2.point_toward(e1.cx, e1.cy)

        dx, dy = ex - sx, ey - sy
        dist = math.hypot(dx, dy) or 1
        nx, ny = -dy / dist, dx / dist
        base = 26 if kind == "include" else 22
        bend = base + max(0.0, 140.0 - dist) * 0.55
        sign = 1 if e1.cx <= e2.cx else -1
        mx, my = (sx + ex) / 2 + nx * bend * sign, (sy + ey) / 2 + ny * bend * sign
        qx, qy = (0.25 * sx + 0.5 * mx + 0.25 * ex), (0.25 * sy + 0.5 * my + 0.25 * ey)
        return {
            "s": (sx, sy), "m": (mx, my), "e": (ex, ey),
            "q": (qx, qy), "n": (nx, ny), "sign": sign,
        }

    def internal_edge_svg(self, src, dst, kind):
        geo = self._edge_geometry(src, dst, kind)
        if not geo:
            return ""
        t = self.theme
        dash = ' stroke-dasharray="7 5"' if kind != "gen" else ""
        marker = "url(#hollowTriangle)" if kind == "gen" else "url(#openArrow-%s)" % kind
        color = t["include"] if kind == "include" else t["extend"] if kind == "extend" else t["gen"]
        (sx, sy), (mx, my), (ex, ey) = geo["s"], geo["m"], geo["e"]
        return (
            '<path d="M %.1f %.1f Q %.1f %.1f, %.1f %.1f" fill="none" stroke="%s" stroke-width="1.5"%s marker-end="%s"/>'
            % (sx, sy, mx, my, ex, ey, color, dash, marker)
        )

    def _box_clearance(self, x, y, half_w, half_h):
        pts = [(x, y), (x - half_w, y), (x + half_w, y), (x, y - half_h), (x, y + half_h)]
        best = 99.0
        for g in self.uc_geom.values():
            e = g["ellipse"]
            for px, py in pts:
                best = min(best, math.hypot((px - e.cx) / (e.rx + 5), (py - e.cy) / (e.ry + 5)))
        return best

    def edge_label_svg(self, src, dst, kind, label):
        geo = self._edge_geometry(src, dst, kind)
        if not geo or not label:
            return ""
        t = self.theme
        (sx, sy), (mx, my), (ex, ey) = geo["s"], geo["m"], geo["e"]
        nx, ny = geo["n"]
        sign = geo["sign"]
        tw = text_width(label, 10.5) + 12
        best, best_clr = geo["q"], -1.0
        chosen = None
        for tfrac in (0.5, 0.42, 0.58, 0.34, 0.66, 0.26, 0.74):
            bx = (1 - tfrac) ** 2 * sx + 2 * (1 - tfrac) * tfrac * mx + tfrac ** 2 * ex
            by = (1 - tfrac) ** 2 * sy + 2 * (1 - tfrac) * tfrac * my + tfrac ** 2 * ey
            for off in (0, 17, -17, 32, -32, 48, -48, 64, -64):
                px, py = bx + nx * off * sign, by + ny * off * sign
                clr = self._box_clearance(px, py, tw / 2, 9)
                box = (px - tw / 2, py - 9, px + tw / 2, py + 9)
                if clr > best_clr:
                    best, best_clr = (px, py), clr
                if clr >= 1.05 and not any(_boxes_overlap(box, b, 5) for b in self.label_boxes):
                    chosen = (px, py)
                    break
            if chosen:
                break
        px, py = chosen or best
        self.label_boxes.append((px - tw / 2, py - 9, px + tw / 2, py + 9))
        color = t["include"] if kind == "include" else t["extend"] if kind == "extend" else t["gen"]
        return (
            '<rect x="%0.1f" y="%0.1f" width="%0.1f" height="17" rx="8.5" fill="%s" stroke="%s" stroke-width="0.8"/>'
            '<text x="%0.1f" y="%0.1f" text-anchor="middle" font-size="10.5" fill="%s">%s</text>'
            % (px - tw / 2, py - 8.5, tw, t["label_bg"], color, px, py + 3.8, color, escape(label))
        )

    def generalization_svg(self, child, parent):
        t = self.theme
        if child in self.actor_geom and parent in self.actor_geom:
            a, b = self.actor_geom[child], self.actor_geom[parent]
            out = -1 if a["side"] == "left" else 1
            ax, ay = a["x"], a["y"] - 4
            bx, by = b["x"], b["y"] - 4
            sx, sy = ax + out * 22, ay
            ex, ey = bx + out * 22, by
            cx = (sx + ex) / 2 + out * 74
            return (
                '<path d="M %.1f %.1f C %.1f %.1f, %.1f %.1f, %.1f %.1f" fill="none" stroke="%s" stroke-width="1.5" marker-end="url(#hollowTriangle)"/>'
                % (sx, sy, cx, sy, cx, ey, ex, ey, t["gen"])
            )
        return self.internal_edge_svg(child, parent, "gen")

    # ---- document
    def render(self):
        t = self.theme
        s = self.spec
        out = []
        out.append(
            '<svg xmlns="http://www.w3.org/2000/svg" width="%d" height="%d" viewBox="0 0 %d %d" font-family="%s" role="img">'
            % (self.width, int(self.height), self.width, int(self.height), FONT)
        )
        title = s.get("title", "Use Case Diagram")
        out.append("<title>%s</title>" % escape(title))
        out.append("<defs>")
        out.append(
            '<linearGradient id="ucGrad" x1="0" y1="0" x2="0" y2="1">'
            '<stop offset="0%%" stop-color="%s"/><stop offset="100%%" stop-color="%s"/></linearGradient>'
            % (t["uc_fill_top"], t["uc_fill_bottom"])
        )
        for key in ("include", "extend"):
            out.append(
                '<marker id="openArrow-%s" viewBox="0 0 12 12" refX="10" refY="6" markerWidth="10" markerHeight="10" orient="auto-start-reverse">'
                '<path d="M 1 1 L 11 6 L 1 11" fill="none" stroke="%s" stroke-width="1.6" stroke-linecap="round"/></marker>'
                % (key, t[key])
            )
        out.append(
            '<marker id="hollowTriangle" viewBox="0 0 14 14" refX="12" refY="7" markerWidth="12" markerHeight="12" orient="auto-start-reverse">'
            '<path d="M 1 1 L 13 7 L 1 13 Z" fill="%s" stroke="%s" stroke-width="1.3"/></marker>'
            % (t["bg"], t["gen"])
        )
        out.append(
            '<filter id="softShadow" x="-25%%" y="-25%%" width="150%%" height="160%%">'
            '<feDropShadow dx="0" dy="3" stdDeviation="5" flood-color="#10203A" flood-opacity="0.10"/></filter>'
        )
        out.append("</defs>")
        out.append('<rect width="100%%" height="100%%" fill="%s"/>' % t["bg"])

        y = 44
        if s.get("title"):
            out.append(
                '<text x="%d" y="%d" font-size="23" font-weight="700" fill="%s" letter-spacing="0.2">%s</text>'
                % (self.margin, y, t["title"], escape(s["title"]))
            )
            y += 26
        if s.get("subtitle"):
            out.append(
                '<text x="%d" y="%d" font-size="13" fill="%s">%s</text>'
                % (self.margin, y, t["muted"], escape(s["subtitle"]))
            )

        out.append(
            '<rect x="%.1f" y="%.1f" width="%.1f" height="%.1f" rx="14" fill="%s" stroke="%s" stroke-width="1.5" filter="url(#softShadow)"/>'
            % (self.bx, self.by, self.bw, self.bh, t["boundary_fill"], t["boundary_stroke"])
        )
        out.append(
            '<path d="M %.1f %.1f h %.1f a 14 14 0 0 1 14 14 v %.1f h -%.1f z" fill="%s"/>'
            % (
                self.bx,
                self.by,
                self.bw - 14,
                18,
                self.bw,
                t["boundary_band"],
            )
        )
        label = s.get("system", "System")
        out.append(
            '<text x="%.1f" y="%.1f" font-size="14" font-weight="700" fill="%s">%s</text>'
            % (self.bx + 22, self.by + 24, t["boundary_label"], escape(label))
        )

        for a, b in self.assoc:
            if a in self.actor_geom and b in self.uc_geom:
                out.append(self.assoc_svg(a, b))
            elif b in self.actor_geom and a in self.uc_geom:
                out.append(self.assoc_svg(b, a))

        for src, dst in self.includes:
            out.append(self.internal_edge_svg(src, dst, "include"))
        for src, dst in self.extends:
            out.append(self.internal_edge_svg(src, dst, "extend"))
        for child, parent in self.gens:
            out.append(self.generalization_svg(child, parent))

        for u in self.ucs:
            out.append(self.use_case_svg(u, self.uc_geom[u["id"]]))

        for a in self.actors:
            out.append(self.actor_svg(a, self.actor_geom[a["id"]]))

        for src, dst in self.includes:
            out.append(self.edge_label_svg(src, dst, "include", "\u00abinclude\u00bb"))
        for src, dst in self.extends:
            out.append(self.edge_label_svg(src, dst, "extend", "\u00abextend\u00bb"))

        if s.get("legend", True):
            out.append(self.legend_svg())

        if s.get("footnote"):
            out.append(
                '<text x="%d" y="%d" font-size="11.5" fill="%s">%s</text>'
                % (self.margin, self.height - 22, t["muted"], escape(s["footnote"]))
            )

        out.append("</svg>")
        return "\n".join(out)

    def legend_svg(self):
        t = self.theme
        items = [("Association", "assoc")]
        if self.includes:
            items.append(("\u00abinclude\u00bb", "include"))
        if self.extends:
            items.append(("\u00abextend\u00bb", "extend"))
        if self.gens:
            items.append(("Generalization", "gen"))

        widths = [text_width(lbl, 11.5) + 52 for lbl, _ in items]
        total = sum(widths) + 16 * (len(items) - 1)
        x = max(self.margin, (self.width - total) / 2)
        y = self.by + self.bh + 34

        out = [
            '<g class="legend" opacity="0.95">',
            '<rect x="%.1f" y="%.1f" width="%.1f" height="34" rx="17" fill="%s" stroke="%s" stroke-width="1"/>'
            % (x - 18, y - 22, total + 36, t["label_bg"], t["legend_border"]),
        ]
        for (lbl, kind), w in zip(items, widths):
            color = t[kind]
            if kind == "assoc":
                out.append(
                    '<line x1="%.1f" y1="%.1f" x2="%.1f" y2="%.1f" stroke="%s" stroke-width="1.4"/>'
                    % (x, y - 5, x + 24, y - 5, color)
                )
            elif kind == "gen":
                out.append(
                    '<line x1="%.1f" y1="%.1f" x2="%.1f" y2="%.1f" stroke="%s" stroke-width="1.5" marker-end="url(#hollowTriangle)"/>'
                    % (x, y - 5, x + 24, y - 5, color)
                )
            else:
                out.append(
                    '<line x1="%.1f" y1="%.1f" x2="%.1f" y2="%.1f" stroke="%s" stroke-width="1.5" stroke-dasharray="6 4" marker-end="url(#openArrow-%s)"/>'
                    % (x, y - 5, x + 24, y - 5, color, kind)
                )
            out.append(
                '<text x="%.1f" y="%.1f" font-size="11.5" fill="%s">%s</text>'
                % (x + 30, y - 1, t["muted"], escape(lbl))
            )
            x += w + 16
        out.append("</g>")
        return "\n".join(out)


# ---------------------------------------------------------------- outputs
def write_text(path, content):
    with open(path, "w", encoding="utf-8", newline="\n") as fh:
        fh.write(content)


def find_browser():
    candidates = [
        os.environ.get("CHROME_PATH", ""),
        r"C:\Program Files\Google\Chrome\Application\chrome.exe",
        r"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe",
        r"C:\Program Files\Microsoft\Edge\Application\msedge.exe",
        r"C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe",
        "/Applications/Google Chrome.app/Contents/MacOS/Google Chrome",
        "/usr/bin/google-chrome",
        "/usr/bin/chromium",
        "/usr/bin/chromium-browser",
    ]
    for c in candidates:
        if c and os.path.exists(c):
            return c
    for name in ("chrome", "google-chrome", "chromium", "msedge"):
        p = shutil.which(name)
        if p:
            return p
    return None


def svg_to_png(svg_path, png_path, width, height, scale=2):
    browser = find_browser()
    if not browser:
        sys.stderr.write("WARN: no Chrome/Edge found; skipped PNG. Open the SVG instead.\n")
        return False
    url = "file:///" + os.path.abspath(svg_path).replace("\\", "/").lstrip("/")
    url = "file:///" + os.path.abspath(svg_path).replace("\\", "/")
    with tempfile.TemporaryDirectory(ignore_cleanup_errors=True) as td:
        cmd = [
            browser,
            "--headless=new",
            "--disable-gpu",
            "--hide-scrollbars",
            "--no-first-run",
            "--no-default-browser-check",
            "--force-device-scale-factor=%s" % scale,
            "--window-size=%d,%d" % (int(width), int(height)),
            "--screenshot=%s" % os.path.abspath(png_path),
            "--user-data-dir=%s" % td,
            url,
        ]
        try:
            subprocess.run(cmd, check=True, stdout=subprocess.DEVNULL, stderr=subprocess.DEVNULL, timeout=90)
        except Exception as exc:
            sys.stderr.write("WARN: PNG render failed (%s)\n" % exc)
            return False
    return os.path.exists(png_path)


def to_plantuml(diagram):
    s = diagram.spec
    actor_ids = {a["id"] for a in diagram.actors}
    lines = ["@startuml", "left to right direction", 'skinparam packageStyle rectangle']
    if s.get("title"):
        lines.append("title %s" % s["title"])
    lines.append('rectangle "%s" {' % s.get("system", "System"))
    for u in diagram.ucs:
        lines.append('  usecase "%s" as %s' % (u.get("name", u["id"]), u["id"]))
    lines.append("}")
    for a in diagram.actors:
        lines.append('actor "%s" as %s' % (a.get("name", a["id"]), a["id"]))
    for a, b in diagram.assoc:
        if a in actor_ids:
            lines.append("%s --> %s" % (a, b))
        elif b in actor_ids:
            lines.append("%s --> %s" % (b, a))
    for a, b in diagram.includes:
        lines.append("%s ..> %s : <<include>>" % (a, b))
    for a, b in diagram.extends:
        lines.append("%s ..> %s : <<extend>>" % (b, a))
    for a, b in diagram.gens:
        lines.append("%s --|> %s" % (a, b))
    lines.append("@enduml")
    return "\n".join(lines)


SCHEMA = __doc__


def main(argv=None):
    ap = argparse.ArgumentParser(description="Render a UML use case diagram from JSON.")
    ap.add_argument("spec", nargs="?", help="path to spec JSON ('-' for stdin)")
    ap.add_argument("--out", "-o", help="output .svg path")
    ap.add_argument("--png", help="also write a PNG (requires Chrome/Edge)")
    ap.add_argument("--scale", type=float, default=2, help="PNG scale factor (default 2)")
    ap.add_argument("--plantuml", help="also write PlantUML source")
    ap.add_argument("--theme", choices=sorted(THEMES), help="override spec theme")
    ap.add_argument("--schema", action="store_true", help="print the spec format and exit")
    args = ap.parse_args(argv)

    if args.schema:
        print(SCHEMA)
        return 0
    if not args.spec:
        die("spec path is required (use --schema to see the format)")

    spec = load_spec(args.spec)
    if args.theme:
        spec["theme"] = args.theme
    validate(spec)

    diagram = Diagram(spec).layout()
    svg = diagram.render()

    out = args.out or os.path.splitext(args.spec)[0] + ".svg"
    write_text(out, svg)
    print("SVG  -> %s  (%dx%d)" % (out, diagram.width, int(diagram.height)))

    if args.png:
        ok = svg_to_png(out, args.png, diagram.width, diagram.height, args.scale)
        if ok:
            print("PNG  -> %s" % args.png)
    if args.plantuml:
        write_text(args.plantuml, to_plantuml(diagram) + "\n")
        print("PUML -> %s" % args.plantuml)
    return 0


if __name__ == "__main__":
    sys.exit(main())
