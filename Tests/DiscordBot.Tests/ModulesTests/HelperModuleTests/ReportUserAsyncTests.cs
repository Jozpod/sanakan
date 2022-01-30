using Discord;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models.Configuration;
using Sanakan.DiscordBot.Modules;
using Sanakan.DiscordBot.Session;
using Sanakan.Tests.Shared;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiscordBot.ModulesTests.HelperModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="HelperModule.ReportUserAsync(ulong, string)"/> method.
    /// </summary>
    [TestClass]
    public class ReportUserAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Return_Error_Message_No_Guild()
        {
            var messageId = 1ul;
            var reason = "reason";
            var guildid = 1ul;

            _guildMock
                .Setup(pr => pr.Id)
                .Returns(guildid);

            _guildConfigRepositoryMock
                .Setup(pr => pr.GetCachedById(guildid))
                .ReturnsAsync(null as GuildOptions);

            SetupSendMessage();

            await _module.ReportUserAsync(messageId, reason);
        }

        [TestMethod]
        public async Task Should_Start_Session()
        {
            var messageId = 1ul;
            var reason = "reason";
            var guildOption = new GuildOptions(1ul, 50);
            guildOption.NotificationChannelId = 1ul;
            guildOption.RaportChannelId = 1ul;
            guildOption.UserRoleId = 1ul;
            guildOption.MuteRoleId = 2ul;
            var guildChannelMock = new Mock<IGuildChannel>(MockBehavior.Strict);
            var textChannelMock = guildChannelMock.As<ITextChannel>();
            var messageMock = guildChannelMock.As<IMessage>();
            var userMessageMock = messageMock.As<IUserMessage>();
            var userMock = new Mock<IUser>(MockBehavior.Strict);
            var roleMock = new Mock<IRole>(MockBehavior.Strict);
            var utcNow = DateTime.UtcNow;

            var userMessageMock1 = guildChannelMock.As<IUserMessage>();

            _guildMock
                .Setup(pr => pr.Id)
                .Returns(guildOption.Id);

            _guildMock
                .Setup(pr => pr.GetChannelAsync(guildOption.RaportChannelId, CacheMode.AllowDownload, null))
                .ReturnsAsync(guildChannelMock.Object);

            _guildConfigRepositoryMock
                .Setup(pr => pr.GetCachedById(guildOption.Id))
                .ReturnsAsync(guildOption);

            _contextMessageMock
                .Setup(pr => pr.DeleteAsync(null))
                .Returns(Task.CompletedTask);

            _messageChannelMock
                .Setup(pr => pr.GetMessageAsync(messageId, CacheMode.AllowDownload, null))
                .ReturnsAsync(messageMock.Object);

            messageMock
                .Setup(pr => pr.Author)
                .Returns(userMock.Object);

            messageMock
                .Setup(pr => pr.Channel)
                .Returns(textChannelMock.Object);

            guildChannelMock
                .Setup(pr => pr.GuildId)
                .Returns(guildOption.Id);

            guildChannelMock
                .Setup(pr => pr.Id)
                .Returns(9ul);

            userMock
                .Setup(pr => pr.IsBot)
                .Returns(false);

            userMock
                .Setup(pr => pr.IsWebhook)
                .Returns(false);

            _systemClockMock
                .Setup(pr => pr.UtcNow)
                .Returns(utcNow);

            messageMock
                .Setup(pr => pr.CreatedAt)
                .Returns(utcNow);

            userMock
                .Setup(pr => pr.Id)
                .Returns(1ul);

            _userMock
                .Setup(pr => pr.Id)
                .Returns(1ul);

            _userMock
                .Setup(pr => pr.Username)
                .Returns("username");

            textChannelMock.SetupSendMessageAsync(userMessageMock.Object);

            _guildMock
                .Setup(pr => pr.GetChannelAsync(guildOption.NotificationChannelId, CacheMode.AllowDownload, null))
                .ReturnsAsync(guildChannelMock.Object);

            _guildMock
                .Setup(pr => pr.GetRole(guildOption.MuteRoleId))
                .Returns(roleMock.Object);

            _guildMock
                .Setup(pr => pr.GetRole(guildOption.UserRoleId.Value))
                .Returns(roleMock.Object);

            roleMock
                .Setup(pr => pr.Id)
                .Returns(1ul);

            _guildUserMock
                .Setup(pr => pr.RoleIds)
                .Returns(new List<ulong>());

            _guildUserMock
                .Setup(pr => pr.Mention)
                .Returns("mention");

            _userMessageMock
                .Setup(pr => pr.AddReactionAsync(It.IsAny<IEmote>(), null))
                .Returns(Task.CompletedTask);

            _userMessageMock
                .Setup(pr => pr.Channel)
                .Returns(_messageChannelMock.Object);

            _sessionManagerMock
                .Setup(pr => pr.Exists<AcceptSession>(1ul))
                .Returns(false);

            _sessionManagerMock
                .Setup(pr => pr.Add(It.IsAny<AcceptSession>()));

            userMessageMock1
                .Setup(pr => pr.ModifyAsync(It.IsAny<Action<MessageProperties>>(), null))
                .Returns(Task.CompletedTask);

            _guildConfigRepositoryMock
                .Setup(pr => pr.SaveChangesAsync(default))
                .Returns(Task.CompletedTask);

            SetupSendMessage();

            await _module.ReportUserAsync(messageId, reason);
        }

        [TestMethod]
        public async Task Should_Report_User()
        {
            var messageId = 1ul;
            var reason = "reason";
            var guildOption = new GuildOptions(1ul, 50);
            guildOption.RaportChannelId = 1ul;
            var guildChannelMock = new Mock<IGuildChannel>(MockBehavior.Strict);
            var textChannelMock = guildChannelMock.As<ITextChannel>();
            var messageMock = guildChannelMock.As<IMessage>();
            var userMessageMock = messageMock.As<IUserMessage>();
            var userMock = new Mock<IUser>(MockBehavior.Strict);
            var utcNow = DateTime.UtcNow;

            var userMessageMock1 = guildChannelMock.As<IUserMessage>();

            _guildMock
                .Setup(pr => pr.Id)
                .Returns(guildOption.Id);

            _guildMock
                .Setup(pr => pr.GetChannelAsync(guildOption.RaportChannelId, CacheMode.AllowDownload, null))
                .ReturnsAsync(guildChannelMock.Object);

            _guildConfigRepositoryMock
                .Setup(pr => pr.GetCachedById(guildOption.Id))
                .ReturnsAsync(guildOption);

            _contextMessageMock
                .Setup(pr => pr.DeleteAsync(null))
                .Returns(Task.CompletedTask);

            _messageChannelMock
                .Setup(pr => pr.GetMessageAsync(messageId, CacheMode.AllowDownload, null))
                .ReturnsAsync(messageMock.Object);

            messageMock
                .Setup(pr => pr.Author)
                .Returns(userMock.Object);

            messageMock
                .Setup(pr => pr.Channel)
                .Returns(textChannelMock.Object);

            guildChannelMock
                .Setup(pr => pr.GuildId)
                .Returns(guildOption.Id);

            guildChannelMock
                .Setup(pr => pr.Id)
                .Returns(9ul);

            userMock
                .Setup(pr => pr.IsBot)
                .Returns(false);

            userMock
                .Setup(pr => pr.IsWebhook)
                .Returns(false);

            _systemClockMock
                .Setup(pr => pr.UtcNow)
                .Returns(utcNow);

            messageMock
                .Setup(pr => pr.CreatedAt)
                .Returns(utcNow);

            userMock
                .Setup(pr => pr.Id)
                .Returns(2ul);

            _userMock
                .Setup(pr => pr.Id)
                .Returns(1ul);

            _userMock
                .Setup(pr => pr.Username)
                .Returns("username");

            textChannelMock.SetupSendMessageAsync(userMessageMock.Object);

            _helperServiceMock
                .Setup(pr => pr.BuildRaportInfo(
                    messageMock.Object,
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<ulong>()))
                .Returns(new EmbedBuilder().Build());

            userMessageMock1
                .Setup(pr => pr.ModifyAsync(It.IsAny<Action<MessageProperties>>(), null))
                .Returns(Task.CompletedTask);

            _guildConfigRepositoryMock
                .Setup(pr => pr.SaveChangesAsync(default))
                .Returns(Task.CompletedTask);

            SetupSendMessage();

            await _module.ReportUserAsync(messageId, reason);
        }
    }
}
