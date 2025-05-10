using CurrencyService.Services;

namespace CurrencyService.BackgroundServices;

public class CurrencyRatesFetcher : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;

    public CurrencyRatesFetcher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        DateTime? syncDate = null;
        while (!stoppingToken.IsCancellationRequested)
        {
            if (syncDate == null || syncDate.Value.Date < DateTime.UtcNow.Date)
            {
                using var scope = _serviceProvider.CreateScope();
                var currencyRateService = scope.ServiceProvider.GetRequiredService<ICurrencyRateService>();
                await currencyRateService.SyncRatesAsync(stoppingToken);
                syncDate = DateTime.UtcNow.Date;
            }

            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }
}
