using System.Globalization;
using System.Security.Claims;
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
    [Authorize(Roles = "Admin,Staff")]
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
        if (request.UserId.HasValue)
            vendor.UserId = request.UserId;
        vendor.Contact = request.Contact?.Trim();
        vendor.MonthlySubscription = request.MonthlySubscription;
        vendor.SubscriptionStatus = request.SubscriptionStatus;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpGet("me")]
    [Authorize(Roles = "Vendor")]
    public async Task<ActionResult<VendorProfileDto>> Me()
    {
        var userId = CurrentUserId();
        if (userId is null)
            return Unauthorized();

        var vendor = await _db.Vendors.AsNoTracking()
            .Include(v => v.User)
            .Include(v => v.Buses)
            .FirstOrDefaultAsync(v => v.UserId == userId.Value);
        if (vendor is null)
            return NotFound(new { message = "No vendor application is linked to this account." });

        return Ok(new VendorProfileDto(
            vendor.Id,
            vendor.Name,
            vendor.Status,
            vendor.Contact,
            vendor.MonthlySubscription,
            vendor.SubscriptionStatus,
            vendor.Buses.Count,
            vendor.User?.FullName ?? string.Empty,
            vendor.User?.Email ?? string.Empty));
    }

    [HttpGet("me/buses")]
    [Authorize(Roles = "Vendor")]
    public async Task<ActionResult<IReadOnlyList<BusDto>>> MyBuses()
    {
        var vendor = await GetCurrentVendorAsync();
        if (vendor is null)
            return NotFound(new { message = "No vendor application is linked to this account." });
        if (vendor.Status != VendorStatus.Approved)
            return StatusCode(StatusCodes.Status403Forbidden, new { message = "Vendor application is not approved yet." });

        var buses = await _db.Buses.AsNoTracking()
            .Where(b => b.VendorId == vendor.Id)
            .OrderBy(b => b.Registration)
            .Select(b => new BusDto(
                b.Id,
                b.Registration,
                b.Capacity,
                b.Ownership,
                b.VendorId,
                vendor.Name,
                b.Active))
            .ToListAsync();
        return Ok(buses);
    }

    [HttpGet("me/trips")]
    [Authorize(Roles = "Vendor")]
    public async Task<ActionResult<IReadOnlyList<VendorTripDto>>> MyTrips()
    {
        var vendor = await GetCurrentVendorAsync();
        if (vendor is null)
            return NotFound(new { message = "No vendor application is linked to this account." });
        if (vendor.Status != VendorStatus.Approved)
            return StatusCode(StatusCodes.Status403Forbidden, new { message = "Vendor application is not approved yet." });

        var trips = await _db.Trips.AsNoTracking()
            .Include(t => t.Route!).ThenInclude(r => r.Stops).ThenInclude(rs => rs.Stop)
            .Include(t => t.Bus)
            .Include(t => t.Bookings)
            .Include(t => t.OfflineTickets)
            .Where(t => t.Bus != null && t.Bus.VendorId == vendor.Id)
            .AsSplitQuery()
            .OrderByDescending(t => t.DepartureAt)
            .ToListAsync();

        return Ok(trips.Select(ToVendorTripDto).ToList());
    }

    [HttpGet("me/settlements")]
    [Authorize(Roles = "Vendor")]
    public async Task<ActionResult<IReadOnlyList<VendorSettlementDto>>> MySettlements()
    {
        var vendor = await GetCurrentVendorAsync();
        if (vendor is null)
            return NotFound(new { message = "No vendor application is linked to this account." });
        if (vendor.Status != VendorStatus.Approved)
            return StatusCode(StatusCodes.Status403Forbidden, new { message = "Vendor application is not approved yet." });

        var settlements = await _db.VendorSettlements.AsNoTracking()
            .Where(s => s.VendorId == vendor.Id)
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

    private Guid? CurrentUserId()
    {
        var raw = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(raw, out var id) ? id : null;
    }

    private async Task<Vendor?> GetCurrentVendorAsync()
    {
        var userId = CurrentUserId();
        if (userId is null)
            return null;
        return await _db.Vendors.AsNoTracking()
            .FirstOrDefaultAsync(v => v.UserId == userId.Value);
    }

    private static VendorTripDto ToVendorTripDto(Trip trip)
    {
        var stops = trip.Route?.Stops.OrderBy(rs => rs.Sequence).ToList() ?? new List<RouteStop>();
        var capacity = trip.Bus?.Capacity ?? 0;
        var booked = trip.Bookings.Count(b =>
            b.Status == BookingStatus.Reserved ||
            b.Status == BookingStatus.Confirmed ||
            b.Status == BookingStatus.Boarded);
        booked += trip.OfflineTickets.Count(t =>
            t.Status == OfflineTicketStatus.Issued ||
            t.Status == OfflineTicketStatus.Boarded);
        return new VendorTripDto(
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
            Math.Max(0, capacity - booked));
    }
}
