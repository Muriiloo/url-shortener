using System.Collections.Concurrent;
using UrlShortener.Application.Abstractions;

namespace UrlShortener.Tests.InMemory;

public class InMemoryUrlCache : IUrlCache
{
    private readonly ConcurrentDictionary<string, CacheEntry> _entries = new();
    private readonly TimeProvider _timeProvider;

    public InMemoryUrlCache() : this(TimeProvider.System)
    {
    }

    public InMemoryUrlCache(TimeProvider timeProvider)
    {
        _timeProvider = timeProvider;
    }

    public IReadOnlyDictionary<string, CacheEntry> Entries => _entries;

    public Task<string?> GetAsync(string shortCode)
    {
        if (!_entries.TryGetValue(shortCode, out var entry))
            return Task.FromResult<string?>(null);

        if (entry.ExpiresAt <= _timeProvider.GetUtcNow())
        {
            _entries.TryRemove(shortCode, out _);
            return Task.FromResult<string?>(null);
        }

        return Task.FromResult<string?>(entry.LongUrl);
    }

    public Task SetAsync(string shortCode, string longUrl, TimeSpan expiration)
    {
        _entries[shortCode] = new CacheEntry(longUrl, expiration, _timeProvider.GetUtcNow().Add(expiration));
        return Task.CompletedTask;
    }

    public record CacheEntry(string LongUrl, TimeSpan Expiration, DateTimeOffset ExpiresAt);
}
