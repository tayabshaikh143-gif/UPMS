namespace UPMS.Api.Bookings;

public class BookingExpiryService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<BookingExpiryService> _logger;

    public BookingExpiryService(IServiceScopeFactory scopeFactory, ILogger<BookingExpiryService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var bookings = scope.ServiceProvider.GetRequiredService<BookingService>();
                var expired = await bookings.ExpireCashBookingsAsync(stoppingToken);
                if (expired > 0)
                    _logger.LogInformation("Expired {Count} unboarded cash booking(s).", expired);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cash booking expiry sweep failed.");
            }

            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
        }
    }
}
