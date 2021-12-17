using Discord;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models;
using Sanakan.DAL.Models.Configuration;
using Sanakan.DAL.Models.Management;
using Sanakan.DiscordBot.Modules;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DiscordBot.ModulesTests.ModerationModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="ModerationModule.ResolveReportAsync(ulong, string?, string)"/> method.
    /// </summary>
    [TestClass]
    public class ResolveReportAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Resolve_Report_Reject()
        {
            var messageId = 1ul;
            var guildConfig = new GuildOptions(1ul, 50);
            guildConfig.UserRoleId = 1ul;
            var report = new Report()
            {
                UserId = 2ul,
                MessageId = messageId,
            };
            var guildUserMock = new Mock<IGuildUser>(MockBehavior.Strict);
            var guildChannelMock = new Mock<IGuildChannel>(MockBehavior.Strict);
            var messageChannelMock = guildChannelMock.As<IMessageChannel>();
            var roleId = 1ul;
            var roleMock = new Mock<IRole>(MockBehavior.Strict);
            guildConfig.Raports.Add(report);
            var roleIds = new List<ulong>();
            var userMessageMock = new Mock<IUserMessage>(MockBehavior.Strict);

            _guildMock
                .Setup(pr => pr.Id)
                .Returns(guildConfig.Id);

            _guildConfigRepositoryMock
                .Setup(pr => pr.GetGuildConfigOrCreateAsync(guildConfig.Id))
                .ReturnsAsync(guildConfig);

            _guildUserMock
                .Setup(pr => pr.Nickname)
                .Returns("nicknames");

            _guildMock
                .Setup(pr => pr.GetUserAsync(report.UserId, CacheMode.AllowDownload, null))
                .ReturnsAsync(guildUserMock.Object);

            _guildMock
                .Setup(pr => pr.GetChannelAsync(It.IsAny<ulong>(), CacheMode.AllowDownload, null))
                .ReturnsAsync(guildChannelMock.Object);

            _guildMock
                .Setup(pr => pr.GetRole(It.IsAny<ulong>()))
                .Returns(roleMock.Object);

            roleMock
                .Setup(pr => pr.Id)
                .Returns(roleId);

            guildUserMock
                .Setup(pr => pr.RoleIds)
                .Returns(roleIds);

            messageChannelMock
                .Setup(pr => pr.GetMessageAsync(messageId, CacheMode.AllowDownload, null))
                .ReturnsAsync(userMessageMock.Object);

            userMessageMock
                .Setup(pr => pr.DeleteAsync(null))
               .Returns(Task.CompletedTask);

            _userRepositoryMock
                .Setup(pr => pr.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            SetupSendMessage((message, embed) =>
            {
                embed.Description.Should().NotBeNull();
            });

            await _module.ResolveReportAsync(messageId, null);
        }

        [TestMethod]
        public async Task Should_Resolve_Report_Warn()
        {
            var messageId = 1ul;
            var guildConfig = new GuildOptions(1ul, 50);
            guildConfig.UserRoleId = 1ul;
            var report = new Report()
            {
                UserId = 2ul,
                MessageId = messageId,
            };
            var user = new User(1ul, DateTime.Now);
            var guildUserMock = new Mock<IGuildUser>(MockBehavior.Strict);
            var guildChannelMock = new Mock<IGuildChannel>(MockBehavior.Strict);
            var messageChannelMock = guildChannelMock.As<IMessageChannel>();
            var roleId = 1ul;
            var roleMock = new Mock<IRole>(MockBehavior.Strict);
            guildConfig.Raports.Add(report);
            var roleIds = new List<ulong>();
            var userMessageMock = new Mock<IUserMessage>(MockBehavior.Strict);
            var duration = TimeSpan.Zero;

            _guildMock
                .Setup(pr => pr.Id)
                .Returns(guildConfig.Id);

            _guildConfigRepositoryMock
                .Setup(pr => pr.GetGuildConfigOrCreateAsync(guildConfig.Id))
                .ReturnsAsync(guildConfig);

            _guildUserMock
                .Setup(pr => pr.Nickname)
                .Returns("nicknames");

            _guildMock
                .Setup(pr => pr.GetUserAsync(report.UserId, CacheMode.AllowDownload, null))
                .ReturnsAsync(guildUserMock.Object);

            _guildMock
                .Setup(pr => pr.GetChannelAsync(It.IsAny<ulong>(), CacheMode.AllowDownload, null))
                .ReturnsAsync(guildChannelMock.Object);

            _guildMock
                .Setup(pr => pr.GetRole(It.IsAny<ulong>()))
                .Returns(roleMock.Object);

            roleMock
                .Setup(pr => pr.Id)
                .Returns(roleId);

            guildUserMock
                .Setup(pr => pr.RoleIds)
                .Returns(roleIds);

            messageChannelMock
                .Setup(pr => pr.GetMessageAsync(messageId, CacheMode.AllowDownload, null))
                .ReturnsAsync(userMessageMock.Object);

            userMessageMock
                .Setup(pr => pr.DeleteAsync(null))
                .Returns(Task.CompletedTask);

            guildUserMock
                .Setup(pr => pr.Id)
                .Returns(user.Id);

            _userRepositoryMock
                .Setup(pr => pr.GetUserOrCreateAsync(user.Id))
                .ReturnsAsync(user);

            _moderatorServiceMock
                .Setup(pr => pr.NotifyUserAsync(guildUserMock.Object, It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            _userRepositoryMock
                .Setup(pr => pr.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            SetupSendMessage((message, embed) =>
            {
                embed.Description.Should().NotBeNull();
            });

            await _module.ResolveReportAsync(messageId, duration);
        }

        [TestMethod]
        public async Task Should_Resolve_Report_Mute()
        {
            var messageId = 1ul;
            var guildConfig = new GuildOptions(1ul, 50);
            guildConfig.UserRoleId = 1ul;
            var report = new Report()
            {
                UserId = 2ul,
                MessageId = messageId,
            };
            var user = new User(1ul, DateTime.Now);
            user.WarningsCount = 5;
            var guildUserMock = new Mock<IGuildUser>(MockBehavior.Strict);
            var guildChannelMock = new Mock<IGuildChannel>(MockBehavior.Strict);
            var messageChannelMock = guildChannelMock.As<IMessageChannel>();
            var roleId = 1ul;
            var roleMock = new Mock<IRole>(MockBehavior.Strict);
            guildConfig.Raports.Add(report);
            var roleIds = new List<ulong>();
            var userMessageMock = new Mock<IUserMessage>(MockBehavior.Strict);
            var duration = TimeSpan.Zero;
            var penaltyInfo = new PenaltyInfo();

            _guildMock
                .Setup(pr => pr.Id)
                .Returns(guildConfig.Id);

            _guildConfigRepositoryMock
                .Setup(pr => pr.GetGuildConfigOrCreateAsync(guildConfig.Id))
                .ReturnsAsync(guildConfig);

            _guildUserMock
                .Setup(pr => pr.Nickname)
                .Returns("nicknames");

            _guildMock
                .Setup(pr => pr.GetUserAsync(report.UserId, CacheMode.AllowDownload, null))
                .ReturnsAsync(guildUserMock.Object);

            _guildMock
                .Setup(pr => pr.GetChannelAsync(It.IsAny<ulong>(), CacheMode.AllowDownload, null))
                .ReturnsAsync(guildChannelMock.Object);

            _guildMock
                .Setup(pr => pr.GetRole(It.IsAny<ulong>()))
                .Returns(roleMock.Object);

            roleMock
                .Setup(pr => pr.Id)
                .Returns(roleId);

            guildUserMock
                .Setup(pr => pr.RoleIds)
                .Returns(roleIds);

            messageChannelMock
                .Setup(pr => pr.GetMessageAsync(messageId, CacheMode.AllowDownload, null))
                .ReturnsAsync(userMessageMock.Object);

            userMessageMock
                .Setup(pr => pr.DeleteAsync(null))
                .Returns(Task.CompletedTask);

            guildUserMock
                .Setup(pr => pr.Id)
                .Returns(user.Id);

            guildUserMock
                .Setup(pr => pr.Mention)
                .Returns("mention");

            _userRepositoryMock
                .Setup(pr => pr.GetUserOrCreateAsync(user.Id))
                .ReturnsAsync(user);

            _userRepositoryMock
                .Setup(pr => pr.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _moderatorServiceMock
                .Setup(pr => pr.MuteUserAsync(
                    guildUserMock.Object,
                    roleMock.Object,
                    null,
                    roleMock.Object,
                    It.IsAny<TimeSpan>(),
                    It.IsAny<string>(),
                    It.IsAny<IEnumerable<ModeratorRoles>>()))
                .ReturnsAsync(penaltyInfo);

            _moderatorServiceMock
               .Setup(pr => pr.NotifyAboutPenaltyAsync(
                    guildUserMock.Object,
                    messageChannelMock.Object,
                    penaltyInfo,
                    It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            SetupSendMessage((message, embed) =>
            {
                embed.Description.Should().NotBeNull();
            });

            await _module.ResolveReportAsync(messageId, duration);
        }
    }
}
