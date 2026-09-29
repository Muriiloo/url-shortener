using System.Collections.Concurrent;
using UrlShortener.Domain.Repository;

namespace UrlShortener.Tests.InMemory;

public class InMemoryUrlRepository : IUrlRepository
{
    private readonly ConcurrentDictionary<string, string> _urls = new();

    public int GetUrlCalls { get; private set; }

    public IReadOnlyDictionary<string, string> Urls => _urls;

    public Task SaveUrl(string shortCode, string longUrl)
    {
        _urls[shortCode] = longUrl;
        return Task.CompletedTask;
    }

    public Task<string?> GetUrl(string shortCode)
    {
        GetUrlCalls++;
        return Task.FromResult(_urls.TryGetValue(shortCode, out var longUrl) ? longUrl : null);
    }
}
