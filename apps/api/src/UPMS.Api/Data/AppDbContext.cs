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
    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<Refund> Refunds => Set<Refund>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<VendorSettlement> VendorSettlements => Set<VendorSettlement>();
    public DbSet<StaffProfile> StaffProfiles => Set<StaffProfile>();
    public DbSet<TripStaffAssignment> TripStaffAssignments => Set<TripStaffAssignment>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<OfflineTicket> OfflineTickets => Set<OfflineTicket>();

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
            e.Property(x => x.Contact).HasMaxLength(120);
            e.Property(x => x.MonthlySubscription).HasPrecision(10, 2);
            e.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.SetNull);
        });

        builder.Entity<VendorSettlement>(e =>
        {
            e.HasIndex(x => new { x.VendorId, x.Period }).IsUnique();
            e.Property(x => x.Period).HasMaxLength(7).IsRequired();
            e.Property(x => x.GrossFares).HasPrecision(10, 2);
            e.Property(x => x.SubscriptionDue).HasPrecision(10, 2);
            e.HasOne(x => x.Vendor).WithMany(x => x.Settlements).HasForeignKey(x => x.VendorId).OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<Bus>(e =>
        {
            e.HasIndex(x => x.Registration).IsUnique();
            e.Property(x => x.Registration).HasMaxLength(30).IsRequired();
            e.Property(x => x.Active).HasDefaultValue(true);
            e.HasOne(x => x.Vendor).WithMany(x => x.Buses).HasForeignKey(x => x.VendorId).OnDelete(DeleteBehavior.SetNull);
        });

        builder.Entity<Trip>(e =>
        {
            e.Property(x => x.Fare).HasPrecision(10, 2);
            e.Property(x => x.CancellationReason).HasMaxLength(250);
            e.HasOne(x => x.Route).WithMany().HasForeignKey(x => x.RouteId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.Bus).WithMany().HasForeignKey(x => x.BusId).OnDelete(DeleteBehavior.SetNull);
            e.HasIndex(x => x.DepartureAt);
        });

        builder.Entity<Booking>(e =>
        {
            e.HasIndex(x => x.Reference).IsUnique();
            e.Property(x => x.Reference).HasMaxLength(20).IsRequired();
            e.Property(x => x.Amount).HasPrecision(10, 2);
            e.HasIndex(x => new { x.TripId, x.Status });
            e.HasOne(x => x.Trip).WithMany(x => x.Bookings).HasForeignKey(x => x.TripId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.Passenger).WithMany().HasForeignKey(x => x.PassengerId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.Payment).WithOne(x => x.Booking).HasForeignKey<Payment>(x => x.BookingId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.Refund).WithOne(x => x.Booking).HasForeignKey<Refund>(x => x.BookingId).OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<Payment>(e =>
        {
            e.Property(x => x.Amount).HasPrecision(10, 2);
            e.Property(x => x.TransactionReference).HasMaxLength(80);
            e.HasIndex(x => x.BookingId).IsUnique();
        });

        builder.Entity<Refund>(e =>
        {
            e.Property(x => x.Amount).HasPrecision(10, 2);
            e.Property(x => x.Reason).HasMaxLength(250);
            e.HasOne(x => x.Payment).WithMany().HasForeignKey(x => x.PaymentId).OnDelete(DeleteBehavior.SetNull);
            e.HasIndex(x => x.BookingId).IsUnique();
        });

        builder.Entity<AuditLog>(e =>
        {
            e.Property(x => x.Action).HasMaxLength(80).IsRequired();
            e.Property(x => x.Entity).HasMaxLength(60).IsRequired();
            e.Property(x => x.ActorName).HasMaxLength(120);
            e.Property(x => x.Details).HasMaxLength(1000);
            e.HasIndex(x => x.At);
        });

        builder.Entity<StaffProfile>(e =>
        {
            e.HasIndex(x => x.UserId).IsUnique();
            e.HasIndex(x => x.EmployeeNo).IsUnique();
            e.Property(x => x.EmployeeNo).HasMaxLength(40).IsRequired();
            e.Property(x => x.Position).HasMaxLength(80).IsRequired();
            e.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<TripStaffAssignment>(e =>
        {
            e.HasIndex(x => new { x.TripId, x.StaffProfileId, x.Role }).IsUnique();
            e.HasOne(x => x.Trip).WithMany(x => x.StaffAssignments).HasForeignKey(x => x.TripId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.StaffProfile).WithMany(x => x.Assignments).HasForeignKey(x => x.StaffProfileId).OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<Notification>(e =>
        {
            e.Property(x => x.Message).HasMaxLength(500).IsRequired();
            e.HasIndex(x => new { x.RecipientId, x.ReadAt });
            e.HasOne(x => x.Trip).WithMany().HasForeignKey(x => x.TripId).OnDelete(DeleteBehavior.SetNull);
            e.HasOne(x => x.Booking).WithMany().HasForeignKey(x => x.BookingId).OnDelete(DeleteBehavior.SetNull);
        });

        builder.Entity<OfflineTicket>(e =>
        {
            e.HasIndex(x => x.Reference).IsUnique();
            e.Property(x => x.Reference).HasMaxLength(20).IsRequired();
            e.Property(x => x.PassengerName).HasMaxLength(120).IsRequired();
            e.Property(x => x.PassengerPhone).HasMaxLength(30).IsRequired();
            e.Property(x => x.Amount).HasPrecision(10, 2);
            e.HasOne(x => x.Trip).WithMany(x => x.OfflineTickets).HasForeignKey(x => x.TripId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.IssuedByUser).WithMany().HasForeignKey(x => x.IssuedByUserId).OnDelete(DeleteBehavior.SetNull);
        });
    }
}
