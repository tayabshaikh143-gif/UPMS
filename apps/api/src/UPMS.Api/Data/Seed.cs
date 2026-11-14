using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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
            db.Buses.AddRange(
                new Bus { Registration = "SND-1001", Capacity = 45, Ownership = Ownership.University },
                new Bus { Registration = "SND-2002", Capacity = 32, Ownership = Ownership.Vendor, Vendor = vendor }
            );

            db.Trips.AddRange(
                new Trip { Route = route, DepartureAt = DateTime.UtcNow.Date.AddDays(1).AddHours(8), Fare = 120m },
                new Trip { Route = route, DepartureAt = DateTime.UtcNow.Date.AddDays(1).AddHours(16), Fare = 120m }
            );
        }

        await db.SaveChangesAsync();
    }
}
