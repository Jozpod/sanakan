using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace Sanakan.ShindenApi.Fake.Builder
{
    [ExcludeFromCodeCoverage]
    public static class Extensions
    {
        public static IServiceCollection AddFakeShindenApi(this IServiceCollection services)
        {
            var databasePath = Path.GetFullPath(Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "../../../../", "FakeShinden.db"));
            services.AddDbContextPool<WebScrapedDbContext>((optionsBuilder) =>
            {
                optionsBuilder.UseSqlite($"Data Source={databasePath};");
            });
            services.AddSingleton<IShindenClient, FakeShindenClient>();
            services.AddHttpClient();
            services.AddSingleton<ShindenWebScraper>();
            return services;
        }
    }
}
