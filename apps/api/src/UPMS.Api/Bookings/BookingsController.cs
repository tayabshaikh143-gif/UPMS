using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UPMS.Api.Data;

namespace UPMS.Api.Bookings;

[ApiController]
[Route("api/bookings")]
[Authorize]
public class BookingsController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly BookingService _bookings;

    public BookingsController(AppDbContext db, BookingService bookings)
    {
        _db = db;
        _bookings = bookings;
    }

    [HttpPost]
    public async Task<ActionResult<BookingDto>> Create(CreateBookingRequest request)
    {
        var userId = CurrentUserId();
        if (userId is null)
            return Unauthorized();

        var result = await _bookings.CreateAsync(userId.Value, request, HttpContext.RequestAborted);
        if (result.Booking is null)
            return StatusCode(result.StatusCode, new { message = result.Error });

        var dto = await GetDtoAsync(result.Booking.Id);
        return CreatedAtAction(nameof(Get), new { id = result.Booking.Id }, dto);
    }

    [HttpGet("my")]
    public async Task<ActionResult<IReadOnlyList<BookingDto>>> My()
    {
        var userId = CurrentUserId();
        if (userId is null)
            return Unauthorized();

        var bookings = await BookingQuery()
            .Where(b => b.PassengerId == userId.Value)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();
        return Ok(bookings.Select(ToDto).ToList());
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Staff")]
    public async Task<ActionResult<IReadOnlyList<BookingDto>>> List([FromQuery] Guid? tripId)
    {
        var query = BookingQuery();
        if (tripId is not null)
            query = query.Where(b => b.TripId == tripId.Value);

        var bookings = await query
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();
        return Ok(bookings.Select(ToDto).ToList());
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<BookingDto>> Get(Guid id)
    {
        var booking = await BookingQuery().FirstOrDefaultAsync(b => b.Id == id);
        if (booking is null)
            return NotFound();

        var userId = CurrentUserId();
        var isStaff = User.IsInRole("Admin") || User.IsInRole("Staff");
        if (!isStaff && booking.PassengerId != userId)
            return Forbid();

        return Ok(ToDto(booking));
    }

    [HttpPost("{id:guid}/cancel")]
    public async Task<ActionResult<BookingDto>> Cancel(Guid id)
    {
        var userId = CurrentUserId();
        if (userId is null)
            return Unauthorized();

        var result = await _bookings.CancelAsync(
            id,
            userId.Value,
            User.IsInRole("Admin"),
            HttpContext.RequestAborted);

        if (result.Booking is null)
            return StatusCode(result.StatusCode, new { message = result.Error });

        var dto = await GetDtoAsync(result.Booking.Id);
        return Ok(dto);
    }

    private Guid? CurrentUserId()
    {
        var raw = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(raw, out var id) ? id : null;
    }

    private IQueryable<Booking> BookingQuery()
    {
        return _db.Bookings.AsNoTracking()
            .Include(b => b.Trip)!.ThenInclude(t => t!.Route)!.ThenInclude(r => r!.Stops).ThenInclude(rs => rs.Stop)
            .Include(b => b.Trip)!.ThenInclude(t => t!.Bus)
            .Include(b => b.Passenger);
    }

    private async Task<BookingDto?> GetDtoAsync(Guid id)
    {
        var booking = await BookingQuery().FirstOrDefaultAsync(b => b.Id == id);
        return booking is null ? null : ToDto(booking);
    }

    private static BookingDto ToDto(Booking booking)
    {
        var trip = booking.Trip!;
        var route = trip.Route!;
        var stops = route.Stops.OrderBy(rs => rs.Sequence).ToList();
        return new BookingDto(
            booking.Id,
            booking.Reference,
            booking.TripId,
            route.Code,
            route.Name,
            stops.FirstOrDefault()?.Stop?.Name ?? string.Empty,
            stops.LastOrDefault()?.Stop?.Name ?? string.Empty,
            trip.DepartureAt,
            trip.Bus?.Registration,
            booking.Amount,
            booking.Status,
            booking.Method,
            booking.PaymentStatus,
            booking.CreatedAt,
            booking.CancelledAt,
            booking.Passenger?.FullName ?? string.Empty,
            booking.Passenger?.Email ?? string.Empty);
    }
}
