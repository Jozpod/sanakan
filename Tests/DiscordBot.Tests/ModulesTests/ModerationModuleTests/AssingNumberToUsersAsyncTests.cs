using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Sanakan.DiscordBot.Modules;
using System.Linq;
using Moq;
using System.Collections;
using System.Collections.Generic;

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

            await _module.AssingNumberToUsersAsync(players);
        }
    }
}
