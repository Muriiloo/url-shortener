namespace UrlShortener.Application.Abstractions;

public interface IUrlCache
{
    Task<string?> GetAsync(string shortCode);
    Task SetAsync(string shortCode, string longUrl, TimeSpan expiration);
}
