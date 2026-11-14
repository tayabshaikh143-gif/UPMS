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

Change the seed values in `appsettings.json` before any real deployment.

## Slice 1 endpoints

| Method | Route | Access |
|---|---|---|
| POST | `/api/auth/register` | public (creates Passenger) |
| POST | `/api/auth/login` | public |
| GET | `/api/auth/me` | authenticated |
| GET | `/api/routes`, `/api/routes/{id}` | authenticated |
| POST/PUT/DELETE | `/api/routes` | Admin |
| POST | `/api/routes/{id}/stops` | Admin |
| GET | `/api/stops` | authenticated |
| POST/PUT/DELETE | `/api/stops` | Admin |
| GET | `/api/vendors` | authenticated |
| POST/PUT | `/api/vendors` | Admin |
| GET | `/api/buses` | authenticated |
| POST/PUT/DELETE | `/api/buses` | Admin |
| GET | `/api/trips?routeId=` | authenticated |
| POST/PUT/DELETE | `/api/trips` | Admin |

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
