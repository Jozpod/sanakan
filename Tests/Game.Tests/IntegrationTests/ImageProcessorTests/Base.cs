using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Moq.Protected;
using Sanakan.Common.Builder;
using Sanakan.Game.Builder;
using Sanakan.Game.Services;
using Sanakan.Game.Services.Abstractions;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace Sanakan.Game.Tests.IntegrationTests.ImageProcessorTests
{
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
            serviceCollection.AddResourceManager()
                .AddFontResources();
            serviceCollection.AddSingleton(httpClient);
            serviceCollection.AddImageResolver();
            serviceCollection.AddSingleton(_httpClientFactoryMock.Object);
            var serviceProvider = serviceCollection.BuildServiceProvider();

            _httpClientFactoryMock
                .Setup(pr => pr.CreateClient(nameof(ImageResolver)))
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
               .ReturnsAsync(() =>
               {
                   var stream = File.OpenRead(Path.Combine("TestData", filePath));
                   return new HttpResponseMessage
                   {
                       StatusCode = HttpStatusCode.OK,
                       Content = new StreamContent(stream),
                   };
               });
        }

        protected async Task ShouldBeEqual(string expectedImageFilePath, Image actualImage)
        {
            var expectedBytes = await File.ReadAllBytesAsync(Path.Combine("TestData", expectedImageFilePath));
            var expectedHash = _sha256Hash.ComputeHash(expectedBytes);

            var actualStream = new MemoryStream();
            actualImage.SaveAsPng(actualStream);
            var actualHash = _sha256Hash.ComputeHash(actualStream.ToArray());

            Utils.CompareByteArrays(actualHash, expectedHash).Should().BeTrue();
        }

        protected async Task SaveImageAsync(Image image, string fileName = "test.png")
        {
            using var fileStream = File.OpenWrite($"../../../TestData/{fileName}");
            await image.SaveAsPngAsync(fileStream);
        }
    }
}
