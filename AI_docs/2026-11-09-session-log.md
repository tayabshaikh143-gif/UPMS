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


## 14. Graphify + lazy-senior-dev (2026-11-14)

Installed Graphify 0.9.108 (PyPI `graphifyy`, Apache-2.0) with `uv`;
registered its skill and built `graphify-out/` (code-only): 459 nodes, 953
edges, 19 communities from 12 code files. Docs/images need an LLM key. The
owner asked to keep `graphify-out/` in the repo (not gitignored) and to map
upcoming steps through Graphify with `graphify update .` after each module.

Built `lazy-senior-dev`, our trimmed adaptation of Ponytail (MIT,
DietrichGebert/ponytail): smallest-complete-change ladder, scope mapping,
safety carve-outs, review/audit checklists, levels lite/full/ultra. Installed
globally and copied to `skills/lazy-senior-dev/` with an ATTRIBUTION file.


## 15. lazy-senior-dev disabled (2026-11-14)

Owner decided not to use the Ponytail-derived `lazy-senior-dev` skill. Its
global folder was moved to `~/.config/opencode/disabled-skills/` and the repo
copy was moved out of the project, so it is no longer loaded. Documentation was
updated to mark it disabled. Nothing else was deleted.

## 16. Skill sync rule + UI/UX skills (2026-11-14)

Added AGENTS.md rule 6: every skill the agent installs or adopts is also copied
into `skills/<name>/` in the repo, with attribution and the upstream license.

Installed four design skills and synced them into the repo: `hallmark` 2.0.0
(MIT), `impeccable` 4.9.1 (Apache 2.0), `ui-ux-pro-max` 2.15.0, plus
`graphify` 0.9.108 and `find-skills` (Vercel Labs, MIT). `ui-ux-pro-max`'s
search paths were patched to the local skill folder. README and skills table
updated.


## 17. grilling / grill-me installed (2026-11-14)

Installed Matt Pocock's `grilling` and `grill-me` skills (via npx skills),
synced into `skills/` with attribution. `grilling` runs a design-tree
interview in rounds and is the tool to use before implementation decisions.

## 18. Slice 1 — backend foundation (2026-11-14)

Stack confirmed: .NET 8 (installed SDK), React 19 + Vite later, EF Core 8, SQL
Server LocalDB (MSSQLLocalDB). Repo layout: `apps/api` and `apps/web`.

Built `apps/api`: ASP.NET Core Web API, Identity + JWT (roles Admin,
Passenger, Staff, Vendor), EF Core entities for Route, Stop, RouteStop, Vendor,
Bus, Trip, LocalDB database `UPMS`, migration `InitialCreate`, demo seed
(route JS-01, 4 stops, 2 buses, vendor, 2 trips) and admin seed
`admin@upms.local`. Endpoints for auth and catalog CRUD are live.

Verified: build clean, migration applied, `/health` ok, admin login returns
JWT, routes/trips seeded, passenger registration works, anonymous POST -> 401,
passenger POST -> 403, admin POST -> 201. Graphify graph refreshed:
2441 nodes / 6023 edges / 93 communities.

## 19. Slice 1 - React web app (2026-11-14)

Built `apps/web` (React 19 + Vite + TypeScript + Tailwind v4) on top of the
Slice 1 API:

- Dependencies: `react-router-dom`, `@tanstack/react-query`, `lucide-react`,
  `tailwindcss` + `@tailwindcss/vite`.
- Navy/teal design tokens in `src/index.css`; shared UI primitives in
  `src/components/ui.tsx`; `AuthContext` with JWT persistence and session
  restore; dev proxy `/api` -> `http://localhost:5080`.
- Pages: login, dashboard, routes, stops, vendors, buses, trips. Create/delete
  actions are role-gated to Admin in the UI; the API still enforces roles.
- `npm run build` passes. Playwright verified login -> dashboard, routes list,
  stop create (`Sehwan Adda`), vendor create (`Mehran Coach Service`), bus create
  (`SND-4411`, vendor-linked), and trip create/delete. Screenshots:
  `upms-login.png`, `upms-dashboard.png`, `upms-routes.png` (repo root, not
  committed).
