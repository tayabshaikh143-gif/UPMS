using System.Data;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using UPMS.Api.Data;

namespace UPMS.Api.Bookings;

[ApiController]
[Route("api/staff")]
[Authorize(Roles = "Staff,Admin")]
public class StaffController : ControllerBase
{
    private static readonly BookingStatus[] ActiveBookingStatuses =
    {
        BookingStatus.Reserved,
        BookingStatus.Confirmed,
        BookingStatus.Boarded
    };

    private readonly AppDbContext _db;
    private readonly BookingOptions _options;
    private readonly IClock _clock;
    private readonly AuditService _audit;

    public StaffController(
        AppDbContext db,
        IOptions<BookingOptions> options,
        IClock clock,
        AuditService audit)
    {
        _db = db;
        _options = options.Value;
        _clock = clock;
        _audit = audit;
    }

    [HttpGet("trips")]
    public async Task<ActionResult<IReadOnlyList<StaffTripDto>>> Trips()
    {
        var now = _clock.Now;
        var from = now.AddHours(-12);
        var to = now.AddDays(7);
        var trips = await _db.Trips.AsNoTracking()
            .Include(t => t.Route!).ThenInclude(r => r.Stops).ThenInclude(rs => rs.Stop)
            .Include(t => t.Bus)
            .Include(t => t.Bookings)
            .Include(t => t.OfflineTickets)
            .Where(t =>
                t.DepartureAt >= from &&
                t.DepartureAt <= to &&
                t.Status != TripStatus.Cancelled)
            .AsSplitQuery()
            .OrderBy(t => t.DepartureAt)
            .ToListAsync();

        return Ok(trips.Select(ToTripDto).ToList());
    }

    [HttpGet("trips/{id:guid}/passengers")]
    public async Task<ActionResult<StaffTripDetailDto>> Passengers(Guid id)
    {
        var trip = await LoadTripAsync(id, asTracking: false);
        if (trip is null)
            return NotFound();

        var passengers = trip.Bookings
            .Where(b => b.Status != BookingStatus.Cancelled)
            .Select(ToPassengerDto)
            .Concat(trip.OfflineTickets.Select(ToPassengerDto))
            .OrderBy(p => p.CreatedAt)
            .ToList();

        return Ok(new StaffTripDetailDto(ToTripDto(trip), passengers));
    }

    [HttpPost("trips/{tripId:guid}/bookings/{bookingId:guid}/board")]
    public async Task<IActionResult> BoardBooking(Guid tripId, Guid bookingId)
    {
        var booking = await _db.Bookings
            .Include(b => b.Trip)
            .Include(b => b.Payment)
            .FirstOrDefaultAsync(b => b.Id == bookingId && b.TripId == tripId);
        if (booking is null)
            return NotFound();
        if (booking.Trip is null)
            return NotFound();

        var windowError = ValidateBoardingWindow(booking.Trip);
        if (windowError is not null)
            return BadRequest(new { message = windowError });

        if (booking.Status is not (BookingStatus.Reserved or BookingStatus.Confirmed))
            return Conflict(new { message = "Only reserved or confirmed bookings can be boarded." });

        var now = _clock.Now;
        if (booking.Method == PaymentMethod.Cash)
        {
            booking.PaymentStatus = PaymentStatus.Paid;
            if (booking.Payment is not null)
            {
                booking.Payment.Status = PaymentStatus.Paid;
                booking.Payment.PaidAt = now;
            }
        }

        booking.Status = BookingStatus.Boarded;
        booking.BoardedAt = now;
        _audit.Add(
            CurrentUserId(),
            "PassengerBoarded",
            "Booking",
            booking.Id,
            $"Booking {booking.Reference} boarded.");

        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpPost("trips/{tripId:guid}/bookings/{bookingId:guid}/no-show")]
    public async Task<IActionResult> NoShowBooking(Guid tripId, Guid bookingId)
    {
        var booking = await _db.Bookings
            .Include(b => b.Trip)
            .FirstOrDefaultAsync(b => b.Id == bookingId && b.TripId == tripId);
        if (booking is null)
            return NotFound();
        if (booking.Trip is null)
            return NotFound();

        var windowError = ValidateBoardingWindow(booking.Trip);
        if (windowError is not null)
            return BadRequest(new { message = windowError });

        if (booking.Status is not (BookingStatus.Reserved or BookingStatus.Confirmed))
            return Conflict(new { message = "Only reserved or confirmed bookings can be marked no-show." });

        booking.Status = BookingStatus.Expired;
        booking.ExpiredAt = _clock.Now;
        _audit.Add(
            CurrentUserId(),
            "PassengerNoShow",
            "Booking",
            booking.Id,
            $"Booking {booking.Reference} marked no-show.");

        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpPost("trips/{tripId:guid}/offline-tickets")]
    public async Task<ActionResult<StaffPassengerDto>> IssueOfflineTicket(
        Guid tripId,
        OfflineTicketRequest request)
    {
        var passengerName = request.PassengerName?.Trim() ?? string.Empty;
        var passengerPhone = request.PassengerPhone?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(passengerName))
            return BadRequest(new { message = "Passenger name is required." });
        if (string.IsNullOrWhiteSpace(passengerPhone))
            return BadRequest(new { message = "Passenger phone is required." });

