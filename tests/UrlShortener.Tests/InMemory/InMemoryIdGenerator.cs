using UrlShortener.Application.Abstractions;

namespace UrlShortener.Tests.InMemory;

public class InMemoryIdGenerator : IIdGenerator
{
    private long _current;

    public InMemoryIdGenerator(long start = 0)
    {
        _current = start;
    }

    public Task<long> GenerateAsync()
    {
        return Task.FromResult(Interlocked.Increment(ref _current));
    }
}
