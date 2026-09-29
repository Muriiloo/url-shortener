namespace UrlShortener.Domain.Entities;

public sealed class Url
{
    public Url(string shortCode, string longUrl, DateTime createdAt)
    {
        ShortCode = shortCode;
        LongUrl = longUrl;
        CreatedAt = createdAt;
    }

    public string ShortCode { get; private set; } = string.Empty;
    public string LongUrl { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
}
