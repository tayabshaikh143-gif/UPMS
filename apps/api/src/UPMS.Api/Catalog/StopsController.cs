using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UPMS.Api.Data;

namespace UPMS.Api.Catalog;

[ApiController]
[Route("api/stops")]
[Authorize]
public class StopsController : ControllerBase
{
    private readonly AppDbContext _db;

    public StopsController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<StopDto>>> List()
    {
        var stops = await _db.Stops.AsNoTracking()
            .OrderBy(s => s.Name)
            .Select(s => new StopDto(s.Id, s.Name, s.Latitude, s.Longitude))
            .ToListAsync();
        return Ok(stops);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<StopDto>> Create(StopRequest request)
    {
        var stop = new Stop { Name = request.Name.Trim(), Latitude = request.Latitude, Longitude = request.Longitude };
        _db.Stops.Add(stop);
        await _db.SaveChangesAsync();
        return Created($"/api/stops/{stop.Id}", new StopDto(stop.Id, stop.Name, stop.Latitude, stop.Longitude));
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(Guid id, StopRequest request)
    {
        var stop = await _db.Stops.FindAsync(id);
        if (stop is null)
            return NotFound();

        stop.Name = request.Name.Trim();
        stop.Latitude = request.Latitude;
        stop.Longitude = request.Longitude;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var stop = await _db.Stops.FindAsync(id);
        if (stop is null)
            return NotFound();

        if (await _db.RouteStops.AnyAsync(rs => rs.StopId == id))
            return Conflict(new { message = "Stop is used by a route and cannot be deleted." });

        _db.Stops.Remove(stop);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
