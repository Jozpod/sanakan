using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sanakan.DAL.Models;
using Sanakan.DAL.Repositories.Abstractions;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Sanakan.DAL.Tests
{
    [TestClass]
    public class GameDeckRepositoryTests : TestBase
    {
        [TestMethod]
        public async Task Should_CRUD_Entity()
        {
            var repository = ServiceProvider.GetRequiredService<IGameDeckRepository>();

            var user = new User(2, DateTime.UtcNow);

            DbContext.Users.Add(user);
            await DbContext.SaveChangesAsync();

            var wish = new WishlistObject
            {
                Id = 1,
                Type = WishlistObjectType.Title,
                ObjectId = 1,
            };

            user.GameDeck.Wishes.Add(wish);
            await DbContext.SaveChangesAsync();

            var actual = await repository.GetByAnimeIdAsync(wish.ObjectId);
            actual.First().Should().BeEquivalentTo(user.GameDeck);
        }
    }
}
