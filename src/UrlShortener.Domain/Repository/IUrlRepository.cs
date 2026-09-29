namespace UrlShortener.Domain.Repository;

public interface IUrlRepository
{
    Task SaveUrl(string shortCode, string longUrl);
    Task<string?> GetUrl(string shortCode);
}
