using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.Game.Services.Abstractions;
using Sanakan.ShindenApi.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sanakan.Game.Tests
{
    /// <summary>
    /// Defines tests for <see cref="IImageProcessor.GetSiteStatisticAsync(UserInfo, Discord.Color, List{LastWatchedRead}?, List{LastWatchedRead}?)"/> method.
    /// </summary>
    [TestClass]
    public class GetSiteStatisticAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Generate_Site_Statistics()
        {
            var shindenInfo = new UserInfo
            {
                Id = 1ul,
                AvatarId = 1ul,
                Name = "test",
                ReadedStatus = new ReadWatchStatuses
                {
                    Total = 160,
                    Completed = 10,
                    Dropped = 20,
                    Hold = 30,
                    InProgress = 40,
                    Plan = 0,
                    Skip = 60,
                },
                ReadTime = new Time
                {
                    Days = 1,
                    Hours = 2,
                    Months = 3,
                    Minutes = 4,
                    Years = 5,
                },
                MeanMangaScore = new MeanScore
                {
                    Rating = 4.20,
                    ScoreCount = 10,
                },
                MeanAnimeScore = new MeanScore
                {
                    Rating = 4.20,
                    ScoreCount = 10,
                },
            };
            var color = Discord.Color.DarkPurple;
            var lastRead = new List<LastWatchedRead>
            {
                new LastWatchedRead
                {
                    Title = "test manga",
                    ChapterNo = 10,
                    ChaptersCount = 20,
                    TitleCoverId = 1,
                }
            };
            var lastWatch = new List<LastWatchedRead>
            {
                new LastWatchedRead
                {
                    Title = "test anime",
                    EpisodeNo = 10,
                    EpisodesCount = 20,
                    TitleCoverId = 1,
                }
            };

            _fileSystemMock
                .Setup(pr => pr.OpenRead("./Pictures/siteStatsBody.png"))
                .Returns(() => Utils.CreateFakeImage(150, 150));

            _fileSystemMock
                .Setup(pr => pr.OpenRead("./Pictures/statsAnime.png"))
                .Returns(() => Utils.CreateFakeImage(150, 150));

            _fileSystemMock
                .Setup(pr => pr.OpenRead("./Pictures/statsManga.png"))
                .Returns(() => Utils.CreateFakeImage(150, 150));

            _imageResolverMock
                .Setup(pr => pr.GetAsync(It.IsAny<Uri>()))
                .ReturnsAsync(() => Utils.CreateFakeImage(150, 150));

            var siteStatistics = await _imageProcessor.GetSiteStatisticAsync(shindenInfo, color, lastRead, lastWatch);
            siteStatistics.Should().NotBeNull();
        }
    }
}
