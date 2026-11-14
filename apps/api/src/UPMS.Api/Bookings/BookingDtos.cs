using UPMS.Api.Data;

namespace UPMS.Api.Bookings;

public record CreateBookingRequest(Guid TripId, PaymentMethod Method);

public record BookingDto(
    Guid Id,
    string Reference,
    Guid TripId,
    string RouteCode,
    string RouteName,
    string Origin,
    string Destination,
    DateTime DepartureAt,
    string? BusRegistration,
    decimal Amount,
    BookingStatus Status,
    PaymentMethod Method,
    PaymentStatus PaymentStatus,
    DateTime CreatedAt,
    DateTime? CancelledAt,
    string PassengerName,
    string PassengerEmail);
