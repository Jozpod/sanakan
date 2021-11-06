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
    public class GetStaffInfoAsyncTests : Base
    {
    

        [TestMethod]
        public async Task Should_Return_Staff_Info()
        {
            _options
                .Setup(pr => pr.CurrentValue)
                .Returns(new ShindenApiConfiguration
                {
                    Token = "test_token"
                });

            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "ShindenApi.Tests.TestData.staff-info-result.json";
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

            var staffId = 1ul;
            var result = await _shindenClient.GetStaffInfoAsync(staffId);
            var test = _cookieContainer.GetCookies(new Uri("test"));
        }
    }
}
