using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UPMS.Api.Bookings;
using UPMS.Api.Data;

namespace UPMS.Api.Data;

public static class Seed
{
    public static async Task RunAsync(IServiceProvider services)
    {
        var db = services.GetRequiredService<AppDbContext>();
        await db.Database.MigrateAsync();

        var roleManager = services.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
        foreach (var role in new[] { "Admin", "Passenger", "Staff", "Vendor" })
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole<Guid>(role));
        }

        var users = services.GetRequiredService<UserManager<AppUser>>();
        var config = services.GetRequiredService<IConfiguration>();
        var adminEmail = config["Seed:AdminEmail"] ?? "admin@upms.local";
        var adminPassword = config["Seed:AdminPassword"] ?? "Admin@12345";

        var admin = await users.FindByEmailAsync(adminEmail);
        if (admin is null)
        {
            admin = new AppUser
            {
                Id = Guid.NewGuid(),
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true,
                FullName = "UPMS Administrator"
            };
            await users.CreateAsync(admin, adminPassword);
            await users.AddToRoleAsync(admin, "Admin");
        }

        if (!await db.Stops.AnyAsync())
        {
            var stops = new[]
            {
                new Stop { Name = "University of Sindh Main Gate", Latitude = 25.3955, Longitude = 68.3140 },
                new Stop { Name = "Jamshoro Bypass", Latitude = 25.4210, Longitude = 68.3080 },
                new Stop { Name = "Kotri Bridge", Latitude = 25.3660, Longitude = 68.3090 },
                new Stop { Name = "Hyderabad City", Latitude = 25.3960, Longitude = 68.3578 }
            };
            db.Stops.AddRange(stops);

            var route = new Route { Code = "JS-01", Name = "Jamshoro Campus to Hyderabad" };
            db.Routes.Add(route);
            for (var i = 0; i < stops.Length; i++)
            {
                db.RouteStops.Add(new RouteStop
                {
                    Route = route,
                    Stop = stops[i],
                    Sequence = i + 1,
                    OffsetMinutes = i * 15
                });
            }

            var vendor = new Vendor { Name = "Indus Transport Services", Status = VendorStatus.Approved };
            db.Vendors.Add(vendor);

            var universityBus = new Bus { Registration = "SND-1001", Capacity = 45, Ownership = Ownership.University };
            var vendorBus = new Bus { Registration = "SND-2002", Capacity = 32, Ownership = Ownership.Vendor, Vendor = vendor };
            db.Buses.AddRange(universityBus, vendorBus);

            db.Trips.AddRange(
                new Trip { Route = route, Bus = universityBus, DepartureAt = DateTime.UtcNow.Date.AddDays(1).AddHours(8), Fare = 120m },
                new Trip { Route = route, Bus = universityBus, DepartureAt = DateTime.UtcNow.Date.AddDays(1).AddHours(16), Fare = 120m }
            );
        }

        var passengerEmail = config["Seed:PassengerEmail"] ?? "student@upms.local";
        var passengerPassword = config["Seed:PassengerPassword"] ?? "Student@123";
        if (await users.FindByEmailAsync(passengerEmail) is null)
        {
            var passenger = new AppUser
            {
                Id = Guid.NewGuid(),
                UserName = passengerEmail,
                Email = passengerEmail,
                PhoneNumber = "03000000000",
                EmailConfirmed = true,
                FullName = "Demo Passenger"
            };
            await users.CreateAsync(passenger, passengerPassword);
            await users.AddToRoleAsync(passenger, "Passenger");
        }

        // Link a demo vendor login to the seeded approved vendor so the vendor
        // portal can be reviewed without creating an application first.
        var vendorEmail = config["Seed:VendorEmail"] ?? "vendor@upms.local";
        var vendorPassword = config["Seed:VendorPassword"] ?? "Vendor@123";
        var demoVendor = await db.Vendors.FirstOrDefaultAsync(v => v.Name == "Indus Transport Services");
        if (demoVendor is not null)
        {
            var vendorUser = await users.FindByEmailAsync(vendorEmail);
            if (vendorUser is null)
            {
                vendorUser = new AppUser
                {
                    Id = Guid.NewGuid(),
                    UserName = vendorEmail,
                    Email = vendorEmail,
                    PhoneNumber = "03001234567",
                    EmailConfirmed = true,
                    FullName = "Indus Transport"
                };
                await users.CreateAsync(vendorUser, vendorPassword);
                await users.AddToRoleAsync(vendorUser, "Vendor");
            }
            else if (!await users.IsInRoleAsync(vendorUser, "Vendor"))
            {
                await users.AddToRoleAsync(vendorUser, "Vendor");
            }

            if (demoVendor.UserId is null)
                demoVendor.UserId = vendorUser.Id;
            demoVendor.Contact ??= "03001234567";
        }

        var fallbackBus = await db.Buses.OrderBy(b => b.Registration).FirstOrDefaultAsync();
        var unassignedTrips = await db.Trips.Where(t => t.BusId == null).ToListAsync();
        if (fallbackBus is not null && unassignedTrips.Count > 0)
        {
            foreach (var trip in unassignedTrips)
                trip.BusId = fallbackBus.Id;
        }

        // Keep the demo useful when the seeded dates have passed: ensure at least
        // two upcoming scheduled trips exist on the first route.
        var clock = services.GetRequiredService<IClock>();
        var hasUpcomingTrip = await db.Trips.AnyAsync(t =>
            t.DepartureAt > clock.Now && t.Status == TripStatus.Scheduled);
        if (!hasUpcomingTrip)
        {
            var demoRoute = await db.Routes.OrderBy(r => r.Code).FirstOrDefaultAsync();
            var demoBus = await db.Buses.Where(b => b.Active).OrderBy(b => b.Registration).FirstOrDefaultAsync();
            if (demoRoute is not null && demoBus is not null)
            {
                db.Trips.AddRange(
                    new Trip
                    {
                        RouteId = demoRoute.Id,
                        BusId = demoBus.Id,
                        DepartureAt = clock.Now.Date.AddDays(1).AddHours(8),
                        Fare = 120m
                    },
                    new Trip
                    {
                        RouteId = demoRoute.Id,
                        BusId = demoBus.Id,
                        DepartureAt = clock.Now.Date.AddDays(1).AddHours(16),
                        Fare = 120m
                    });
            }
        }

        // Give the demo vendor one upcoming trip so the vendor portal has data.
        var demoVendorBus = await db.Buses
            .Where(b => b.Active && b.VendorId != null)
            .OrderBy(b => b.Registration)
            .FirstOrDefaultAsync();
        if (demoVendorBus is not null &&
            !await db.Trips.AnyAsync(t =>
                t.BusId == demoVendorBus.Id &&
                t.DepartureAt > clock.Now &&
                t.Status == TripStatus.Scheduled))
        {
            var demoRoute = await db.Routes.OrderBy(r => r.Code).FirstOrDefaultAsync();
            if (demoRoute is not null)
            {
                db.Trips.Add(new Trip
                {
                    RouteId = demoRoute.Id,
                    BusId = demoVendorBus.Id,
                    DepartureAt = clock.Now.Date.AddDays(2).AddHours(10),
                    Fare = 150m
                });
            }
        }

        await db.SaveChangesAsync();
    }
}
