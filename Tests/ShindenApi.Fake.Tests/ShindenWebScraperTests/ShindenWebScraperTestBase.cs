using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Moq.Protected;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Sanakan.ShindenApi.Fake.Tests
{
    public class ShindenWebScraperTestBase
    {
        protected readonly Mock<HttpClientHandler> _httpClientHandlerMock = new(MockBehavior.Strict);
        protected readonly Mock<IHttpClientFactory> _httpClientFactoryMock = new(MockBehavior.Strict);
        protected readonly ShindenWebScraper _shindenWebScraper;
        private readonly HttpClient _httpClient;

        public ShindenWebScraperTestBase()
        {
            _httpClient = new HttpClient(_httpClientHandlerMock.Object);
            _httpClient.BaseAddress = new Uri("https://test.com");

            _httpClientFactoryMock
                .Setup(pr => pr.CreateClient(nameof(ShindenWebScraper)))
                .Returns(_httpClient);

            _shindenWebScraper = new ShindenWebScraper(
                NullLogger<ShindenWebScraper>.Instance,
                _httpClientFactoryMock.Object);
        }

        protected void MockHttpOk(string fileName, HttpMethod? httpMethod = null)
        {
            httpMethod ??= HttpMethod.Get;
            var stream = File.OpenRead(Path.Combine("TestData", fileName));

            _httpClientHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(pr => pr.Method == httpMethod),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StreamContent(stream!),
                });
        }
    }
}
