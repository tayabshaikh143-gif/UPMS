using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UPMS.Api.Data;

namespace UPMS.Api.Catalog;

[ApiController]
[Route("api/buses")]
[Authorize]
public class BusesController : ControllerBase
{
    private readonly AppDbContext _db;

    public BusesController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<BusDto>>> List()
    {
        var buses = await _db.Buses.AsNoTracking()
            .OrderBy(b => b.Registration)
            .Select(b => new BusDto(b.Id, b.Registration, b.Capacity, b.Ownership, b.VendorId, b.Vendor != null ? b.Vendor.Name : null))
            .ToListAsync();
        return Ok(buses);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<BusDto>> Create(BusRequest request)
    {
        if (request.Ownership == Ownership.Vendor && request.VendorId is null)
            return BadRequest(new { message = "Vendor-owned buses need a vendorId." });

        var bus = new Bus
        {
            Registration = request.Registration.Trim(),
            Capacity = request.Capacity,
            Ownership = request.Ownership,
            VendorId = request.VendorId
        };
        _db.Buses.Add(bus);
        await _db.SaveChangesAsync();
        return Created($"/api/buses/{bus.Id}", new BusDto(bus.Id, bus.Registration, bus.Capacity, bus.Ownership, bus.VendorId, null));
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(Guid id, BusRequest request)
    {
        var bus = await _db.Buses.FindAsync(id);
        if (bus is null)
            return NotFound();

        bus.Registration = request.Registration.Trim();
        bus.Capacity = request.Capacity;
        bus.Ownership = request.Ownership;
        bus.VendorId = request.VendorId;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var bus = await _db.Buses.FindAsync(id);
        if (bus is null)
            return NotFound();

        _db.Buses.Remove(bus);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
