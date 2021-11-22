﻿using FluentAssertions;
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
using Sanakan.Web.Controllers;
using System.Threading.Tasks;

namespace Sanakan.Web.Tests.IntegrationTests
{
    /// <summary>
    /// Defines tests for <see cref="TokenController"/>.
    /// </summary>
    [TestClass]
    public class TokenControllerTests : TestBase
    {
        [TestMethod]
        public async Task Should_Unauthorize()
        {
            var response = await _client.PostAsJsonAsync("api/token", string.Empty);
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
        }

        [TestMethod]
        public async Task Should_Forbid()
        {
            var apiKey = "random key";
            var response = await _client.PostAsJsonAsync("api/token", apiKey);
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Forbidden);
        }

        [TestMethod]
        public async Task Should_Return_Token()
        {
            var apiKey = "test key";
            var response = await _client.PostAsJsonAsync("api/token", apiKey);
            var tokenData = await response.Content.ReadFromJsonAsync<TokenData>();
            tokenData.Should().NotBeNull();
            tokenData.Expire.Should().BeAfter(DateTime.UtcNow);
            tokenData.Token.Should().NotBeNullOrEmpty();
        }
    }
}
