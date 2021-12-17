using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using Sanakan.Common;
using Sanakan.Common.Builder;
using Sanakan.Common.Configuration;
using Sanakan.Game.Builder;
using Sanakan.Game.Services;
using Sanakan.Game.Services.Abstractions;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace Sanakan.Game.Tests
{
    public abstract class Base
    {
        protected static IImageProcessor _imageProcessor;
        protected static Mock<IHttpClientFactory> _httpClientFactoryMock = new(MockBehavior.Strict);
        protected static Mock<HttpClientHandler> _httpClientHandlerMock = new(MockBehavior.Strict);
        protected static Mock<IOptionsMonitor<ImagingConfiguration>> _imagingConfigurationMock = new(MockBehavior.Strict);
        protected static Mock<IResourceManager> _resourceManagerMock = new(MockBehavior.Strict);
        protected static Mock<IFileSystem> _fileSystemMock = new(MockBehavior.Strict);

        protected Stream CreateFakeImage() => new MemoryStream(Convert.FromBase64String("R0lGODlhAQABAIAAAAAAAP///yH5BAEAAAAALAAAAAABAAEAAAIBRAA7"));

        protected void MockHttpGetImage(Stream stream)
        {
            _httpClientHandlerMock
               .Protected()
               .Setup<Task<HttpResponseMessage>>("SendAsync",
                   ItExpr.Is<HttpRequestMessage>(pr => pr.Method == HttpMethod.Get),
                   ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(() => {
                   return new HttpResponseMessage
                   {
                       StatusCode = HttpStatusCode.OK,
                       Content = new StreamContent(stream),
                   };
               });
        }

        static Base()
        {
            var httpClient = new HttpClient(_httpClientHandlerMock.Object);

            _imagingConfigurationMock
                .Setup(pr => pr.CurrentValue)
                .Returns(new ImagingConfiguration
                {
                    CharacterImageWidth = 100,
                    CharacterImageHeight = 100,
                });

            var assembly = typeof(IImageProcessor).Assembly;

            _resourceManagerMock
                .Setup(pr => pr.GetResourceStream(It.IsAny<string>()))
                .Returns(() => assembly.GetManifestResourceStream("Sanakan.Game.Fonts.Digital.ttf")!);

            _httpClientFactoryMock
                .Setup(pr => pr.CreateClient(nameof(IImageProcessor)))
                .Returns(httpClient);

            _imageProcessor = new ImageProcessor(
                _imagingConfigurationMock.Object,
                _resourceManagerMock.Object,
                _fileSystemMock.Object,
                _httpClientFactoryMock.Object);
        }
    }
}
