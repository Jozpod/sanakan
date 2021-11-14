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
using Moq.Protected;
using System.Threading;
using System.Net;
using System.Security.Cryptography;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;
using FluentAssertions;

namespace Sanakan.Game.Tests
{
    [TestClass]
    public abstract class Base
    {
        protected static IImageProcessor _imageProcessor;
        protected static Mock<IHttpClientFactory> _httpClientFactoryMock = new(MockBehavior.Strict);
        protected static Mock<HttpClientHandler> _httpClientHandlerMock = new(MockBehavior.Strict);
        private static readonly SHA256 _sha256Hash;

        static Base()
        {
            _sha256Hash = SHA256.Create();
            var httpClient = new HttpClient(_httpClientHandlerMock.Object);

            var serviceCollection = new ServiceCollection();

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            var configurationRoot = builder.Build();

            serviceCollection.AddOptions();
            serviceCollection.AddSingleton(configurationRoot);
            serviceCollection.AddGameServices();
            serviceCollection.AddFileSystem();
            serviceCollection.AddConfiguration(configurationRoot);
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

        protected void MockHttpGetImage(string filePath)
        {
            _httpClientHandlerMock
               .Protected()
               .Setup<Task<HttpResponseMessage>>("SendAsync",
                   ItExpr.Is<HttpRequestMessage>(pr => pr.Method == HttpMethod.Get),
                   ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(() => {
                   var stream = File.OpenRead(filePath);
                   return new HttpResponseMessage
                   {
                       StatusCode = HttpStatusCode.OK,
                       Content = new StreamContent(stream),
                   };
               });
        }

        protected async Task ShouldBeEqual(string expectedImageFilePath, Image<Rgba32> actualImage)
        {
            var expectedBytes = await File.ReadAllBytesAsync(expectedImageFilePath);
            var expectedHash = _sha256Hash.ComputeHash(expectedBytes);

            var actualStream = new MemoryStream();
            actualImage.SaveAsPng(actualStream);
            var actualHash = _sha256Hash.ComputeHash(actualStream.ToArray());

            Utils.CompareByteArrays(actualHash, expectedHash).Should().BeTrue();
        }

        protected void  SaveImage(Image<Rgba32> image)
        {
            var fileStream = File.OpenWrite("../../../TestData/test.png");
            image.SaveAsPng(fileStream);
        }
    }
}
