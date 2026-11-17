using UPMS.Api.Data;

namespace UPMS.Api.Bookings;

public class AuditService
{
    private readonly AppDbContext _db;
    private readonly IClock _clock;

    public AuditService(AppDbContext db, IClock clock)
    {
        _db = db;
        _clock = clock;
    }

    public void Add(
        Guid? actorId,
        string action,
        string entity,
        Guid? entityId,
        string? details = null,
        string? actorName = null)
    {
        _db.AuditLogs.Add(new AuditLog
        {
            ActorId = actorId,
            ActorName = actorName,
            Action = action,
            Entity = entity,
            EntityId = entityId,
            Details = details,
            At = _clock.Now
        });
    }
}
