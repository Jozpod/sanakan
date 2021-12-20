using Sanakan.DAL;
using Sanakan.DAL.Models;
using Sanakan.DAL.Models.Configuration;
using System;
using System.Threading.Tasks;

namespace Sanakan.DiscordBot.Integration.Tests
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
            guildConfig.WaifuRoleId = options.WaifuRoleId;

            var rootUser = new User(1ul, DateTime.UtcNow);
            dbContext.Users.Add(rootUser);
            await dbContext.SaveChangesAsync();
            var kyleCard = new Card(1ul, "Kyle Rittenhouse", "Kyle Rittenhouse", 100, 50, Rarity.A, Dere.Bodere, DateTime.UtcNow);
            rootUser.GameDeck.Cards.Add(kyleCard);
            await dbContext.SaveChangesAsync();

            var fakeUser = new User(userId, DateTime.UtcNow);
            fakeUser.ShindenId = 1ul;
            fakeUser.ScCount = 10000;
            fakeUser.TcCount = 10000;
            dbContext.Users.Add(fakeUser);
            await dbContext.SaveChangesAsync();
            var card = new Card(2ul, "Test Card", "Test Card", 100, 50, Rarity.A, Dere.Bodere, DateTime.UtcNow);
            fakeUser.GameDeck.Cards.Add(card);
            var boosterPacks = new[]{
                new BoosterPack
                {
                    Name = BoosterPackTypes.Activity,
                    CardCount = 5,
                    CardSourceFromPack = CardSource.Activity,
                },
                new BoosterPack
                {
                    Name = BoosterPackTypes.Activity,
                    CardCount = 3,
                    CardSourceFromPack = CardSource.Activity,
                },
            };
            foreach (var boosterPack in boosterPacks)
            {
                fakeUser.GameDeck.BoosterPacks.Add(boosterPack);
            }
            await dbContext.SaveChangesAsync();
           
            foreach (var item in Enum.GetValues<ItemType>())
            {
                fakeUser.GameDeck.Items.Add(new Item
                {
                    Name = item.ToString(),
                    Count = 10,
                    Type = item,
                });
            }
            await dbContext.SaveChangesAsync();

            var botUser = new User(botUserId, DateTime.UtcNow);
            dbContext.Users.Add(botUser);
            await dbContext.SaveChangesAsync();

            var card1 = new Card(3ul, "card 1", "card 1", 100, 50, Rarity.A, Dere.Bodere, DateTime.UtcNow);
            var card2 = new Card(4ul, "card 2", "card 2", 80, 80, Rarity.A, Dere.Bodere, DateTime.UtcNow);
            var card3 = new Card(5ul, "card 3", "card 3", 90, 100, Rarity.A, Dere.Bodere, DateTime.UtcNow);
            fakeUser.GameDeck.Cards.Add(card1);
            fakeUser.GameDeck.Cards.Add(card2);
            fakeUser.GameDeck.Cards.Add(card3);
            await dbContext.SaveChangesAsync();

            var item1 = new Item { Name = nameof(ItemType.AffectionRecoveryBig), Type = ItemType.AffectionRecoveryBig, Count = 100 };
            fakeUser.GameDeck.Items.Add(item1);
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
