using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using Sanakan.Common.Configuration;
using Sanakan.ShindenApi;
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
    public class GetAllCharactersFromAnimeAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Get_All_Character_Ids()
        {
            _options
                .Setup(pr => pr.CurrentValue)
                .Returns(new ShindenApiConfiguration
                {
                    Token = "test_token"
                });

            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "ShindenApi.Tests.TestData.all-characters-from-anime-result.json";
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

            var result = await _shindenClient.GetAllCharactersFromAnimeAsync();
            result.Value.First().Should().Be(1);
        }
    }
}
