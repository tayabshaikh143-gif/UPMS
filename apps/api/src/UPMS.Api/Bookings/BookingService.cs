using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using UPMS.Api.Data;

namespace UPMS.Api.Bookings;

public record BookingActionResult(Booking? Booking, string? Error, int StatusCode);

public class BookingService
{
    private static readonly BookingStatus[] ActiveStatuses =
    {
        BookingStatus.Reserved,
        BookingStatus.Confirmed,
        BookingStatus.Boarded
    };

    private readonly AppDbContext _db;
    private readonly BookingOptions _options;
    private readonly IClock _clock;
    private readonly AuditService _audit;

    public BookingService(AppDbContext db, IOptions<BookingOptions> options, IClock clock, AuditService audit)
    {
        _db = db;
        _options = options.Value;
        _clock = clock;
        _audit = audit;
    }

    public async Task<BookingActionResult> CreateAsync(
        Guid passengerId,
        CreateBookingRequest request,
        CancellationToken cancellationToken)
    {
        // Serializable range locks make the capacity check + insert atomic, so two
        // passengers cannot take the last seat at the same time.
        await using var transaction = await _db.Database.BeginTransactionAsync(
            IsolationLevel.Serializable,
            cancellationToken);

        var trip = await _db.Trips
            .Include(t => t.Bus)
            .FirstOrDefaultAsync(t => t.Id == request.TripId, cancellationToken);

        if (trip is null)
            return new BookingActionResult(null, "Trip not found.", StatusCodes.Status404NotFound);
        if (trip.Status is TripStatus.Cancelled or TripStatus.Completed)
            return new BookingActionResult(null, "This trip is not open for booking.", StatusCodes.Status400BadRequest);
        if (trip.Bus is null)
            return new BookingActionResult(null, "No bus is assigned to this trip.", StatusCodes.Status400BadRequest);
        if (trip.DepartureAt <= _clock.Now)
            return new BookingActionResult(null, "This trip has already departed.", StatusCodes.Status400BadRequest);

        var activeBookings = await _db.Bookings.CountAsync(
            b => b.TripId == trip.Id && ActiveStatuses.Contains(b.Status),
            cancellationToken);

        if (activeBookings >= trip.Bus.Capacity)
            return new BookingActionResult(null, "This trip is fully booked.", StatusCodes.Status409Conflict);

        var isOnline = request.Method == PaymentMethod.Online;
        var now = _clock.Now;
        var reference = $"UPMS-{Guid.NewGuid().ToString("N")[..8].ToUpperInvariant()}";
        var booking = new Booking
        {
            Reference = reference,
            TripId = trip.Id,
            PassengerId = passengerId,
            Method = request.Method,
            Status = isOnline ? BookingStatus.Confirmed : BookingStatus.Reserved,
            PaymentStatus = isOnline ? PaymentStatus.Approved : PaymentStatus.Pending,
            Amount = trip.Fare,
            CreatedAt = now
        };
        booking.Payment = new Payment
        {
            Booking = booking,
            Amount = trip.Fare,
            Method = request.Method,
            Status = isOnline ? PaymentStatus.Approved : PaymentStatus.Pending,
            TransactionReference = isOnline ? $"DEMO-{reference}" : null,
            ApprovedAt = isOnline ? now : null,
            CreatedAt = now
        };

        _db.Bookings.Add(booking);
        _audit.Add(
            passengerId,
            "BookingCreated",
            "Booking",
            booking.Id,
            $"Reference {reference}; method {request.Method}.");
        if (isOnline)
        {
            _audit.Add(
                passengerId,
                "PaymentApproved",
                "Payment",
                booking.Payment.Id,
                $"Demo gateway approved {trip.Fare:0.00}.");
        }

        await _db.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return new BookingActionResult(booking, null, StatusCodes.Status201Created);
    }

