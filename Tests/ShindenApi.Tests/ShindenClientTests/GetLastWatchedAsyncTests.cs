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
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Sanakan.ShindenApi.Tests
{
    [TestClass]
    public class GetLastWatchedAsyncTests : Base
    {
    
        [TestMethod]
        public async Task Should_Return_Last_Watched_Details()
        {
            MockHttpOk("last-watched-result.json", HttpMethod.Get);

            var expected = new List<LastWatchedRead>()
            {
                new LastWatchedRead
                {
                    TitleCoverId = 1,
                    IsFiler = true,
                    ViewCount = 100,
                }
            };

            var userId = 1ul;
            var result = await _shindenClient.GetLastWatchedAsync(userId);
            result.Value.Should().BeEquivalentTo(expected);
        }
    }
}
