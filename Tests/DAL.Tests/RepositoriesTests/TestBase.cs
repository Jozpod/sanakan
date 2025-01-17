﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sanakan.Common.Builder;
using Sanakan.Common.Configuration;
using Sanakan.DAL.Builder;
using Sanakan.DAL.Models;
using Sanakan.DAL.Models.Configuration;
using Sanakan.DAL.Models.Management;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Sanakan.DAL.Tests
{
    [TestClass]
    public class TestBase
    {
        protected static SanakanDbContext dbContext = null;
        protected static ServiceProvider serviceProvider = null;

        [AssemblyInitialize]
        public static async Task ClassInitialize(TestContext context)
        {
            var serviceCollection = new ServiceCollection();

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
#if RELEASE
                .AddJsonFile("appsettings.in-memory.json", optional: true, reloadOnChange: true);
#else
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
#endif

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
        }

        public static async Task GenerateTestData()
        {
            var user1 = new User(1, DateTime.UtcNow);
            var user2 = new User(2, DateTime.UtcNow);
            var user3 = new User(3, DateTime.UtcNow);
            user3.ShindenId = 1ul;
            var user4 = new User(4, DateTime.UtcNow);
            user4.ShindenId = 2ul;
            var user5 = new User(5, DateTime.UtcNow);
            user5.ShindenId = 2ul;

            dbContext.Users.AddRange(user1, user2, user3, user4, user5);
            await dbContext.SaveChangesAsync();

            var wish = new WishlistObject
            {
                Id = 1,
                Type = WishlistObjectType.Title,
                ObjectName = "test",
                ObjectId = 1,
                GameDeckId = user1.GameDeck.Id,
            };

            var guildConfig = new GuildOptions(1, 50);

            dbContext.Guilds.Add(guildConfig);
            await dbContext.SaveChangesAsync();

            dbContext.Wishes.Add(wish);
            await dbContext.SaveChangesAsync();

            var card1 = new Card(1, "test card", "test card", 10, 20, Rarity.A, Dere.Bodere, DateTime.UtcNow);

            var card2 = new Card(1, "test card", "test card", 10, 20, Rarity.B, Dere.Bodere, DateTime.UtcNow);
            card2.LastOwnerId = 1ul;

            user1.GameDeck.Cards.Add(card1);
            await dbContext.SaveChangesAsync();

            user2.GameDeck.Cards.Add(card2);
            await dbContext.SaveChangesAsync();

            var timeStatus = new TimeStatus(StatusType.Card);
            timeStatus.GuildId = 1;
            timeStatus.UserId = 1;
            user1.TimeStatuses.Add(timeStatus);
            await dbContext.SaveChangesAsync();

            var penaltyInfo = new PenaltyInfo
            {
                Id = 1,
                Type = PenaltyType.Ban,
                Duration = TimeSpan.FromDays(1),
                GuildId = 1,
                Reason = "test",
                StartedOn = DateTime.UtcNow,
                UserId = 1,
            };
            dbContext.Penalties.Add(penaltyInfo);
            await dbContext.SaveChangesAsync();
        }

        [AssemblyCleanup]
        public static async Task ClassCleanup()
        {
            if (dbContext != null)
            {
                await dbContext.Database.EnsureDeletedAsync();
            }
        }
    }
}
