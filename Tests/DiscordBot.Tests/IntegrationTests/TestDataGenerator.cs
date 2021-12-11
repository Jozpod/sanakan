using Sanakan.DAL;
using Sanakan.DAL.Models;
using Sanakan.DAL.Models.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sanakan.DiscordBot.Tests.IntegrationTests
{
    public static class TestDataGenerator
    {
        public static async Task PopulateDatabaseAsync(
            SanakanDbContext dbContext,
            ulong userId,
            ulong botUserId,
            DiscordIntegrationTestOptions options)
        {
            var guildConfig = new GuildOptions(options.GuildId, 50);
            guildConfig.MuteRoleId = options.MuteRoleId;
            guildConfig.UserRoleId = options.UserRoleId;

            var user = new User(userId, DateTime.UtcNow);
            user.ShindenId = 1ul;
            user.ScCount = 10000;
            user.TcCount = 10000;
            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync();

            var botUser = new User(botUserId, DateTime.UtcNow);
            dbContext.Users.Add(botUser);
            await dbContext.SaveChangesAsync();

            var card1 = new Card(1ul, "card 1", "card 1", 100, 50, Rarity.A, Dere.Bodere, DateTime.UtcNow);
            var card2 = new Card(2ul, "card 2", "card 2", 80, 80, Rarity.A, Dere.Bodere, DateTime.UtcNow);
            var card3 = new Card(3ul, "card 3", "card 3", 90, 100, Rarity.A, Dere.Bodere, DateTime.UtcNow);
            user.GameDeck.Cards.Add(card1);
            user.GameDeck.Cards.Add(card2);
            user.GameDeck.Cards.Add(card3);
            await dbContext.SaveChangesAsync();

            var item1 = new Item { Name = nameof(ItemType.AffectionRecoveryBig), Type = ItemType.AffectionRecoveryBig, Count = 100 };
            user.GameDeck.Items.Add(item1);
            await dbContext.SaveChangesAsync();

            dbContext.Guilds.Add(guildConfig);
            await dbContext.SaveChangesAsync();

            guildConfig.WaifuConfig = new WaifuConfiguration
            {
                TrashCommandsChannelId = options.MainChannelId,
            };
            await dbContext.SaveChangesAsync();
        }
    }
}
