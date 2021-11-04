using Discord;
using Discord.Commands;
using Discord.WebSocket;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models.Configuration;
using System.Linq;
using System.Threading.Tasks;


namespace DiscordBot.ServicesTests.LandManagerTests
{
    [TestClass]
    public class DetermineLandTests : Base
    {
        [TestMethod]
        public void Should_Return_Land_By_Given_Name()
        {
            var roleMock = new Mock<IRole>();
            roleMock
                .Setup(pr => pr.Id)
                .Returns(1);

            var expected = new MyLand
            {
                Name = "test",
                ManagerId = 1,
            };
            var lands = new[]
            {
                expected,
            };
            var roles = new[]
            {
                roleMock.Object,
            };
            var name = "test";

            var result = _landManager.DetermineLand(lands, roles, name);
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void Should_Return_Null()
        {
            var lands = Enumerable.Empty<MyLand>();
            var roles = Enumerable.Empty<IRole>();
            var name = "test";

            var result = _landManager.DetermineLand(lands, roles, name);
            result.Should().BeNull();
        }
    }
}
