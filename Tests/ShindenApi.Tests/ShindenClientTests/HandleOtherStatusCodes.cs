using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sanakan.ShindenApi.Models;
using Sanakan.ShindenApi.Models.Enums;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Sanakan.ShindenApi.Tests
{
    [TestClass]
    public class HandleOtherStatusCodes : Base
    {
        [TestMethod]
        [DataRow(HttpStatusCode.NotFound)]
        [DataRow(HttpStatusCode.ServiceUnavailable)]
        [DataRow(HttpStatusCode.BadRequest)]
        [DataRow(HttpStatusCode.InternalServerError)]
        public async Task Should_Handle_Invalid_Status_Codes(HttpStatusCode statusCode)
        {
            MockHttp(HttpMethod.Get, statusCode);

            var result = await _shindenClient.GetAllCharactersFromAnimeAsync();
            result.Value.Should().BeNull();
            result.StatusCode.Should().Be(statusCode);
        }

        [TestMethod]
        public async Task Should_Handle_Invalid_Json()
        {
            MockHttpOk("staff-info-result.json", HttpMethod.Get);

            var result = await _shindenClient.GetAllCharactersFromMangaAsync();
            result.Value.Should().BeNull();
            result.StatusCode.Should().Be(HttpStatusCode.OK);
            result.ParseError.Should().NotBeNull();
        }
    }
}
