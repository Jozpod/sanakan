using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sanakan.Common;
using Sanakan.Common.Builder;
using Sanakan.Common.Cache;
using System.IO;
using System.Threading.Tasks;

namespace Sanakan.Common.Tests
{
    /// <summary>
    /// Defines tests for <see cref="MSCacheManager"/> class.
    /// </summary>
    [TestClass]
    public class MSCacheManagerTests
    {
        private ServiceProvider _serviceProvider;
        private readonly ICacheManager _cacheManager;

        public MSCacheManagerTests()
        {
            var serviceCollection = new ServiceCollection();

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            var configurationRoot = builder.Build();

            serviceCollection.AddOptions();
            serviceCollection.AddCache(configurationRoot.GetSection("Cache"));
            serviceCollection.AddSingleton(configurationRoot);
            _serviceProvider = serviceCollection.BuildServiceProvider();
            _cacheManager = _serviceProvider.GetRequiredService<ICacheManager>();
        }

        [TestMethod]
        public void Should_Cache()
        {
            var result = _cacheManager.Get<Entity>("test");
            result.Should().BeNull();

            _cacheManager.Add("test", new Entity());
            result = _cacheManager.Get<Entity>("test");
            result.Should().NotBeNull();
        }

        [TestMethod]
        public void Should_Expire_Cache()
        {
            _cacheManager.Add("test", new Entity());
            _cacheManager.ExpireTag("test");
            var result = _cacheManager.Get<Entity>("test");
            result.Should().BeNull();

            _cacheManager.Add("test", new Entity(), "parent");
            _cacheManager.ExpireTag("parent");
            result = _cacheManager.Get<Entity>("test");
            result.Should().BeNull();
        }

        public class Entity
        {
            public int Id { get; set; }

            public string Value { get; set; } = string.Empty;
        }
    }
}
