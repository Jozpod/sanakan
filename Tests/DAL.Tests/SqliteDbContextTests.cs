using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sanakan.Common.Builder;
using Sanakan.Common.Configuration;
using Sanakan.DAL.Builder;
using Sanakan.DAL.Models;
using Sanakan.DAL.Models.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Sanakan.DAL.Tests
{
    [TestClass]
    public class SqliteDbContextTests
    {
        protected SanakanDbContext dbContext = null;
        protected ServiceProvider serviceProvider = null;

        [TestMethod]
        public async Task Should_Initialize_Model()
        {
            var serviceCollection = new ServiceCollection();

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.sqlite.json", optional: true, reloadOnChange: true);

            var configurationRoot = builder.Build();

            serviceCollection.AddOptions();
            serviceCollection.AddSystemClock();
            serviceCollection.AddSanakanDbContext(configurationRoot);
            serviceCollection.AddSingleton(configurationRoot);
            serviceCollection.AddCache(configurationRoot.GetSection("Cache"));
            serviceCollection.Configure<DatabaseConfiguration>(configurationRoot.GetSection("Database"));
            serviceCollection.Configure<SanakanConfiguration>(configurationRoot);
            serviceCollection.AddRepositories();
            serviceProvider = serviceCollection.BuildServiceProvider();

            dbContext = serviceProvider.GetRequiredService<SanakanDbContext>();

            await dbContext.Database.EnsureCreatedAsync();
            await GenerateTestData();
            await dbContext.Database.EnsureDeletedAsync();
        }

        public async Task GenerateTestData()
        {
            var user = new User(1, DateTime.UtcNow);

            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync();

            var wish = new WishlistObject
            {
                Id = 1,
                Type = WishlistObjectType.Title,
                ObjectName = "test",
                ObjectId = 1,
                GameDeckId = user.GameDeck.Id,
            };

            var guildConfig = new GuildOptions(1, 50);

            dbContext.Guilds.Add(guildConfig);
            await dbContext.SaveChangesAsync();

            dbContext.Wishes.Add(wish);
            await dbContext.SaveChangesAsync();

            var card = new Card(1, "test card", "test card", 10, 20, Rarity.A, Dere.Bodere, DateTime.UtcNow);

            user.GameDeck.Cards.Add(card);
            await dbContext.SaveChangesAsync();

            var timeStatus = new TimeStatus(StatusType.Card);
            timeStatus.GuildId = 1;
            timeStatus.UserId = 1;
            user.TimeStatuses.Add(timeStatus);
            await dbContext.SaveChangesAsync();
        }
    }
}
