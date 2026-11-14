# Project status — UPMS

Last updated: 2026-11-14

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
| 7 | Technology stack confirmation | confirmed — React 19 + ASP.NET Core 8 + EF Core 8 + SQL Server LocalDB |
| 8 | Implementation and testing | in progress — Slices 1–2 API + web; Slice 2 committed and pushed |

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
| `a22f36e` | feat(skills): sync design, graph and interview skills; add skill-sync rule |
| `3d9f185` | feat(diagrams): add complete ER model in table and Chen notation |
| `ee51438` | feat(api): slice 1 foundation, auth and catalog |
| `3c22d1c` | feat(web): add slice 1 admin UI with route stops and edit actions |
| `89d4a1e` | feat(bookings): add ticket booking, capacity control and receipts |

## Current implementation — Slices 1 and 2

- `apps/api`: ASP.NET Core 8 Web API with Identity + JWT, EF Core 8, LocalDB
  database `UPMS`, migration `InitialCreate`, seeded demo catalog and admin
  `admin@upms.local`.
- `apps/web`: React 19 + Vite + TypeScript + Tailwind v4 app with login,
  dashboard and routes/stops/vendors/buses/trips pages. Admin-only actions are
  hidden in the UI and enforced by the API. Upcoming trips show the route name
  and the actual origin → destination stops. Stops are grouped by route as an
  ordered origin-to-destination timeline; admins can append or remove stops
  without entering coordinates. Routes, vendors, buses and trips all support
  inline edit forms.
- Graphify master graph refreshed after Slice 2: 2730 nodes / 6836 edges / 112
  communities (`graphify-out/`).
- Live verification: login, dashboard, create/edit/delete for routes, vendors,
  buses and trips, route-stop add/remove, and duplicate/error handling all pass
  in Playwright. Production build passes.
- The web app is committed as `3c22d1c` and pushed to `main`.
- Slice 2 adds bus assignment to trips, capacity/remaining-seat calculations,
  passenger registration, online/cash booking, my-bookings, cancellation and an
  admin bookings list. Capacity uses a serializable transaction; cash bookings
  expire 5 minutes before departure via a background sweep.
- Online payment is a demonstration gateway that confirms immediately. Online
  cancellations are marked `Refunded` (simulated).
- Migration `AddBookings` applied; demo passenger seeded as
  `student@upms.local` / `Student@123`.
- Slice 2 verification: online and cash booking, capacity decrement, cancellation
  release, 5 parallel requests against capacity 1 (one `201`, four `409`), and a
  cash booking expiring via the background sweep.
- Passengers can open a printable ticket receipt from My bookings; it shows the
  reference, passenger, journey, departure, bus, payment and fare.
- Slice 2 is committed as `89d4a1e` and pushed to `main`.

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
- Architecture and UI/UX wireframes (Step 6).
- Slice 3: staff boarding, offline ticket sales, incidents/replacement bus and
  operational reports.
- Decide whether to remove demo/test rows and test bookings from the LocalDB
  database.
