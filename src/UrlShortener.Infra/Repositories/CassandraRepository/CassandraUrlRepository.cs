using Cassandra;
using UrlShortener.Domain.Repository;

namespace UrlShortener.Infra.Repositories.CassandraRepository;

internal sealed class CassandraUrlRepository : IUrlRepository
{
    private readonly ISession _session;
    public CassandraUrlRepository(ISession session)
    {
        _session = session;
    }

    public async Task<string?> GetUrl(string shortCode)
    {
        var query = "select long_url from urls where short_code = ?";

        var result = await _session.ExecuteAsync(new SimpleStatement(query, shortCode));

        var url = result.FirstOrDefault();

        if (url is null)
            return null;

        return url.GetValue<string>("long_url");
    }

    public async Task SaveUrl(string shortCode, string longUrl)
    {
        var query = "insert into urls (short_code, long_url, created_at) values (?,?,?)";

        await _session.ExecuteAsync(new SimpleStatement(query, shortCode, longUrl, DateTime.UtcNow));
    }
}
