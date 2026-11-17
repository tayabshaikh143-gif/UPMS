using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UPMS.Api.Data;

namespace UPMS.Api.Bookings;

[ApiController]
[Route("api/audit")]
[Authorize(Roles = "Admin")]
public class AuditController : ControllerBase
{
    private readonly AppDbContext _db;

    public AuditController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<AuditLogDto>>> List([FromQuery] int take = 100)
    {
        take = Math.Clamp(take, 1, 500);
        var logs = await _db.AuditLogs.AsNoTracking()
            .OrderByDescending(a => a.At)
            .Take(take)
            .Select(a => new AuditLogDto(
                a.Id,
                a.ActorId,
                a.ActorName,
                a.Action,
                a.Entity,
                a.EntityId,
                a.Details,
                a.At))
            .ToListAsync();
        return Ok(logs);
    }
}

public record AuditLogDto(
    Guid Id,
    Guid? ActorId,
    string? ActorName,
    string Action,
    string Entity,
    Guid? EntityId,
    string? Details,
    DateTime At);
