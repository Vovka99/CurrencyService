using CurrencyService.Models;

namespace CurrencyService.Database.Repositories;

public interface ICurrencyRateRepository
{
    Task AddAsync(IEnumerable<CurrencyRate> currencyRates, CancellationToken cancellationToken);
    Task<CurrencyRate> GetAsync(DateTime date, CancellationToken cancellationToken);
    Task<decimal> GetAverageAsync(DateTime start, DateTime end, CancellationToken cancellationToken);
    Task<DateTime?> GetLatestDateAsync(CancellationToken cancellationToken);
}
