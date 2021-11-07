using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using Sanakan.Common.Configuration;
using Sanakan.ShindenApi;
using Sanakan.ShindenApi.Models;
using Shinden.API;
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
    public class GetEpisodesAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Return_Epsiodes()
        {
            MockHttpOk("episodes-result.json", HttpMethod.Get);

            var expected = new TitleEpisodes
            {
                ConnectedEpisodes = new List<Episode>
                {

                },
                Episodes = new List<Episode>
                {
                    new Episode
                    {
                        IsAccepted = true,
                        Langs = new List<string>(),
                    }
                }
            };

            var epsiodeId = 1ul;
            var result = await _shindenClient.GetEpisodesAsync(epsiodeId);
            result.Value.Should().BeEquivalentTo(expected);
        }
    }
}
