using CurrencyService.Models;

namespace CurrencyService.HttpClients;

public interface ICurrencyRateApiClient
{
    Task<List<CurrencyRate>> GetCurrencyRatesAsync(DateTime startDate, DateTime endDate);
}