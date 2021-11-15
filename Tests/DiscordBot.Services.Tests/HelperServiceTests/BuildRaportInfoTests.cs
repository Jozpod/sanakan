using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models.Configuration;
using Sanakan.DAL.Repositories.Abstractions;
using System.Threading.Tasks;
using Discord.Rest;
using System.IO;
using Sanakan.Preconditions;
using Microsoft.Extensions.DependencyInjection;
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
