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
using Sanakan.DAL.Repositories;

namespace Sanakan.Web.Tests.IntegrationTests
{
    /// <summary>
    /// Defines tests for <see cref="WaifuController"/>.
    /// </summary>
    public partial class TestBase
    {
        /// <summary>
        /// Defines test for <see cref="WaifuController.GetUserIdsOwningCharacterCardAsync(ulong)"/> method.
        /// </summary>
        [TestMethod]
        public async Task Should_Return_User_Ids()
        {
            var characterId = 1ul;
            var userIds = await _client.GetFromJsonAsync<IEnumerable<ulong>>($"api/waifu/users/owning/character/{characterId}");
            userIds.Should().NotBeNull();
        }

        /// <summary>
        /// Defines test for <see cref="WaifuController.GetUserCardsAsync(ulong)"/> method.
        /// </summary>
        [TestMethod]
        public async Task Should_Return_User_Cards()
        {
            var shindenUserId = 1ul;
            var cards = await _client.GetFromJsonAsync<IEnumerable<Card>>($"user/{shindenUserId}/cards");
            cards.Should().NotBeNull();
        }

        /// <summary>
        /// Defines test for <see cref="WaifuController.GetUsersCardsByShindenIdWithOffsetAndFilterAsync(ulong, uint, uint, DAL.Repositories.CardsQueryFilter)"/> method.
        /// </summary>
        [TestMethod]
        public async Task Should_Return_Filtered_User_Cards()
        {
            var shindenUserId = 1ul;
            var offset = 0;
            var count = 0;
            var filter = new CardsQueryFilter
            {

            };
            var response = await _client.PostAsJsonAsync($"user/{shindenUserId}/cards/{offset}/{count}", filter);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<FilteredCards>();
            result.Should().NotBeNull();
            result.TotalCards.Should().Be(1);
            result.Cards.Should().NotBeEmpty();
        }
    }
}