        await using var transaction = await _db.Database.BeginTransactionAsync(
            IsolationLevel.Serializable,
            HttpContext.RequestAborted);

        var trip = await _db.Trips
            .Include(t => t.Bus)
            .FirstOrDefaultAsync(t => t.Id == tripId, HttpContext.RequestAborted);
        if (trip is null)
            return NotFound();
        if (trip.Bus is null)
            return BadRequest(new { message = "No bus is assigned to this trip." });

        var windowError = ValidateBoardingWindow(trip);
        if (windowError is not null)
            return BadRequest(new { message = windowError });

        var activeBookings = await _db.Bookings.CountAsync(
            b => b.TripId == trip.Id && ActiveBookingStatuses.Contains(b.Status),
            HttpContext.RequestAborted);
        var activeOfflineTickets = await _db.OfflineTickets.CountAsync(
            t => t.TripId == trip.Id &&
                 (t.Status == OfflineTicketStatus.Issued || t.Status == OfflineTicketStatus.Boarded),
            HttpContext.RequestAborted);
        if (activeBookings + activeOfflineTickets >= trip.Bus.Capacity)
            return Conflict(new { message = "This trip is fully booked." });

        var amount = request.Amount ?? trip.Fare;
        if (amount <= 0)
            return BadRequest(new { message = "Ticket amount must be greater than zero." });

        string reference;
        do
        {
            reference = $"OFF-{Guid.NewGuid().ToString("N")[..8].ToUpperInvariant()}";
        } while (await _db.OfflineTickets.AnyAsync(
            t => t.Reference == reference,
            HttpContext.RequestAborted));

        var now = _clock.Now;
        var ticket = new OfflineTicket
        {
            Reference = reference,
            TripId = trip.Id,
            PassengerName = passengerName,
            PassengerPhone = passengerPhone,
            Amount = amount,
            Method = PaymentMethod.Cash,
            PaymentStatus = PaymentStatus.Paid,
            IssuedByUserId = CurrentUserId(),
            IssuedAt = now,
            Status = OfflineTicketStatus.Issued
        };
        _db.OfflineTickets.Add(ticket);
        _audit.Add(
            CurrentUserId(),
            "OfflineTicketIssued",
            "OfflineTicket",
            ticket.Id,
            $"{reference} issued for {passengerName} ({passengerPhone}); amount {amount:0.00}.");

        await _db.SaveChangesAsync(HttpContext.RequestAborted);
        await transaction.CommitAsync(HttpContext.RequestAborted);

