using Cassandra;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using UrlShortener.Application.Abstractions;
using UrlShortener.Domain.Repository;
using UrlShortener.Infra.Cache;
using UrlShortener.Infra.HashingShortCode;
using UrlShortener.Infra.IdGenerator;
using UrlShortener.Infra.Repositories.CassandraRepository;

namespace UrlShortener.Infra;

public static class DependencyInjection
{
    public static IServiceCollection AddInfra(this IServiceCollection services, IConfiguration configuration)
    {
        var redisConfig = configuration.GetSection("Redis").Get<RedisConfig>();

        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = redisConfig.Configuration;
            options.InstanceName = redisConfig.InstanceName;
        });

        var cluster = Cluster.Builder()
            .AddContactPoint("localhost")
            .WithPort(9042)
            .Build();

        var session = cluster.Connect("url_shortener");
        services.AddSingleton<ISession>(session);

        services.AddSingleton<IConnectionMultiplexer>(_ =>
        ConnectionMultiplexer.Connect(redisConfig.Configuration));

        services.AddSingleton<IIdGenerator, RedisIdGenerator>();
        

        services.AddScoped<IUrlRepository, CassandraUrlRepository>();
        services.AddScoped<IHashingShortCode, SqidsGenerator>();
        services.AddSingleton<IUrlCache, RedisUrlCache>();

        return services;
    }
}
