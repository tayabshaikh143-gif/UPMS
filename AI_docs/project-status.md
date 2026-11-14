# Project status — UPMS

Last updated: 2026-11-09

## What this project is

University Point Management System (UPMS): a web-based university transport
platform for route/trip management, advance ticket booking, online payment,
cash-at-boarding, capacity control, offline ticket issuance, and incident
handling. Initial target: University of Sindh, Jamshoro (single-university MVP).

## Delivery steps

| Step | Deliverable | Status |
|---|---|---|
| 1 | Project proposal | drafted earlier; file not in this repo yet |
| 2 | SRS v1.0 | done — `docs/UPMS_Step_2_SRS_v1.0.docx` |
| 3 | Use case diagram + detailed descriptions | diagrams done (simple + detailed); `.docx` of descriptions pending |
| 4 | ERD and relational database design | in progress — 8 ER diagrams (6 subject + table-style complete + Chen complete), schema document pending |
| 5 | Activity and sequence diagrams | done — 6 activity + 6 sequence diagrams |
| 6 | Architecture and UI/UX wireframes | pending |
| 7 | Technology stack confirmation | pending (React + ASP.NET Core + SQL Server + EF Core is only a proposal) |
| 8 | Implementation and testing | in progress — Slice 1 backend (foundation + auth + catalog) running |

## Repository

- URL: https://github.com/tayabshaikh143-gif/UPMS (public)
- Default branch: `main`
- Local path: `D:\Tayab Shaikh\UPMS`
- GitHub auth: OAuth token stored in Windows Credential Manager (via GCM), so
  `git push` works without logging in again.

### Commits so far

| Commit | Message |
|---|---|
| `5f4a7b3` | docs: initial import of UPMS SRS, diagrams and use case skill |
| `8c5f363` | feat(skills): add diagram-design skill for richer diagram formats |
| `e902b5a` | feat(diagrams): add simplified two-column UPMS use case diagram |
| `e998d66` | feat(diagrams): add booking activity diagram in editorial style |
| `da71fc9` | feat(diagrams): add cancellation, boarding, no-show, delay and breakdown activity diagrams |
| `593a5ba` | chore(diagrams): move activity diagrams into diagrams/activity |
| `e023892` | feat(diagrams): add booking-domain UML class diagram |
| `12ff197` | feat(diagrams): add network, people and operations class diagrams |
| `ae6ce4d` | docs: add project working rules and corrections log |
| `62c0701` | feat(diagrams): add six sequence diagrams for booking operations |

## Open decisions (from SRS v1.0)

1. Online cancellation refund amount/fee and the actual refund workflow.
2. Default boarding start time before departure (admin-configurable; no default agreed).
3. Minimum passenger information for an offline ticket.
4. Online payment session hold expiry.
5. Fare structure and vendor settlement rules.
6. Whether a real payment gateway is feasible (otherwise label the flow as a simulation).

## Known follow-ups

- Build the written relational schema for Step 4 (tables, types, keys, indexes).
- Create the Step 3 `.docx` with detailed use case descriptions.
- Move `UPMS_Chat_Context_Export.docx` into `docs/` once it is closed in Word.
- Add the Step 1 proposal to the repo.
- Build ERD + relational schema (Step 4).
- Architecture and UI/UX wireframes (Step 6).
- Confirm the technology stack (Step 7).
