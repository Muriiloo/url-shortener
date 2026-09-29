using UrlShortener.Application.Exceptions;
using UrlShortener.Application.UseCases;
using UrlShortener.Tests.InMemory;

namespace UrlShortener.Tests.UseCases;

public class RedirectLongUrlUseCaseTests
{
    private const string ShortCode = "abc123";
    private const string LongUrl = "https://www.google.com";

    private readonly InMemoryUrlRepository _repository = new();
    private readonly FakeTimeProvider _timeProvider = new(new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero));
    private readonly InMemoryUrlCache _cache;
    private readonly RedirectLongUrlUseCase _useCase;

    public RedirectLongUrlUseCaseTests()
    {
        _cache = new InMemoryUrlCache(_timeProvider);
        _useCase = new RedirectLongUrlUseCase(_repository, _cache);
    }

    [Fact]
    public async Task ExecuteAsync_UrlInCache_ReturnsCachedUrlWithoutQueryingRepository()
    {
        await _cache.SetAsync(ShortCode, LongUrl, TimeSpan.FromMinutes(1));

        var result = await _useCase.ExecuteAsync(ShortCode);

        Assert.Equal(LongUrl, result);
        Assert.Equal(0, _repository.GetUrlCalls);
    }

    [Fact]
    public async Task ExecuteAsync_UrlNotInCacheButInRepository_ReturnsUrlFromRepository()
    {
        await _repository.SaveUrl(ShortCode, LongUrl);

        var result = await _useCase.ExecuteAsync(ShortCode);

        Assert.Equal(LongUrl, result);
        Assert.Equal(1, _repository.GetUrlCalls);
    }

    [Fact]
    public async Task ExecuteAsync_UrlNotInCacheButInRepository_StoresUrlInCacheForTenSeconds()
    {
        await _repository.SaveUrl(ShortCode, LongUrl);

        await _useCase.ExecuteAsync(ShortCode);

        var entry = Assert.Contains(ShortCode, _cache.Entries);
        Assert.Equal(LongUrl, entry.LongUrl);
        Assert.Equal(TimeSpan.FromSeconds(10), entry.Expiration);
    }

    [Fact]
    public async Task ExecuteAsync_SecondCall_ReturnsFromCacheWithoutQueryingRepositoryAgain()
    {
        await _repository.SaveUrl(ShortCode, LongUrl);

        await _useCase.ExecuteAsync(ShortCode);
        var result = await _useCase.ExecuteAsync(ShortCode);

        Assert.Equal(LongUrl, result);
        Assert.Equal(1, _repository.GetUrlCalls);
    }

    [Fact]
    public async Task ExecuteAsync_CacheExpired_QueriesRepositoryAgain()
    {
        await _repository.SaveUrl(ShortCode, LongUrl);

        await _useCase.ExecuteAsync(ShortCode);
        _timeProvider.Advance(TimeSpan.FromSeconds(11));
        var result = await _useCase.ExecuteAsync(ShortCode);

        Assert.Equal(LongUrl, result);
        Assert.Equal(2, _repository.GetUrlCalls);
    }

    [Fact]
    public async Task ExecuteAsync_UrlNotFound_ThrowsNotFoundException()
    {
        var exception = await Assert.ThrowsAsync<NotFoundException>(() => _useCase.ExecuteAsync(ShortCode));

        Assert.Equal("LongURL not found.", exception.Message);
    }

    [Fact]
    public async Task ExecuteAsync_UrlNotFound_DoesNotStoreInCache()
    {
        await Assert.ThrowsAsync<NotFoundException>(() => _useCase.ExecuteAsync(ShortCode));

        Assert.Empty(_cache.Entries);
    }

    [Fact]
    public async Task ExecuteAsync_EmptyShortCode_ThrowsInvalidCastException()
    {
        var exception = await Assert.ThrowsAsync<InvalidCastException>(() => _useCase.ExecuteAsync(string.Empty));

        Assert.Equal("Invalid shortCode.", exception.Message);
        Assert.Equal(0, _repository.GetUrlCalls);
    }
}
