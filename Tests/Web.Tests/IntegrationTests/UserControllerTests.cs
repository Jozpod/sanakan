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
using Moq;
using Discord;
using Sanakan.ShindenApi;

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
            var username = "username";
            var userSearchResults = new Result<List<UserSearchResult>>
            {
                Value = new List<UserSearchResult>
                {
                    new UserSearchResult
                    {
                        Id = 1ul,
                        Avatar = "test",
                        Name = "test",
                    },
                },
            };

            _factory.ShindenClientMock
                .Setup(pr => pr.SearchUserAsync(username))
                .ReturnsAsync(userSearchResults);

            var response = await _client.PostAsJsonAsync($"api/user/find", username);
            response.EnsureSuccessStatusCode();
            var users = await response.Content.ReadFromJsonAsync<IEnumerable<UserSearchResult>>();
            users.Should().NotBeNull();
            users.Should().HaveCount(1);
        }

        /// <summary>
        /// Defines test for <see cref="UserController.RegisterUserAsync(UserRegistration)"/> method.
        /// </summary>
        [TestMethod]
        public async Task Should_Connect_User()
        {
            var userId = 3ul;
            var model = new UserRegistration
            {
                DiscordUserId = 2ul,
                Username = "test user",
                ForumUserId = userId,
            };

            var searchResult = new ShindenApi.Result<List<UserSearchResult>>
            {
                Value = new List<UserSearchResult>
                {
                    new UserSearchResult()
                    {
                        Id = userId,
                    }
                }
            };

            var userResult = new ShindenApi.Result<UserInfo>
            {
                Value = new UserInfo
                {
                    Id = userId,
                }
            };

            _factory.ShindenClientMock
                .Setup(pr => pr.SearchUserAsync(model.Username))
                .ReturnsAsync(searchResult);

            _factory.ShindenClientMock
                .Setup(pr => pr.GetUserInfoAsync(userId))
                .ReturnsAsync(userResult);

            _factory.DiscordClientMock
                .Setup(pr => pr.GetUserAsync(model.DiscordUserId, CacheMode.AllowDownload, null))
                .ReturnsAsync(new Mock<IUser>().Object);

            var response = await _client.PutAsJsonAsync($"api/user/register", model);
            response.EnsureSuccessStatusCode();
        }
    }
}
