using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sanakan.Common.Builder;
using Sanakan.DiscordBot;
using Sanakan.DiscordBot.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sanakan.Common.Tests
{
    /// <summary>
    /// Defines tests for <see cref="IResourceManager.GetResourceStream(string)"/> method.
    /// </summary>
    [TestClass]
    public class ResourceManagerTests
    {
        private ServiceProvider _serviceProvider;
        private readonly IResourceManager _resourceManager;

        public ResourceManagerTests()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddFileSystem();
            serviceCollection.AddResourceManager();
            _serviceProvider = serviceCollection.BuildServiceProvider();
            _resourceManager = _serviceProvider.GetRequiredService<IResourceManager>();
        }
    }
}
