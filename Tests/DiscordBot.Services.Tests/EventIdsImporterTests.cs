using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using Sanakan.DiscordBot.Services;
using Sanakan.DiscordBot.Services.Abstractions;
using System;
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

        private void SetupHttp(HttpStatusCode httpStatusCode, string? content = null)
        {
            _httpClientHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(pr => pr.Method == HttpMethod.Get),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = httpStatusCode,
                    Content = content == null ? null : new StringContent(content),
                });
        }

        [TestMethod]
        public async Task Should_Handle_Invalid_Url()
        {
            var url = new Uri("https://test.com/test.txt");

            SetupHttp(HttpStatusCode.NotFound);

            var result = await _eventIdsImporter.RunAsync(url);
            result.State.Should().Be(EventIdsImporterState.InvalidStatusCode);
        }

        [TestMethod]
        public async Task Should_Handle_Invalid_File_Format()
        {
            var url = new Uri("https://test.com/test.txt");

            SetupHttp(HttpStatusCode.OK, "test");

            var result = await _eventIdsImporter.RunAsync(url);
            result.State.Should().Be(EventIdsImporterState.InvalidFileFormat);
            result.Exception.Should().NotBeNull();
        }

        [TestMethod]
        public async Task Should_Return_Event_Ids()
        {
            var url = new Uri("https://test.com/test.txt");
            var content = "1;2;3";

            SetupHttp(HttpStatusCode.OK, content);

            var result = await _eventIdsImporter.RunAsync(url);
            result.State.Should().Be(EventIdsImporterState.Ok);
            result.Value.Should().HaveCount(3);
        }
    }
}
