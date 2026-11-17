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
    Failed,
    Approved
}

public enum RefundStatus
{
    Pending,
    Processed,
    Failed
}

public enum SettlementStatus
{
    Pending,
    Paid
}

public enum StaffRole
{
    Driver,
    BoardingStaff,
    Operations
}

public enum NotificationChannel
{
    InApp,
    Email,
    Sms
}

public enum OfflineTicketStatus
{
    Issued,
    Boarded,
    Cancelled
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
    public Guid? UserId { get; set; }
    public AppUser? User { get; set; }
    public string? Contact { get; set; }
    public decimal MonthlySubscription { get; set; }
    public SettlementStatus SubscriptionStatus { get; set; } = SettlementStatus.Pending;
    public List<Bus> Buses { get; set; } = new();
    public List<VendorSettlement> Settlements { get; set; } = new();
}

public class Bus
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Registration { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public Ownership Ownership { get; set; }
    public bool Active { get; set; } = true;
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
    public DateTime? CancelledAt { get; set; }
    public string? CancellationReason { get; set; }
    public List<Booking> Bookings { get; set; } = new();
    public List<TripStaffAssignment> StaffAssignments { get; set; } = new();
    public List<OfflineTicket> OfflineTickets { get; set; } = new();
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
    public Payment? Payment { get; set; }
    public Refund? Refund { get; set; }
}

public class Payment
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid BookingId { get; set; }
    public Booking? Booking { get; set; }
    public decimal Amount { get; set; }
    public PaymentMethod Method { get; set; }
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
    public string? TransactionReference { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public DateTime? PaidAt { get; set; }
    public Guid? RecordedByStaffId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}

public class Refund
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid BookingId { get; set; }
    public Booking? Booking { get; set; }
    public Guid? PaymentId { get; set; }
    public Payment? Payment { get; set; }
    public decimal Amount { get; set; }
    public string? Reason { get; set; }
    public RefundStatus Status { get; set; } = RefundStatus.Pending;
    public DateTime? ProcessedAt { get; set; }
    public Guid? ProcessedByStaffId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}

public class AuditLog
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid? ActorId { get; set; }
    public string? ActorName { get; set; }
    public string Action { get; set; } = string.Empty;
    public string Entity { get; set; } = string.Empty;
    public Guid? EntityId { get; set; }
    public string? Details { get; set; }
    public DateTime At { get; set; } = DateTime.Now;
}

public class VendorSettlement
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid VendorId { get; set; }
    public Vendor? Vendor { get; set; }
    public string Period { get; set; } = string.Empty;
    public decimal GrossFares { get; set; }
    public decimal SubscriptionDue { get; set; }
    public SettlementStatus Status { get; set; } = SettlementStatus.Pending;
    public DateTime? PaidAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}

public class StaffProfile
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public AppUser? User { get; set; }
    public string EmployeeNo { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public bool Active { get; set; } = true;
    public List<TripStaffAssignment> Assignments { get; set; } = new();
}

public class TripStaffAssignment
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TripId { get; set; }
    public Trip? Trip { get; set; }
    public Guid StaffProfileId { get; set; }
    public StaffProfile? StaffProfile { get; set; }
    public StaffRole Role { get; set; }
    public DateTime AssignedAt { get; set; } = DateTime.Now;
}

public class Notification
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid RecipientId { get; set; }
    public AppUser? Recipient { get; set; }
    public Guid? TripId { get; set; }
    public Trip? Trip { get; set; }
    public Guid? BookingId { get; set; }
    public Booking? Booking { get; set; }
    public string Message { get; set; } = string.Empty;
    public NotificationChannel Channel { get; set; } = NotificationChannel.InApp;
    public DateTime SentAt { get; set; } = DateTime.Now;
    public DateTime? ReadAt { get; set; }
}

public class OfflineTicket
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Reference { get; set; } = string.Empty;
    public Guid TripId { get; set; }
    public Trip? Trip { get; set; }
    public string PassengerName { get; set; } = string.Empty;
    public string PassengerPhone { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public PaymentMethod Method { get; set; } = PaymentMethod.Cash;
    public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Paid;
    public Guid? IssuedByUserId { get; set; }
    public AppUser? IssuedByUser { get; set; }
    public DateTime IssuedAt { get; set; } = DateTime.Now;
    public OfflineTicketStatus Status { get; set; } = OfflineTicketStatus.Issued;
}
