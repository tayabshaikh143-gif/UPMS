# Diagram inventory

All diagrams are **self-contained HTML + inline SVG** produced with the
`diagram-design` skill (editorial paper style), then exported to PNG (2x) and
SVG with the skill's scripts. Use case diagrams come from the custom
`use-case-diagram` skill instead.

## Use case — `diagrams/`

| File | Contents |
|---|---|
| `UPMS_UseCase_Diagram_Simple.{png,svg,json}` | Recommended view. 2 columns, 12 goals, Passenger left; Staff, Vendor, Administrator right; no crossing lines |
| `UPMS_UseCase_Diagram.{png,svg,json}` | Detailed view, 18 use cases, includes Book→Check Capacity, Offline→Check Capacity; extends Pay Online→Book, Collect Cash→Board |

## Activity — `diagrams/activity/`

| File prefix | Flow |
|---|---|
| `UPMS_Activity_Diagram_Booking` | Search/select trip → capacity check → online or cash → confirm + ticket |
| `UPMS_Activity_Diagram_Cancellation` | 20-min cutoff → cancel → release capacity → refund if online |
| `UPMS_Activity_Diagram_Boarding` | Scan ticket → validate once → collect cash if due → mark boarded |
| `UPMS_Activity_Diagram_NoShow` | T-5 check → expire + release → passenger present → offline ticket + cash |
| `UPMS_Activity_Diagram_Delay` | Report delay → admin review → update ETA → notify |
| `UPMS_Activity_Diagram_Breakdown` | Report → replacement available? → assign same trip or cancel + refund → notify |

Each has `.html`, `.png`, `.svg`.

## Class — `diagrams/class/`

| File prefix | Classes | Focal |
|---|---|---|
| `UPMS_Class_Diagram_Booking` | Passenger, Booking, Payment, Trip, Route, Vendor, Bus | Booking |
| `UPMS_Class_Diagram_Network` | Route, RouteStop, Stop, Trip, TripVehicleAssignment, Bus, Vendor | Trip |
| `UPMS_Class_Diagram_People` | User, PassengerProfile, Staff, AuditLog | User |
| `UPMS_Class_Diagram_Operations` | Booking, CheckIn, Refund, Incident, Notification, Trip | Incident |

Each has `.html`, `.png`, `.svg`.

## Sequence — `diagrams/sequence/`| File prefix | Lifelines | Fragment |
|---|---|---|
| `UPMS_Sequence_Diagram_Booking` | Passenger, Booking Service, Payment Gateway, Database | `alt [online / cash]` |
| `UPMS_Sequence_Diagram_Cancellation` | Passenger, Service, Database, Gateway | `alt [20+ min / less]` |
| `UPMS_Sequence_Diagram_Boarding` | Staff, Service, Database | `alt [valid / invalid]` |
| `UPMS_Sequence_Diagram_NoShow` | Scheduler, Service, Database, Staff App | `loop [each booking]` |
| `UPMS_Sequence_Diagram_Delay` | Staff, Service, Admin, Database, Passenger | none (async notification) |
| `UPMS_Sequence_Diagram_Breakdown` | Staff, Service, Admin, Database, Passenger | `alt [replacement / cancel]` |

Each has `.html`, `.png`, `.svg`.

## ER — `diagrams/erd/`

Six subject-area ER diagrams plus one complete overview, each with large 280px entities and 13.5px field text:

| File prefix | Entities | Focal |
|---|---|---|
| `UPMS_ERD_Complete_Chen` | Chen notation: all 18 entities, 17 relationships, primary-key ovals; teal entities, navy relationships, magenta attributes (SmartDraw palette) | — |
| `UPMS_ERD_Complete` | all 18 entities and 17 relationships (full software) | User |
| `UPMS_ERD_Booking` | Passenger, Booking, Payment, Trip | Booking |
| `UPMS_ERD_Network` | Trip, Route, TripVehicleAssignment, Bus, Vendor | Trip |
| `UPMS_ERD_Stops` | Route, RouteStop (join), Stop | — |
| `UPMS_ERD_People` | User, PassengerProfile, Staff, AuditLog | User |
| `UPMS_ERD_Operations` | Booking, CheckIn, Refund | Booking |
| `UPMS_ERD_Incidents` | Trip, Incident, Notification | Incident |

Fields carry `#` primary keys and `→` foreign keys; cardinality is `1` / `N` /
`0..1` at each end. Each has `.html`, `.png`, `.svg`.

## Visual rules used everywhere

- Paper `#f5f5f5`, ink `#2d3142`, muted `#4f5d75`, accent `#bf4520`.
- Instrument Serif title, Geist names, Geist Mono labels/sublabels.
- Max 2 coral elements; coral = focal class / primary result / confirmed path.
- Orthogonal connectors only; masked labels with a gap; legend at the bottom.
- No shadows, no gradients beyond the subtle node fill, no Mermaid output.

## Regenerating

Use case:

```bash
python skills/use-case-diagram/scripts/usecase_diagram.py \
  diagrams/UPMS_UseCase_Diagram_Simple.json \
  --out diagrams/UPMS_UseCase_Diagram_Simple.svg \
  --png diagrams/UPMS_UseCase_Diagram_Simple.png
```

Editorial diagrams: open the `.html` in Chrome (source of truth), then export
with `skills/diagram-design/scripts/export_svg.py` and `export_png.py`.