- Fixed a time drift found during testing: `datetime-local` values were
  converted to UTC before saving, then displayed as local time, shifting the
  schedule by the PKT offset. The picker value is now sent unchanged as a
  wall-clock time, and dates display with `dateStyle: medium` /
  `timeStyle: short`.
- Improved API error messages (ProblemDetails `title`/`detail`/field errors) and
  added confirmation prompts before route/trip deletion.
- Duplicate route codes and bus registrations now return `409 Conflict` with a
  readable message instead of a database `500`; bus create/update also checks
  that a selected vendor exists and that vendor-owned buses have a vendor.
- Docs updated: root `README.md`, `apps/web/README.md`, `project-status.md`.
- Graphify master graph refreshed after the web app: 2608 nodes / 6453 edges /
  97 communities.

Committed and pushed as `3c22d1c` after owner approval. Test rows are kept as
demo data, and all screenshots stay local (gitignored) by owner request.

## 20. UI feedback - brand size and journey details (2026-11-14)

Owner asked to make the UPMS brand larger and show where each upcoming trip
travels from and to.

- Sidebar brand enlarged from small uppercase text to a 24px `UPMS` wordmark
  with a navy graduation-cap tile; mobile header and login wordmark also
  enlarged.
- `TripDto` now carries `routeName`, `origin` and `destination`. The API derives
  origin/destination from the first and last ordered stops on the route, so
  routes without stops fall back to the route name.
- Dashboard upcoming trips and the Trips page now show a `Journey` column
  (`origin → destination`) plus the route name under the code.
- Verified live against LocalDB; `npm run build` and `dotnet build` both pass.
- Graphify refreshed: 2608 nodes / 6453 edges / 97 communities.
- Screenshots refreshed in `docs/screenshots/`.
- Test/demo routes currently in LocalDB: `JS-01`, `JS-02`, `TEST-01`.

## 21. Route-based stops and edit actions (2026-11-14)

Owner feedback: the flat stops table with coordinates was wrong. Stops should
appear under the trip route, between origin and destination, and every catalog
section needs an edit action.

- Stops page rebuilt as route cards. Each card shows the route's ordered stops
  as an origin-to-destination timeline with `Origin` / `Destination` badges and
  cumulative minutes. Latitude/longitude inputs and columns are gone.
- New API endpoints: `POST /api/routes/{id}/stops/by-name` creates/reuses a stop
  and appends it with the next sequence and +15 minute offset; `DELETE
  /api/routes/{id}/stops/{routeStopId}` removes a stop from a route.
- Admins can add a stop by name and remove a stop from a route, with
  confirmation. Changes invalidate the route list, route-stops board and trips.
- Edit actions added to Routes, Vendors, Buses and Trips. Each row has a pencil
  button that opens the shared form prefilled and submits `PUT`; create still
  uses `POST`.
- Trip journey falls back to `Stops not set` when a route has no ordered stops,
  instead of repeating the route name.
- Verified: add/remove route stops on JS-02, edit round-trips for a route,
  vendor, bus and trip; web and API builds pass.
- Graphify refreshed: 2629 nodes / 6486 edges / 101 communities.
- Committed and pushed as `3c22d1c`; screenshots stay local and gitignored.
- Screenshots refreshed in `docs/screenshots/`.

## 22. Slice 2 - booking and capacity (2026-11-14)

Owner asked to move to the next step. Implemented Slice 2 around the SRS
booking rules.

Backend:

- `Trip` now has `BusId`/`Bus`; trip DTOs expose bus registration, capacity,
  booked count and remaining seats. Existing trips are auto-assigned to the
  first bus on startup if they have no bus.
- `Booking` entity, statuses (`Reserved`, `Confirmed`, `Cancelled`, `Expired`,
  `Boarded`), payment methods (`Online`, `Cash`) and payment statuses
  (`Pending`, `Paid`, `Refunded`, `Failed`).
- Migration `AddBookings` applied to LocalDB. Seed passenger added:
  `student@upms.local` / `Student@123`.
- `BookingService` creates bookings inside a `Serializable` transaction so the
  capacity check and insert are atomic; online payment is simulated and confirms
  immediately, cash stays `Reserved`/`Pending`.
