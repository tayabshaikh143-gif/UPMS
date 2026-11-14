# Session log — 2026-11-09

First full working session on UPMS with the AI agent. Everything below was done
in one session; this file exists so a new session can continue without guessing.

## 1. Context loaded

Read both planning documents:

- `UPMS_Chat_Context_Export.docx` — project decisions, scope, roles, open items.
- `docs/UPMS_Step_2_SRS_v1.0.docx` — SRS v1.0 (FR-01..26, BR-01..10, NFR-01..10,
  acceptance criteria, entity list, workflows).

Key rules fixed: no student-ID verification; online or cash-at-boarding payment;
20-minute cancellation cutoff; T-5 cash no-show expiry; no waiting list; offline
tickets recorded centrally; routes approved by admin; replacement bus stays on the
same trip; atomic capacity checks; a ticket boards once.

## 2. Use case diagram skill built

Problem: no installed UML/use-case skill. Researched UML 2.5 rules online
(Visual Paradigm guide, agilemodeling guidelines, go-uml checklist, SE Book,
include/extend paper) and built a custom skill at:

`C:\Users\Taha\.config\opencode\skill\use-case-diagram\`

Contents: `SKILL.md` (8-step workflow + quality gate), `references/use-case-rules.md`
(actor/use case/relationship rules, review checklist, SRS extraction recipe),
`references/spec-reference.md`, `scripts/usecase_diagram.py` (JSON → SVG/PNG,
stdlib only, headless Chrome for PNG, optional PlantUML), `examples/upms.json`,
`examples/upms-simple.json`, `examples/mini.json`.

Renderer features added while working: explicit two-column layouts (`columns`),
optional legend (`legend: false`), actor barycenter placement with fanned anchors,
include/extend curve routing, masked labels with collision avoidance, light/dark
themes, `--plantuml` export.

## 3. GitHub publishing

Account: `tayabshaikh143-gif` (no dot in the username). The earlier stored token
was expired; logged in with GitHub device flow, then created the repo through the
API and pushed. OAuth token saved in Windows Credential Manager via GCM.

- Repo: https://github.com/tayabshaikh143-gif/UPMS (public)
- Description and topics set (uml, use-case-diagram, srs, university, transport,
  python, opencode).

## 4. diagram-design skill installed

Cloned `cathrynlavery/diagram-design` (MIT, v2.6) and installed it globally at
`C:\Users\Taha\.config\opencode\skill\diagram-design\` (290 files, 5.19 MB).
Also copied into the repo at `skills/diagram-design/` with its MIT `LICENSE` and
an `ATTRIBUTION.md`.

Used it for all editorial-style diagrams (activity, class, sequence).

## 5. Use case diagrams

- Detailed: `diagrams/UPMS_UseCase_Diagram.*` — 18 use cases, 4 actors,
  «include» (Book/Offline → Check Capacity) and «extend» (Pay Online → Book;
  Collect Cash → Board).
- Simplified (owner rejected the first as cluttered): 
  `diagrams/UPMS_UseCase_Diagram_Simple.*` — 2 columns, 12 use cases,
  passenger goals left, staff/vendor/admin right, no crossing lines.

Correction logged in `AGENTS.md`: keep use case diagrams readable, 2 columns,
group by actor, no crossings.

## 6. Activity diagrams (6)

Folder `diagrams/activity/`, each `.html` + `.png` + `.svg`:

Booking, Cancellation, Boarding, NoShow, Delay, Breakdown.

Editorial style from diagram-design: filled start dot, action boxes, decision
diamonds, bullseye final, orthogonal connectors, light tints (white = human,
blue-grey = system, neutral = terminal, coral = one focal rule per diagram).

## 7. Class diagrams (4)

Folder `diagrams/class/`, each `.html` + `.png` + `.svg`:

| Diagram | Classes |
|---|---|
| Booking | Passenger, Booking, Payment, Trip, Route, Vendor, Bus |
| Network | Route, RouteStop, Stop, Trip, TripVehicleAssignment, Bus, Vendor |
| People | User, PassengerProfile, Staff, AuditLog |
| Operations | Booking, CheckIn, Refund, Incident, Notification, Trip |

UML compartments, multiplicities, composition/aggregation markers, full
relationship legend in each diagram. All 17 SRS entities are covered across the
four diagrams (Booking/Trip repeated as boundary classes in the Operations view).

## 8. Sequence diagrams (6)

Folder `diagrams/sequence/`, each `.html` + `.png` + `.svg`:

Booking, Cancellation, Boarding, NoShow, Delay, Breakdown.

4–5 lifelines, activation bars, sync/return/async arrows, one `alt` or `loop`
fragment where the SRS has a branch, one coral headline result per diagram.
All passed the skill's `self_check.py`.

## 9. Working rules

Created `AGENTS.md` at repo root: GitHub permission before commit/push, commit
only when a module is complete (ask, and remind if forgotten), no deletion
without permission, never repeat a reported mistake (corrections log), scope and
delivery rules. Committed as `ae6ce4d`.

## 10. End state of the session

- 6 activity + 4 class + 6 sequence diagrams + 2 use case diagrams in the repo.
- README updated with the layout and Step 5 marked done.
- Step 4 (ERD + relational database design) was requested next; only the ER
  reference was read before the session was paused.

## 12. ER diagrams (Step 4, first part)

Created six subject-area ER diagrams in `diagrams/erd/` (rebuilt larger on 2026-11-14 for readability), matching the class-diagram domains:

| Diagram | Entities |
|---|---|
| `UPMS_ERD_Booking` | Passenger, Booking, Payment, Trip |
| `UPMS_ERD_Network` | Trip, Route, TripVehicleAssignment, Bus, Vendor |
| `UPMS_ERD_Stops` | Route, RouteStop (join), Stop |
| `UPMS_ERD_People` | User, PassengerProfile, Staff, AuditLog |
| `UPMS_ERD_Operations` | Booking, CheckIn, Refund |
| `UPMS_ERD_Incidents` | Trip, Incident, Notification |

Each entity lists `#` primary keys and `→` foreign keys; relationships carry
`1` / `N` / `0..1` cardinalities. All four passed the skill self-check and were
exported to PNG + SVG.

Still pending for Step 4: the written relational schema (table definitions,
data types, indexes, constraints) as a document.

## 11. Mistakes and fixes (short list)

- First use case diagram was cluttered → replaced with a clean 2-column version.
- Activity diagrams initially sat in `diagrams/` root → moved to
  `diagrams/activity/`.
- Sequence diagram labels initially overlapped lifelines and lines (sloped arrows
  caused by patching only `y1`) → fixed y2, moved guards clear of lifelines,
  shortened one guard label.


## 13. Complete ER diagram (2026-11-14)

Added `UPMS_ERD_Complete` in `diagrams/erd/`: all 18 entities and 17 relationships
in one 1840x1240 canvas, zoned by subject area (identity, booking, network,
fleet/incidents). The six subject ER diagrams remain as the readable detail views.

