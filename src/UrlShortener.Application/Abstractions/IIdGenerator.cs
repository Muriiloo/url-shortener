namespace UrlShortener.Application.Abstractions;

public interface IIdGenerator
{
    Task<long> GenerateAsync();
}
