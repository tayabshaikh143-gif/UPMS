# UPMS requirements and design review

Date: 2026-11-15  
Scope: SRS v1.0, use case/activity/class/sequence/ER diagrams, and the Slice 1–2
implementation. This review informed SRS v1.1; see §10 and §13 of the SRS for the
final decisions.

## 1. Short assessment

The project is coherent: the SRS, diagrams and implementation all describe a
single-university transport booking system with capacity-based tickets, online
or cash payment, cancellation, no-show expiry, offline sales, incidents and
replacement buses.

The main issue is not the overall direction. It is that the written requirements
and diagrams are still at **v1.0 draft** while Slices 1–2 have already made
several implementation decisions. Some planned entities are also missing from
the diagrams, and a few implementation choices conflict with the written model.
Most of the missing features are expected later slices, not defects.

## 2. Findings that need a decision

| ID | Severity | Finding | Evidence | Recommendation |
|---|---|---|---|---|
| R1 | High | **Time zone and schedule semantics are undefined.** The code stores `DateTime` without an offset and uses `DateTime.Now`; the frontend sends the wall-clock value from `datetime-local`. This works on one machine in Pakistan but breaks if the API runs in UTC or another region. | SRS §10 item 4 explicitly asks to define the timezone; `BookingService`, `BookingExpiryService`, `TripsPage` use wall-clock values. | Decide: store UTC + display `Asia/Karachi`, or explicitly store `Asia/Karachi` wall-clock. Put the choice in config and test cancellation/no-show around the cutoff. |
| R2 | High | **Trip cancellation does not cascade to bookings.** An admin can set a trip to `Cancelled`, but existing bookings stay `Confirmed`/`Paid`; there is no refund record, alternate transport or notification. | SRS FR-23, BR-10 and acceptance criteria require this. `TripsController.Update` only changes `Trip.Status`. | Add a trip-cancellation workflow (cancel/refund affected bookings, record reason, notify) or block cancelling a trip with active bookings until that workflow exists. |
| R3 | High | **Replacement-bus history is not modelled.** The SRS and diagrams define `TripVehicleAssignment` with `assigned_at`, `replaced_at` and a reason. The code only stores `Trip.BusId`. | SRS §7; class `Network`; ERD `Network`/`Complete`. | Add `TripVehicleAssignment` (or an assignment history table) before implementing FR-21/22. Keep `Trip.BusId` only as the current-assignment shortcut if needed. |
| R4 | High | **Vendor and staff identities are not linked to operational records.** `Vendor` has no user link, and there is no staff profile or trip/staff assignment. The roles exist in Identity, but a vendor cannot log in to “their” buses/trips and staff cannot see “assigned” trips. | SRS role table, FR-18/26, §7 “Driver/Staff … and assignment”; class `People`; ERD `Complete`; code has only Identity roles. | Add `Vendor.UserId`, a `StaffProfile`, and `TripStaffAssignment` (or `Trip.StaffId`) before the vendor/staff portals. |
| R5 | High | **Offline ticket data and passenger phone are missing.** The SRS leaves the minimum offline-ticket fields open, but the diagrams already show a passenger phone. The code has no phone and no offline-ticket/issuer model. | SRS §10 item 3, FR-14; class/ERD `Passenger`/`PassengerProfile`; code `AppUser` has only name/email. | Decide the minimum (recommended: name + phone), add phone to the passenger profile, and add an offline-ticket record with issuer and timestamp before Slice 3. |
| R6 | High | **Payment, refund and audit records are simplified away.** The SRS/diagrams define separate `Payment`, `Refund` and `AuditLog` entities. The code keeps payment method/status/amount on `Booking`, has no refund record and no audit table. | SRS §7, NFR-08; class `Booking`/`Operations`; ERD `Booking`/`Complete`; code `Booking` fields. | Decide whether the MVP keeps embedded payment fields (then update the SRS/diagrams), or add `Payment` + `Refund` + `AuditLog` before real payments/offline sales. At minimum add audit logging for booking, cancellation, capacity release and payment changes. |
| R7 | Medium | **Passengers cannot browse intermediate stops or search trips.** FR-02 asks passengers to browse routes, stops, trip dates and capacity. The booking page lists trips with only origin → destination; the route-stops timeline is admin-only. | SRS FR-02, §6.1 “searches for a route/trip”; `Book.tsx`, `Stops.tsx`. | Add an expandable “View stops” section on each booking card and basic route/date filters, or change the SRS to say passengers browse trips only. |
| R8 | Medium | **Bus active status is missing.** The SRS entity list includes an active status for buses; the code has no `Active` field, so an inactive/retired bus can still be assigned to a trip. | SRS §7 `Bus`; `Entities.cs`; `BusesController`. | Add `Bus.Active`; hide/block inactive buses in trip assignment and booking. |
| R9 | Medium | **Boarding start is not configurable, and cutoffs are duplicated.** FR-11 requires a configurable boarding start; the code has no boarding process. The 20-minute cancellation and 5-minute cash cutoffs are hardcoded in the API and repeated in the UI. | SRS FR-11/FR-12 and §10 item 2; `BookingService`, `Book.tsx`, `MyBookings.tsx`. | Add configuration for `CancellationCutoffMinutes`, `CashNoShowMinutes` and `BoardingStartMinutes`; read the same values in API and UI. |
| R10 | Medium | **“One booking = one seat” is an assumption, not a stated rule.** The SRS says a booking is a passenger’s reservation for one trip but does not say whether one booking can buy multiple tickets. | SRS §1.4/§5 BR-02; code creates exactly one seat per booking. | Confirm and document: one seat per booking for MVP (recommended), or add a quantity/group-booking field. |
| R11 | Low | **Admin cancellation override is undocumented.** Admin can cancel after the 20-minute cutoff; BR-03 only closes *self-service* cancellation, so this may be intended, but it is not written. | SRS BR-03; `BookingService.CancelAsync` uses `isAdmin` to bypass the cutoff. | Document “Admin may cancel after cutoff for operational reasons” or remove the override. |
| R12 | Low | **Trip capacity has two sources of truth in the diagrams.** The class diagram gives `Trip.capacity`, while the ERD says capacity belongs to `Bus` and the code derives it from the assigned bus. | Class `Booking`/`Operations`; ERD `Network`; SRS BR-01. | Use bus capacity as the source of truth (matches BR-01) and remove/annotate `Trip.capacity` in the class diagrams. |

