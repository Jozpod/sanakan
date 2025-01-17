using Discord;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models.Configuration;
using Sanakan.DiscordBot.Services.Abstractions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiscordBot.ServicesTests.LandManagerTests
{
    /// <summary>
    /// Defines tests for <see cref="ILandManager.GetMembersList(UserLand, IGuild)"/> method.
    /// </summary>
    [TestClass]
    public class GetMembersListTests : Base
    {
        [TestMethod]
        public async Task Should_Return_Embeds_With_Members()
        {
            var land = new UserLand
            {
                UnderlingId = 1ul,
            };
            var guildUserMock = new Mock<IGuildUser>(MockBehavior.Strict);
            var guildMock = new Mock<IGuild>(MockBehavior.Strict);
            var users = new List<IGuildUser>() { guildUserMock.Object };
            var roleIds = new List<ulong>() { land.UnderlingId };
            var mention = new string(Enumerable.Repeat('a', 2000).ToArray());

            guildUserMock
               .Setup(pr => pr.Mention)
               .Returns(mention);

            guildUserMock
                .Setup(pr => pr.RoleIds)
                .Returns(roleIds);

            guildMock
                .Setup(pr => pr.GetUsersAsync(CacheMode.CacheOnly, null))
                .ReturnsAsync(users);

            var result = await _landManager.GetMembersList(land, guildMock.Object);
            result.Should().NotBeEmpty();
            result.First().Description.Should().NotBeNullOrEmpty();
        }
    }
}