- Cancellation closes 20 minutes before departure for passengers; Admin can
  override. Online cancellations are marked `Refunded` (simulated).
- `BookingExpiryService` sweeps unboarded cash bookings 5 minutes before
  departure and marks them `Expired`, releasing capacity.
- Endpoints: `POST /api/bookings`, `GET /api/bookings/my`,
  `GET /api/bookings` (Admin/Staff), `GET /api/bookings/{id}`,
  `POST /api/bookings/{id}/cancel`. Trips now require an assigned bus.
- Trip deletion is blocked when bookings exist.

Frontend:

- Passenger registration page; login now routes admins to the dashboard and
  passengers to `/book`.
- Role-aware navigation. Passengers see `Book a trip` and `My bookings`; admins
  see the catalog plus `Bookings`.
- `Book` shows upcoming trips with journey, bus, fare, occupancy and seats left,
  with an inline booking panel for online/cash.
- `My bookings` shows reference, trip, payment, status and cancellation.
- `Bookings` admin page lists all reservations and passenger details.
- Trip form now selects a bus; trip table shows bus and `left / booked` seats.
  Dashboard adds bookings count and seats column.

Verified live:

- Online booking -> capacity 1/45; cash booking -> `Reserved`/`Pending`;
  cancellation -> `Cancelled`/`Refunded` and capacity released.
- Concurrency: 5 parallel bookings against capacity 1 -> one `201`, four `409`.
- Cash no-show: trip moved to departure + 4 minutes, cash booking expired by the
  background sweep, capacity restored; trip time restored afterwards.
- Web and API builds pass; admin/passenger screenshots refreshed locally.
- Graphify refreshed: 2730 nodes / 6836 edges / 112 communities.
- Committed and pushed as `89d4a1e` after owner approval. Screenshots and the
  LocalDB test rows stay out of the repository.

## 23. Ticket receipt modal (2026-11-15)

Owner asked: after booking, clicking `Show ticket` should show a receipt.

- `My bookings` now has a `Show ticket` action beside `Cancel`.
- Added `TicketReceipt` modal: booking reference, passenger, route/journey,
  departure, bus, payment method/status, fare, status and booked timestamp.
- Receipt has a `Print receipt` action and print-only CSS; Escape or backdrop
  click closes it, and background scrolling is locked while open.
- Moved booking status/payment labels into `lib/bookingLabels.ts` so My bookings,
  admin bookings and the receipt stay consistent.
- Verified the modal in Playwright; print media shows only the receipt on one
  page. Screenshot: `docs/screenshots/13-ticket-receipt.png`.
- `npm run build` passes. Committed and pushed as `89d4a1e` after owner approval.
- Graphify refreshed after the receipt change: 2730 nodes / 6836 edges / 112
  communities.

## 24. Requirements and design review (2026-11-15)

Owner asked for a full review of the SRS, diagrams and current implementation,
with any requirement conflicts or discrepancies reported before fixing them.

- Reviewed SRS v1.0, 2 use case diagrams, 6 activity, 6 sequence, 4 class and
  8 ER diagrams, plus the Slice 1–2 code.
- Wrote `docs/UPMS_Requirements_Review_2026-11-15.md` with prioritised findings,
  diagram/document inconsistencies, missing deliverables, planned scope and a
  decision checklist.
- Highest-priority items: undefined timezone/schedule semantics; trip
  cancellation not cascading to bookings/refunds; missing
  `TripVehicleAssignment` history; missing vendor/staff identity and trip
  assignment links; missing offline-ticket/passenger phone data; missing
  Payment/Refund/AuditLog records; passengers cannot browse intermediate stops.
- No SRS, diagram or code change has been made yet. Report is uncommitted and
  awaiting owner decisions.

## 25. SRS v1.1 + diagram alignment (2026-11-17)

Owner approved the suggested next order and confirmed three changes to the
proposed defaults: boarding starts 20 minutes before departure, online payment
shows Pending → Approved, and vendor-operated bus fares are retained by the
vendor who pays the university a monthly subscription.

