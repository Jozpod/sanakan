using Discord;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models.Configuration;
using Sanakan.DiscordBot.Modules;
using Sanakan.DiscordBot.Session;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiscordBot.ModulesTests.FunModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="FunModule.GiveMuteAsync"/> method.
    /// </summary>
    [TestClass]
    public class GiveMuteAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Mute_User_And_Send_Message()
        {
            var guildOptions = new GuildOptions(1ul, 50);
            var roleIds = new List<ulong>();
            var textChannelMock = new Mock<ITextChannel>(MockBehavior.Strict);
            var userMessageMock = new Mock<IUserMessage>(MockBehavior.Strict);
            var roleMock = new Mock<IRole>(MockBehavior.Strict);
            guildOptions.NotificationChannelId = 1ul;
            guildOptions.UserRoleId = 1ul;
            guildOptions.MuteRoleId = 2ul;
            var userId = 1ul;

            roleMock
                .Setup(pr => pr.Id)
                .Returns(guildOptions.MuteRoleId);

            _guildMock
                .Setup(pr => pr.Id)
                .Returns(guildOptions.Id);

            _guildMock
                .Setup(pr => pr.GetChannelAsync(guildOptions.NotificationChannelId, CacheMode.AllowDownload, null))
                .ReturnsAsync(textChannelMock.Object);

            _systemClockMock
                .Setup(pr => pr.UtcNow)
                .Returns(DateTime.UtcNow);

            _guildMock
                .Setup(pr => pr.GetRole(guildOptions.UserRoleId.Value))
                .Returns(roleMock.Object);

            _guildMock
                .Setup(pr => pr.GetRole(guildOptions.MuteRoleId))
                .Returns(roleMock.Object);

            _guildUserMock
                .Setup(pr => pr.Id)
                .Returns(1ul);

            _guildUserMock
                .Setup(pr => pr.Mention)
                .Returns("user mention");

            _guildUserMock
                .Setup(pr => pr.RoleIds)
                .Returns(roleIds);
            
            _guildConfigRepositoryMock
                .Setup(pr => pr.GetCachedGuildFullConfigAsync(guildOptions.Id))
                .ReturnsAsync(guildOptions);

            _randomNumberGeneratorMock
                .Setup(pr => pr.GetRandomValue(365))
                .Returns(1);

            _sessionManagerMock
                .Setup(pr => pr.RemoveIfExists<AcceptSession>(userId));

            _sessionManagerMock
                .Setup(pr => pr.Add(It.IsAny<AcceptSession>()));

            SetupSendMessage((message, embed) =>
            {
                embed.Should().NotBeNull();
                embed.Description.Should().NotBeNull();
            });

            _userMessageMock
                .Setup(pr => pr.AddReactionAsync(It.IsAny<IEmote>(), null))
                .Returns(Task.CompletedTask);

            _userMessageMock
                .Setup(pr => pr.Id)
                .Returns(1ul);

            _userMessageMock
                .Setup(pr => pr.Channel)
                .Returns(textChannelMock.Object);

            await _module.GiveMuteAsync();
        }
    }
}
