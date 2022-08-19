using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using Sanakan.Common;
using Sanakan.Common.Configuration;
using System;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Sanakan.ShindenApi.Tests
{
    [TestClass]
    public abstract class Base
    {
        private const string _resourcePath = "Sanakan.ShindenApi.Tests.TestData.{0}";
        protected readonly IShindenClient _shindenClient;
        protected readonly CookieContainer _cookieContainer = new();
        protected readonly Mock<IOptionsMonitor<ShindenApiConfiguration>> _options = new(MockBehavior.Strict);
        protected readonly Mock<ISystemClock> _systemClockMock = new(MockBehavior.Strict);
        protected readonly Mock<HttpClientHandler> _httpClientHandlerMock = new(MockBehavior.Strict);
        protected HttpClient _httpClient;
        private readonly Assembly _assembly = typeof(Base).Assembly;

        public Base()
        {
            _cookieContainer = new CookieContainer();
            _httpClientHandlerMock.Object.CookieContainer = _cookieContainer;
            _httpClient = new HttpClient(_httpClientHandlerMock.Object);
            _httpClient.BaseAddress = new Uri("https://test.com");

            _options
                .Setup(pr => pr.CurrentValue)
                .Returns(new ShindenApiConfiguration
                {
                    Token = "test_token",
                    UserAgent = "test",
                    SessionExiry = TimeSpan.FromHours(1),
                });

            _systemClockMock
                .Setup(pr => pr.UtcNow)
                .Returns(DateTime.UtcNow);

            _shindenClient = new ShindenClient(
                _httpClient,
                _cookieContainer,
                _options.Object,
                _systemClockMock.Object,
                NullLogger<ShindenClient>.Instance);
        }

        protected void MockHttp(HttpMethod httpMethod, HttpStatusCode statusCode)
        {
            _httpClientHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(pr => pr.Method == httpMethod),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = statusCode,
                });
        }

        protected void MockHttpOk(string resourceName, HttpMethod httpMethod)
        {
            var resourcePath = string.Format(_resourcePath, resourceName);
            var stream = _assembly.GetManifestResourceStream(resourcePath);

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
