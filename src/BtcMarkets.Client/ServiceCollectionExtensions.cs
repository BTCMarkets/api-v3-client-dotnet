using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace BtcMarkets.Client;

public static class ServiceCollectionExtensions
{
    public static IHttpClientBuilder AddBtcMarkets(this IServiceCollection services, Action<BtcMarketsOptions> configureOptions)
    {
        services.Configure(configureOptions);
        
        return services.AddHttpClient<IBtcMarketsClient, BtcMarketsClient>()
            .ConfigureHttpClient((sp, client) =>
            {
                var options = sp.GetRequiredService<IOptions<BtcMarketsOptions>>().Value;
                if (!string.IsNullOrEmpty(options.BaseUrl))
                {
                    client.BaseAddress = new Uri(options.BaseUrl);
                }
            });
    }
}
