using Discord;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.DiscordBot.Session.Abstractions;
using Sanakan.ShindenApi;
using Sanakan.ShindenApi.Models;
using Sanakan.Tests.Shared;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sanakan.DiscordBot.Session
{
    /// <summary>
    /// Defines tests for <see cref="SearchSession"/> class.
    /// </summary>
    [TestClass]
    public class SearchSessionTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock = new(MockBehavior.Strict);
        private readonly ServiceProvider _serviceProvider;
        private readonly Mock<IMessageChannel> _messageChannelMock = new(MockBehavior.Strict);
        private readonly Mock<IUserMessage> _userMessageMock = new(MockBehavior.Strict);
        private readonly Mock<IShindenClient> _shindenClientMock = new(MockBehavior.Strict);
        private readonly List<QuickSearchResult> _animeMangaList = new();
        private readonly List<CharacterSearchResult> _characterList = new();
        private readonly Mock<IMessage> _messageMock = new(MockBehavior.Strict);
        private SearchSession _session;

        public SearchSessionTests()
        {
            var messages = new[] { _messageMock.Object };

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton(_shindenClientMock.Object);
            serviceCollection.AddSingleton(_userRepositoryMock.Object);

            _serviceProvider = serviceCollection.BuildServiceProvider();
            _session = new(
                1ul,
                DateTime.UtcNow,
                messages,
                _animeMangaList,
                _characterList);
        }

        [TestMethod]
        public async Task Should_Select_Anime()
        {
            var messageMock = _userMessageMock.As<IMessage>();

            messageMock
               .Setup(pr => pr.Content)
               .Returns("1");

            var context = new SessionContext
            {
                UserId = 1ul,
                Message = _userMessageMock.Object,
                Channel = _messageChannelMock.Object,
            };
            var result = new QuickSearchResult
            {
                TitleId = 1,
            };
            var animeMangaInfoResult = new ShindenResult<AnimeMangaInfo>
            {
                Value = new AnimeMangaInfo
                {
                    Title = new TitleEntry
                    {
                        Title = "test",
                        Description = new AnimeMangaInfoDescription
                        {
                            OtherDescription = "test",
                        },
                        CoverId = 1,
                        TitleOther = new List<TitleOther>(),
                        Type = IllustrationType.Anime,
                        Anime = new AnimeInfo(),
                    }
                }
            };
            _animeMangaList.Add(result);

            _shindenClientMock
                .Setup(pr => pr.GetAnimeMangaInfoAsync(result.TitleId))
                .ReturnsAsync(animeMangaInfoResult);

            _messageChannelMock.SetupSendMessageAsync(null);

            messageMock
                .Setup(pr => pr.DeleteAsync(null))
                .Returns(Task.CompletedTask);

            await _session.ExecuteAsync(context, _serviceProvider);
        }

        [TestMethod]
        public async Task Should_Select_Character()
        {
            var messageMock = _userMessageMock.As<IMessage>();

            messageMock
               .Setup(pr => pr.Content)
               .Returns("1");

            var context = new SessionContext
            {
                UserId = 1ul,
                Message = _userMessageMock.Object,
                Channel = _messageChannelMock.Object,
            };
            var result = new CharacterSearchResult
            {
                Id = 1,
            };
            var animeMangaInfoResult = new ShindenResult<CharacterInfo>
            {
                Value = new CharacterInfo(),
            };
            _characterList.Add(result);

            _shindenClientMock
                .Setup(pr => pr.GetCharacterInfoAsync(result.Id))
                .ReturnsAsync(animeMangaInfoResult);

            _messageChannelMock.SetupSendMessageAsync(null);

            messageMock
                .Setup(pr => pr.DeleteAsync(null))
                .Returns(Task.CompletedTask);

            await _session.ExecuteAsync(context, _serviceProvider);
        }

        [TestMethod]
        public async Task Should_Remove_Messages()
        {
            var messageId = 1ul;

            _messageMock
                .Setup(pr => pr.Id)
                .Returns(messageId);

            _messageMock
                .Setup(pr => pr.Channel)
                .Returns(_messageChannelMock.Object);

            _messageChannelMock
                .Setup(pr => pr.GetMessageAsync(messageId, CacheMode.AllowDownload, null))
                .ReturnsAsync(_messageMock.Object);

            _messageMock
               .Setup(pr => pr.DeleteAsync(null))
               .Returns(Task.CompletedTask);

            await _session.DisposeAsync();
        }
    }
}
