using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sanakan.Common.Builder;
using Sanakan.Common.Configuration;
using Sanakan.Configuration;
using Sanakan.DAL.Builder;
using Sanakan.DAL.Repositories.Abstractions;
using System.IO;
using System.Threading.Tasks;

namespace Sanakan.DAL.Tests
{
    [TestClass]
    public class TestBase
    {
        protected static SanakanDbContext DbContext;
        protected static ServiceProvider ServiceProvider;

        [AssemblyInitialize()]
        public static async Task AssemblyInitialize(TestContext context)
        {
            var serviceCollection = new ServiceCollection();

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            var configurationRoot = builder.Build();

            serviceCollection.AddOptions();
            serviceCollection.AddSanakanDbContext(configurationRoot);
            serviceCollection.AddSingleton(configurationRoot);
            serviceCollection.AddCache(configurationRoot.GetSection("Cache"));
            serviceCollection.Configure<DatabaseConfiguration>(configurationRoot.GetSection("Database"));
            serviceCollection.Configure<SanakanConfiguration>(configurationRoot);
            serviceCollection.AddRepositories();
            ServiceProvider = serviceCollection.BuildServiceProvider();

            DbContext = ServiceProvider.GetRequiredService<SanakanDbContext>();

            try
            {
                await DbContext.Database.EnsureCreatedAsync();
            }
            catch (System.Exception ex)
            {

            }
        }

        [AssemblyCleanup()]
        public static async Task AssemblyCleanup()
        {
            if (DbContext != null)
            {
                await DbContext.Database.EnsureDeletedAsync();
            }
        }
    }
}
