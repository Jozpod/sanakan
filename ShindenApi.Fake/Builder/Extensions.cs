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
            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var fileName = "FakeShinden.db";
#if DEBUG
            var databasePath = Path.Combine(baseDirectory, "../../../../", fileName);
#else
            var databasePath = Path.Combine(baseDirectory, fileName);
#endif
            databasePath = Path.GetFullPath(databasePath);

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
