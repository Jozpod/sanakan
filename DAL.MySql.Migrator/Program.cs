using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MySqlConnector;
using Sanakan.Common.Builder;
using Sanakan.Common.Configuration;
using Sanakan.DAL.Builder;
using Sanakan.DAL.MySql.Builder;
using Sanakan.DAL.MySql.Migrator.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;


namespace Sanakan.DAL.MySql.Migrator
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var serviceCollection = new ServiceCollection();

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            var configurationRoot = builder.Build();

            serviceCollection.AddLogging();
            serviceCollection.AddOptions();
            serviceCollection.AddSystemClock();
            serviceCollection.AddSanakanDbContext(configurationRoot);
            serviceCollection.AddSingleton(configurationRoot);
            serviceCollection.Configure<SourceDatabaseConfiguration>(configurationRoot.GetSection("SourceDatabase"));
            serviceCollection.AddCache(configurationRoot.GetSection("Cache"));
            serviceCollection.Configure<SanakanConfiguration>(configurationRoot);
            serviceCollection.AddDatabaseMigrator();
            serviceCollection.AddFileSystem();
            serviceCollection.AddScoped((serviceProvider) =>
            {
                var dbContext = serviceProvider.GetRequiredService<SanakanDbContext>();
                return dbContext.Database;
            });

            IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();
            var serviceScopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using var serviceScope = serviceScopeFactory.CreateScope();
            serviceProvider = serviceScope.ServiceProvider;

            var databaseMigrator = serviceProvider.GetRequiredService<DatabaseMigrator>();
            await databaseMigrator.RunAsync();
        }
    }
}
