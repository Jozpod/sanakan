using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Sanakan.ShindenApi.Builder
{
    public static class Extensions
    {
        public static IServiceCollection AddShindenApi(this IServiceCollection services)
        {
            services.AddHttpClient<IShindenClient, ShindenClient>((serviceProvider, httpClient) =>
            {
                var options = serviceProvider.GetRequiredService<IOptionsMonitor<ShindenClientOptions>>();

                httpClient.DefaultRequestHeaders.Add("Accept-Language", "pl");
                httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
                httpClient.DefaultRequestHeaders.Add("User-Agent", $"{options.CurrentValue.UserAgent}");

                if (options.CurrentValue.Marmolade != null)
                {
                    httpClient.DefaultRequestHeaders.Add(options.CurrentValue.Marmolade, "marmolada");
                }
            });
            return services;
        }
    }
}
