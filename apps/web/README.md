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
| `/` | Dashboard | Catalog counts and upcoming trips |
| `/routes` | Routes | Admin create/edit/delete |
| `/stops` | Route stops | Ordered stops per route; admin add/remove by name |
| `/vendors` | Vendors | Admin create/edit with approval status |
| `/buses` | Buses | Admin create/edit, vendor selection for vendor-owned buses |
| `/trips` | Trips | Admin create/edit/delete |

Role-gated actions are hidden for non-admin users; the API still enforces
authorization on every request.

## Notes

- Trip departure times are entered and displayed as wall-clock schedule times.
  The picker value is sent to the API unchanged so the stored time matches what
  the admin typed.
- Stops are managed per route. Adding a stop only asks for its name; coordinates
  are not collected in this slice.
- The frontend covers Slice 1 only. Booking, capacity, payments and offline
  ticketing arrive in later slices.
