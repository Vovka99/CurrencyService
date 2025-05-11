namespace CurrencyService.Models;

public class CurrencyRate
{
    public string CurrencyCode { get; init; }
    public decimal Rate { get; init; }
    public DateTime Date { get; init; }
}
