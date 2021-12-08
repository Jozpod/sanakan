using Discord;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.Common;
using Sanakan.DAL.Models;
using Sanakan.DAL.Models.Configuration;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.Game.Services.Abstractions;
using Sanakan.TaskQueue.MessageHandlers;
using Sanakan.TaskQueue.Messages;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
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
        public async Task Should_Handle_Message()
        {
            var message = new AddExperienceMessage()
            {
                DiscordUserId = 1ul,
                User = _guildUserMock.Object,
                CalculateExperience = true,
                Channel = _messageChannelMock.Object,
                GuildId = 1ul,
            };
            var user = new User(message.DiscordUserId, DateTime.UtcNow);
            var roleId = 1ul;
            var roleIds = new List<ulong> { roleId };
            var roles = new List<IRole>() { _roleMock.Object };
            var guildOptions = new GuildOptions(1ul, 50ul);

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
                .Returns(DateTime.UtcNow);

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

            _imageProcessorMock
                .Setup(pr => pr.GetLevelUpBadgeAsync(
                    It.IsAny<string>(),
                    It.IsAny<ulong>(),
                    It.IsAny<string>(),
                    It.IsAny<Discord.Color>()))
                .ReturnsAsync(new Image<Rgba32>(300, 300));

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
                .Setup(pr => pr.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            
            await _messageHandler.HandleAsync(message);
        }
    }
}
