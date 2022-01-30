using Discord;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.Daemon.HostedService;
using Sanakan.DAL.Models;
using Sanakan.DAL.Models.Configuration;
using Sanakan.DiscordBot.Abstractions;
using Sanakan.Game.Models;
using Sanakan.TaskQueue.Messages;
using Sanakan.Tests.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Sanakan.Daemon.Tests.HostedServices.SpawnHostedServiceTests
{
    /// <summary>
    /// Defines tests for <see cref="SpawnHostedService.HandleMessageAsync(IMessage)"/> event handler.
    /// </summary>
    [TestClass]
    public class HandleMessageAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Add_Experience_And_Spawn_Card()
        {
            var userMessageMock = new Mock<IUserMessage>(MockBehavior.Strict);
            var guildUserMock = new Mock<IGuildUser>(MockBehavior.Strict);
            var guildMock = new Mock<IGuild>(MockBehavior.Strict);
            var messageChannelMock = new Mock<IMessageChannel>(MockBehavior.Strict);
            var textChannelMock = messageChannelMock.As<ITextChannel>();
            var guildId = 1ul;
            var userId = 1ul;
            var tags = Enumerable.Empty<ITag>().ToList();
            var guildOptions = new GuildOptions(guildId, 50ul)
            {
                WaifuConfig = new WaifuConfiguration
                {
                    SpawnChannelId = 1ul,
                    TrashSpawnChannelId = 1ul,
                },
                MuteRoleId = 1ul,
            };
            var utcNow = DateTime.UtcNow;
            var characterInfo = new ShindenApi.Models.CharacterInfo
            {

            };
            var card = new Card(1ul, "title", "name", 100, 50, Rarity.E, Dere.Dandere, DateTime.UtcNow);
            var safariImage = new SafariImage();
            var reactionUsers = new[]
            {
                new List<IUser>() { guildUserMock.Object },
            }.ToAsyncEnumerable();
            var roleIds = Enumerable.Empty<ulong>().ToList();
            var databaseUser = new User(userId, DateTime.UtcNow);
            databaseUser.GameDeck.MaxNumberOfCards = 5;
            databaseUser.GameDeck.Cards.Add(card);

            userMessageMock
                .Setup(pr => pr.Author)
                .Returns(guildUserMock.Object);

            guildUserMock
                .Setup(pr => pr.IsBot)
                .Returns(false);

            guildUserMock
                .Setup(pr => pr.Id)
                .Returns(userId);

            guildUserMock
                .Setup(pr => pr.Mention)
                .Returns("test-user");

            guildUserMock
                .Setup(pr => pr.IsWebhook)
                .Returns(false);

            guildUserMock
                .Setup(pr => pr.RoleIds)
                .Returns(roleIds);

            guildUserMock
                .Setup(pr => pr.Guild)
                .Returns(guildMock.Object)
                .Verifiable();

            guildMock
                .Setup(pr => pr.Id)
                .Returns(guildId);

            guildMock
                .Setup(pr => pr.GetTextChannelAsync(1ul, CacheMode.AllowDownload, null))
                .ReturnsAsync(textChannelMock.Object);

            guildMock
                .Setup(pr => pr.GetRole(1ul))
                .Returns<IRole?>(null);

            userMessageMock
                .Setup(pr => pr.Channel)
                .Returns(messageChannelMock.Object)
                .Verifiable();

            userMessageMock
                .Setup(pr => pr.Tags)
                .Returns(tags)
                .Verifiable();

            userMessageMock
                .Setup(pr => pr.Content)
                .Returns("test message")
                .Verifiable();

            _guildConfigRepositoryMock
                .Setup(pr => pr.GetCachedById(guildId))
                .ReturnsAsync(guildOptions);

            _blockingPriorityQueueMock
                .Setup(pr => pr.TryEnqueue(It.IsAny<BaseMessage>()))
                .Returns(true)
                .Verifiable();

            _randomNumberGeneratorMock
                .Setup(pr => pr.TakeATry(It.IsAny<int>()))
                .Returns(true);

            _systemClockMock
                .Setup(pr => pr.UtcNow)
                .Returns(utcNow);

            _waifuServiceMock
                .Setup(pr => pr.GetRandomCharacterAsync())
                .ReturnsAsync(characterInfo);

            _waifuServiceMock
               .Setup(pr => pr.GenerateNewCard(null, characterInfo))
               .Returns(card);

            _waifuServiceMock
                .Setup(pr => pr.GetRandomSarafiImage())
                .ReturnsAsync(safariImage);

            _waifuServiceMock
                .Setup(pr => pr.SendAndGetSafariImageUrlAsync(safariImage, messageChannelMock.Object))
                .ReturnsAsync("https://test.com");

            messageChannelMock.SetupSendMessageAsync(userMessageMock.Object);

            textChannelMock
               .Setup(pr => pr.Guild)
               .Returns(guildMock.Object);

            userMessageMock
                .Setup(pr => pr.GetReactionUsersAsync(Emojis.RaisedHand, 300, null))
                .Returns(reactionUsers);

            userMessageMock
                .Setup(pr => pr.RemoveAllReactionsAsync(null))
                .Returns(Task.CompletedTask);

            userMessageMock
               .Setup(pr => pr.AddReactionAsync(Emojis.RaisedHand, null))
               .Returns(Task.CompletedTask);

            _randomNumberGeneratorMock
              .Setup(pr => pr.GetOneRandomFrom(It.IsAny<IEnumerable<IUser>>()))
              .Returns(guildUserMock.Object);

            _userRepositoryMock
                .Setup(pr => pr.GetCachedAsync(userId))
                .ReturnsAsync(databaseUser);

            var cancellationTokenSource = new CancellationTokenSource();
            await _service.StartAsync(cancellationTokenSource.Token);

            _discordClientAccessorMock.Raise(pr => pr.LoggedIn += null);
            _discordClientAccessorMock.Raise(pr => pr.MessageReceived += null, userMessageMock.Object);

            guildUserMock.Verify();
            _blockingPriorityQueueMock.Verify();
        }

    }
}
