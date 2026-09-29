using StackExchange.Redis;
using UrlShortener.Application.Abstractions;

namespace UrlShortener.Infra.IdGenerator;

public class RedisIdGenerator : IIdGenerator
{
    private readonly IDatabase _database;

    public RedisIdGenerator(IConnectionMultiplexer connection)
    {
        _database = connection.GetDatabase();
    }

    public async Task<long> GenerateAsync()
    {
        return await _database.StringIncrementAsync("url_shortener:id");
    }
}
