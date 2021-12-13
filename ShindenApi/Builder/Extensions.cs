using System.Diagnostics.CodeAnalysis;
using System.Net;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Sanakan.Common.Configuration;

namespace Sanakan.ShindenApi.Builder
{
    [ExcludeFromCodeCoverage]
    public static class Extensions
    {
        public static IServiceCollection AddShindenApi(this IServiceCollection services)
        {
            services.AddTransient<CookieContainer>();
            services.AddHttpClient<IShindenClient, ShindenClient>((serviceProvider, httpClient) =>
            {
                var options = serviceProvider.GetRequiredService<IOptionsMonitor<ShindenApiConfiguration>>();

                httpClient.BaseAddress = options.CurrentValue.BaseUrl;
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
