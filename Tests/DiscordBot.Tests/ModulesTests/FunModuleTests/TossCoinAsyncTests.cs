using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Sanakan.DiscordBot.Services;
using Sanakan.DiscordBot.Modules;
using System;
using Sanakan.DAL.Models;
using Moq;
using FluentAssertions;
using Discord;

namespace DiscordBot.ModulesTests.FunModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="FunModule.TossCoinAsync(CoinSide, int)"/> method.
    /// </summary>
    [TestClass]
    public class TossCoinAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Toss_Coin_And_Send_Message_Describing_Result()
        {
            var discordUserId = 1ul;
            var coinSide = CoinSide.Head;
            var amount = 1000;
            var utcNow = DateTime.UtcNow;
            var user = new User(discordUserId, utcNow);
            user.ScCount = 2000;

            _userMock
                .Setup(pr => pr.Id)
                .Returns(discordUserId);

            _userMock
                .Setup(pr => pr.Mention)
                .Returns("mention");

            _userRepositoryMock
                .Setup(pr => pr.GetUserOrCreateAsync(discordUserId))
                .ReturnsAsync(user);

            _randomNumberGeneratorMock
                .Setup(pr => pr.GetRandomValue(2))
                .Returns(1);

            _userRepositoryMock
                .Setup(pr => pr.SaveChangesAsync(default))
                .Returns(Task.CompletedTask);

            _cacheManagerMock
                .Setup(pr => pr.ExpireTag(It.IsAny<string[]>()));

            SetupSendMessage((message, embed) =>
            {
                embed.Should().NotBeNull();
                embed.Description.Should().NotBeNullOrEmpty();
            });

            _messageChannelMock
                .Setup(pr => pr.SendFileAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<bool>(),
                    It.IsAny<Embed>(),
                    It.IsAny<RequestOptions>(),
                    It.IsAny<bool>(),
                    It.IsAny<AllowedMentions>(),
                    It.IsAny<MessageReference>()))
                .ReturnsAsync(null as IUserMessage);

            await _module.TossCoinAsync(coinSide, amount);
        }
    }
}
