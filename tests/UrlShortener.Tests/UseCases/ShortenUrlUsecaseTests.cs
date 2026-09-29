using Microsoft.Extensions.Options;
using UrlShortener.Application.Options;
using UrlShortener.Application.UseCases;
using UrlShortener.Tests.InMemory;

namespace UrlShortener.Tests.UseCases;

public class ShortenUrlUsecaseTests
{
    private const string BaseUrl = "http://localhost:5000";

    private readonly InMemoryUrlRepository _repository = new();
    private readonly InMemoryIdGenerator _idGenerator = new();
    private readonly FakeHashingShortCode _hashing = new();
    private readonly ShortenUrlUsecase _useCase;

    public ShortenUrlUsecaseTests()
    {
        var options = Options.Create(new BaseUrlOptions { Url = BaseUrl });
        _useCase = new ShortenUrlUsecase(_repository, options, _idGenerator, _hashing);
    }

    [Fact]
    public async Task Execute_ValidUrl_ReturnsShortUrlWithBaseUrlAndShortCode()
    {
        var result = await _useCase.Execute("https://www.google.com");

        Assert.Equal($"{BaseUrl}/code1", result);
    }

    [Fact]
    public async Task Execute_ValidUrl_SavesUrlInRepository()
    {
        const string longUrl = "https://www.google.com";

        await _useCase.Execute(longUrl);

        var saved = Assert.Single(_repository.Urls);
        Assert.Equal("code1", saved.Key);
        Assert.Equal(longUrl, saved.Value);
    }

    [Fact]
    public async Task Execute_MultipleUrls_SavesEachWithDistinctShortCode()
    {
        var first = await _useCase.Execute("https://www.google.com");
        var second = await _useCase.Execute("https://www.github.com");

        Assert.NotEqual(first, second);
        Assert.Equal(2, _repository.Urls.Count);
        Assert.Equal("https://www.google.com", _repository.Urls["code1"]);
        Assert.Equal("https://www.github.com", _repository.Urls["code2"]);
    }

    [Fact]
    public async Task Execute_SameUrlTwice_GeneratesDifferentShortCodes()
    {
        const string longUrl = "https://www.google.com";

        var first = await _useCase.Execute(longUrl);
        var second = await _useCase.Execute(longUrl);

        Assert.NotEqual(first, second);
        Assert.All(_repository.Urls.Values, value => Assert.Equal(longUrl, value));
    }

    [Fact]
    public async Task Execute_EmptyUrl_ThrowsInvalidCastException()
    {
        var exception = await Assert.ThrowsAsync<InvalidCastException>(() => _useCase.Execute(string.Empty));

        Assert.Equal("Invalid URL.", exception.Message);
    }

    [Fact]
    public async Task Execute_EmptyUrl_DoesNotSaveInRepository()
    {
        await Assert.ThrowsAsync<InvalidCastException>(() => _useCase.Execute(string.Empty));

        Assert.Empty(_repository.Urls);
    }
}
