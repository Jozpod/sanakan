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

namespace Sanakan.Web.Tests.IntegrationTests
{
    /// <summary>
    /// Defines tests for <see cref="UserController"/>.
    /// </summary>
    public partial class TestBase
    {

        [TestMethod]
        public async Task Should_Return_Discord_User()
        {
            await Task.Delay(TimeSpan.FromSeconds(5));
            var discordUserId = 1ul;
            await AuthorizeAsync();

            var user = await _client.GetFromJsonAsync<User>($"api/user/discord/{discordUserId}");
            user.Should().NotBeNull();
        }
    }
}
