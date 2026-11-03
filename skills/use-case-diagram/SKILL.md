---
name: use-case-diagram
description: Build correct, clean UML use case diagrams from an SRS, a feature list, or a rough system description — identify actors, keep use cases at goal level, apply «include» / «extend» / generalization correctly, and render a polished SVG/PNG with a local renderer. Use this skill whenever the user asks for a use case diagram, UML use cases, an actor-goal model, a system boundary diagram, or wants requirements turned into diagrams, even if they never say the word "UML".
metadata:
  scope: global
---

# Use Case Diagram

A use case diagram shows **who** uses the system and **what goals** they achieve
with it. The value is in the model, not the picture: if the actors are wrong or
the include/extend arrows point the wrong way, a pretty diagram is still a wrong
diagram.

This skill does two things:

1. Helps you produce a defensible model from requirements (actors, goals,
   relationships, traceability).
2. Renders it with `scripts/usecase_diagram.py` — deterministic layout, clean
   light/dark theme, SVG + PNG, optional PlantUML export. No internet needed.

## Workflow

### 1. Fix the scope first
Name the system and the boundary in one sentence ("UPMS: university transport
booking and operations"). If the source is a full SRS, decide whether to draw one
overall diagram or split by module. One readable diagram beats one giant one:
split when there are more than ~18 use cases or more than ~6 actors per side.

### 2. Find the actors
- Read the roles section of the SRS first; each role becomes a candidate actor.
- Add external systems the requirements mention (payment gateway, SMS gateway,
  university ERP) and mark them `"external": true`.
- Name roles, not people or job titles: "Administrator", not "Mr. Ali".
- Never make the system itself an actor. Never put an actor inside the boundary.
- Use actor generalization only when a child genuinely inherits all parent
  behaviour. `Student Member` inherits everything `Member` can do — that is a
  valid generalization. "Admin can do what Passenger does" is usually not.

### 3. Find the use cases (goals, not steps)
For each functional requirement ask: *what goal does this serve, for which
actor?* Merge requirements that serve one goal; split requirements that serve
several.
- Name with **verb + object**: "Book Ticket", "Issue Offline Ticket".
- Keep one abstraction level. "Book Ticket" is a goal; "Enter OTP" is a step and
  belongs in a use case description or an activity diagram.
- Business rules (BR-xx) rarely become use cases; they constrain them and may
  justify an include or an extend.
- Every use case should trace to at least one FR/BR. Keep that trace in the
  accompanying document, not on the diagram.

### 4. Add relationships — and get the direction right
| Situation | Relationship | Arrow |
|---|---|---|
| Actor takes part in the goal | Association | solid line, **no arrowhead** |
| Base case **always** does X, and X is reused | «include» | dashed, from base → included |
| Base case is valid without X; X is conditional | «extend» | dashed, from extension → **base** |
| Child is a specialised parent | Generalization | solid, hollow triangle at parent |

Rules worth repeating because they are the usual mistakes:
- «extend» points **at the base**, not away from it.
- «include» must be mandatory and reused. Optional behaviour is «extend».
- Keep relationship depth to one level; chains of includes are a smell.
- No cycles. Do not invent arrows to decorate the diagram.

Detailed semantics, decision tests and a review checklist live in
`references/use-case-rules.md` — read it when the model is non-trivial or will be
reviewed/graded.

### 5. Order the use cases for a readable layout
The renderer fills **top-to-bottom, then left-to-right** (2 columns up to 12 use
cases, 3 up to 18, 4 beyond). Order is your main layout control:
- group use cases by their primary actor, in the same order as the actors;
- put shared «include» targets next to the cases that include them;
- put «extend» extensions directly above or below their base;
- keep shared use cases on the side with more connections.

### 6. Write the spec, then render

```bash
python scripts/usecase_diagram.py spec.json --out diagram.svg --png diagram.png
```

Minimal spec (full field list: `references/spec-reference.md`, or run
`python scripts/usecase_diagram.py --schema`):

```json
{
  "title": "Campus Library — Use Case Diagram",
  "subtitle": "v1.0",
  "system": "Campus Library",
  "actors": [
    {"id": "member", "name": "Member", "side": "left"},
    {"id": "student", "name": "Student Member", "side": "left"},
    {"id": "librarian", "name": "Librarian", "side": "right"}
  ],
  "use_cases": [
    {"id": "borrow", "name": "Borrow Book"},
    {"id": "check_limits", "name": "Check Borrowing Limits"},
    {"id": "reserve", "name": "Reserve Book"}
  ],
  "associations": [["member", "borrow"], ["member", "reserve"]],
  "includes": [["borrow", "check_limits"]],
  "extends": [["reserve", "borrow"]],
  "generalizations": [["student", "member"]]
}
```

Working examples: `examples/upms.json` (18 use cases, 4 actors) and
`examples/mini.json` (includes, extends, actor generalization).

### 7. Look at the rendered image before delivering
Read the PNG. Check that no label sits on a bubble, no arrowhead is hidden, no
actor overlaps another, and that lines do not cross the whole diagram. Fix by
reordering the use case list, not by hand-editing the SVG.

### 8. Deliver
Deliver the **PNG** for review and the **SVG** for editing, plus a short model
note (actors, use cases, FR trace) so the diagram is auditable. If the user is
writing a Word/PDF report, insert the PNG into the document; `--plantuml` also
produces source for teams that prefer PlantUML.

## Quality gate

Do not hand over a diagram until all of these are true:

1. Every bubble is a goal, not a click, screen, or validation rule.
2. All use cases inside the boundary; all actors outside.
3. Every use case has an actor association, or is an include/extend target of one.
4. Every actor has at least one association.
5. «include» is mandatory + reused; «extend» is optional + points at the base.
6. No include/extend cycles, no deep chains.
7. Names are verb + object and match the SRS vocabulary.
8. The picture is readable at normal zoom with no overlapping labels.
9. Each use case traces to a requirement ID in the model note.
10. Scope is stated in the title or subtitle (system + version).

## Common failures and fixes

| Failure | Fix |
|---|---|
| Diagram is a flowchart | Collapse steps into goals; optional branches become «extend» |
| Actors inside the box, or system as an actor | Move actors outside; remove the system actor |
| Include arrows for optional features | Change to «extend» and point at the base |
| Every use case connected to every actor | Model the role that actually initiates each goal |
| Too many use cases to read | Split by module or actor group; keep one boundary per module |
| Lines crossing the diagram | Reorder use cases so each actor's goals sit on that side |
| Long names breaking bubbles | Shorten to two or three words; explain detail in the use case description |

## Reference files

- `references/use-case-rules.md` — UML 2.5 semantics, actor/use case/relationship
  rules, naming, layout guidance, review checklist, SRS extraction recipe.
- `references/spec-reference.md` — every spec field, CLI flag, validation rules,
  troubleshooting.
- `scripts/usecase_diagram.py` — renderer (Python 3.9+, stdlib only; PNG needs
  Chrome or Edge; falls back to SVG-only with a warning).
- `examples/` — complete specs you can copy and edit.
