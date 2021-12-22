using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sanakan.Common.Builder;
using Sanakan.ShindenApi.Fake.Builder;
using System;
using System.IO;
using System.Threading.Tasks;


namespace Sanakan.ShindenApi.Fake
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var serviceCollection = new ServiceCollection();
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            var configurationRoot = builder.Build();

            serviceCollection.AddOptions();
            serviceCollection.AddSystemClock();
            serviceCollection.AddSingleton(configurationRoot);
            serviceCollection.AddCache(configurationRoot.GetSection("Cache"));
            serviceCollection.AddConfiguration(configurationRoot);
            serviceCollection.AddFileSystem();
            serviceCollection.AddScoped<Importer>();
            serviceCollection.AddFakeShindenApi();
            serviceCollection.AddLogging(config => config.AddConsole());

             IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();
            var serviceScopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using var serviceScope = serviceScopeFactory.CreateScope();
            serviceProvider = serviceScope.ServiceProvider;

            var importer = serviceProvider.GetRequiredService<Importer>();

            await importer.RunAsync();
        }
    }
}
