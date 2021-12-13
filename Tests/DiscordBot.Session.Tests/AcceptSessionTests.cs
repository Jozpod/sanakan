using Discord;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models.Management;
using Sanakan.DiscordBot.Abstractions;
using Sanakan.DiscordBot.Abstractions.Configuration;
using Sanakan.DiscordBot.Services.Abstractions;
using System;
using System.Threading.Tasks;
using static Sanakan.DiscordBot.Session.AcceptSession;

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
        private readonly AcceptSessionPayload _payload;
        private readonly Mock<IModeratorService> _moderatorServiceMock = new(MockBehavior.Strict);
        
        public AcceptSessionTests()
        {
            _payload = new AcceptSessionPayload
            {

            };
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<IIconConfiguration>(new DefaultIconConfiguration());
            serviceCollection.AddSingleton(_moderatorServiceMock.Object);
            _serviceProvider = serviceCollection.BuildServiceProvider();

            _session = new(1ul, DateTime.UtcNow, _payload);
        }

        [TestMethod]
        public async Task Should_Handle_Accept()
        {
            _payload.MessageId = 1ul;
            var messageChannelMock = new Mock<IMessageChannel>(MockBehavior.Strict);
            var userMessageMock = new Mock<IUserMessage>(MockBehavior.Strict);
            var guildUserMock = new Mock<IGuildUser>(MockBehavior.Strict);
            var reactionMock = new Mock<IReaction>(MockBehavior.Strict);
            _payload.User = guildUserMock.Object;
            _payload.Channel = messageChannelMock.Object;
            var context = new SessionContext
            {
                UserId = 1ul,
                Message = userMessageMock.Object,
                AddReaction = reactionMock.Object,
            };
            var penaltyInfo = new PenaltyInfo();

            guildUserMock
               .Setup(pr => pr.Mention)
               .Returns("user mention");

            reactionMock
                .Setup(pr => pr.Emote)
                .Returns(Emojis.Checked);

            userMessageMock
                .Setup(pr => pr.Id)
                .Returns(_payload.MessageId);

            userMessageMock
                .Setup(pr => pr.DeleteAsync(null))
                .Returns(Task.CompletedTask);

            messageChannelMock
                .Setup(pr => pr.GetMessageAsync(_payload.MessageId, CacheMode.AllowDownload, null))
                .ReturnsAsync(userMessageMock.Object);

            messageChannelMock
                .Setup(pr => pr.SendMessageAsync(
                    It.IsAny<string>(),
                    It.IsAny<bool>(),
                    It.IsAny<Embed>(),
                    It.IsAny<RequestOptions>(),
                    It.IsAny<AllowedMentions>(),
                    It.IsAny<MessageReference>()))
                .ReturnsAsync(userMessageMock.Object);

            _moderatorServiceMock
                .Setup(pr => pr.MuteUserAsync(
                    _payload.User,
                    _payload.MuteRole,
                    null,
                    _payload.UserRole,
                    _payload.Duration,
                    It.IsAny<string>(),
                    null))
                .ReturnsAsync(penaltyInfo);

            _moderatorServiceMock
                .Setup(pr => pr.NotifyAboutPenaltyAsync(_payload.User, _payload.NotifyChannel, penaltyInfo, "Sanakan"))
                .Returns(Task.CompletedTask);

            await _session.ExecuteAsync(context, _serviceProvider);
        }

        [TestMethod]
        public async Task Should_Handle_Decline()
        {
            _payload.MessageId = 1ul;
            var messageChannelMock = new Mock<IMessageChannel>(MockBehavior.Strict);
            var userMessageMock = new Mock<IUserMessage>(MockBehavior.Strict);
            var guildUserMock = new Mock<IGuildUser>(MockBehavior.Strict);
            var reactionMock = new Mock<IReaction>(MockBehavior.Strict);
            _payload.User = guildUserMock.Object;
            _payload.Channel = messageChannelMock.Object;
            var context = new SessionContext
            {
                UserId = 1ul,
                Message = userMessageMock.Object,
                AddReaction = reactionMock.Object,
            };

            reactionMock
                .Setup(pr => pr.Emote)
                .Returns(Emotes.DeclineEmote);

            userMessageMock
                .Setup(pr => pr.Id)
                .Returns(_payload.MessageId);

            messageChannelMock
                .Setup(pr => pr.GetMessageAsync(_payload.MessageId, CacheMode.AllowDownload, null))
                .ReturnsAsync(userMessageMock.Object);

            await _session.ExecuteAsync(context, _serviceProvider);
        }
    }
}