- Created `docs/UPMS_Step_2_SRS_v1.1.docx` (v1.0 retained as history):
  - document control upgraded to v1.1 implementation baseline;
  - FR-27..FR-32 added for timezone, payment statuses, vendor settlement, bus
    active status, trip-cancellation cascade and audit logging;
  - BR-11..BR-16 and §6.5 vendor settlement added;
  - §7 data list updated (VendorSettlement, Payment statuses, CheckIn method,
    Notification links, phone, TripVehicleAssignment reason);
  - §10 old open decisions replaced by resolved decisions and remaining risks;
  - §11 stack marked approved;
  - §13 decision log added.
- Updated selected diagrams: use case subtitles to SRS v1.1, class Trip capacity
  annotated from Bus, Payment approvedAt, RouteStop.isPickup removed, booking
  activity Pending → Approved, cancellation full refund, no-show T-20/T-5,
  sequence cancellation full refund.
- Regenerated affected SVGs/PNGs with the bundled renderers.
- Added `UPMS_ERD_v1.1_Additions` (HTML/SVG/PNG) showing VendorSettlement,
  Vendor user link, Bus.Active, Payment/Refund fields, TripStaffAssignment,
  Notification links and AuditLog fields; linked it from the complete ERD.
- Remaining structural diagram/code alignment is tracked in
  `docs/UPMS_Requirements_Review_2026-11-15.md`.

## 26. SRS v1.1 code alignment - phase 1 (2026-11-17)

Started the high-priority code alignment from the review.

- Added `BookingOptions` + `IClock`: Asia/Karachi wall-clock, cancellation 20,
  cash no-show 5, boarding start 20, online hold 10 minutes. API now reads
  `Bookings:*` from `appsettings.json`; BookingService and expiry use the clock.
- Added `Bus.Active` with `AddSrsV11Alignment` and `BusActiveDefault` migrations
  (existing buses set active). Trip create/update rejects inactive buses; bus
  update blocks capacity below active bookings; admin Buses page has status.
- Added `Trip.CancelledAt` + `CancellationReason`. Updating a trip to Cancelled
  cascades active bookings to Cancelled/Refunded and records the reason; restoring
  the trip clears those fields. Trips page shows a reason field when Cancelled.
- `TripDto` now includes ordered `stops`, `cancelledAt` and
  `cancellationReason`; passenger Book page has a search box and expandable
  stop lists.
- Seed now ensures two upcoming demo trips exist when the seeded dates pass.
- Verified: trip cancellation cascade (booking status 2, payment 2, reason
  saved, trip restored), inactive bus rejected with 400, future trips visible,
  trip stops/search in the passenger UI, web/API builds pass.
- Pending: separate Payment/Refund/AuditLog records, VendorSettlement + Vendor
  user link, staff/trip assignment, Notification trip/booking links, phone
  capture and offline-ticket model.
- Graphify refreshed: 2764 nodes / 6931 edges / 111 communities.

## 27. SRS v1.1 code alignment - phase 2 (2026-11-17)

Added the remaining data-model pieces and wired the booking ledger.

- New entities: `Payment`, `Refund`, `AuditLog`, `VendorSettlement`,
  `StaffProfile`, `TripStaffAssignment`, `Notification`, `OfflineTicket`.
- Vendor gained `UserId`, contact, monthly subscription and subscription status;
  registration now stores an optional phone number.
- `PaymentStatus` gained `Approved` (4). Existing online paid bookings were
  migrated to Approved; cash uses Paid.
- Migration `AddPaymentAuditVendorPhase2` created the tables and backfilled
  Payments/Refunds from the existing Booking snapshot.
- BookingService now creates Payment rows, writes audit actions, creates Refund
  records on cancellation and updates payment status. Trip cancellation cascades
  bookings/refunds and creates in-app notifications for affected passengers.
- Added APIs: vendor settlements (`GET/POST /api/vendors/{id}/settlements`),
  my notifications + mark read, and admin audit list.
- Verified: backfill (10 payments / 5 refunds), new booking creates
  BookingCreated + PaymentApproved audit rows, cancel creates a Refund and
  BookingCancelled/RefundProcessed rows, vendor settlement create/list works,
  trip cancellation creates notifications and TripCancelled audit.
