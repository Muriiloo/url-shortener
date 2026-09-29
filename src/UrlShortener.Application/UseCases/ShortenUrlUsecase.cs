using Microsoft.Extensions.Options;
using UrlShortener.Application.Abstractions;
using UrlShortener.Application.Options;
using UrlShortener.Domain.Repository;

namespace UrlShortener.Application.UseCases;

public class ShortenUrlUsecase
{
    private readonly IUrlRepository _urlRepo;
    private readonly IIdGenerator _idGenerator;
    private readonly BaseUrlOptions _options;
    private readonly IHashingShortCode _hashing;
    public ShortenUrlUsecase(IUrlRepository urlRepo, IOptions<BaseUrlOptions> options, IIdGenerator idGenerator, IHashingShortCode hashing)
    {
        _urlRepo = urlRepo;
        _options = options.Value;
        _idGenerator = idGenerator;
        _hashing = hashing;
    }

    public async Task<string> Execute(string longUrl)
    {
        if (longUrl.Length == 0)
            throw new InvalidCastException("Invalid URL.");

        var id = await _idGenerator.GenerateAsync();

        var shortCode = _hashing.Hashing(id);

        var shortUrl = $"{_options.Url}/{shortCode}";

        await _urlRepo.SaveUrl(shortCode, longUrl);

        return shortUrl;
    }
}
