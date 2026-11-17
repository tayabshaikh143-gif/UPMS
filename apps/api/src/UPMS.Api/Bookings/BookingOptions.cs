using Microsoft.Extensions.Options;

namespace UPMS.Api.Bookings;

public class BookingOptions
{
    public string TimeZoneId { get; set; } = "Asia/Karachi";
    public int CancellationCutoffMinutes { get; set; } = 20;
    public int CashNoShowMinutes { get; set; } = 5;
    public int BoardingStartMinutes { get; set; } = 20;
    public int OnlinePaymentHoldMinutes { get; set; } = 10;
}

public interface IClock
{
    DateTime Now { get; }
}

public class Clock : IClock
{
    private readonly TimeZoneInfo _timeZone;

    public Clock(IOptions<BookingOptions> options)
    {
        _timeZone = ResolveTimeZone(options.Value.TimeZoneId);
    }

    public DateTime Now => TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, _timeZone);

    private static TimeZoneInfo ResolveTimeZone(string timeZoneId)
    {
        try
        {
            return TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
        }
        catch (TimeZoneNotFoundException)
        {
            // Windows uses "Pakistan Standard Time" for Asia/Karachi.
            return TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");
        }
        catch (InvalidTimeZoneException)
        {
            return TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");
        }
    }
}
