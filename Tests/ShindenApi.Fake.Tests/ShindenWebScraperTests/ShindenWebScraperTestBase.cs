using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Moq.Protected;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sanakan.ShindenApi.Fake.Tests
{
    public class ShindenWebScraperTestBase
    {
        private readonly HttpClient _httpClient;
        protected readonly Mock<HttpClientHandler> _httpClientHandlerMock = new(MockBehavior.Strict);

        public ShindenWebScraper _shindenWebScraper { get; set; }

        protected void MockHttpOk(string fileName, HttpMethod? httpMethod = null)
        {
            httpMethod ??= HttpMethod.Get;
            var stream = File.OpenRead(Path.Combine("TestData", fileName));

            _httpClientHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.Is<HttpRequestMessage>(pr => pr.Method == httpMethod),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StreamContent(stream!),
                });
        }

        public ShindenWebScraperTestBase()
        {
            _httpClient = new HttpClient(_httpClientHandlerMock.Object);
            _httpClient.BaseAddress = new Uri("https://test.com");

            _shindenWebScraper = new ShindenWebScraper(
                NullLogger<ShindenWebScraper>.Instance,
                _httpClient);
        }
    }
}
