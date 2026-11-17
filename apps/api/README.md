# UPMS API

ASP.NET Core 8 Web API + EF Core 8 + SQL Server LocalDB.

## Run

```powershell
dotnet run --project apps/api/src/UPMS.Api --urls http://localhost:5080
```

- Swagger: http://localhost:5080/swagger
- Health: http://localhost:5080/health

The app applies migrations and seeds demo data on startup.

## Seeded accounts

| Role | Email | Password |
|---|---|---|
| Admin | `admin@upms.local` | `Admin@12345` |
| Passenger | `student@upms.local` | `Student@123` |
| Vendor | `vendor@upms.local` | `Vendor@123` |
| Staff | `staff@upms.local` | `Staff@123` |

Change the seed values in `appsettings.json` before any real deployment.

## Slice 1 endpoints

| Method | Route | Access |
|---|---|---|
| POST | `/api/auth/register` | public (creates Passenger) |
| POST | `/api/auth/register-vendor` | public (creates a pending Vendor application) |
| POST | `/api/auth/login` | public |
| GET | `/api/auth/me` | authenticated |
| GET | `/api/routes`, `/api/routes/{id}` | authenticated |
| POST/PUT/DELETE | `/api/routes` | Admin |
| POST | `/api/routes/{id}/stops` | Admin |
| POST | `/api/routes/{id}/stops/by-name` | Admin — creates/reuses a stop and appends it |
| DELETE | `/api/routes/{id}/stops/{routeStopId}` | Admin |
| GET | `/api/stops` | authenticated |
| POST/PUT/DELETE | `/api/stops` | Admin |
| GET | `/api/vendors` | Admin, Staff |
| POST/PUT | `/api/vendors` | Admin |
| GET | `/api/vendors/me` | Vendor (own application/profile) |
| GET | `/api/vendors/me/buses` | Vendor (approved only) |
| GET | `/api/vendors/me/trips` | Vendor (approved only) |
| GET | `/api/vendors/me/settlements` | Vendor (approved only) |
| GET | `/api/buses` | authenticated |
| POST/PUT/DELETE | `/api/buses` | Admin |
| GET | `/api/trips?routeId=` | authenticated |
| POST/PUT/DELETE | `/api/trips` | Admin |
| POST | `/api/bookings` | authenticated (creates a booking for the current user) |
| GET | `/api/bookings/my` | authenticated (own bookings) |
| GET | `/api/bookings` | Admin, Staff |
| GET | `/api/bookings/{id}` | owner, Admin or Staff |
| POST | `/api/bookings/{id}/cancel` | owner or Admin |
| GET | `/api/notifications/my` | authenticated (own notifications) |
| POST | `/api/notifications/{id}/read` | authenticated (own notification) |
| GET | `/api/audit?take=` | Admin |
| GET | `/api/vendors/{id}/settlements` | Admin, Staff |
| POST | `/api/vendors/{id}/settlements` | Admin |
| GET | `/api/staff/trips` | Staff, Admin |
| GET | `/api/staff/trips/{id}/passengers` | Staff, Admin |
| POST | `/api/staff/trips/{tripId}/bookings/{bookingId}/board` | Staff, Admin |
| POST | `/api/staff/trips/{tripId}/bookings/{bookingId}/no-show` | Staff, Admin |
| POST | `/api/staff/trips/{tripId}/offline-tickets` | Staff, Admin |
| POST | `/api/staff/trips/{tripId}/offline-tickets/{ticketId}/board` | Staff, Admin |

Catalog writes validate references and uniqueness: duplicate route codes or bus
registrations return `409 Conflict` with a readable `message`, missing routes or
vendors return `400 Bad Request`, and admin-only endpoints return `403` for
other roles.

Trip responses include the route name and the journey endpoints (`origin` and
`destination`), derived from the route's first and last ordered stops. They also
include the ordered `stops` list and cancellation fields (`cancelledAt`,
`cancellationReason`).

Trips require an assigned bus. Capacity and remaining seats come from that bus,
minus active bookings (`Reserved`, `Confirmed`, `Boarded`). Inactive buses cannot
be assigned to a trip. Booking creation runs in a serializable transaction so the
last seat cannot be double-booked.

Cancelling a trip through `PUT /api/trips/{id}` with `status = Cancelled` also
cancels its active bookings, marks paid bookings refunded and records the reason.
The same update can restore a cancelled trip; this clears the cancellation fields.

Time zone and cutoff settings live in `appsettings.json` under `Bookings`:

```json
"Bookings": {
  "TimeZoneId": "Asia/Karachi",
  "CancellationCutoffMinutes": 20,
  "CashNoShowMinutes": 5,
  "BoardingStartMinutes": 20,
  "OnlinePaymentHoldMinutes": 10
}
```

Booking rules in this slice:

- One booking reserves one seat; no numbered seat selection.
- Online payment uses a demonstration gateway and confirms immediately.
- Cash bookings start as `Reserved` with payment `Pending`.
- Unboarded cash bookings expire 5 minutes before departure (background sweep).
- Passenger cancellation is allowed until 20 minutes before departure.
- Online cancellations mark the payment as `Refunded` (simulated).
- Boarding opens 20 minutes before departure by default (configurable).
- Payments and refunds are written to separate `Payments` and `Refunds` records;
  `Bookings` keeps a payment snapshot for quick reads.
- Online approval uses `PaymentStatus.Approved` (4); cash collection uses `Paid`.
- Boarding starts 20 minutes before departure. Staff can mark a booking `Boarded`
  or `Expired` (no-show); boarding a cash booking records payment as `Paid`.
- Offline cash tickets capture passenger name and phone, are written to
  `OfflineTickets`, and count toward trip capacity alongside bookings.
- Trip cancellation also cancels/refunds open offline tickets.
- Booking, payment, cancellation and trip-cancellation actions are written to
  `AuditLogs`; boarding and offline-ticket actions are audited too.
- Trip cancellation creates in-app `Notifications` for affected passengers.
- Vendors have an optional user link, contact, monthly subscription and
  settlement records.
- Vendor self-registration creates a `Vendor` login and a `Pending` vendor
  application. Admin approval unlocks the vendor portal (`/vendor`): own buses,
  assigned trips and settlements.
- Staff profiles and trip/staff assignments are modelled; the staff console
  currently shows all trips. Admin staff management and assignment enforcement
  are follow-ups.
- Registration accepts an optional phone number for later offline-ticket use.

## Database

Connection string: `Server=(localdb)\MSSQLLocalDB;Database=UPMS;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True`

Migrations:

```powershell
dotnet ef migrations add <Name> --project apps/api/src/UPMS.Api --startup-project apps/api/src/UPMS.Api
dotnet ef database update --project apps/api/src/UPMS.Api --startup-project apps/api/src/UPMS.Api
```

## Project layout

```
apps/api/
  UPMS.sln
  src/UPMS.Api/
    Auth/       JWT options, token service, auth controller
    Catalog/    routes, stops, vendors, buses, trips controllers + DTOs
    Data/       entities, DbContext, seed
    Migrations/
    Program.cs
```
