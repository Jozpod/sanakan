using Sanakan.DAL;
using Sanakan.DAL.Models;
using System;
using System.Threading.Tasks;

namespace Sanakan.Web.Integration.Tests
{
    public static class TestDataGenerator
    {
        public static async Task RunAsync(SanakanDbContext dbContext)
        {
            var user = new User(1ul, DateTime.UtcNow);
            user.ShindenId = 1ul;
            var card = new Card(1ul, "title", "name", 100, 50, Rarity.A, Dere.Bodere, DateTime.UtcNow);

            var notConnectedUser = new User(2ul, DateTime.UtcNow);

            dbContext.Users.AddRange(new[] { user, notConnectedUser });
            await dbContext.SaveChangesAsync();

            user.GameDeck.Cards.Add(card);
            await dbContext.SaveChangesAsync();
        }
    }
}
