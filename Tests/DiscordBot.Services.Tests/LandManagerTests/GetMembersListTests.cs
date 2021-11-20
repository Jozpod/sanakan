using Discord;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models.Configuration;
using System.Threading.Tasks;
using FluentAssertions;

namespace DiscordBot.ServicesTests.LandManagerTests
{
    [TestClass]
    public class GetMembersListTests : Base
    {
        [TestMethod]
        public async Task Should_Return_Success()
        {
            var land = new MyLand{

            };
            var guildMock = new Mock<IGuild>();

            var result = await _landManager.GetMembersList(land, guildMock.Object);
            result.Should().NotBeEmpty();
        }
    }
}
