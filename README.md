# UPMS — University Point Management System

A web-based university transport platform for route/trip management, advance ticket
booking, online payment, cash-at-boarding, capacity control, offline ticket
issuance, and operational incident handling.

Initial target deployment: **University of Sindh, Jamshoro** (single-university MVP).

## Project status

| Step | Deliverable | Status |
|---|---|---|
| 1 | Project proposal | drafted (not in this repo yet) |
| 2 | Software Requirements Specification (SRS v1.0) | done — `docs/UPMS_Step_2_SRS_v1.0.docx` |
| 3 | Use case diagram + use case descriptions | diagram done, descriptions pending |
| 4 | ERD and relational database design | pending |
| 5 | Activity and sequence diagrams | done — 6 activity + 6 sequence diagrams |
| 6 | Architecture and UI/UX wireframes | pending |
| 7 | Technology stack confirmation | proposed only |
| 8 | Implementation and testing | pending |

## Repository layout

```
docs/                       SRS document
  UPMS_Step_2_SRS_v1.0.docx
UPMS_Chat_Context_Export.docx   planning/continuity brief (move into docs/ once closed in Word)
diagrams/                   rendered project diagrams
  activity/                 six activity diagrams (Booking, Cancellation, Boarding,
                            NoShow, Delay, Breakdown) as .html / .png / .svg
  sequence/                 six sequence diagrams (same scenarios) as .html / .png / .svg
  class/                    four UML class diagrams (.html / .png / .svg):
                            Booking, Network, People, Operations
  UPMS_UseCase_Diagram_Simple.png / .svg / .json   clean 2-column view (recommended)
  UPMS_UseCase_Diagram.png / .svg / .json          detailed view with «include»/«extend»
Initial_daigrams/           early Mermaid workflow sketches
AI_docs/                    session memory: status, logs, inventory, tooling
skills/
  use-case-diagram/         opencode skill: UML use case modelling + SVG/PNG renderer
    SKILL.md
    scripts/usecase_diagram.py
    references/
    examples/
  diagram-design/           third-party opencode skill: 45 diagram types, HTML + SVG
    SKILL.md                (MIT, see LICENSE and ATTRIBUTION.md)
    references/
    assets/
    scripts/
```

## Key rules fixed in the SRS

- No student-ID/card verification; any person may book subject to capacity.
- Payment: online, or cash collected at boarding and recorded by staff.
- Passenger cancellation allowed up to 20 minutes before departure; capacity is
  released immediately.
- Cash booking not boarded by 5 minutes before departure expires automatically.
- No waiting list — released capacity is sold to passengers physically present.
- Offline tickets must be recorded in the same central system.
- Routes are administered by the university; vendors operate only on approved routes.
- Delay/breakdown reporting with replacement bus assignment on the same trip record.
- Capacity checks are atomic; a ticket cannot be boarded twice.

## Regenerating the use case diagram

The diagram is generated from a JSON spec by the bundled skill renderer
(Python 3.9+, stdlib only; PNG export uses headless Chrome or Edge).

```bash
python skills/use-case-diagram/scripts/usecase_diagram.py \
  diagrams/UPMS_UseCase_Diagram.json \
  --out diagrams/UPMS_UseCase_Diagram.svg \
  --png diagrams/UPMS_UseCase_Diagram.png
```

To use the skill in opencode, copy `skills/use-case-diagram` into your global
skills directory (`~/.config/opencode/skill/`). See `SKILL.md` for the modelling
workflow, relationship rules, and quality checklist.

## Skills in this repo

Two opencode skills are bundled so the diagram workflow is reproducible:

| Skill | Use it for |
|---|---|
| `skills/use-case-diagram` | UML use case diagrams (actors, goals, «include»/«extend»/generalization) with a deterministic local renderer |
| `skills/diagram-design` | Architecture, flowcharts, sequence, ER, deployment, timeline and 40+ other visual types as self-contained HTML + SVG |

Install either one by copying its folder into `~/.config/opencode/skill/`.
`diagram-design` is third-party software by Cathryn Lavery, used under the MIT
license; see its `ATTRIBUTION.md`.

## Proposed technology stack (not yet approved)

React frontend, ASP.NET Core Web API backend, SQL Server database, Entity
Framework Core. This is a proposal only.

## Notes

- The SRS is a draft for an academic MVP and should be validated with the
  supervisor and university transport administration before implementation.
- No secrets or credentials belong in this repository.
