using UPMS.Api.Data;

namespace UPMS.Api.Bookings;

public record StaffTripDto(
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
    int RemainingSeats,
    DateTime BoardingStartsAt,
    bool BoardingOpen);

public record StaffPassengerDto(
    Guid Id,
    string Source,
    string Reference,
    string PassengerName,
    string? Contact,
    PaymentMethod Method,
    PaymentStatus PaymentStatus,
    int Status,
    decimal Amount,
    DateTime CreatedAt,
    DateTime? BoardedAt);

public record StaffTripDetailDto(
    StaffTripDto Trip,
    IReadOnlyList<StaffPassengerDto> Passengers);

public record OfflineTicketRequest(
    string PassengerName,
    string PassengerPhone,
    decimal? Amount = null);
