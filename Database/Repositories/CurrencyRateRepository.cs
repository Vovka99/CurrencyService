using CurrencyService.Models;
using Dapper;

namespace CurrencyService.Database.Repositories;

public class CurrencyRateRepository : ICurrencyRateRepository
{
    private readonly IDbConnectionFactory _dbConnectionFactory;
    
    public CurrencyRateRepository(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }
    
    public async Task<CurrencyRate> GetCurrencyRateAsync(DateTime date, CancellationToken cancellationToken)
    {
        const string sql = """
                           SELECT date, rate, currency_code
                           FROM currency_rates
                           WHERE date = @date
                           """;

        using var connection = _dbConnectionFactory.CreateConnection();
        var command = new CommandDefinition(sql, date, cancellationToken: cancellationToken);
        return await connection.QuerySingleOrDefaultAsync<CurrencyRate>(command);
    }

    public async Task AddCurrencyRateAsync(CurrencyRate currencyRate, CancellationToken cancellationToken)
    {
        const string sql = """
                           INSERT INTO currency_rates (date, rate, currency_code)
                           VALUES (@Date, @Rate, @CurrencyCode)
                           ON CONFLICT (date, currency_code) DO NOTHING;
                           """;

        using var connection = _dbConnectionFactory.CreateConnection();
        var command = new CommandDefinition(sql, currencyRate, cancellationToken: cancellationToken);
        await connection.ExecuteAsync(command);
    }
    
    public async Task<decimal> GetAverageRateAsync(DateTime start, DateTime end, CancellationToken cancellationToken)
    {
        const string sql = """
                           SELECT AVG(rate)
                           FROM currency_rates
                           WHERE date BETWEEN @start AND @end
                           """;

        using var connection = _dbConnectionFactory.CreateConnection();
        var command = new CommandDefinition(sql, new { start, end }, cancellationToken: cancellationToken);
        return await connection.ExecuteScalarAsync<decimal>(command);
    }
    
    public async Task<DateTime?> GetLatestDateAsync(CancellationToken cancellationToken)
    {
        const string sql = """
                           SELECT MAX(date)
                           FROM currency_rates
                           """;

        using var connection = _dbConnectionFactory.CreateConnection();
        return await connection.ExecuteScalarAsync<DateTime?>(sql);
    }
}
