using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UrlShortener.Application.Options;
using UrlShortener.Application.UseCases;

namespace UrlShortener.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection service, IConfiguration configure)
    {
        service.Configure<BaseUrlOptions>(
            configure.GetSection("BaseUrl"));
        service.Configure<BaseUrlOptions>(configure.GetSection("BaseUrl"));
        service.AddScoped<ShortenUrlUsecase>();
        service.AddScoped<RedirectLongUrlUseCase>();
        return service;
    }
}
