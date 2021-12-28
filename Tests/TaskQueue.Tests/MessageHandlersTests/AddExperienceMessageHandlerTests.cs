using Discord;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.Common;
using Sanakan.DAL.Models;
using Sanakan.DAL.Models.Analytics;
using Sanakan.DAL.Models.Configuration;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.Game.Services.Abstractions;
using Sanakan.TaskQueue.MessageHandlers;
using Sanakan.TaskQueue.Messages;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Sanakan.TaskQueue.Tests.MessageHandlersTests
{
    [TestClass]
    public class AddExperienceMessageHandlerTests
    {
        private readonly AddExperienceMessageHandler _messageHandler;
        private readonly Mock<ISystemClock> _systemClockMock = new(MockBehavior.Strict);
        private readonly Mock<IImageProcessor> _imageProcessorMock = new(MockBehavior.Strict);
        private readonly Mock<IUserRepository> _userRepositoryMock = new(MockBehavior.Strict);
        private readonly Mock<IGuildConfigRepository> _guildConfigRepositoryMock = new(MockBehavior.Strict);
        private readonly Mock<IUserAnalyticsRepository> _userAnalyticsRepositoryMock = new(MockBehavior.Strict);
        private readonly Mock<IGuildUser> _guildUserMock = new(MockBehavior.Strict);
        private readonly Mock<IGuild> _guildMock = new(MockBehavior.Strict);
        private readonly Mock<IRole> _roleMock = new(MockBehavior.Strict);
        private readonly Mock<IMessageChannel> _messageChannelMock = new(MockBehavior.Strict);
        
        public AddExperienceMessageHandlerTests()
        {
            _guildUserMock
                .Setup(pr => pr.Guild)
                .Returns(_guildMock.Object);

            _messageHandler = new(
                _systemClockMock.Object,
                _imageProcessorMock.Object,
                _userRepositoryMock.Object,
                _guildConfigRepositoryMock.Object,
                _userAnalyticsRepositoryMock.Object);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task Should_Handle_Message_Send_Badge_And_Add_Level_Role(bool monthPassed)
        {
            var utcNow = DateTime.UtcNow;
            var message = new AddExperienceMessage()
            {
                DiscordUserId = 1ul,
                User = _guildUserMock.Object,
                CalculateExperience = true,
                Channel = _messageChannelMock.Object,
                GuildId = 1ul,
            };
            var user = new User(message.DiscordUserId, utcNow);
            user.ExperienceCount = 49;
            user.MeasuredOn = monthPassed ? utcNow - TimeSpan.FromDays(30) : utcNow;
            var roleId = 3ul;
            var roles = new List<IRole>() { _roleMock.Object };
            var guildOptions = new GuildOptions(1ul, 50ul);
            var levelRoleMock = new Mock<IRole>(MockBehavior.Strict);
            var nextLevelRoleMock = new Mock<IRole>(MockBehavior.Strict);
            guildOptions.RolesPerLevel.Add(new LevelRole
            {
                Level = 2,
                RoleId = 1ul,
            });
            guildOptions.RolesPerLevel.Add(new LevelRole
            {
                Level = 5,
                RoleId = 2ul,
            });
            var roleIds = new List<ulong> { roleId, 2ul, };

            _roleMock
                .Setup(pr => pr.Id)
                .Returns(roleId);

            _roleMock
                .Setup(pr => pr.Position)
                .Returns(1);

            _roleMock
                .Setup(pr => pr.Color)
                .Returns(Color.Blue);

            _systemClockMock
                .Setup(pr => pr.UtcNow)
                .Returns(utcNow.Date);

            _systemClockMock
                .Setup(pr => pr.StartOfMonth)
                .Returns(utcNow.Date);

            _guildUserMock
                .Setup(pr => pr.Nickname)
                .Returns("nickname");

            _guildUserMock
                .Setup(pr => pr.GetAvatarUrl(ImageFormat.Auto, 128))
                .Returns("https://test.com/image.png");

            _guildUserMock
                .Setup(pr => pr.RoleIds)
                .Returns(roleIds);
            
            _guildMock
                .Setup(pr => pr.Roles)
                .Returns(roles);

            _guildMock
                .Setup(pr => pr.GetRole(0))
                .Returns(_roleMock.Object);

            _guildMock
                .Setup(pr => pr.GetRole(1))
                .Returns(levelRoleMock.Object);

            levelRoleMock
                .Setup(pr => pr.Id)
                .Returns(1ul);

            _guildMock
                .Setup(pr => pr.GetRole(2))
                .Returns(nextLevelRoleMock.Object);

            nextLevelRoleMock
                .Setup(pr => pr.Id)
                .Returns(2ul);

            _guildUserMock
                .Setup(pr => pr.AddRoleAsync(levelRoleMock.Object, null))
                .Returns(Task.CompletedTask);

            _guildUserMock
                .Setup(pr => pr.RemoveRoleAsync(nextLevelRoleMock.Object, null))
                .Returns(Task.CompletedTask);

            _imageProcessorMock
                .Setup(pr => pr.GetLevelUpBadgeAsync(
                    It.IsAny<string>(),
                    It.IsAny<ulong>(),
                    It.IsAny<string>(),
                    It.IsAny<Discord.Color>()))
                .ReturnsAsync(new Image<Rgba32>(300, 300));

            _messageChannelMock
                .Setup(pr => pr.SendFileAsync(
                    It.IsAny<Stream>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<bool>(),
                    It.IsAny<Embed>(),
                    It.IsAny<RequestOptions>(),
                    It.IsAny<bool>(),
                    It.IsAny<AllowedMentions>(),
                    It.IsAny<MessageReference>()))
                .ReturnsAsync(null as IUserMessage);

            _guildConfigRepositoryMock
                .Setup(pr => pr.GetCachedGuildFullConfigAsync(message.GuildId))
                .ReturnsAsync(guildOptions);
            
            _userRepositoryMock
                .Setup(pr => pr.GetUserOrCreateAsync(message.DiscordUserId))
                .ReturnsAsync(user);

            _userRepositoryMock
                .Setup(pr => pr.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _userAnalyticsRepositoryMock
                .Setup(pr => pr.Add(It.IsAny<UserAnalytics>()));

            _userAnalyticsRepositoryMock
                .Setup(pr => pr.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            
            await _messageHandler.HandleAsync(message);
        }
    }
}
