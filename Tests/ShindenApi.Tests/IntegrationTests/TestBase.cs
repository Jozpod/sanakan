using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sanakan.Common.Builder;
using Sanakan.ShindenApi.Builder;
using System.IO;
using System.Threading.Tasks;

namespace Sanakan.ShindenApi.Tests.IntegrationTests
{
    [TestClass]
    public class TestBase
    {
        private readonly IShindenClient _shindenClient;

        public TestBase()
        {
            var services = new ServiceCollection();
            var builder = new ConfigurationBuilder()
              .SetBasePath(Directory.GetCurrentDirectory())
              .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
              .AddEnvironmentVariables();

            var configurationRoot = builder.Build();

            services.AddOptions();
            services.AddSystemClock();
            services.AddConfiguration(configurationRoot);
            services.AddShindenApi();

            var serviceProvider = services.BuildServiceProvider();

            _shindenClient = serviceProvider.GetRequiredService<IShindenClient>();
        }

        [TestMethod]
        public async Task Should_Log_In()
        {
            var result = await _shindenClient.LoginAsync("test", "test");
            result.Value.Should().BeNull();
        }
    }
}
