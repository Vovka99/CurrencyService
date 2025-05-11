namespace CurrencyService.Services;

public interface ICurrencyRateService
{
    Task<decimal> GetRateAsync(DateTime date, CancellationToken cancellationToken);
    Task<decimal> GetAverageRateAsync(DateTime start, DateTime end, CancellationToken cancellationToken);
    Task SyncRatesAsync(CancellationToken cancellationToken);
}
