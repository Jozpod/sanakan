using Discord;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models.Configuration;
using Sanakan.DiscordBot.Services.Abstractions;
using System.Collections.Generic;
using System.Linq;

namespace DiscordBot.ServicesTests.LandManagerTests
{
    /// <summary>
    /// Defines tests for <see cref="ILandManager.DetermineLand(IEnumerable{UserLand}, IEnumerable{ulong}, string?)"/> method.
    /// </summary>
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

            var expected = new UserLand
            {
                Name = "test",
                ManagerId = 1,
            };
            var lands = new[]
            {
                expected,
            };
            var roleIds = new[]
            {
                1ul,
            };
            var name = "test";

            var result = _landManager.DetermineLand(lands, roleIds, name);
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void Should_Return_Land_By_Role()
        {
            var roleMock = new Mock<IRole>();
            roleMock
                .Setup(pr => pr.Id)
                .Returns(1);

            var expected = new UserLand
            {
                Name = "test",
                ManagerId = 1,
            };
            var lands = new[]
            {
                expected,
            };
            var roleIds = new[]
            {
                1ul,
            };

            var result = _landManager.DetermineLand(lands, roleIds);
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("test")]
        public void Should_Return_Null(string name)
        {
            var lands = Enumerable.Empty<UserLand>();
            var roles = Enumerable.Empty<ulong>();

            var result = _landManager.DetermineLand(lands, roles, name);
            result.Should().BeNull();
        }
    }
}
