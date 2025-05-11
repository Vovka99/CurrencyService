using CurrencyService.Database.Repositories;
using CurrencyService.HttpClients;

namespace CurrencyService.Services.Impl;

public class CurrencyRateService : ICurrencyRateService
{
    private const int DATE_SYNC_MONTHS = 3;
    
    private readonly ICurrencyRateApiClient _currencyRateApiClient;
    private readonly ICurrencyRateRepository _currencyRateRepository;

    public CurrencyRateService(ICurrencyRateRepository currencyRateRepository, ICurrencyRateApiClient currencyRateApiClient)
    {
        _currencyRateApiClient = currencyRateApiClient;
        _currencyRateRepository = currencyRateRepository;
    }

    public async Task<decimal> GetRateAsync(DateTime date, CancellationToken cancellationToken)
    {
        var rate = await _currencyRateRepository.GetAsync(date, cancellationToken);
        
        return rate?.Rate ?? 0;
    }

    public async Task<decimal> GetAverageRateAsync(DateTime start, DateTime end, CancellationToken cancellationToken)
    {
        if (end > DateTime.UtcNow.Date)
        {
            end = DateTime.UtcNow.Date;
        }

        return await _currencyRateRepository.GetAverageAsync(start, end, cancellationToken);
    }

    public async Task SyncRatesAsync(CancellationToken cancellationToken)
    {
        var latestDate = await _currencyRateRepository.GetLatestDateAsync(cancellationToken);

        var startDate = latestDate?.AddDays(1) ?? DateTime.UtcNow.Date.AddMonths(-DATE_SYNC_MONTHS);

        var rates = await _currencyRateApiClient.GetCurrencyRatesAsync(startDate);
        if (rates.Count > 0)
        {
            await _currencyRateRepository.AddAsync(rates, cancellationToken);
        }
    }
}
