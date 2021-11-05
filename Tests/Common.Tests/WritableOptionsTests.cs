using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sanakan.Common;
using Sanakan.Common.Builder;
using Sanakan.Common.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sanakan.DAL.Tests
{
    [TestClass]
    public class WritableOptionsTests
    {
        private ServiceProvider _serviceProvider;

        public WritableOptionsTests()
        {
            var serviceCollection = new ServiceCollection();

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            var configurationRoot = builder.Build();
            
            serviceCollection.AddOptions();
            serviceCollection.AddWritableOption<SanakanConfiguration>(configurationRoot.GetSection(""));
            serviceCollection.AddSingleton(configurationRoot);
            _serviceProvider = serviceCollection.BuildServiceProvider();
        }

        [TestMethod]
        public async Task Should_Cache()
        {
            var sanakanConfiguration = _serviceProvider.GetRequiredService<IWritableOptions<SanakanConfiguration>>();
        }
    }
}
