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
            .Select(b => new BusDto(b.Id, b.Registration, b.Capacity, b.Ownership, b.VendorId, b.Vendor != null ? b.Vendor.Name : null, b.Active))
            .ToListAsync();
        return Ok(buses);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<BusDto>> Create(BusRequest request)
    {
        if (request.Ownership == Ownership.Vendor && request.VendorId is null)
            return BadRequest(new { message = "Vendor-owned buses need a vendorId." });

        var registration = request.Registration.Trim();
        if (await _db.Buses.AnyAsync(b => b.Registration == registration))
            return Conflict(new { message = $"Bus {registration} is already registered." });
        if (request.VendorId is not null && !await _db.Vendors.AnyAsync(v => v.Id == request.VendorId.Value))
            return BadRequest(new { message = "Vendor not found." });

        var bus = new Bus
        {
            Registration = registration,
            Capacity = request.Capacity,
            Ownership = request.Ownership,
            VendorId = request.VendorId,
            Active = request.Active
        };
        _db.Buses.Add(bus);
        await _db.SaveChangesAsync();
        return Created($"/api/buses/{bus.Id}", new BusDto(bus.Id, bus.Registration, bus.Capacity, bus.Ownership, bus.VendorId, null, bus.Active));
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(Guid id, BusRequest request)
    {
        var bus = await _db.Buses.FindAsync(id);
        if (bus is null)
            return NotFound();

        if (request.Ownership == Ownership.Vendor && request.VendorId is null)
            return BadRequest(new { message = "Vendor-owned buses need a vendorId." });

        var registration = request.Registration.Trim();
        if (await _db.Buses.AnyAsync(b => b.Id != id && b.Registration == registration))
            return Conflict(new { message = $"Bus {registration} is already registered." });
        if (request.VendorId is not null && !await _db.Vendors.AnyAsync(v => v.Id == request.VendorId.Value))
            return BadRequest(new { message = "Vendor not found." });

        var activeCounts = await _db.Trips
            .Where(t => t.BusId == id)
            .Select(t => new
            {
                Count = t.Bookings.Count(b =>
                    b.Status == BookingStatus.Reserved ||
                    b.Status == BookingStatus.Confirmed ||
                    b.Status == BookingStatus.Boarded)
            })
            .ToListAsync();
        if (activeCounts.Any(x => x.Count > request.Capacity))
            return Conflict(new { message = "Bus capacity is lower than active bookings on one of its trips." });

        bus.Registration = registration;
        bus.Capacity = request.Capacity;
        bus.Ownership = request.Ownership;
        bus.VendorId = request.VendorId;
        bus.Active = request.Active;
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
