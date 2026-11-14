using System.Data;
using Microsoft.EntityFrameworkCore;
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

    public BookingService(AppDbContext db)
    {
        _db = db;
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
        if (trip.DepartureAt <= DateTime.Now)
            return new BookingActionResult(null, "This trip has already departed.", StatusCodes.Status400BadRequest);

        var activeBookings = await _db.Bookings.CountAsync(
            b => b.TripId == trip.Id && ActiveStatuses.Contains(b.Status),
            cancellationToken);

        if (activeBookings >= trip.Bus.Capacity)
            return new BookingActionResult(null, "This trip is fully booked.", StatusCodes.Status409Conflict);

        var isOnline = request.Method == PaymentMethod.Online;
        var booking = new Booking
        {
            Reference = $"UPMS-{Guid.NewGuid().ToString("N")[..8].ToUpperInvariant()}",
            TripId = trip.Id,
            PassengerId = passengerId,
            Method = request.Method,
            Status = isOnline ? BookingStatus.Confirmed : BookingStatus.Reserved,
            PaymentStatus = isOnline ? PaymentStatus.Paid : PaymentStatus.Pending,
            Amount = trip.Fare,
            CreatedAt = DateTime.Now
        };

        _db.Bookings.Add(booking);
        await _db.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return new BookingActionResult(booking, null, StatusCodes.Status201Created);
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
            .FirstOrDefaultAsync(b => b.Id == bookingId, cancellationToken);

        if (booking is null)
            return new BookingActionResult(null, "Booking not found.", StatusCodes.Status404NotFound);
        if (!isAdmin && booking.PassengerId != userId)
            return new BookingActionResult(null, "You can only cancel your own booking.", StatusCodes.Status403Forbidden);
        if (booking.Status is BookingStatus.Cancelled or BookingStatus.Expired)
            return new BookingActionResult(null, "This booking is already cancelled.", StatusCodes.Status400BadRequest);
        if (booking.Status == BookingStatus.Boarded)
            return new BookingActionResult(null, "A boarded ticket cannot be cancelled.", StatusCodes.Status400BadRequest);

        if (!isAdmin && booking.Trip is not null && booking.Trip.DepartureAt <= DateTime.Now.AddMinutes(20))
            return new BookingActionResult(
                null,
                "Cancellation closes 20 minutes before departure.",
                StatusCodes.Status400BadRequest);

        booking.Status = BookingStatus.Cancelled;
        booking.CancelledAt = DateTime.Now;
        if (booking.PaymentStatus == PaymentStatus.Paid)
            booking.PaymentStatus = PaymentStatus.Refunded;

        await _db.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return new BookingActionResult(booking, null, StatusCodes.Status200OK);
    }

    public async Task<int> ExpireCashBookingsAsync(CancellationToken cancellationToken)
    {
        var cutoff = DateTime.Now.AddMinutes(5);
        var expired = await _db.Bookings
            .Where(b =>
                b.Status == BookingStatus.Reserved &&
                b.Method == PaymentMethod.Cash &&
                b.Trip!.DepartureAt <= cutoff)
            .ToListAsync(cancellationToken);

        if (expired.Count == 0)
            return 0;

        var now = DateTime.Now;
        foreach (var booking in expired)
        {
            booking.Status = BookingStatus.Expired;
            booking.ExpiredAt = now;
        }

        await _db.SaveChangesAsync(cancellationToken);
        return expired.Count;
    }
}
