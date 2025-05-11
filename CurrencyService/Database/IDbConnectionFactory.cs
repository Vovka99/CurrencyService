using Npgsql;

namespace CurrencyService.Database;

public interface IDbConnectionFactory
{
    NpgsqlConnection CreateConnection();
}
