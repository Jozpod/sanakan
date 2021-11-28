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
using Sanakan.Web.Controllers;
using System.Threading.Tasks;
using static Sanakan.Web.ResponseExtensions;
using Sanakan.DAL.Models;
using System.Net.Http.Headers;
using System.Text.Json;
using Sanakan.Common.Converters;
using Sanakan.ShindenApi.Models;

namespace Sanakan.Web.Tests.IntegrationTests
{
    /// <summary>
    /// Defines tests for <see cref="UserController"/>.
    /// </summary>
    public partial class TestBase
    {

        /// <summary>
        /// Defines test for <see cref="UserController.GetUserByDiscordIdAsync(ulong)"/> method.
        /// </summary>
        [TestMethod]
        public async Task Should_Return_Discord_User()
        {
            var discordUserId = 1ul;

            var user = await _client.GetFromJsonAsync<User>($"api/user/discord/{discordUserId}");
            user.Should().NotBeNull();
        }

        /// <summary>
        /// Defines test for <see cref="UserController.GetUserIdByNameAsync(string)"/> method.
        /// </summary>
        [TestMethod]
        public async Task Should_Find_Shinden_User()
        {
            var response = await _client.PostAsJsonAsync($"api/user/find", "user");
            response.EnsureSuccessStatusCode();
            var users = await response.Content.ReadFromJsonAsync<IEnumerable<UserSearchResult>>();
            users.Should().NotBeNull();
            users.Should().HaveCount(1);
        }
    }
}
