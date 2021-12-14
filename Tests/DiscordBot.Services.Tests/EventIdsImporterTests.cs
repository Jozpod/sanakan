using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using Sanakan.DiscordBot.Services;
using Sanakan.DiscordBot.Services.Abstractions;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace DiscordBot.ServicesTests
{
    [TestClass]
    public class EventIdsImporterTests
    {
        private readonly IEventIdsImporter _eventIdsImporter;
        private readonly Mock<HttpClientHandler> _httpClientHandlerMock = new(MockBehavior.Strict);
        private readonly Assembly _assembly = typeof(EventIdsImporterTests).Assembly;
        protected HttpClient _httpClient;

        public EventIdsImporterTests()
        {
            _httpClient = new HttpClient(_httpClientHandlerMock.Object);

            _eventIdsImporter = new EventIdsImporter(_httpClient);
        }

        [TestMethod]
        public async Task Should_Return_Event_Ids()
        {
            var url = "https://test.com/test.txt";
            var content = "1;2;3";

            _httpClientHandlerMock
               .Protected()
               .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(pr => pr.Method == HttpMethod.Get),
                    ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(new HttpResponseMessage
               {
                   StatusCode = HttpStatusCode.OK,
                   Content = new StringContent(content),
               });

            var result = await _eventIdsImporter.RunAsync(url);
            result.State.Should().Be(EventIdsImporterState.Ok);
            result.Value.Should().HaveCount(3);
        }
    }
}
