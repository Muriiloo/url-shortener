using StackExchange.Redis;
using UrlShortener.Application.Abstractions;

namespace UrlShortener.Infra.Cache;

public class RedisUrlCache : IUrlCache
{
    private readonly IDatabase _database;

    public RedisUrlCache(IConnectionMultiplexer database)
    {
        _database = database.GetDatabase();
    }

    public async Task<string?> GetAsync(string shortCode)
    {
        var cacheKey = $"url:{shortCode}";
        var url = await _database.StringGetAsync(cacheKey);

        return url.HasValue ? url.ToString() : null;
    }

    public async Task SetAsync(string shortCode, string longUrl, TimeSpan expiration)
    {
        var cacheKey = $"url:{shortCode}";
        await _database.StringSetAsync(cacheKey, longUrl, expiration);
    }
}
