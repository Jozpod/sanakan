using Discord;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DiscordBot.Abstractions;
using Sanakan.DiscordBot.Abstractions.Configuration;
using Sanakan.DiscordBot.Session.Abstractions;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Sanakan.DiscordBot.Session.Tests
{
    /// <summary>
    /// Defines tests for <see cref="ListSession{T}"/> class.
    /// </summary>
    [TestClass]
    public class ListSessionTests
    {
        private readonly ListSession _session;
        private readonly Mock<IUser> _userMock = new(MockBehavior.Strict);
        private readonly Mock<IUserMessage> _userMessageMock = new (MockBehavior.Strict);
        private readonly Mock<IMessageChannel> _messageChannelMock = new (MockBehavior.Strict);
        private readonly Mock<IReaction> _reactionMock = new (MockBehavior.Strict);
        private readonly ServiceProvider _serviceProvider;

        public ListSessionTests()
        {
            var items = Enumerable.Range(0, 10).Select(pr => $"Item {pr}").ToList();

            _session = new(
                1ul,
                DateTime.UtcNow,
                items,
                _userMock.Object,
                _userMessageMock.Object,
                new EmbedBuilder());

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<IIconConfiguration>(new DefaultIconConfiguration());
            _serviceProvider = serviceCollection.BuildServiceProvider();
        }

        [TestMethod]
        public async Task Should_Quit_Wrong_Message()
        {
            var messageId = 1ul;
            var context = new SessionContext
            {
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
        public async Task Should_Modify_Message_On_Add_Reaction()
        {
            var messageId = 1ul;
            var context = new SessionContext
            {
                Message = _userMessageMock.Object,
                AddReaction = _reactionMock.Object,
            };

            _userMessageMock
                .Setup(pr => pr.Id)
                .Returns(messageId);

            _userMessageMock
                .Setup(pr => pr.Channel)
                .Returns(_messageChannelMock.Object);

            _userMessageMock
                .Setup(pr => pr.ModifyAsync(It.IsAny<Action<MessageProperties>>(), null))
                .Returns(Task.CompletedTask);

            _messageChannelMock
                .Setup(pr => pr.GetMessageAsync(messageId, CacheMode.AllowDownload, null))
                .ReturnsAsync(_userMessageMock.Object);

            _reactionMock
                .Setup(pr => pr.Emote)
                .Returns(Emojis.RightwardsArrow);

            await _session.ExecuteAsync(context, _serviceProvider);
        }

        [TestMethod]
        public async Task Should_Modify_Message_On_Remove_Reaction()
        {
            var messageId = 1ul;
            var context = new SessionContext
            {
                Message = _userMessageMock.Object,
                AddReaction = _reactionMock.Object,
            };

            _userMessageMock
                .Setup(pr => pr.Id)
                .Returns(messageId);

            _userMessageMock
                .Setup(pr => pr.Channel)
                .Returns(_messageChannelMock.Object);

            _userMessageMock
                .Setup(pr => pr.ModifyAsync(It.IsAny<Action<MessageProperties>>(), null))
                .Returns(Task.CompletedTask);

            _messageChannelMock
                .Setup(pr => pr.GetMessageAsync(messageId, CacheMode.AllowDownload, null))
                .ReturnsAsync(_userMessageMock.Object);

            _reactionMock
                .SetupSequence(pr => pr.Emote)
                .Returns(Emojis.RightwardsArrow)
                .Returns(Emojis.LeftwardsArrow);

            await _session.ExecuteAsync(context, _serviceProvider);
            await _session.ExecuteAsync(context, _serviceProvider);
        }

        [TestMethod]
        public async Task Should_Remove_All_Reactions()
        {
            _userMessageMock
                .Setup(pr => pr.RemoveAllReactionsAsync(null))
                .Returns(Task.CompletedTask);

            await _session.DisposeAsync();
        }

        [TestMethod]
        public async Task Should_Remove_Bot_Reactions()
        {
            _session.ServiceProvider = _serviceProvider;

            _userMessageMock
                .Setup(pr => pr.RemoveAllReactionsAsync(null))
                .ThrowsAsync(new Exception());

            _userMessageMock
               .Setup(pr => pr.RemoveReactionAsync(It.IsAny<IEmote>(), _userMock.Object, null))
               .Returns(Task.CompletedTask);

            await _session.DisposeAsync();
        }
    }
}
