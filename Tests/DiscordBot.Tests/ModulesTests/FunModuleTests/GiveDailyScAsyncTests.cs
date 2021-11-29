using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Sanakan.DiscordBot.Modules;
using Sanakan.DAL.Models;
using System;
using Moq;

namespace DiscordBot.ModulesTests.FunModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="FunModule.GiveDailyScAsync"/> method.
    /// </summary>
    [TestClass]
    public class GiveDailyScAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Send_Message()
        {
            var discordUserId = 1ul;
            var user = new User(discordUserId, DateTime.UtcNow);

            _userRepositoryMock
                .Setup(pr => pr.GetUserOrCreateAsync(discordUserId))
                .ReturnsAsync(user);

            await _module.GiveDailyScAsync();
        }
    }
}
