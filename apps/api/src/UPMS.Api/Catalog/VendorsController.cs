using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UPMS.Api.Data;

namespace UPMS.Api.Catalog;

[ApiController]
[Route("api/vendors")]
[Authorize]
public class VendorsController : ControllerBase
{
    private readonly AppDbContext _db;

    public VendorsController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<VendorDto>>> List()
    {
        var vendors = await _db.Vendors.AsNoTracking()
            .OrderBy(v => v.Name)
            .Select(v => new VendorDto(v.Id, v.Name, v.Status, v.Buses.Count))
            .ToListAsync();
        return Ok(vendors);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<VendorDto>> Create(VendorRequest request)
    {
        var vendor = new Vendor { Name = request.Name.Trim(), Status = request.Status };
        _db.Vendors.Add(vendor);
        await _db.SaveChangesAsync();
        return Created($"/api/vendors/{vendor.Id}", new VendorDto(vendor.Id, vendor.Name, vendor.Status, 0));
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(Guid id, VendorRequest request)
    {
        var vendor = await _db.Vendors.FindAsync(id);
        if (vendor is null)
            return NotFound();

        vendor.Name = request.Name.Trim();
        vendor.Status = request.Status;
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
