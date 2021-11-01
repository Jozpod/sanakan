using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using Sanakan.Common.Configuration;
using Sanakan.ShindenApi;
using System;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace ShindenApi.Tests
{
    [TestClass]
    public class GetFavouriteCharactersAsyncTests
    {
        protected readonly ShindenClient _shindenClient;
        private readonly CookieContainer _cookieContainer = new();
        private readonly Mock<IOptionsMonitor<ShindenApiConfiguration>> _options = new();
        private readonly Mock<HttpClientHandler> _httpClientHandlerMock = new();

        public GetFavouriteCharactersAsyncTests()
        {
            _cookieContainer = new CookieContainer();
            //_httpClientHandler = new HttpClientHandler() { CookieContainer = _cookieContainer };
            var httpClient = new HttpClient(_httpClientHandlerMock.Object);
            httpClient.BaseAddress = new Uri("https://test.com");

            _shindenClient = new ShindenClient(
                httpClient,
                _cookieContainer,
                _options.Object,
                NullLogger<ShindenClient>.Instance);
        }

        [TestMethod]
        public async Task Should_LogIn_And_Put_Cookies()
        {
            _options
                .Setup(pr => pr.CurrentValue)
                .Returns(new ShindenApiConfiguration
                {
                    Token = "test_token"
                });

            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "ShindenApi.Tests.TestData.login-result.json";
            var stream = assembly.GetManifestResourceStream(resourceName);

            _httpClientHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StreamContent(stream),
                });

            var result = await _shindenClient.LoginAsync("test", "test");
            var test = _cookieContainer.GetCookies(new Uri("test"));
        }
    }
}
