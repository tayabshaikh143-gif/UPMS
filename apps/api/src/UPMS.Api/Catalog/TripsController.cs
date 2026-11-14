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

        var trip = new Trip
        {
            RouteId = request.RouteId,
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

        trip.RouteId = request.RouteId;
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

        _db.Trips.Remove(trip);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    private static TripDto ToDto(Trip trip, Route route)
    {
        var stops = route.Stops.OrderBy(rs => rs.Sequence).ToList();
        var origin = stops.FirstOrDefault()?.Stop?.Name;
        var destination = stops.LastOrDefault()?.Stop?.Name;
        return new TripDto(
            trip.Id,
            route.Id,
            route.Code,
            route.Name,
            origin ?? string.Empty,
            destination ?? string.Empty,
            trip.DepartureAt,
            trip.Fare,
            trip.Status);
    }
}
