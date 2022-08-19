using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sanakan.Common.Builder;
using Sanakan.DAL.Builder;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Sanakan.DAL.MySql.Schema
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            var serviceCollection = new ServiceCollection();
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environmentName}.json", optional: true, reloadOnChange: true);

            var configurationRoot = builder.Build();

            serviceCollection.AddOptions();
            serviceCollection.AddSystemClock();
            serviceCollection.AddSanakanDbContext(configurationRoot);
            serviceCollection.AddSingleton(configurationRoot);
            serviceCollection.AddCache(configurationRoot.GetSection("Cache"));
            serviceCollection.AddConfiguration(configurationRoot);
            serviceCollection.AddRepositories();
            serviceCollection.AddFileSystem();
            serviceCollection.AddSingleton<TableScripter>();
            serviceCollection.AddSingleton<TestDataGenerator>();
            serviceCollection.AddSingleton<QueryScripter>();
            serviceCollection.AddScoped((serviceProvider) =>
            {
                var dbContext = serviceProvider.GetRequiredService<SanakanDbContext>();
                return dbContext.Database;
            });

            IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();
            var serviceScopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using var serviceScope = serviceScopeFactory.CreateScope();
            serviceProvider = serviceScope.ServiceProvider;

            var dbContext = serviceProvider.GetRequiredService<SanakanDbContext>();
            var tableScripter = serviceProvider.GetRequiredService<TableScripter>();
            var testDataGenerator = serviceProvider.GetRequiredService<TestDataGenerator>();
            var queryScripter = serviceProvider.GetRequiredService<QueryScripter>();
            var databaseFacade = dbContext.Database;

            var created = await databaseFacade.EnsureCreatedAsync();
            await tableScripter.RunAsync();

            if (created)
            {
                await testDataGenerator.RunAsync();
            }

            await queryScripter.RunAsync();
            await databaseFacade.EnsureDeletedAsync();
        }
    }
}
