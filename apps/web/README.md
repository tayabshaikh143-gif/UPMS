# UPMS Web

React frontend for the UPMS admin experience: sign in, dashboard, and catalog
management for routes, stops, vendors, buses and trips.

## Stack

- React 19 + TypeScript + Vite
- Tailwind CSS v4 (design tokens in `src/index.css`)
- React Router, TanStack Query, lucide-react

## Run locally

Start the API first (it listens on `http://localhost:5080`):

```powershell
dotnet run --project apps/api/src/UPMS.Api/UPMS.Api.csproj --urls http://localhost:5080
```

Then start the web app:

```powershell
cd apps/web
npm install
npm run dev
```

Open `http://127.0.0.1:5173`. The dev server proxies `/api` and `/health` to
the API, so no extra CORS setup is needed.

Demo administrator: `admin@upms.local` / `Admin@12345`.
Demo passenger: `student@upms.local` / `Student@123`.

## Scripts

| Command | Purpose |
|---|---|
| `npm run dev` | Vite dev server with HMR and API proxy |
| `npm run build` | Type-check (`tsc -b`) and production build |
| `npm run lint` | Oxlint |
| `npm run preview` | Preview the production build |

## Pages

| Route | Page | Notes |
|---|---|---|
| `/login` | Sign in | JWT stored in `localStorage`, restored on reload |
| `/register` | Register | Passenger account or vendor application; both sign in after submit |
| `/` | Dashboard | Admin catalog counts, bookings and upcoming trips |
| `/routes` | Routes | Admin create/edit/delete |
| `/stops` | Route stops | Ordered stops per route; admin add/remove by name |
| `/vendors` | Vendors | Admin create/edit, pending applications with approve/reject, contact and subscription |
| `/buses` | Buses | Admin create/edit, active/inactive status, vendor selection |
| `/trips` | Trips | Admin create/edit/delete, bus assignment, seat counts, cancellation reason |
| `/bookings` | Bookings | Admin list of all reservations |
| `/book` | Book a trip | Passenger trip cards with search, stops, online/cash booking |
| `/my-bookings` | My bookings | Passenger ticket references, status, printable receipt and cancellation |
| `/vendor` | Vendor portal | Vendor profile, approval status, own buses, assigned trips and settlements |
| `/staff` | Boarding console | Staff trip list, passenger check-in, no-show and offline cash tickets |

Passengers are routed to `/book` after login and see only the booking pages.
Vendors are routed to `/vendor`; pending or rejected applications see their
status instead of buses/trips/settlements. Staff are routed to `/staff` and see
only the boarding console. Admin-only pages are hidden from the sidebar and
guarded in the router; the API still enforces authorization on every request.

## Notes

- Trip departure times are entered and displayed as wall-clock schedule times.
  The picker value is sent to the API unchanged so the stored time matches what
  the admin typed.
- Stops are managed per route. Adding a stop only asks for its name; coordinates
  are not collected in this slice.
- Online payment is a demonstration gateway and confirms immediately. Cash
  bookings stay reserved until the passenger is marked boarded or the 5-minute
  pre-departure cutoff expires them.
- Boarding opens 20 minutes before departure by default; the cancellation and
  cash cutoffs are shared constants in `src/lib/bookingLabels.ts`.
- The frontend covers Slices 1–2 plus the SRS v1.1 alignment (bus status,
  cancellation reason, passenger stops/search). Boarding, staff offline tickets,
  incidents and reports arrive in later slices.
