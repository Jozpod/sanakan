﻿using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sanakan.Common;
using Sanakan.Common.Builder;
using System.IO;
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
            serviceCollection.AddCache(configurationRoot.GetSection("Cache"));
            serviceCollection.AddSingleton(configurationRoot);
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
            cacheManager.ExpireTag("test");
            result = cacheManager.Get<Entity>("test");
            result.Should().BeNull();
        }

        public class Entity
        {
            public int Id { get; set; }
            public string Value { get; set; }
        }
    }
}