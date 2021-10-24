using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sanakan.Common.Builder;
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
        private static SanakanDbContext _dbContext;
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
            serviceCollection.AddDbContext<SanakanDbContext>();
            serviceCollection.AddSingleton(configurationRoot);
            serviceCollection.AddCache(configurationRoot.GetSection("Cache"));
            serviceCollection.Configure<SanakanConfiguration>(configurationRoot);
            serviceCollection.AddRepositories();
            ServiceProvider = serviceCollection.BuildServiceProvider();

            _dbContext = ServiceProvider.GetRequiredService<SanakanDbContext>();
            await _dbContext.Database.EnsureCreatedAsync();
        }

        [AssemblyCleanup()]
        public static async Task AssemblyCleanup()
        {
            if (_dbContext != null)
            {
                await _dbContext.Database.EnsureDeletedAsync();
            }
        }
    }
}
