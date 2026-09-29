using UrlShortener.Application.Abstractions;
using UrlShortener.Application.Exceptions;
using UrlShortener.Domain.Repository;

namespace UrlShortener.Application.UseCases;

public class RedirectLongUrlUseCase
{
    private readonly IUrlRepository _urlRepo;
    private readonly IUrlCache _urlCache;
    public RedirectLongUrlUseCase(IUrlRepository urlRepo, IUrlCache urlCache)
    {
        _urlRepo = urlRepo;
        _urlCache = urlCache;
    }

    public async Task<string?> ExecuteAsync(string shortCode)
    
    {
        if (shortCode.Length <= 0)
            throw new InvalidCastException("Invalid shortCode.");

        var urlCaching = await _urlCache.GetAsync(shortCode);

        if (urlCaching is not null)
            return urlCaching;

        var longUrl = await _urlRepo.GetUrl(shortCode);

        if (longUrl is null)
            throw new NotFoundException("LongURL not found.");

        await _urlCache.SetAsync(shortCode, longUrl, TimeSpan.FromSeconds(10));

        return longUrl;
    }
}
