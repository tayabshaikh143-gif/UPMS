using UPMS.Api.Data;

namespace UPMS.Api.Catalog;

public record RouteDto(Guid Id, string Code, string Name, bool Active, int StopCount);
public record RouteStopDto(Guid Id, Guid StopId, string StopName, int Sequence, int OffsetMinutes);
public record RouteDetailDto(Guid Id, string Code, string Name, bool Active, IReadOnlyList<RouteStopDto> Stops);
public record RouteRequest(string Code, string Name, bool Active = true);

public record StopDto(Guid Id, string Name, double Latitude, double Longitude);
public record StopRequest(string Name, double Latitude, double Longitude);

public record VendorDto(
    Guid Id,
    string Name,
    VendorStatus Status,
    int BusCount,
    Guid? UserId,
    string? Contact,
    decimal MonthlySubscription,
    SettlementStatus SubscriptionStatus);
public record VendorRequest(
    string Name,
    VendorStatus Status,
    Guid? UserId = null,
    string? Contact = null,
    decimal MonthlySubscription = 0,
    SettlementStatus SubscriptionStatus = SettlementStatus.Pending);
public record VendorSettlementDto(
    Guid Id,
    Guid VendorId,
    string Period,
    decimal GrossFares,
    decimal SubscriptionDue,
    SettlementStatus Status,
    DateTime? PaidAt,
    DateTime CreatedAt);
public record VendorSettlementRequest(
    string Period,
    decimal GrossFares,
    decimal SubscriptionDue,
    SettlementStatus Status = SettlementStatus.Pending);
public record VendorProfileDto(
    Guid Id,
    string Name,
    VendorStatus Status,
    string? Contact,
    decimal MonthlySubscription,
    SettlementStatus SubscriptionStatus,
    int BusCount,
    string ContactPerson,
    string Email);
public record VendorTripDto(
    Guid Id,
    string RouteCode,
    string RouteName,
    string Origin,
    string Destination,
    DateTime DepartureAt,
    decimal Fare,
    TripStatus Status,
    string? BusRegistration,
    int Capacity,
    int BookedCount,
    int RemainingSeats);

public record BusDto(Guid Id, string Registration, int Capacity, Ownership Ownership, Guid? VendorId, string? VendorName, bool Active);
public record BusRequest(string Registration, int Capacity, Ownership Ownership, Guid? VendorId, bool Active = true);

public record TripDto(
    Guid Id,
    Guid RouteId,
    string RouteCode,
    string RouteName,
    string Origin,
    string Destination,
    DateTime DepartureAt,
    decimal Fare,
    TripStatus Status,
    Guid? BusId,
    string? BusRegistration,
    int Capacity,
    int BookedCount,
    int RemainingSeats,
    IReadOnlyList<RouteStopDto> Stops,
    DateTime? CancelledAt,
    string? CancellationReason);
public record TripRequest(
    Guid RouteId,
    Guid? BusId,
    DateTime DepartureAt,
    decimal Fare,
    TripStatus Status = TripStatus.Scheduled,
    string? CancellationReason = null);

public record RouteStopRequest(Guid StopId, int Sequence, int OffsetMinutes);
public record RouteStopNameRequest(string Name);
