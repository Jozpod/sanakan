using Discord;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models;
using Sanakan.DiscordBot.Abstractions;
using Sanakan.DiscordBot.Abstractions.Configuration;
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
        private readonly ListSession<Card>.ListSessionPayload _payload;
        private readonly ListSession<Card> _session;
        private readonly Mock<IUserMessage> _userMessageMock = new (MockBehavior.Strict);
        private readonly Mock<IMessageChannel> _messageChannelMock = new (MockBehavior.Strict);
        private readonly Mock<IReaction> _reactionMock = new (MockBehavior.Strict);
        private readonly ServiceProvider _serviceProvider;

        public ListSessionTests()
        {
            _payload = new ListSession<Card>.ListSessionPayload
            {
                Embed = new EmbedBuilder(),
                ItemsPerPage = 5,
                ListItems = Enumerable.Range(0, 10).Select(pr => {
                    var card = new Card(
                        (ulong)pr,
                        $"Character {pr}",
                        $"Title {pr}",
                        pr * 10,
                        pr * 5,
                        Rarity.A,
                        Dere.Dandere,
                        DateTime.UtcNow);
                    card.Id = (ulong)pr;
                    return card;
                }).ToList(),
            };
            _session = new(1ul, DateTime.UtcNow, _payload);

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<IIconConfiguration>(new DefaultIconConfiguration());
            _serviceProvider = serviceCollection.BuildServiceProvider();
        }

        [TestMethod]
        public async Task Should_Modify_Message_On_Add_Reaction()
        {
            var messageId = 1ul;
            _payload.Message = _userMessageMock.Object;
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
            _payload.CurrentPage = 1;
            _payload.Message = _userMessageMock.Object;
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
                .Returns(Emojis.LeftwardsArrow);

            await _session.ExecuteAsync(context, _serviceProvider);
        }
    }
}
