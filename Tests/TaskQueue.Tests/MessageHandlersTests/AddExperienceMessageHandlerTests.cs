using Discord;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.Common;
using Sanakan.DAL.Models;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.Game.Services.Abstractions;
using Sanakan.TaskQueue.MessageHandlers;
using Sanakan.TaskQueue.Messages;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
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
        
        public AddExperienceMessageHandlerTests()
        {
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
            };
            var user = new User(message.DiscordUserId, DateTime.UtcNow);

            _guildUserMock
                .Setup(pr => pr.Nickname)
                .Returns("nickname")
                .Verifiable();

            _imageProcessorMock
                .Setup(pr => pr.GetLevelUpBadgeAsync(
                    It.IsAny<string>(),
                    It.IsAny<ulong>(),
                    It.IsAny<string>(),
                    It.IsAny<Discord.Color>()))
                .ReturnsAsync(new Image<Rgba32>(300, 300))
                .Verifiable();
                   
            _userRepositoryMock
                .Setup(pr => pr.GetUserOrCreateAsync(message.DiscordUserId))
                .ReturnsAsync(user)
                .Verifiable();

            _userRepositoryMock
                .Setup(pr => pr.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            _userAnalyticsRepositoryMock
                .Setup(pr => pr.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask)
                .Verifiable();
            
            await _messageHandler.HandleAsync(message);

            _imageProcessorMock.Verify();
            _userRepositoryMock.Verify();
            _guildUserMock.Verify();
        }
    }
}
