# Project status — UPMS

Last updated: 2026-11-17

## What this project is

University Point Management System (UPMS): a web-based university transport
platform for route/trip management, advance ticket booking, online payment,
cash-at-boarding, capacity control, offline ticket issuance, and incident
handling. Initial target: University of Sindh, Jamshoro (single-university MVP).

## Delivery steps

| Step | Deliverable | Status |
|---|---|---|
| 1 | Project proposal | drafted earlier; file not in this repo yet |
| 2 | SRS v1.1 | done — `docs/UPMS_Step_2_SRS_v1.1.docx` (v1.0 retained as history) |
| 3 | Use case diagram + detailed descriptions | diagrams done (simple + detailed); `.docx` of descriptions pending |
| 4 | ERD and relational database design | in progress — 8 ER diagrams (6 subject + table-style complete + Chen complete), schema document pending |
| 5 | Activity and sequence diagrams | done — 6 activity + 6 sequence diagrams |
| 6 | Architecture and UI/UX wireframes | pending |
| 7 | Technology stack confirmation | confirmed — React 19 + ASP.NET Core 8 + EF Core 8 + SQL Server LocalDB |
| 8 | Implementation and testing | in progress - Slices 1-2 + SRS v1.1 alignment + vendor onboarding + staff boarding |

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
| `13a3dc1` | feat: align UPMS with SRS v1.1 (payments, audit, vendor settlement) |
| `ae869f8` | feat(vendors): add self-registration, approval and vendor portal |

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
- Graphify master graph refreshed after phase 2: 2920 nodes / 7245 edges / 117
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
- SRS v1.1 created with the confirmed decisions and a decision log. Selected
  diagrams were aligned (SRS v1.1 label, capacity from bus, approved payment,
  20-minute boarding, full refund); structural diagram updates are captured in
  `UPMS_ERD_v1.1_Additions`.
- SRS v1.1 code alignment started: configurable timezone/cutoffs, `Bus.Active`,
  trip cancellation reason + booking cascade/refund status, trip stops in the
  DTO and passenger search/stops on the booking page.
- Phase 2 data model is in place: separate `Payments`, `Refunds` and `AuditLogs`;
  `VendorSettlement`, Vendor user/contact/subscription fields, `StaffProfile`,
  `TripStaffAssignment`, `Notification` and `OfflineTicket` entities. Existing
  bookings/payments were backfilled; online payments now show `Approved`.
- Booking create/cancel and trip cancellation write payment/refund/audit rows;
  trip cancellation creates in-app notifications. Vendor settlements, my
  notifications and the admin audit list have API endpoints.
- Registration accepts an optional phone number.
- Vendor self-registration is live: the register page offers Student or Vendor.
  Vendor applications start `Pending`; the admin Vendors page can approve/reject
  them in one click. Approved vendors get a `/vendor` portal with their own
  buses, assigned trips and settlements.
- Demo vendor login: `vendor@upms.local` / `Vendor@123` (linked to Indus
  Transport Services).
- Register toggle label is now Passenger instead of Student; the system role was
  already `Passenger`.
- Staff boarding console is live at `/staff`: trip list, merged passenger list,
  board/no-show, offline cash ticket issue/board, boarding-window enforcement
  and audit entries. Offline tickets count toward capacity, and trip
  cancellation cancels/refunds them.
- Demo staff login: `staff@upms.local` / `Staff@123` with a seeded
  `StaffProfile` (`EMP-001`, Boarding Staff).

## Decisions fixed in SRS v1.1

1. Online cancellation before the 20-minute cutoff: full refund, no fee; the
   demo gateway is simulated and administrator exceptions are recorded.
2. Boarding opens 20 minutes before departure by default (configurable); the
   cash no-show cutoff remains 5 minutes before departure.
3. Offline tickets capture passenger name + phone, issuing staff and timestamp.
4. All schedule times are Asia/Karachi wall-clock time; server/database session
   timezone must match. UTC migration is future work.
5. Online payment sessions have a 10-minute hold; the demo gateway confirms
   instantly.
6. Vendor-operated bus fares are retained by the vendor, who pays the university
   a monthly subscription. University-owned bus fares accrue to the university.
7. One booking reserves one seat for one passenger; group booking is out of scope.
8. Admin cancellation after the cutoff is allowed for operational reasons and is
   audited.
9. Notifications are in-app only for the MVP.
10. Separate Payment/Refund records and an audit log are implemented (phase 2);
    Booking keeps a payment snapshot for quick reads.

## Open decisions and risks

1. Real payment-provider integration and provider refund workflow.
2. Subscription collection/reporting and monthly settlement enforcement.
3. Timezone deployment outside Asia/Karachi (UTC migration).
4. Email/SMS/push notification delivery.
5. Detailed operational reports and audit-log retention.

## Known follow-ups

- Build the written relational schema for Step 4 (tables, types, keys, indexes).
- Create the Step 3 `.docx` with detailed use case descriptions.
- Move `UPMS_Chat_Context_Export.docx` into `docs/` once it is closed in Word.
- Add the Step 1 proposal to the repo.
- Architecture and UI/UX wireframes (Step 6).
- Reconcile remaining code with SRS v1.1: staff/trip assignment admin UI,
  notification UI, vendor bus maintenance, admin staff management and audit
  coverage for all admin actions.
- Slice 3 remaining: incidents/replacement bus and operational reports; staff
  boarding and offline ticket sales are implemented.
- Decide whether to remove demo/test rows and test bookings from the LocalDB
  database.
- Decide what to do with the 12 MB raw session export in `AI_docs/sessions/`
  (commit, keep local, or drop now that the digest exists).

## Session archive

- `AI_docs/sessions/2026-11-03_2026-11-15-session-e5e0c278-digest.md` — digest
  of the long build chat `ses_e5e0c2789ffeTpaTQFNjk2wr9K` (2026-11-03 →
  2026-11-15): 44 owner requests, deliverables, decisions, corrections, open
  items.
- `AI_docs/sessions/session-075912de-d724-4fdf-a17a-d91add73d749.md` — the raw
  export of that chat (~12 MB, untracked).
- The live session was moved into this project on 2026-11-15 with
  `session_move`; it now belongs to the UPMS project
  `21081e0fc24af1ae3063977eae4fab8023faada0`. A stale legacy row for the same
  session still sits under `project_id = global` in the old session table.
