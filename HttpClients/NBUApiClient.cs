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

    public async Task<List<CurrencyRate>> GetCurrencyRatesAsync(DateTime startDate, DateTime endDate)
    {
        var start = startDate.ToString("yyyyMMdd");
        var end = endDate.ToString("yyyyMMdd");

        var response = await _httpClient.GetAsync(
            $"https://bank.gov.ua/NBU_Exchange/exchange_site?start={start}&end={end}&valcode={CURRENCY_CODE}&sort=exchangedate&order=desc&json");
        
        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException($"Unable to get current rate: {response.StatusCode}");
        }

        var responseString = await response.Content.ReadAsStringAsync();
        var rates = JsonSerializer.Deserialize<List<NbuCurrencyRateDto>>(responseString,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        
        return rates?.Select(r => new CurrencyRate
        {
            CurrencyCode = r.Cc,
            Rate = r.Rate,
            Date = DateTime.ParseExact(r.ExchangeDate,"dd.MM.yyyy", CultureInfo.InvariantCulture)
        }).ToList() ?? [];
    }
}
