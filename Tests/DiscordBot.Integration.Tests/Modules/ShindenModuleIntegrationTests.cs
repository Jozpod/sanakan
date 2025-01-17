﻿using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DiscordBot.Integration.Tests.CommandBuilders;
using Sanakan.ShindenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sanakan.DiscordBot.Integration.Tests
{
#if DEBUG
    [TestClass]
#endif
    public class ShindenModuleIntegrationTests : TestBase
    {
        [TestMethod]
        public async Task TC301_Should_Get_Site_Statistics()
        {
            var userInfo = new ShindenApi.ShindenResult<UserInfo>
            {
                Value = new UserInfo
                {
                    Id = 1ul,
                    Name = "test user",
                }
            };

            var lastWatchResult = new ShindenApi.ShindenResult<List<LastWatchedRead>>
            {
                Value = new List<LastWatchedRead>
                {
                    new LastWatchedRead
                    {
                        Title = "test anime",
                        EpisodeNo = 10,
                        EpisodesCount = 20,
                        TitleCoverId = 1,
                    }
                }
            };

            var lastReadResult = new ShindenApi.ShindenResult<List<LastWatchedRead>>
            {
                Value = new List<LastWatchedRead>
                {
                    new LastWatchedRead
                    {
                        Title = "test manga",
                        ChapterNo = 10,
                        ChaptersCount = 20,
                        TitleCoverId = 1,
                    }
                }
            };

            _shindenClientMock
                .Setup(pr => pr.GetUserInfoAsync(It.IsAny<ulong>()))
                .ReturnsAsync(userInfo);

            _shindenClientMock
                .Setup(pr => pr.GetLastReadAsync(It.IsAny<ulong>(), 5))
                .ReturnsAsync(lastReadResult);

            _shindenClientMock
                .Setup(pr => pr.GetLastWatchedAsync(It.IsAny<ulong>(), 5))
                .ReturnsAsync(lastReadResult);

            var commandMessage = ShindenCommandBuilder.GetSiteStatistic(prefix, FakeUser.Mention);
            await channel.SendMessageAsync(commandMessage);

            var message = await WaitForMessageAsync();
            message.Should().NotBeNull();
            var embed = message.Embeds.FirstOrDefault();
            embed.Should().NotBeNull();
        }

        [TestMethod]
        public async Task TC302_Should_Get_New_Epsiodes()
        {
            var result = new ShindenApi.ShindenResult<List<NewEpisode>>()
            {
                Value = new List<NewEpisode>
                {
                    new NewEpisode
                    {
                        EpisodeNumber = 1,
                        EpisodeLength = TimeSpan.FromMinutes(30),
                        SubtitlesLanguage = ShindenApi.Models.Enums.Language.Japanese,
                        TitleId = 1,
                        EpisodeId = 1,
                        CoverId = 1,
                        AddDate = DateTime.Now,
                    }
                }
            };

            _shindenClientMock
                .Setup(pr => pr.GetNewEpisodesAsync())
                .ReturnsAsync(result);

            var commandMessage = ShindenCommandBuilder.ShowNewEpisodes(prefix);
            await channel.SendMessageAsync(commandMessage);

            var message = await WaitForMessageAsync();
            message.Should().NotBeNull();
            var embed = message.Embeds.FirstOrDefault();
            embed.Should().NotBeNull();
        }

        [TestMethod]
        public async Task TC302_Should_Connect_User()
        {
            var shindenId = 42069ul;
            var shindenUrl = $"https://shinden.pl/user/{shindenId}-test";
            var userInfoResult = new ShindenApi.ShindenResult<UserInfo>
            {
                Value = new UserInfo
                {
                    Name = "FakeUser",
                }
            };

            _shindenClientMock
                .Setup(pr => pr.GetUserInfoAsync(shindenId))
                .ReturnsAsync(userInfoResult);

            var commandMessage = ShindenCommandBuilder.Connect(prefix, shindenUrl);
            await channel.SendMessageAsync(commandMessage);
        }
    }
}