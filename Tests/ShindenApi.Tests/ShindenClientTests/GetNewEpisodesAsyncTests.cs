using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using Sanakan.Common.Configuration;
using Sanakan.ShindenApi;
using Sanakan.ShindenApi.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Sanakan.ShindenApi.Tests
{
    [TestClass]
    public class GetNewEpisodesAsyncTests : Base
    {

        [TestMethod]
        public async Task Should_Return_New_Epsiodes()
        {
            MockHttpOk("new-epsiodes-result.json", HttpMethod.Get);

            var expected = new List<NewEpisode>
            {
                new NewEpisode
                {
                    EpisodeLength = TimeSpan.FromMinutes(20),
                    Title = string.Empty,
                    TitleId = 1,
                    CoverId = 1,
                    EpisodeNumber = 1,
                    EpisodeId = 1,
                    Langs = new string[]{ string.Empty },
                }
            };

            var result = await _shindenClient.GetNewEpisodesAsync();
            result.Value.Should().BeEquivalentTo(expected);
        }
    }
}