- Pending: staff/trip assignment endpoints/UI, offline-ticket issuance flow,
  notification UI, vendor/staff portals and broader audit coverage.
- Graphify refreshed: 2920 nodes / 7245 edges / 117 communities.
- Committed as `13a3dc1` on `main` and pushed.

## 28. Vendor self-registration and portal (2026-11-17)

- Register page now offers **Student** or **Vendor**.
- `POST /api/auth/register-vendor` creates a `Vendor` login plus a `Pending`
  vendor application in one transaction (`organizationName`, `contactPerson`,
  `email`, `password`, `phone`). Status/subscription are hardcoded, never taken
  from the client.
- Admin Vendors page now shows contact, and pending rows get one-click
  approve/reject actions. The vendor account link (`userId`) is preserved on
  update.
- New vendor endpoints: `GET /api/vendors/me`, `/me/buses`, `/me/trips`,
  `/me/settlements`. The buses/trips/settlements endpoints return `403` until
  the application is approved.
- New `/vendor` portal: pending/rejected status screen, then buses, assigned
  trips and settlement tables after approval. Vendors are routed to `/vendor`
  after login; the sidebar shows only the vendor portal.
- Seed: demo vendor login `vendor@upms.local` / `Vendor@123` linked to Indus
  Transport Services; one upcoming trip is seeded on the vendor bus for the
  portal demo. `Vendors` list is now Admin/Staff only.
- Verified: vendor registration returns the Vendor role and Pending status;
  pending `/me/buses` returns 403; admin approval unlocks the portal; approved
  applicant can still sign in; demo vendor shows 1 bus, 1 trip, 1 settlement.
- Screenshots: `docs/screenshots/21-vendor-register.png`,
  `22-vendor-pending.png`, `23-admin-vendor-applications.png`,
  `24-admin-vendor-approved.png`, `25-vendor-portal.png`.
- Pending: vendor bus maintenance, staff portal, offline tickets, notification UI
  and broader admin audit coverage.
- Committed as `ae869f8` on `main`.

## 29. Staff boarding console (2026-11-17)

- Register toggle label changed from Student to Passenger (role was already
  `Passenger`).
- New Staff endpoints (Staff/Admin only):
  `GET /api/staff/trips`, `GET /api/staff/trips/{id}/passengers`,
  `POST .../bookings/{id}/board`, `POST .../bookings/{id}/no-show`,
  `POST .../offline-tickets`, `POST .../offline-tickets/{id}/board`.
- Staff console at `/staff`: trip list with boarding state, merged booking +
  offline-ticket passenger list, board/no-show actions, offline cash ticket form
  and refresh. Staff are routed to `/staff` and see only the Boarding nav item.
- Boarding rules: opens 20 minutes before departure; cash bookings marked
  boarded are recorded as `Paid`; no-show sets `Expired`; boarding/offline
  actions write `AuditLogs` entries.
- Offline tickets count toward capacity everywhere (booking create, trip DTOs,
  vendor portal, staff console). Trip cancellation cancels/refunds open offline
  tickets and audits each one.
- Seed: `staff@upms.local` / `Staff@123`, `StaffProfile` EMP-001 Boarding Staff.
- Verified: passenger role gets 403 on staff APIs; early boarding returns 400;
  online + cash bookings boarded; offline ticket issued/boarded; no-show expires;
  capacity-1 bus rejects a booking after an offline ticket (409); trip
  cancellation flips offline ticket to Cancelled/Refunded; API and web builds pass.
- Screenshots: `docs/screenshots/26-staff-console.png`,
  `27-staff-boarding-actions.png`, `28-staff-offline-issued.png`; register
  screenshot `21-vendor-register.png` refreshed for the Passenger label.
- Local QA data created for verification: near-term trip, cash/online bookings,
  a `QA-CAP-1` bus and its cancelled capacity test trip.
- Pending: admin staff management, trip/staff assignment UI, vendor bus
  maintenance, notification UI, incidents/reports and broader audit coverage.
- Committed as `c3366a4` on `main`.





