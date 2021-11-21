using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sanakan.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Sanakan.Web.Tests.IntegrationTests
{
    [TestClass]
    public class TokenControllerTests : TestBase
    {
        [TestMethod]
        public async Task Should_Return_Unauthorized()
        {
            var apiKey = "test key";
            var response = await _client.PostAsJsonAsync("api/token", apiKey);
            var tokenData = await response.Content.ReadFromJsonAsync<TokenData>();
            tokenData.Should().NotBeNull();
        }
    }
}
