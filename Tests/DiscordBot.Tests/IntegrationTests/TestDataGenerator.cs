﻿using Sanakan.DAL;
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
            DiscordIntegrationTestOptions options)
        {
            var guildConfig = new GuildOptions(options.GuildId, 50);
            guildConfig.MuteRoleId = options.MuteRoleId;
            guildConfig.UserRoleId = options.UserRoleId;

            var user = new User(userId, DateTime.UtcNow);
            user.ShindenId = 1ul;
            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync();

            var card = new Card(1ul, "test", "test", 100, 50, Rarity.A, Dere.Bodere, DateTime.UtcNow);
            //card.ImageUrl = new Uri();
            user.GameDeck.Cards.Add(card);
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
