using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DiscordBot.Modules;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiscordBot.ModulesTests.ModerationModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="ModerationModule.AssingNumberToUsersAsync(string[])"/> method.
    /// </summary>
    [TestClass]
    public class AssingNumberToUsersAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Assign_Numbers_To_Players_And_Send_Message()
        {
            var players = Enumerable.Range(1, 10).Select(pr => $"Player {pr}").ToArray();

            _randomNumberGeneratorMock
                .Setup(pr => pr.GetOneRandomFrom(It.IsAny<IEnumerable<int>>()))
                .Returns<IEnumerable<int>>(pr => pr.First());

            _randomNumberGeneratorMock
                .Setup(pr => pr.GetOneRandomFrom(It.IsAny<IEnumerable<string>>()))
                .Returns<IEnumerable<string>>(pr => pr.First());

            SetupSendMessage((message, embed) =>
            {
                embed.Should().NotBeNull();
                embed.Description.Should().NotBeNullOrEmpty();
            });

            await _module.AssingNumberToUsersAsync(players);
        }
    }
}
