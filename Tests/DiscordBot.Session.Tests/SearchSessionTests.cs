using Discord;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.DiscordBot.Abstractions.Configuration;
using Sanakan.Game.Models;
using Sanakan.ShindenApi;
using Sanakan.ShindenApi.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Sanakan.DiscordBot.Session.SearchSession;

namespace Sanakan.DiscordBot.Session
{
    /// <summary>
    /// Defines tests for <see cref="SearchSession"/> class.
    /// </summary>
    [TestClass]
    public class SearchSessionTests
    {
        private readonly SearchSession _session;
        private readonly Mock<IUserRepository> _userRepositoryMock = new(MockBehavior.Strict);
        private readonly ServiceProvider _serviceProvider;
        private readonly SearchSessionPayload _payload;
        private readonly Mock<IMessageChannel> _messageChannelMock = new(MockBehavior.Strict);
        private readonly Mock<IUserMessage> _userMessageMock = new(MockBehavior.Strict);
        private readonly Mock<IShindenClient> _shindenClientMock = new(MockBehavior.Strict);

        public SearchSessionTests()
        {
            _payload = new SearchSessionPayload();
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton(_shindenClientMock.Object);
            serviceCollection.AddSingleton(_userRepositoryMock.Object);

            _serviceProvider = serviceCollection.BuildServiceProvider();
            _session = new(1ul, DateTime.UtcNow, _payload);
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
                        TitleOther = new List<TitleOther>
                        {

                        },
                    }
                }
            };
            _payload.AnimeMangaList.Add(result);

            _shindenClientMock
                .Setup(pr => pr.GetAnimeMangaInfoAsync(result.TitleId))
                .ReturnsAsync(animeMangaInfoResult);

            _messageChannelMock
                .Setup(pr => pr.SendMessageAsync(
                    It.IsAny<string>(),
                    It.IsAny<bool>(),
                    It.IsAny<Embed>(),
                    It.IsAny<RequestOptions>(),
                    It.IsAny<AllowedMentions>(),
                    It.IsAny<MessageReference>()))
                .ReturnsAsync(null as IUserMessage);

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
                Value = new CharacterInfo
                {

                }
            };
            _payload.CharacterList.Add(result);

            _shindenClientMock
                .Setup(pr => pr.GetCharacterInfoAsync(result.Id))
                .ReturnsAsync(animeMangaInfoResult);

            _messageChannelMock
                .Setup(pr => pr.SendMessageAsync(
                    It.IsAny<string>(),
                    It.IsAny<bool>(),
                    It.IsAny<Embed>(),
                    It.IsAny<RequestOptions>(),
                    It.IsAny<AllowedMentions>(),
                    It.IsAny<MessageReference>()))
                .ReturnsAsync(null as IUserMessage);

            messageMock
                .Setup(pr => pr.DeleteAsync(null))
                .Returns(Task.CompletedTask);

            await _session.ExecuteAsync(context, _serviceProvider);
        }
    }
}
