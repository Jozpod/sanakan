using Discord;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models.Configuration;
using System.Threading.Tasks;
using FluentAssertions;
using Sanakan.DiscordBot.Services.Abstractions;

namespace DiscordBot.ServicesTests.LandManagerTests
{
    /// <summary>
    /// Defines tests for <see cref="ILandManager.GetMembersList(MyLand, IGuild)"/> method.
    /// </summary>
    [TestClass]
    public class GetMembersListTests : Base
    {
        [TestMethod]
        public async Task Should_Return_Embeds_With_Members()
        {
            var land = new MyLand
            {

            };
            var guildMock = new Mock<IGuild>();

            var result = await _landManager.GetMembersList(land, guildMock.Object);
            result.Should().NotBeEmpty();
        }
    }
}
