using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace UPMS.Api.Data;

public class AppDbContext : IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Route> Routes => Set<Route>();
    public DbSet<Stop> Stops => Set<Stop>();
    public DbSet<RouteStop> RouteStops => Set<RouteStop>();
    public DbSet<Vendor> Vendors => Set<Vendor>();
    public DbSet<Bus> Buses => Set<Bus>();
    public DbSet<Trip> Trips => Set<Trip>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Route>(e =>
        {
            e.HasIndex(x => x.Code).IsUnique();
            e.Property(x => x.Code).HasMaxLength(20).IsRequired();
            e.Property(x => x.Name).HasMaxLength(120).IsRequired();
        });

        builder.Entity<Stop>(e =>
        {
            e.Property(x => x.Name).HasMaxLength(120).IsRequired();
        });

        builder.Entity<RouteStop>(e =>
        {
            e.HasIndex(x => new { x.RouteId, x.Sequence }).IsUnique();
            e.HasOne(x => x.Route).WithMany(x => x.Stops).HasForeignKey(x => x.RouteId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.Stop).WithMany().HasForeignKey(x => x.StopId).OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<Vendor>(e =>
        {
            e.Property(x => x.Name).HasMaxLength(120).IsRequired();
        });

        builder.Entity<Bus>(e =>
        {
            e.HasIndex(x => x.Registration).IsUnique();
            e.Property(x => x.Registration).HasMaxLength(30).IsRequired();
            e.HasOne(x => x.Vendor).WithMany(x => x.Buses).HasForeignKey(x => x.VendorId).OnDelete(DeleteBehavior.SetNull);
        });

        builder.Entity<Trip>(e =>
        {
            e.Property(x => x.Fare).HasPrecision(10, 2);
            e.HasOne(x => x.Route).WithMany().HasForeignKey(x => x.RouteId).OnDelete(DeleteBehavior.Restrict);
        });
    }
}
