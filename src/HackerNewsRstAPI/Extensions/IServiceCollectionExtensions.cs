using HackerNewsRstAPI.Controllers;
using HackerNewsRstAPI.Data;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System.Configuration;
using System.Net.Http;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddRedisCache(this IServiceCollection services, IConfiguration configuration)
    {
        var options = configuration.GetRequiredSection(RedisOptions.SectionName).Get<RedisOptions>() ?? throw new ConfigurationErrorsException();
        services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(options.ConnectionString, configure => configure.Password = options.Password));
        services.AddSingleton(ctx => ctx.GetRequiredService<IConnectionMultiplexer>().GetDatabase());

        services.Decorate<INewsProvider, RedisNewsProvider>();
        return services;
    }

    public static IServiceCollection AddHackerNews(this IServiceCollection services, IConfiguration configuration)
    {
        var settings = configuration.GetRequiredSection(HackerNewsOptions.SectionName).Get<HackerNewsOptions>() ?? throw new ConfigurationErrorsException();
        var options = Options.Create(settings);
        services.AddSingleton(options);
        services.AddHttpClient(HackerNewsOptions.SectionName, httpClient =>
        {
            httpClient.BaseAddress = settings.Url;
        });
        services.AddSingleton<INewsProvider, HackerNewProvider>();
        return services;
    }
}
