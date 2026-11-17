using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UPMS.Api.Data;

namespace UPMS.Api.Bookings;

[ApiController]
[Route("api/notifications")]
[Authorize]
public class NotificationsController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IClock _clock;

    public NotificationsController(AppDbContext db, IClock clock)
    {
        _db = db;
        _clock = clock;
    }

    [HttpGet("my")]
    public async Task<ActionResult<IReadOnlyList<NotificationDto>>> My()
    {
        var userId = CurrentUserId();
        if (userId is null)
            return Unauthorized();

        var notifications = await _db.Notifications.AsNoTracking()
            .Where(n => n.RecipientId == userId.Value)
            .OrderByDescending(n => n.SentAt)
            .Select(n => new NotificationDto(
                n.Id,
                n.Message,
                n.TripId,
                n.BookingId,
                n.Channel,
                n.SentAt,
                n.ReadAt))
            .ToListAsync();
        return Ok(notifications);
    }

    [HttpPost("{id:guid}/read")]
    public async Task<IActionResult> MarkRead(Guid id)
    {
        var userId = CurrentUserId();
        if (userId is null)
            return Unauthorized();

        var notification = await _db.Notifications.FirstOrDefaultAsync(n =>
            n.Id == id && n.RecipientId == userId.Value);
        if (notification is null)
            return NotFound();

        notification.ReadAt ??= _clock.Now;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    private Guid? CurrentUserId()
    {
        var raw = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(raw, out var id) ? id : null;
    }
}

public record NotificationDto(
    Guid Id,
    string Message,
    Guid? TripId,
    Guid? BookingId,
    NotificationChannel Channel,
    DateTime SentAt,
    DateTime? ReadAt);
