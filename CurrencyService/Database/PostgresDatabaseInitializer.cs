using Dapper;

namespace CurrencyService.Database;

public class PostgresDatabaseInitializer : IDatabaseInitializer
{
    private readonly IDbConnectionFactory _dbConnectionFactory;
    
    public PostgresDatabaseInitializer(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task InitializeAsync()
    {
        const string sql = """
                           CREATE TABLE IF NOT EXISTS currency_rates (
                               date DATE NOT NULL,
                               rate DECIMAL(10, 4) NOT NULL,
                               currency_code VARCHAR(3) NOT NULL,
                               PRIMARY KEY (date, currency_code)
                           )
                           """;

        using var connection = _dbConnectionFactory.CreateConnection();
        await connection.ExecuteAsync(sql);
    }
}
