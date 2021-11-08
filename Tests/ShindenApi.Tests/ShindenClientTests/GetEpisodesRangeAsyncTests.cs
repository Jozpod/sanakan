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
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Sanakan.ShindenApi.Tests
{
    [TestClass]
    public class GetEpisodesRangeAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Return_Epsiode_Range()
        {
            MockHttpOk("episodes-range-result.json", HttpMethod.Get);

            var expected = new EpisodesRange
            {
                MaxNo = 20,
                MinNo = 1,
                Status = string.Empty,
            };

            var episodeId = 1ul;
            var result = await _shindenClient.GetEpisodesRangeAsync(episodeId);
            result.Value.Should().BeEquivalentTo(expected);
        }
    }
}
