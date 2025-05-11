using Dapper;
using Npgsql;

namespace CurrencyService.Database;

public class PostgresDatabaseInitializer : IDatabaseInitializer
{
    private readonly IConfiguration _configuration;

    public PostgresDatabaseInitializer(IConfiguration configuration)
    {
        _configuration = configuration;
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

        using var connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        await connection.ExecuteAsync(sql);
    }
}
