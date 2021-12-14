using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sanakan.Api.Models;
using Sanakan.DAL.Models;
using Sanakan.DAL.Repositories;
using Sanakan.Web.Controllers;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Sanakan.Web.Integration.Tests
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
            var cards = await _client.GetFromJsonAsync<IEnumerable<Card>>($"api/waifu/user/{shindenUserId}/cards");
            cards.Should().NotBeNull();
        }

        /// <summary>
        /// Defines test for <see cref="WaifuController.GetUsersCardsByShindenIdWithOffsetAndFilterAsync(ulong, uint, uint, CardsQueryFilter)"/> method.
        /// </summary>
        [TestMethod]
        public async Task Should_Return_Filtered_User_Cards()
        {
            var shindenUserId = 1ul;
            var offset = 0;
            var take = 1;
            var filter = new CardsQueryFilter()
            {
                SearchText = "title",
            };
            var response = await _client.PostAsJsonAsync($"api/waifu/user/{shindenUserId}/cards/{offset}/{take}", filter);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<FilteredCards>();
            result.Should().NotBeNull();
            result.TotalCards.Should().Be(1);
            result.Cards.Should().NotBeEmpty();
        }
    }
}