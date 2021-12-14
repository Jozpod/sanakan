
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Sanakan.Web.Test
{
    [TestClass]
    public class RequestBodyReaderTests
    {
        private readonly IRequestBodyReader _requestBodyReader;
        private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock = new(MockBehavior.Strict);
        private readonly Mock<HttpRequest> _httpRequestMock = new(MockBehavior.Strict);

        public RequestBodyReaderTests()
        {
            var httpContextMock = new Mock<HttpContext>(MockBehavior.Strict);

            _httpContextAccessorMock
                .Setup(pr => pr.HttpContext)
                .Returns(httpContextMock.Object);

            httpContextMock
                .Setup(pr => pr.Request)
                .Returns(_httpRequestMock.Object);

            _requestBodyReader = new RequestBodyReader(Encoding.UTF8, _httpContextAccessorMock.Object);
        }

        
        [TestMethod]
        public async Task Should_Read_Request_Body()
        {
            var content = "test content";
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));

            _httpRequestMock
                .Setup(pr => pr.Body)
                .Returns(stream);

            var actual = await _requestBodyReader.GetStringAsync();
            actual.Should().Be(content);
        }
    }
}
