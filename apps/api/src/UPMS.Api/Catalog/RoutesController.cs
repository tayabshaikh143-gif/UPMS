using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UPMS.Api.Data;
using Route = UPMS.Api.Data.Route;

namespace UPMS.Api.Catalog;

[ApiController]
[Route("api/routes")]
[Authorize]
public class RoutesController : ControllerBase
{
    private readonly AppDbContext _db;

    public RoutesController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<RouteDto>>> List()
    {
        var routes = await _db.Routes.AsNoTracking()
            .OrderBy(r => r.Code)
            .Select(r => new RouteDto(r.Id, r.Code, r.Name, r.Active, r.Stops.Count))
            .ToListAsync();
        return Ok(routes);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<RouteDetailDto>> Get(Guid id)
    {
        var route = await _db.Routes.AsNoTracking()
            .Include(r => r.Stops).ThenInclude(rs => rs.Stop)
            .FirstOrDefaultAsync(r => r.Id == id);
        if (route is null)
            return NotFound();

        var stops = route.Stops
            .OrderBy(rs => rs.Sequence)
            .Select(rs => new RouteStopDto(rs.Id, rs.StopId, rs.Stop?.Name ?? string.Empty, rs.Sequence, rs.OffsetMinutes))
            .ToList();
        return Ok(new RouteDetailDto(route.Id, route.Code, route.Name, route.Active, stops));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<RouteDto>> Create(RouteRequest request)
    {
        var code = request.Code.Trim();
        if (await _db.Routes.AnyAsync(r => r.Code == code))
            return Conflict(new { message = $"Route code {code} already exists." });

        var route = new Route { Code = code, Name = request.Name.Trim(), Active = request.Active };
        _db.Routes.Add(route);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = route.Id }, new RouteDto(route.Id, route.Code, route.Name, route.Active, 0));
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(Guid id, RouteRequest request)
    {
        var route = await _db.Routes.FindAsync(id);
        if (route is null)
            return NotFound();

        var code = request.Code.Trim();
        if (await _db.Routes.AnyAsync(r => r.Id != id && r.Code == code))
            return Conflict(new { message = $"Route code {code} already exists." });

        route.Code = code;
        route.Name = request.Name.Trim();
        route.Active = request.Active;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpPost("{id:guid}/stops")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AddStop(Guid id, RouteStopRequest request)
    {
        if (!await _db.Routes.AnyAsync(r => r.Id == id) || !await _db.Stops.AnyAsync(s => s.Id == request.StopId))
            return BadRequest(new { message = "Route or stop not found." });

        _db.RouteStops.Add(new RouteStop
        {
            RouteId = id,
            StopId = request.StopId,
            Sequence = request.Sequence,
            OffsetMinutes = request.OffsetMinutes
        });
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpPost("{id:guid}/stops/by-name")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AppendStop(Guid id, RouteStopNameRequest request)
    {
        var route = await _db.Routes.Include(r => r.Stops).FirstOrDefaultAsync(r => r.Id == id);
        if (route is null)
            return NotFound();

        var name = request.Name.Trim();
        if (name.Length == 0)
            return BadRequest(new { message = "Stop name is required." });

        var stop = await _db.Stops.FirstOrDefaultAsync(s => s.Name == name);
        if (stop is null)
        {
            stop = new Stop { Name = name };
            _db.Stops.Add(stop);
        }

        if (route.Stops.Any(rs => rs.StopId == stop.Id))
            return Conflict(new { message = $"{name} is already on this route." });

        var last = route.Stops.OrderBy(rs => rs.Sequence).LastOrDefault();
        _db.RouteStops.Add(new RouteStop
        {
            RouteId = id,
            StopId = stop.Id,
            Sequence = (last?.Sequence ?? 0) + 1,
            OffsetMinutes = last is null ? 0 : last.OffsetMinutes + 15
        });
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:guid}/stops/{routeStopId:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> RemoveStop(Guid id, Guid routeStopId)
    {
        var routeStop = await _db.RouteStops.FirstOrDefaultAsync(rs => rs.Id == routeStopId && rs.RouteId == id);
        if (routeStop is null)
            return NotFound();

        _db.RouteStops.Remove(routeStop);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var route = await _db.Routes.FindAsync(id);
        if (route is null)
            return NotFound();

        if (await _db.Trips.AnyAsync(t => t.RouteId == id))
            return Conflict(new { message = "Route has trips and cannot be deleted." });

        _db.Routes.Remove(route);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
