using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sanakan.ShindenApi.Models;
using Shinden.API;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Sanakan.ShindenApi.Tests
{
    [TestClass]
    public class GetEpisodesAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Return_Epsiodes()
        {
            MockHttpOk("episodes-result.json", HttpMethod.Get);

            var expected = new TitleEpisodes
            {
                ConnectedEpisodes = new List<Episode>(),
                Episodes = new List<Episode>
                {
                    new Episode
                    {
                        AirChannell = string.Empty,
                        EpisodeId = 1,
                        EpisodeTitle = string.Empty,
                        IsFiler = true,
                        TitleId = 1,
                        TitleMainId = string.Empty,
                        EpisodeTime = TimeSpan.FromMinutes(40),
                        EpisodeNo = 1,
                        EpisodeTitleId = 1,
                        Title = string.Empty,
                        TitleType = string.Empty,
                        IsAccepted = true,
                        Langs = new List<string>(),
                    }
                }
            };

            var episodeId = 1ul;
            var result = await _shindenClient.GetEpisodesAsync(episodeId);
            result.Value.Should().BeEquivalentTo(expected);
        }
    }
}