        return Created(
            $"/api/staff/trips/{tripId}/offline-tickets/{ticket.Id}",
            ToPassengerDto(ticket));
    }

    [HttpPost("trips/{tripId:guid}/offline-tickets/{ticketId:guid}/board")]
    public async Task<IActionResult> BoardOfflineTicket(Guid tripId, Guid ticketId)
    {
        var ticket = await _db.OfflineTickets
            .Include(t => t.Trip)
            .FirstOrDefaultAsync(t => t.Id == ticketId && t.TripId == tripId);
        if (ticket is null)
            return NotFound();
        if (ticket.Trip is null)
            return NotFound();

        var windowError = ValidateBoardingWindow(ticket.Trip);
        if (windowError is not null)
            return BadRequest(new { message = windowError });

        if (ticket.Status != OfflineTicketStatus.Issued)
            return Conflict(new { message = "Only issued offline tickets can be boarded." });

        ticket.Status = OfflineTicketStatus.Boarded;
        _audit.Add(
            CurrentUserId(),
            "OfflineTicketBoarded",
            "OfflineTicket",
            ticket.Id,
            $"Offline ticket {ticket.Reference} boarded.");

        await _db.SaveChangesAsync();
        return NoContent();
    }

    private async Task<Trip?> LoadTripAsync(Guid id, bool asTracking)
    {
        var query = _db.Trips.AsQueryable();
        if (!asTracking)
            query = query.AsNoTracking();

        return await query
            .Include(t => t.Route!).ThenInclude(r => r.Stops).ThenInclude(rs => rs.Stop)
            .Include(t => t.Bus)
            .Include(t => t.Bookings).ThenInclude(b => b.Passenger)
            .Include(t => t.OfflineTickets)
            .AsSplitQuery()
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    private StaffTripDto ToTripDto(Trip trip)
    {
        var stops = trip.Route?.Stops.OrderBy(rs => rs.Sequence).ToList() ?? new List<RouteStop>();
        var capacity = trip.Bus?.Capacity ?? 0;
        var booked = trip.Bookings.Count(b => ActiveBookingStatuses.Contains(b.Status));
        booked += trip.OfflineTickets.Count(t =>
            t.Status == OfflineTicketStatus.Issued || t.Status == OfflineTicketStatus.Boarded);
        var boardingStartsAt = trip.DepartureAt.AddMinutes(-_options.BoardingStartMinutes);
        var boardingOpen = trip.Status is not (TripStatus.Cancelled or TripStatus.Completed) &&
                           _clock.Now >= boardingStartsAt;

        return new StaffTripDto(
            trip.Id,
            trip.Route?.Code ?? string.Empty,
            trip.Route?.Name ?? string.Empty,
            stops.FirstOrDefault()?.Stop?.Name ?? string.Empty,
            stops.LastOrDefault()?.Stop?.Name ?? string.Empty,
            trip.DepartureAt,
            trip.Fare,
            trip.Status,
            trip.Bus?.Registration,
            capacity,
            booked,
            Math.Max(0, capacity - booked),
            boardingStartsAt,
            boardingOpen);
    }

    private string? ValidateBoardingWindow(Trip trip)
    {
        if (trip.Status is TripStatus.Cancelled or TripStatus.Completed)
            return "This trip is not open for boarding.";
        if (_clock.Now < trip.DepartureAt.AddMinutes(-_options.BoardingStartMinutes))
            return $"Boarding opens {_options.BoardingStartMinutes} minutes before departure.";
        return null;
    }

    private static StaffPassengerDto ToPassengerDto(Booking booking)
    {
        return new StaffPassengerDto(
            booking.Id,
            "Booking",
            booking.Reference,
            booking.Passenger?.FullName ?? "Passenger",
            booking.Passenger?.PhoneNumber ?? booking.Passenger?.Email,
            booking.Method,
            booking.PaymentStatus,
            (int)booking.Status,
            booking.Amount,
            booking.CreatedAt,
            booking.BoardedAt);
    }

    private static StaffPassengerDto ToPassengerDto(OfflineTicket ticket)
    {
        return new StaffPassengerDto(
            ticket.Id,
            "Offline",
            ticket.Reference,
            ticket.PassengerName,
            ticket.PassengerPhone,
            ticket.Method,
            ticket.PaymentStatus,
            (int)ticket.Status,
            ticket.Amount,
            ticket.IssuedAt,
            null);
    }

    private Guid? CurrentUserId()
    {
        var raw = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(raw, out var id) ? id : null;
    }
}
