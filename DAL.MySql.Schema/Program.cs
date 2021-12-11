using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sanakan.Common;
using Sanakan.Common.Builder;
using Sanakan.Common.Configuration;
using Sanakan.DAL.Builder;
using Sanakan.DAL.Models;
using Sanakan.DAL.Models.Configuration;
using Sanakan.DAL.Models.Management;
using Sanakan.DAL.Repositories.Abstractions;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Sanakan.DAL.MySql.Schema
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