## 3. Diagram and document inconsistencies

1. **`RouteStop.isPickup` exists only in the class `Network` diagram.** It is not
   in the SRS, the ERD or the code. Either add pickup/drop flags everywhere or
   remove the field.
2. **`Passenger` / `PassengerProfile` are duplicated.** The ERD has a `Passenger`
   entity and a `PassengerProfile`; the class model has `PassengerProfile`; the
   code uses one `AppUser`. Decide whether phone/profile data lives on the user
   or a separate profile, and make all three views match.
3. **The User role is a single enum in the ERD but multi-role in Identity.** The
   code supports several roles per user. Update the ERD to a `UserRole` join
   table, or state that only the primary role is shown in reports.
4. **`Notification` is linked only to `incident_id`.** FR-19 also requires delay,
   cancellation and replacement-bus notices. Add optional `trip_id`/`booking_id`
   links or a generic related-entity reference.
5. **`Payment` and `Refund` fields are incomplete.** The SRS asks for method,
   status, transaction reference and recorded-by staff; the ERD ships only
   `amount`/`paid_at` (and `Refund` lacks a status). Align the ERD with the SRS
   before payment/report work.
6. **`CheckIn` is missing its method field** in the ERD although the class diagram
   has `CheckInMethod`.
7. **`TripVehicleAssignment` has `reason` only in the class diagram**, not in the
   ERD.
8. **SRS still says the technology stack is “not yet approved.”** The stack is
   confirmed and running (React 19, ASP.NET Core 8, EF Core 8, SQL Server
   LocalDB). Update SRS §11.
9. **The SRS is still a v1.0 draft** while implementation has decided several
   open items (mock gateway, one-seat booking, 5-minute expiry, bus capacity).
   Add a decision log and update the SRS to v1.1.

## 4. Missing deliverables

- Step 3 detailed use case descriptions `.docx` (the diagram exists; the
  descriptions are still pending).
- Step 4 written relational schema (tables, types, keys, indexes).
- Step 6 architecture and UI/UX wireframes.
- Step 1 project proposal in the repository.
- Test plan/test cases mapped to the SRS acceptance criteria.

## 5. Planned but not implemented (not defects)

These are in the SRS and diagrams but belong to later slices:

- Staff boarding/check-in and “boarded once” enforcement.
- Offline ticket issuance, cash recording and issuing-staff data.
- Delay/breakdown incidents and replacement-bus workflow.
- In-app notifications.
- Operational reports.
- Vendor application/portal and staff portal.
- Administrator user management and payment records.
- Real payment gateway (current gateway is clearly labelled as a demo).
- QR ticket (the unique booking reference satisfies FR-08’s “reference **or**
  QR” wording).
- Audit-log coverage and backup procedure.

## 6. Recommended order

1. Decide R1 (timezone), R5 (offline-ticket fields) and R10 (one seat per
   booking). These block Slice 3.
2. Add R2 (trip cancellation/refund) and R3 (replacement-bus history) before the
   incidents/replacement slice.
3. Add R4 (vendor/staff links), R6 (payment/refund/audit) and R8 (bus active)
   before opening vendor/staff portals.
4. Update SRS v1.1 and the diagrams to match the decisions (R12, RouteStop
   pickup flag, Passenger/Profile, role model, Payment/Refund fields).
5. Add the missing deliverables (use case descriptions, schema document,
   wireframes, test plan) in parallel with Slice 3.

## 7. Decisions to confirm

- [ ] Timezone: UTC storage + PKT display, or explicit `Asia/Karachi`
      wall-clock?
- [ ] Online cancellation refund: full refund until the 20-minute cutoff, no
      fee, mock provider?
- [ ] Offline ticket minimum: name + phone (recommended), or more?
- [ ] Boarding start: configurable default (e.g. 30 minutes before departure)
      with cash cutoff fixed at 5 minutes?
- [ ] One seat per booking for MVP?
- [ ] Payment/refund model: keep embedded for MVP, or add separate
      `Payment`/`Refund` tables now?
- [ ] Should Admin be allowed to cancel after the passenger cutoff?
- [ ] Should the SRS/diagrams be updated first, or code first for the next
      slice?
