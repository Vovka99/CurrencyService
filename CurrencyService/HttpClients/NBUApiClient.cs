using System.Globalization;
using System.Text.Json;
using CurrencyService.Models.DTOs;
using CurrencyService.Models;

namespace CurrencyService.HttpClients;

public class NBUApiClient : ICurrencyRateApiClient
{
    private const string CURRENCY_CODE = "USD";
    
    private readonly HttpClient _httpClient;

    public NBUApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<CurrencyRate>> GetCurrencyRatesAsync(DateTime? startDate, DateTime? endDate = null)
    {
        var url = "https://bank.gov.ua/NBU_Exchange/exchange_site?";

        if (startDate.HasValue)
        {
            url += $"start={startDate.Value:yyyyMMdd}&";
        }

        if (endDate.HasValue)
        {
            url += $"end={endDate.Value:yyyyMMdd}&";
        }

        url += $"&valcode={CURRENCY_CODE}&sort=exchangedate&order=desc&json";

        var response = await _httpClient.GetAsync(url);
        
        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException($"Unable to get current rates: {response.StatusCode}");
        }

        var responseString = await response.Content.ReadAsStringAsync();
        var rates = JsonSerializer.Deserialize<List<NbuCurrencyRateDto>>(responseString,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        if (rates == null)
        {
            throw new InvalidOperationException("Invalid data received from NBU");
        }

        return rates.Select(r => new CurrencyRate
        {
            CurrencyCode = r.Cc,
            Rate = r.Rate,
            Date = DateTime.ParseExact(r.ExchangeDate,"dd.MM.yyyy", CultureInfo.InvariantCulture)
        }).ToList();
    }
}
