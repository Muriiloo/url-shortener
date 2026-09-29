using UrlShortener.Infra.HashingShortCode;

namespace UrlShortener.Tests.Infra;

public class SqidsGeneratorTests
{
    private readonly SqidsGenerator _generator = new();

    [Fact]
    public void Hashing_SameId_ReturnsSameCode()
    {
        Assert.Equal(_generator.Hashing(42), _generator.Hashing(42));
    }

    [Fact]
    public void Hashing_DifferentIds_ReturnDifferentCodes()
    {
        var codes = Enumerable.Range(1, 1000).Select(id => _generator.Hashing(id)).ToList();

        Assert.Equal(codes.Count, codes.Distinct().Count());
    }

    [Theory]
    [InlineData(1)]
    [InlineData(123456)]
    [InlineData(long.MaxValue)]
    public void Hashing_ValidId_ReturnsAlphanumericCode(long id)
    {
        var code = _generator.Hashing(id);

        Assert.False(string.IsNullOrWhiteSpace(code));
        Assert.All(code, c => Assert.True(char.IsAsciiLetterOrDigit(c)));
    }
}
