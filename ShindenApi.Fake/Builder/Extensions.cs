using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace Sanakan.ShindenApi.Fake.Builder
{
    [ExcludeFromCodeCoverage]
    public static class Extensions
    {
        public static IServiceCollection AddFakeShindenApi(this IServiceCollection services)
        {
            services.AddDbContextPool<WebScrapedDbContext>((optionsBuilder) =>
            {
                optionsBuilder.UseSqlite("Data Source=FakeShinden.db;");
            });
            services.AddSingleton<IShindenClient, FakeShindenClient>();
            services.AddSingleton<ShindenWebScraper>();
            return services;
        }
    }
}
