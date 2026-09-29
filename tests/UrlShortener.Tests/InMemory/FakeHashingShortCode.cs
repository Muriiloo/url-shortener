using UrlShortener.Application.Abstractions;

namespace UrlShortener.Tests.InMemory;

public class FakeHashingShortCode : IHashingShortCode
{
    public string Hashing(long id) => $"code{id}";
}
