using Discord;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading.Tasks;
using FluentAssertions;

namespace DiscordBot.ServicesTests.HelperServiceTests
{
    [TestClass]
    public class BuildRaportInfoTests : Base
    {
        [TestMethod]
        public async Task Should_Return_Success()
        {
            var messageMock = new Mock<IMessage>();
            var reportAuthor = "test report author";
            var reason = "test report author";
            var reportId = 1ul;
            var result = _helperService.BuildRaportInfo(messageMock.Object, reportAuthor, reason, reportId);
            result.Should().NotBeNull();
        }
    }
}
