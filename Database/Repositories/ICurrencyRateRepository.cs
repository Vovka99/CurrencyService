using CurrencyService.Models;

namespace CurrencyService.Database.Repositories;

public interface ICurrencyRateRepository
{
    Task AddCurrencyRateAsync(CurrencyRate currencyRate, CancellationToken cancellationToken);
    Task<CurrencyRate> GetCurrencyRateAsync(DateTime date, CancellationToken cancellationToken);
    Task<decimal> GetAverageRateAsync(DateTime start, DateTime end, CancellationToken cancellationToken);
    Task<DateTime?> GetLatestDateAsync(CancellationToken cancellationToken);
}
