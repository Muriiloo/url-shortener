namespace UrlShortener.Application.Abstractions;

public interface IHashingShortCode
{
    string Hashing(long id);
}
