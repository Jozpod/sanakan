using Discord;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models;
using Sanakan.DAL.Models.Configuration;
using Sanakan.DiscordBot.Modules;
using Sanakan.TaskQueue.Messages;
using Sanakan.Tests.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiscordBot.ModulesTests.DebugModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="DebugModule.GiveawayCardsAsync(ulong, uint, uint)"/> method.
    /// </summary>
    [TestClass]
    public class GiveawayCardsAsyncTests : Base
    {

        [TestMethod]
        public async Task Should_Give_Away_Card_And_Send_Confirm_Message()
        {
            var cardCount = 1u;
            var duration = TimeSpan.FromMinutes(5);
            var user = new User(1ul, DateTime.UtcNow);
            var guildOptions = new GuildOptions(1ul, 50);
            guildOptions.WaifuRoleId = 1ul;
            guildOptions.MuteRoleId = 2ul;
            var roleMock = new Mock<IRole>(MockBehavior.Strict);
            var reactionUsers = new[]
            {
                new List<IUser>() { _guildUserMock.Object },
            }.ToAsyncEnumerable();
            var roleIds = new List<ulong>();

            _systemClockMock
                .Setup(pr => pr.UtcNow)
                .Returns(DateTime.UtcNow);

            _guildMock
                .Setup(pr => pr.Id)
                .Returns(guildOptions.Id);

            _guildMock
                .Setup(pr => pr.GetRole(guildOptions.WaifuRoleId.Value))
                .Returns<IRole?>(null);

            _guildMock
                .Setup(pr => pr.GetRole(guildOptions.MuteRoleId))
                .Returns(roleMock.Object);

            roleMock
               .Setup(pr => pr.Id)
               .Returns(guildOptions.MuteRoleId);

            _guildUserMock
                .Setup(pr => pr.Id)
                .Returns(user.Id);

            _guildUserMock
                .Setup(pr => pr.RoleIds)
                .Returns(roleIds);

            _guildConfigRepositoryMock
                .Setup(pr => pr.GetCachedGuildFullConfigAsync(guildOptions.Id))
                .ReturnsAsync(guildOptions);

            _userMessageMock
                .Setup(pr => pr.AddReactionAsync(It.IsAny<IEmote>(), null))
                .Returns(Task.CompletedTask);

            _userMessageMock
                .Setup(pr => pr.RemoveReactionAsync(It.IsAny<IEmote>(), _currentUserMock.Object, null))
                .Returns(Task.CompletedTask);

            _userMessageMock
                .Setup(pr => pr.GetReactionUsersAsync(It.IsAny<IEmote>(), 300, null))
                .Returns(reactionUsers);

            _randomNumberGeneratorMock
                .Setup(pr => pr.Shuffle(It.IsAny<IEnumerable<IUser>>()))
                .Returns<IEnumerable<IUser>>(pr => pr);

            _taskManagerMock
                .Setup(pr => pr.Delay(It.IsAny<TimeSpan>()))
                .Returns(Task.CompletedTask);

            _randomNumberGeneratorMock
                .Setup(pr => pr.GetOneRandomFrom(It.IsAny<IEnumerable<IUser>>()))
                .Returns(_guildUserMock.Object);

            _userRepositoryMock
                .Setup(pr => pr.GetCachedFullUserAsync(user.Id))
                .ReturnsAsync(user);

            _blockingPriorityQueueMock
                .Setup(pr => pr.TryEnqueue(It.IsAny<LotteryMessage>()))
                .Returns(true);

            _userMessageMock
                .Setup(pr => pr.RemoveAllReactionsAsync(null))
                .Returns(Task.CompletedTask);

            _messageChannelMock.SetupSendMessageAsync(_userMessageMock.Object);

            await _module.GiveawayCardsAsync(user.Id, cardCount, duration);
        }
    }
}
