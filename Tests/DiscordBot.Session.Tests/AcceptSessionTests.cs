using Discord;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.Common;
using Sanakan.DAL.Models.Management;
using Sanakan.DiscordBot.Abstractions;
using Sanakan.DiscordBot.Abstractions.Configuration;
using Sanakan.DiscordBot.Services.Abstractions;
using Sanakan.DiscordBot.Session.Abstractions;
using Sanakan.Tests.Shared;
using System;
using System.Threading.Tasks;

namespace Sanakan.DiscordBot.Session.Tests
{
    /// <summary>
    /// Defines tests for <see cref="AcceptSession"/> class.
    /// </summary>
    [TestClass]
    public class AcceptSessionTests
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly AcceptSession _session;
        private readonly Mock<IModeratorService> _moderatorServiceMock = new(MockBehavior.Strict);
        private readonly Mock<IRandomNumberGenerator> _randomNumberGeneratorMock = new(MockBehavior.Strict);
        private readonly Mock<IUserMessage> _userMessageMock = new(MockBehavior.Strict);
        private readonly Mock<IMessageChannel> _messageChannelMock = new(MockBehavior.Strict);
        private readonly Mock<IMessageChannel> _notifyChannelMock = new(MockBehavior.Strict);
        private readonly Mock<IUser> _botUserMock = new(MockBehavior.Strict);
        private readonly Mock<IGuildUser> _guildUserMock = new(MockBehavior.Strict);
        private readonly Mock<IReaction> _reactionMock = new(MockBehavior.Strict);
        private readonly Mock<IRole> _muteRoleMock = new(MockBehavior.Strict);
        private readonly Mock<IRole> _userRoleMock = new(MockBehavior.Strict);

        public AcceptSessionTests()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<IIconConfiguration>(new DefaultIconConfiguration());
            serviceCollection.AddSingleton(_moderatorServiceMock.Object);
            serviceCollection.AddSingleton(_randomNumberGeneratorMock.Object);
            _serviceProvider = serviceCollection.BuildServiceProvider();

            _session = new(
                1ul,
                DateTime.UtcNow,
                _botUserMock.Object,
                _guildUserMock.Object,
                _userMessageMock.Object,
                _messageChannelMock.Object,
                _notifyChannelMock.Object,
                _userRoleMock.Object,
                _muteRoleMock.Object);
        }

        [TestMethod]
        public async Task Should_Quit_Wrong_Message()
        {
            var messageId = 1ul;
            var context = new SessionContext
            {
                UserId = 1ul,
                Message = _userMessageMock.Object,
                AddReaction = _reactionMock.Object,
            };

            _userMessageMock
                .SetupSequence(pr => pr.Id)
                .Returns(2ul)
                .Returns(messageId);

            await _session.ExecuteAsync(context, _serviceProvider);
        }

        [TestMethod]
        public async Task Should_Quit_Wrong_User()
        {
            var messageId = 1ul;
            var context = new SessionContext
            {
                UserId = 2ul,
                Message = _userMessageMock.Object,
                AddReaction = _reactionMock.Object,
            };

            _userMessageMock
                .SetupSequence(pr => pr.Id)
                .Returns(messageId);

            await _session.ExecuteAsync(context, _serviceProvider);
        }

        [TestMethod]
        public async Task Should_Quit_Wrong_Emoji()
        {
            var messageId = 1ul;
            var context = new SessionContext
            {
                UserId = 1ul,
                Message = _userMessageMock.Object,
                AddReaction = _reactionMock.Object,
            };

            _userMessageMock
                .SetupSequence(pr => pr.Id)
                .Returns(messageId);

            _reactionMock
                .Setup(pr => pr.Emote)
                .Returns(Emojis.HandSign);

            await _session.ExecuteAsync(context, _serviceProvider);
        }

        [TestMethod]
        public async Task Should_Handle_Accept()
        {
            var messageId = 1ul;
            var context = new SessionContext
            {
                UserId = 1ul,
                Message = _userMessageMock.Object,
                AddReaction = _reactionMock.Object,
            };
            var penaltyInfo = new PenaltyInfo();

            _guildUserMock
               .Setup(pr => pr.Mention)
               .Returns("user mention");

            _reactionMock
                .Setup(pr => pr.Emote)
                .Returns(Emojis.Checked);

            _userMessageMock
                .Setup(pr => pr.Id)
                .Returns(messageId);

            _userMessageMock
                .Setup(pr => pr.DeleteAsync(null))
                .Returns(Task.CompletedTask);

            _messageChannelMock
                .Setup(pr => pr.GetMessageAsync(messageId, CacheMode.AllowDownload, null))
                .ReturnsAsync(_userMessageMock.Object);

            _randomNumberGeneratorMock
                .Setup(pr => pr.GetRandomValue(365))
                .Returns(1);

            _messageChannelMock.SetupSendMessageAsync(_userMessageMock.Object);

            _moderatorServiceMock
                .Setup(pr => pr.MuteUserAsync(
                    _guildUserMock.Object,
                    _muteRoleMock.Object,
                    null,
                    _userRoleMock.Object,
                    It.IsAny<TimeSpan>(),
                    It.IsAny<string>(),
                    null))
                .ReturnsAsync(penaltyInfo);

            _moderatorServiceMock
                .Setup(pr => pr.NotifyAboutPenaltyAsync(
                    _guildUserMock.Object,
                    _notifyChannelMock.Object,
                    penaltyInfo,
                    "Sanakan"))
                .Returns(Task.CompletedTask);

            await _session.ExecuteAsync(context, _serviceProvider);
        }

        [TestMethod]
        public async Task Should_Handle_Decline()
        {
            var messageId = 1ul;
            var context = new SessionContext
            {
                UserId = 1ul,
                Message = _userMessageMock.Object,
                AddReaction = _reactionMock.Object,
            };

            _reactionMock
                .Setup(pr => pr.Emote)
                .Returns(Emotes.DeclineEmote);

            _userMessageMock
                .Setup(pr => pr.Id)
                .Returns(messageId);

            _messageChannelMock
                .Setup(pr => pr.GetMessageAsync(messageId, CacheMode.AllowDownload, null))
                .ReturnsAsync(_userMessageMock.Object);

            await _session.ExecuteAsync(context, _serviceProvider);
        }

        [TestMethod]
        public async Task Should_Remove_Reaction()
        {
            _session.ServiceProvider = _serviceProvider;

            _userMessageMock
                .Setup(pr => pr.RemoveAllReactionsAsync(null))
                .ThrowsAsync(new Exception());

            _userMessageMock
                .Setup(pr => pr.RemoveReactionAsync(It.IsAny<IEmote>(), It.IsAny<IUser>(), null))
                .Returns(Task.CompletedTask);

            await _session.DisposeAsync();
        }

        [TestMethod]
        public async Task Should_Remove_Reactions()
        {
            _userMessageMock
                .Setup(pr => pr.RemoveAllReactionsAsync(null))
                .Returns(Task.CompletedTask);

            await _session.DisposeAsync();
        }
    }
}
