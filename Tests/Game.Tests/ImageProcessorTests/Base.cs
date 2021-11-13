using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.Common;
using Sanakan.DAL.Models;
using Sanakan.DiscordBot.Services;
using Sanakan.ShindenApi;
using Sanakan.Game.Services;
using Shinden.API;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Sanakan.Game.Services.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using Sanakan.Game.Builder;
using Sanakan.Common.Builder;

namespace Sanakan.Game.Tests
{
    [TestClass]
    public abstract class Base
    {
        protected readonly IImageProcessor _imageProcessor;
        protected readonly Mock<IHttpClientFactory> _httpClientFactoryMock = new(MockBehavior.Strict);
        public Base()
        {
            var httpClient = new HttpClient();

            var serviceCollection = new ServiceCollection();

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            var configurationRoot = builder.Build();

            serviceCollection.AddOptions();
            serviceCollection.AddSingleton(configurationRoot);
            serviceCollection.AddGameServices();
            serviceCollection.AddFileSystem();
            serviceCollection.AddResourceManager();
            serviceCollection.AddFontResources();
            serviceCollection.AddSingleton(httpClient);
            serviceCollection.AddSingleton(_httpClientFactoryMock.Object);
            var serviceProvider = serviceCollection.BuildServiceProvider();

            _httpClientFactoryMock
                .Setup(pr => pr.CreateClient(nameof(IImageProcessor)))
                .Returns(httpClient);

            _imageProcessor = serviceProvider.GetRequiredService<IImageProcessor>();

        }
    }
}