    public async Task<int> CancelTripBookingsAsync(Guid tripId, Guid? actorId, CancellationToken cancellationToken)
    {
        var bookings = await _db.Bookings
            .Include(b => b.Payment)
            .Include(b => b.Refund)
            .Where(b => b.TripId == tripId && b.Status != BookingStatus.Cancelled && b.Status != BookingStatus.Expired)
            .ToListAsync(cancellationToken);

        var now = _clock.Now;
        foreach (var booking in bookings)
        {
            booking.Status = BookingStatus.Cancelled;
            booking.CancelledAt = now;

            var wasPaid = booking.PaymentStatus is PaymentStatus.Paid or PaymentStatus.Approved ||
                          booking.Payment?.Status is PaymentStatus.Paid or PaymentStatus.Approved;
            if (wasPaid)
            {
                booking.PaymentStatus = PaymentStatus.Refunded;
                if (booking.Payment is not null)
                    booking.Payment.Status = PaymentStatus.Refunded;
                if (booking.Refund is null)
                {
                    booking.Refund = new Refund
                    {
                        Booking = booking,
                        PaymentId = booking.Payment?.Id,
                        Amount = booking.Amount,
                        Reason = "Trip cancelled",
                        Status = RefundStatus.Processed,
                        ProcessedAt = now,
                        CreatedAt = now
                    };
                    _db.Refunds.Add(booking.Refund);
                }

                _audit.Add(
                    actorId,
                    "RefundProcessed",
                    "Refund",
                    booking.Refund.Id,
                    $"Trip cancellation refund {booking.Amount:0.00} for {booking.Reference}.");
            }

            _audit.Add(
                actorId,
                "BookingCancelled",
                "Booking",
                booking.Id,
                $"Trip cancellation; {booking.Reference}.");

            _db.Notifications.Add(new Notification
            {
                RecipientId = booking.PassengerId,
                TripId = booking.TripId,
                BookingId = booking.Id,
                Message = $"Your booking {booking.Reference} was cancelled because the trip was cancelled.",
                Channel = NotificationChannel.InApp,
                SentAt = now
            });
        }

        return bookings.Count;
    }

    public async Task<BookingActionResult> CancelAsync(
        Guid bookingId,
        Guid userId,
        bool isAdmin,
        CancellationToken cancellationToken)
    {
        await using var transaction = await _db.Database.BeginTransactionAsync(
            IsolationLevel.Serializable,
            cancellationToken);

        var booking = await _db.Bookings
            .Include(b => b.Trip)
            .Include(b => b.Payment)
            .Include(b => b.Refund)
            .FirstOrDefaultAsync(b => b.Id == bookingId, cancellationToken);

        if (booking is null)
            return new BookingActionResult(null, "Booking not found.", StatusCodes.Status404NotFound);
        if (!isAdmin && booking.PassengerId != userId)
            return new BookingActionResult(null, "You can only cancel your own booking.", StatusCodes.Status403Forbidden);
        if (booking.Status is BookingStatus.Cancelled or BookingStatus.Expired)
            return new BookingActionResult(null, "This booking is already cancelled.", StatusCodes.Status400BadRequest);
        if (booking.Status == BookingStatus.Boarded)
            return new BookingActionResult(null, "A boarded ticket cannot be cancelled.", StatusCodes.Status400BadRequest);

        if (!isAdmin && booking.Trip is not null && booking.Trip.DepartureAt <= _clock.Now.AddMinutes(_options.CancellationCutoffMinutes))
            return new BookingActionResult(
                null,
                $"Cancellation closes {_options.CancellationCutoffMinutes} minutes before departure.",
                StatusCodes.Status400BadRequest);

        booking.Status = BookingStatus.Cancelled;
        booking.CancelledAt = _clock.Now;
        var wasPaid = booking.PaymentStatus is PaymentStatus.Paid or PaymentStatus.Approved ||
                      booking.Payment?.Status is PaymentStatus.Paid or PaymentStatus.Approved;
        if (wasPaid)
        {
            booking.PaymentStatus = PaymentStatus.Refunded;
            if (booking.Payment is not null)
                booking.Payment.Status = PaymentStatus.Refunded;
            if (booking.Refund is null)
            {
                booking.Refund = new Refund
                {
                    Booking = booking,
                    PaymentId = booking.Payment?.Id,
                    Amount = booking.Amount,
                    Reason = isAdmin ? "Administrator cancellation" : "Passenger cancellation",
                    Status = RefundStatus.Processed,
                    ProcessedAt = _clock.Now,
                    CreatedAt = _clock.Now
                };
                _db.Refunds.Add(booking.Refund);
            }

            _audit.Add(
                isAdmin ? null : userId,
                "RefundProcessed",
                "Refund",
                booking.Refund.Id,
                $"Cancellation refund {booking.Amount:0.00} for {booking.Reference}.");
        }

        _audit.Add(
            isAdmin ? null : userId,
            "BookingCancelled",
            "Booking",
            booking.Id,
            $"Booking {booking.Reference} cancelled.");

        await _db.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return new BookingActionResult(booking, null, StatusCodes.Status200OK);
    }

    public async Task<int> ExpireCashBookingsAsync(CancellationToken cancellationToken)
    {        var cutoff = _clock.Now.AddMinutes(_options.CashNoShowMinutes);
        var expired = await _db.Bookings
            .Where(b =>
                b.Status == BookingStatus.Reserved &&
                b.Method == PaymentMethod.Cash &&
                b.Trip!.DepartureAt <= cutoff)
            .ToListAsync(cancellationToken);

        if (expired.Count == 0)
            return 0;

        var now = _clock.Now;
        foreach (var booking in expired)
        {
            booking.Status = BookingStatus.Expired;
            booking.ExpiredAt = now;
        }

        await _db.SaveChangesAsync(cancellationToken);
        return expired.Count;
    }
}
