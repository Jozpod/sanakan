using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sanakan.DAL.Models;
using Sanakan.DAL.Models.Configuration;
using Sanakan.DAL.Repositories.Abstractions;
using System;
using System.Threading.Tasks;

namespace Sanakan.DAL.Tests
{
    [TestClass]
    public class CardRepositoryTests : TestBase
    {
        [TestMethod]
        public async Task Should_CRUD_Entity()
        {
            var repository = ServiceProvider.GetRequiredService<ICardRepository>();

            var user = new User(1, DateTime.UtcNow);

            DbContext.Users.Add(user);
            await DbContext.SaveChangesAsync();

            var card = new Card(
                1, "test card", "test card",
                10, 20, Rarity.A,
                Dere.Bodere, DateTime.UtcNow);
           
            user.GameDeck.Cards.Add(card);
            await DbContext.SaveChangesAsync();

            var actual = await repository.GetByIdAsync(card.Id);
            actual.Should().BeEquivalentTo(card, pr => pr
                .Excluding(npr => npr.ArenaStats)
                .Excluding(npr => npr.GameDeck)
                .Excluding(npr => npr.ExpeditionDate)
                .Excluding(npr => npr.CreatedOn));
        }
    }
}
