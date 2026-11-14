using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UPMS.Api.Data;
using Route = UPMS.Api.Data.Route;

namespace UPMS.Api.Catalog;

[ApiController]
[Route("api/trips")]
[Authorize]
public class TripsController : ControllerBase
{
    private readonly AppDbContext _db;

    public TripsController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<TripDto>>> List([FromQuery] Guid? routeId)
    {
        var query = _db.Trips.AsNoTracking()
            .Include(t => t.Route!).ThenInclude(r => r.Stops).ThenInclude(rs => rs.Stop)
            .Include(t => t.Bus)
            .Include(t => t.Bookings)
            .AsSplitQuery()
            .AsQueryable();
        if (routeId is not null)
            query = query.Where(t => t.RouteId == routeId);

        var trips = await query
            .OrderBy(t => t.DepartureAt)
            .ToListAsync();
        return Ok(trips.Select(t => ToDto(t, t.Route!)).ToList());
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<TripDto>> Create(TripRequest request)
    {
        var route = await _db.Routes
            .Include(r => r.Stops).ThenInclude(rs => rs.Stop)
            .FirstOrDefaultAsync(r => r.Id == request.RouteId);
        if (route is null)
            return BadRequest(new { message = "Route not found." });

        if (request.BusId is null)
            return BadRequest(new { message = "A trip needs an assigned bus." });

        var bus = await _db.Buses.FindAsync(request.BusId.Value);
        if (bus is null)
            return BadRequest(new { message = "Bus not found." });

        var trip = new Trip
        {
            RouteId = request.RouteId,
            Route = route,
            BusId = bus.Id,
            Bus = bus,
            DepartureAt = request.DepartureAt,
            Fare = request.Fare,
            Status = request.Status
        };
        _db.Trips.Add(trip);
        await _db.SaveChangesAsync();
        return Created($"/api/trips/{trip.Id}", ToDto(trip, route));
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(Guid id, TripRequest request)
    {
        var trip = await _db.Trips.FindAsync(id);
        if (trip is null)
            return NotFound();
        if (!await _db.Routes.AnyAsync(r => r.Id == request.RouteId))
            return BadRequest(new { message = "Route not found." });
        if (request.BusId is null)
            return BadRequest(new { message = "A trip needs an assigned bus." });

        var bus = await _db.Buses.FindAsync(request.BusId.Value);
        if (bus is null)
            return BadRequest(new { message = "Bus not found." });

        var activeBookings = await _db.Bookings.CountAsync(b =>
            b.TripId == id &&
            (b.Status == BookingStatus.Reserved || b.Status == BookingStatus.Confirmed || b.Status == BookingStatus.Boarded));
        if (bus.Capacity < activeBookings)
            return Conflict(new { message = $"Bus capacity ({bus.Capacity}) is lower than the {activeBookings} active booking(s) on this trip." });

        trip.RouteId = request.RouteId;
        trip.BusId = bus.Id;
        trip.DepartureAt = request.DepartureAt;
        trip.Fare = request.Fare;
        trip.Status = request.Status;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var trip = await _db.Trips.FindAsync(id);
        if (trip is null)
            return NotFound();

        if (await _db.Bookings.AnyAsync(b => b.TripId == id))
            return Conflict(new { message = "Trip has bookings and cannot be deleted." });

        _db.Trips.Remove(trip);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    private static TripDto ToDto(Trip trip, Route route)
    {
        var stops = route.Stops.OrderBy(rs => rs.Sequence).ToList();
        var origin = stops.FirstOrDefault()?.Stop?.Name;
        var destination = stops.LastOrDefault()?.Stop?.Name;
        var capacity = trip.Bus?.Capacity ?? 0;
        var booked = trip.Bookings.Count(b =>
            b.Status == BookingStatus.Reserved ||
            b.Status == BookingStatus.Confirmed ||
            b.Status == BookingStatus.Boarded);
        return new TripDto(
            trip.Id,
            route.Id,
            route.Code,
            route.Name,
            origin ?? string.Empty,
            destination ?? string.Empty,
            trip.DepartureAt,
            trip.Fare,
            trip.Status,
            trip.BusId,
            trip.Bus?.Registration,
            capacity,
            booked,
            Math.Max(0, capacity - booked));
    }
}
