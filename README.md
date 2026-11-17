# UPMS — University Point Management System

A web-based university transport platform for route/trip management, advance ticket
booking, online payment, cash-at-boarding, capacity control, offline ticket
issuance, and operational incident handling.

Initial target deployment: **University of Sindh, Jamshoro** (single-university MVP).

## Project status

| Step | Deliverable | Status |
|---|---|---|
| 1 | Project proposal | drafted (not in this repo yet) |
| 2 | Software Requirements Specification (SRS v1.1) | done — `docs/UPMS_Step_2_SRS_v1.1.docx` (v1.0 kept as history) |
| 3 | Use case diagram + use case descriptions | diagram done, descriptions pending |
| 4 | ERD and relational database design | diagrams done, schema document pending |
| 5 | Activity and sequence diagrams | done — 6 activity + 6 sequence diagrams |
| 6 | Architecture and UI/UX wireframes | pending |
| 7 | Technology stack confirmation | confirmed — React + ASP.NET Core + EF Core + SQL Server |
| 8 | Implementation and testing | in progress — Slice 1 catalog + Slice 2 booking/capacity |

## Repository layout

```
docs/                       SRS documents
  UPMS_Step_2_SRS_v1.0.docx  original SRS draft (history)
  UPMS_Step_2_SRS_v1.1.docx  implementation baseline + decision log
  UPMS_Requirements_Review_2026-11-15.md  requirements/design review
UPMS_Chat_Context_Export.docx   planning/continuity brief (move into docs/ once closed in Word)
diagrams/                   rendered project diagrams
  activity/                 six activity diagrams (Booking, Cancellation, Boarding,
                            NoShow, Delay, Breakdown) as .html / .png / .svg
  sequence/                 six sequence diagrams (same scenarios) as .html / .png / .svg
  class/                    four UML class diagrams (.html / .png / .svg):
                            Booking, Network, People, Operations
  erd/                      eight ER diagrams (.html / .png / .svg):
                            Complete (table style), Complete_Chen (Chen notation),
                            Booking, Network, Stops, People, Operations, Incidents
  UPMS_UseCase_Diagram_Simple.png / .svg / .json   clean 2-column view (recommended)
  UPMS_UseCase_Diagram.png / .svg / .json          detailed view with «include»/«extend»
Initial_daigrams/           early Mermaid workflow sketches
apps/                       implemented software
  api/                      ASP.NET Core 8 + EF Core + LocalDB (Slice 1)
    UPMS.sln
    src/UPMS.Api/
  web/                      React 19 + Vite + TypeScript + Tailwind v4 (Slice 1)
    src/
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
  hallmark/                 anti-AI-slop design skill (MIT, v2.0.0)
  impeccable/               frontend design vocabulary + audit (Apache 2.0, v4.9.1)
  ui-ux-pro-max/            UI/UX design intelligence database (v2.15.0)
  graphify/                 code knowledge graph skill (Apache 2.0, v0.9.108)
  find-skills/              skill discovery via the `skills` CLI (MIT)
  grilling/                 design-tree interview skill (Matt Pocock)
  grill-me/                 alias that calls grilling
```

## Key rules fixed in the SRS

- No student-ID/card verification; any person may book subject to capacity.
- Payment: online, or cash collected at boarding and recorded by staff.
- Boarding opens 20 minutes before departure by default (configurable); cash
  no-show cutoff remains 5 minutes before departure.
- All schedule times use Asia/Karachi wall-clock time.
- Online cancellation up to 20 minutes before departure; full refund with no fee
  for the MVP; capacity is released immediately.
- Vendor-operated bus fares are retained by the vendor, who pays the university a
  monthly subscription; university-owned bus fares accrue to the university.
- One booking reserves one seat; no numbered seat selection and no group booking.
- No waiting list — released capacity is sold to passengers physically present.
- Offline tickets must be recorded in the same central system (name + phone,
  issuing staff, timestamp).
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

Every skill adopted for this project is vendored under `skills/`, with
`ATTRIBUTION.md` and the upstream license where one exists. The two diagram
skills keep the documentation workflow reproducible:

| Skill | Use it for |
|---|---|
| `skills/use-case-diagram` | UML use case diagrams (actors, goals, «include»/«extend»/generalization) with a deterministic local renderer |
| `skills/diagram-design` | Architecture, flowcharts, sequence, ER, deployment, timeline and 40+ other visual types as self-contained HTML + SVG |

Install any skill by copying its folder into `~/.config/opencode/skill/`.
`diagram-design` is third-party software by Cathryn Lavery, used under the MIT
license; see its `ATTRIBUTION.md`.

## Technology stack

Confirmed: React 19 + TypeScript + Vite + Tailwind CSS v4 frontend, ASP.NET
Core 8 Web API, Entity Framework Core 8, SQL Server LocalDB for local
development, JWT authentication.

Run the API on `http://localhost:5080` and the web app on
`http://127.0.0.1:5173`. See `apps/api/README.md` and `apps/web/README.md`.

## Notes

- The SRS is a draft for an academic MVP and should be validated with the
  supervisor and university transport administration before implementation.
- No secrets or credentials belong in this repository.

