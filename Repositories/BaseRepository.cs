using Npgsql;
using System.Data;
using Twittertask.Settings;

namespace Twittertask.Repositories;

public class BaseRepository
{

    private readonly IConfiguration _configuration;
    public BaseRepository(IConfiguration configuration)
    {
        _configuration = configuration;
        Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
    }

    public NpgsqlConnection NewConnection => new NpgsqlConnection(_configuration
    .GetSection(nameof(PostgresSettings)).Get<PostgresSettings>().ConnectionString);




}