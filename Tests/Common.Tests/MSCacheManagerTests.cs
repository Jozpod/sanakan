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
    public class MSCacheManagerTests
    {
        private ServiceProvider _serviceProvider;

        public MSCacheManagerTests()
        {
            var serviceCollection = new ServiceCollection();

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            var configurationRoot = builder.Build();
            
            serviceCollection.AddOptions();
            serviceCollection.AddCache();
            serviceCollection.AddSingleton(configurationRoot);
            serviceCollection.Configure<MSCacheManagerOptions>(configurationRoot.GetSection("Cache"));
            _serviceProvider = serviceCollection.BuildServiceProvider();
        }

        [TestMethod]
        public async Task Should_Cache()
        {
            var cacheManager = _serviceProvider.GetRequiredService<ICacheManager>();
            var result = cacheManager.Get<Entity>("test");
            result.Should().BeNull();
            cacheManager.Add("test", new Entity());
            result = cacheManager.Get<Entity>("test");
            result.Should().NotBeNull();
        }

        public class Entity
        {
            public int Id { get; set; }
            public string Value { get; set; }
        }
    }
}
