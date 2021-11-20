using Discord.Commands;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading.Tasks;
using FluentAssertions;
using System;

namespace DiscordBot.ServicesTests.ModeratorServiceTests
{
    [TestClass]
    public class GetMutedListAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Ban_User_And_Return_Penalty_Info()
        {
            var commandContextMock = new Mock<ICommandContext>();
            var duration = TimeSpan.FromMinutes(1);

            var result = await _moderatorService.GetMutedListAsync(commandContextMock.Object);
            result.Should().NotBeNull();
        }
    }
}
