using Microsoft.AspNetCore.Identity;

namespace UPMS.Api.Data;

public class AppUser : IdentityUser<Guid>
{
    public string FullName { get; set; } = string.Empty;
}

public enum Ownership
{
    University,
    Vendor
}

public enum VendorStatus
{
    Pending,
    Approved,
    Rejected
}

public enum TripStatus
{
    Scheduled,
    Delayed,
    Cancelled,
    Completed
}

public enum BookingStatus
{
    Reserved,
    Confirmed,
    Cancelled,
    Expired,
    Boarded
}

public enum PaymentMethod
{
    Online,
    Cash
}

public enum PaymentStatus
{
    Pending,
    Paid,
    Refunded,
    Failed
}

public class Route
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool Active { get; set; } = true;
    public List<RouteStop> Stops { get; set; } = new();
}

public class Stop
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}

public class RouteStop
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid RouteId { get; set; }
    public Route? Route { get; set; }
    public Guid StopId { get; set; }
    public Stop? Stop { get; set; }
    public int Sequence { get; set; }
    public int OffsetMinutes { get; set; }
}

public class Vendor
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public VendorStatus Status { get; set; } = VendorStatus.Pending;
    public List<Bus> Buses { get; set; } = new();
}

public class Bus
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Registration { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public Ownership Ownership { get; set; }
    public Guid? VendorId { get; set; }
    public Vendor? Vendor { get; set; }
}

public class Trip
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid RouteId { get; set; }
    public Route? Route { get; set; }
    public Guid? BusId { get; set; }
    public Bus? Bus { get; set; }
    public DateTime DepartureAt { get; set; }
    public decimal Fare { get; set; }
    public TripStatus Status { get; set; } = TripStatus.Scheduled;
    public List<Booking> Bookings { get; set; } = new();
}

public class Booking
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Reference { get; set; } = string.Empty;
    public Guid TripId { get; set; }
    public Trip? Trip { get; set; }
    public Guid PassengerId { get; set; }
    public AppUser? Passenger { get; set; }
    public BookingStatus Status { get; set; } = BookingStatus.Reserved;
    public PaymentMethod Method { get; set; }
    public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;
    public decimal Amount { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? CancelledAt { get; set; }
    public DateTime? ExpiredAt { get; set; }
    public DateTime? BoardedAt { get; set; }
}
