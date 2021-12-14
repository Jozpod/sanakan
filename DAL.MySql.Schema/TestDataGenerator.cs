using Sanakan.DAL.Models;
using Sanakan.DAL.Models.Configuration;
using Sanakan.DAL.Models.Management;
using System;
using System.Threading.Tasks;

namespace Sanakan.DAL.MySql.Schema
{
    public class TestDataGenerator
    {
        private readonly SanakanDbContext _dbContext;

        public TestDataGenerator(SanakanDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task RunAsync()
        {
            var user1 = new User(1, DateTime.UtcNow);
            var user2 = new User(2, DateTime.UtcNow);
            user2.ShindenId = 1ul;
            var guildConfig = new GuildOptions(1, 50);
            var card1 = new Card(
            1, "test card 1", "test card 1",
            10, 20, Rarity.A,
            Dere.Bodere, DateTime.UtcNow);

            var user3 = new User(3, DateTime.UtcNow);
            var card2 = new Card(
               2, "test card 2", "test card 2",
               10, 20, Rarity.B,
               Dere.Bodere, DateTime.UtcNow);

            var penalty = new PenaltyInfo
            {
                UserId = 1ul,
                Reason = "test",
                GuildId = 1ul,
                Type = PenaltyType.Ban,
                StartedOn = DateTime.UtcNow,
                Duration = TimeSpan.FromMinutes(10),
            };

            var timeStatus1 = new TimeStatus(StatusType.Daily)
            {
                UserId = 1ul,
                GuildId = 1ul,
            };

            var timeStatus2 = new TimeStatus(StatusType.Globals)
            {
                UserId = 1ul,
                GuildId = 1ul,
            };

            var wishlistObject1 = new WishlistObject
            {
                ObjectName = "anime",
                Type = WishlistObjectType.Title,
                ObjectId = 1ul,
                GameDeckId = 1ul,
            };

            var wishlistObject2 = new WishlistObject
            {
                ObjectName = "card",
                Type = WishlistObjectType.Card,
                ObjectId = 2ul,
                GameDeckId = 1ul,
            };

            _dbContext.Users.AddRange(new[] { user1, user2, user3 });
            await _dbContext.SaveChangesAsync();

            _dbContext.Wishes.AddRange(new[] { wishlistObject1, wishlistObject2 });
            await _dbContext.SaveChangesAsync();

            _dbContext.TimeStatuses.AddRange(new[] { timeStatus1, timeStatus2 });
            await _dbContext.SaveChangesAsync();

            _dbContext.Guilds.Add(guildConfig);
            await _dbContext.SaveChangesAsync();

            card1.GameDeckId = user1.GameDeck.Id;
            _dbContext.Cards.Add(card1);
            await _dbContext.SaveChangesAsync();

            card2.GameDeckId = user3.GameDeck.Id;
            _dbContext.Cards.Add(card2);
            await _dbContext.SaveChangesAsync();

            _dbContext.Penalties.Add(penalty);
            await _dbContext.SaveChangesAsync();
        }
    }
}
