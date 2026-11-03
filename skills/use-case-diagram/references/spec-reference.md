# Spec + CLI Reference

## Command

```bash
python scripts/usecase_diagram.py spec.json --out diagram.svg
python scripts/usecase_diagram.py spec.json --out diagram.svg --png diagram.png
python scripts/usecase_diagram.py spec.json --out diagram.svg --png diagram.png --theme dark
python scripts/usecase_diagram.py spec.json --out diagram.svg --plantuml diagram.puml
python scripts/usecase_diagram.py --schema
```

- `--out` defaults to the spec path with `.svg`.
- `--png` uses headless Chrome or Edge. If neither is installed, the SVG is still
  written; open it in a browser and export, or install Chrome.
- `--scale 2` (default) renders a crisp 2x PNG. Use 1 for fast drafts, 3 for print.
- `--theme light|dark` overrides the theme in the spec.
- `--plantuml` writes equivalent PlantUML for pasting into other tools or docs.

## Spec fields

| Field | Required | Meaning |
|---|---|---|
| `title` | no | Large heading above the diagram |
| `subtitle` | no | Secondary line (version, course, date) |
| `system` | no | Name shown on the system boundary rectangle |
| `theme` | no | `light` (default) or `dark` |
| `actors` | yes | list of actor objects |
| `use_cases` | yes | list of use case objects |
| `associations` | no | `[actor_id, use_case_id]` pairs |
| `includes` | no | `[base_id, included_id]` — arrow points at the included use case |
| `extends` | no | `[extension_id, base_id]` — arrow points at the base use case |
| `generalizations` | no | `[child_id, parent_id]` — hollow triangle at the parent |
| `footnote` | no | small line under the legend |

Actor object: `{"id": "...", "name": "...", "side": "left"|"right", "external": true|false}`

Use case object: `{"id": "...", "name": "..."}`

- `id` is the machine key used by relationships (no spaces, unique).
- `name` is what appears in the bubble; wrap it naturally, keep it short.
- `side` decides which edge the actor stands on. Default is `left`.
- `external` adds a small "external system" note above the actor name.

## Order matters

Use cases are laid out **in array order**, filled top-to-bottom, then left-to-right
(2 columns up to 12 use cases, 3 up to 18, 4 beyond that).

To reduce crossing lines, list use cases grouped by their primary actor, and place
shared/include targets next to the use cases that include them. For example:

```
col 0: passenger goals
col 1: staff goals + shared capacity check
col 2: admin/vendor goals
```

## Validation and warnings

The script exits with an error when:

- an id is missing, duplicated, or used by both an actor and a use case;
- a relationship names an unknown id, or connects an element to itself;
- an association connects two use cases (use include/extend/generalization);
- an actor `side` is not `left` or `right`.

It prints warnings (and still renders) when:

- a use case has no actor association and no relationship;
- the same relationship is listed twice;
- include or extend relationships form a cycle.

## Troubleshooting

| Symptom | Fix |
|---|---|
| PNG not created | Chrome/Edge not found. Pass `CHROME_PATH` env var, or open the SVG manually. |
| Text overflows a bubble | Shorten the name, or split one long goal into two use cases. |
| Diagram is too wide | Fewer use cases per column (reorder), or split the diagram by module. |
| Lines cross the whole diagram | Reorder use cases so each actor's goals sit on that actor's side. |
| Actors overlap | Fewer actors per side, or move a shared actor to the other side. |
| Two actors share one use case and lines tangle | Acceptable, but put the shared use case near the side with more connections. |

## Suggested file naming

```
docs/use-case-diagram.svg
docs/use-case-diagram.png
docs/use-case-model.md      # actor list, use case list, trace to FR IDs
```

Deliver the PNG for review (it always opens) and keep the SVG for editing/scaling.
