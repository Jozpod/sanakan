using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using Sanakan.Common;
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
    public abstract class Base
    {
        protected readonly ShindenClient _shindenClient;
        protected readonly CookieContainer _cookieContainer = new();
        protected readonly Mock<IOptionsMonitor<ShindenApiConfiguration>> _options = new();
        protected readonly Mock<ISystemClock> _systemClockMock = new();
        protected readonly Mock<HttpClientHandler> _httpClientHandlerMock = new();

        public Base()
        {
            _cookieContainer = new CookieContainer();
            //_httpClientHandler = new HttpClientHandler() { CookieContainer = _cookieContainer };
            var httpClient = new HttpClient(_httpClientHandlerMock.Object);
            httpClient.BaseAddress = new Uri("https://test.com");

            _shindenClient = new ShindenClient(
                httpClient,
                _cookieContainer,
                _options.Object,
                _systemClockMock.Object,
                NullLogger<ShindenClient>.Instance);
        }
    }
}
