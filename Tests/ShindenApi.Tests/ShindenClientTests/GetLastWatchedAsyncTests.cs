using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using Sanakan.Common.Configuration;
using Sanakan.ShindenApi;
using Shinden.API;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace ShindenApi.Tests
{
    [TestClass]
    public class GetLastWatchedAsyncTests : Base
    {
    
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
            var resourceName = "ShindenApi.Tests.TestData.last-watched-result.json";
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

            var expected = new LastWatchedReaded
            {

            };

            var userId = 1ul;
            var result = await _shindenClient.GetLastReadAsync(userId);
            result.Value.First().Should().BeEquivalentTo(expected);
        }
    }
}
