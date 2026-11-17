using System.Globalization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UPMS.Api.Bookings;
using UPMS.Api.Data;

namespace UPMS.Api.Catalog;

[ApiController]
[Route("api/vendors")]
[Authorize]
public class VendorsController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IClock _clock;

    public VendorsController(AppDbContext db, IClock clock)
    {
        _db = db;
        _clock = clock;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<VendorDto>>> List()
    {
        var vendors = await _db.Vendors.AsNoTracking()
            .OrderBy(v => v.Name)
            .Select(v => new VendorDto(
                v.Id,
                v.Name,
                v.Status,
                v.Buses.Count,
                v.UserId,
                v.Contact,
                v.MonthlySubscription,
                v.SubscriptionStatus))
            .ToListAsync();
        return Ok(vendors);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<VendorDto>> Create(VendorRequest request)
    {
        var vendor = new Vendor
        {
            Name = request.Name.Trim(),
            Status = request.Status,
            UserId = request.UserId,
            Contact = request.Contact?.Trim(),
            MonthlySubscription = request.MonthlySubscription,
            SubscriptionStatus = request.SubscriptionStatus
        };
        _db.Vendors.Add(vendor);
        await _db.SaveChangesAsync();
        return Created($"/api/vendors/{vendor.Id}", new VendorDto(
            vendor.Id,
            vendor.Name,
            vendor.Status,
            0,
            vendor.UserId,
            vendor.Contact,
            vendor.MonthlySubscription,
            vendor.SubscriptionStatus));
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
        vendor.UserId = request.UserId;
        vendor.Contact = request.Contact?.Trim();
        vendor.MonthlySubscription = request.MonthlySubscription;
        vendor.SubscriptionStatus = request.SubscriptionStatus;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpGet("{id:guid}/settlements")]
    [Authorize(Roles = "Admin,Staff")]
    public async Task<ActionResult<IReadOnlyList<VendorSettlementDto>>> Settlements(Guid id)
    {
        if (!await _db.Vendors.AnyAsync(v => v.Id == id))
            return NotFound();

        var settlements = await _db.VendorSettlements.AsNoTracking()
            .Where(s => s.VendorId == id)
            .OrderByDescending(s => s.Period)
            .Select(s => new VendorSettlementDto(
                s.Id,
                s.VendorId,
                s.Period,
                s.GrossFares,
                s.SubscriptionDue,
                s.Status,
                s.PaidAt,
                s.CreatedAt))
            .ToListAsync();
        return Ok(settlements);
    }

    [HttpPost("{id:guid}/settlements")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<VendorSettlementDto>> AddSettlement(Guid id, VendorSettlementRequest request)
    {
        if (!await _db.Vendors.AnyAsync(v => v.Id == id))
            return NotFound();

        if (!DateTime.TryParseExact(
                request.Period,
                "yyyy-MM",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out _))
            return BadRequest(new { message = "Period must be in YYYY-MM format." });

        if (await _db.VendorSettlements.AnyAsync(s => s.VendorId == id && s.Period == request.Period))
            return Conflict(new { message = "A settlement for this period already exists." });

        var settlement = new VendorSettlement
        {
            VendorId = id,
            Period = request.Period,
            GrossFares = request.GrossFares,
            SubscriptionDue = request.SubscriptionDue,
            Status = request.Status,
            PaidAt = request.Status == SettlementStatus.Paid ? _clock.Now : null,
            CreatedAt = _clock.Now
        };
        _db.VendorSettlements.Add(settlement);
        await _db.SaveChangesAsync();
        return Created(
            $"/api/vendors/{id}/settlements/{settlement.Id}",
            new VendorSettlementDto(
                settlement.Id,
                settlement.VendorId,
                settlement.Period,
                settlement.GrossFares,
                settlement.SubscriptionDue,
                settlement.Status,
                settlement.PaidAt,
                settlement.CreatedAt));
    }
}
