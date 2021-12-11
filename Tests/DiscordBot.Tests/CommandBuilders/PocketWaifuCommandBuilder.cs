﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sanakan.DiscordBot.Modules;
using Discord;
using Sanakan.DiscordBot.Services;
using Sanakan.DAL.Models;

namespace Sanakan.DiscordBot.Tests.IntegrationTests
{
    /// <summary>
    /// Provides methods to build commands in <see cref="PocketWaifuModule"/>.
    /// </summary>
    public static class PocketWaifuCommandBuilder
    {
        /// <summary>
        /// <see cref="PocketWaifuModule.AddToWishlistAsync(WishlistObjectType, ulong)"/>.
        /// </summary>
        public static string AddToWishlist(string prefix) => $"{prefix}wadd";

        /// <summary>
        /// <see cref="PocketWaifuModule.BuyItemActivityAsync(int, string)"/>.
        /// </summary>
        public static string BuyItemActivity(string prefix, int itemNumber = 0, string info = "0") => $"{prefix}ac shop";

        /// <summary>
        /// <see cref="PocketWaifuModule.BuyItemAsync(int, string)"/>.
        /// </summary>
        public static string BuyItem(string prefix, int itemNumber = 0, string info = "0") => $"{prefix}shop";

        /// <summary>
        /// <see cref="PocketWaifuModule.BuyItemPvPAsync(int, string)"/>.
        /// </summary>
        public static string BuyItemPvP(string prefix, int itemNumber = 0, string info = "0") => $"{prefix}pvp shop";

        /// <summary>
        /// <see cref="PocketWaifuModule.SendCardToExpeditionAsync(ulong, ExpeditionCardType)"/>.
        /// </summary>
        public static string SendCardToExpedition(string prefix, ulong wid, string expeditionCardType) => $"{prefix}wyprawa {wid} {expeditionCardType}";

        /// <summary>
        /// <see cref="PocketWaifuModule.ShowItemsAsync(int)"/>.
        /// </summary>
        public static string ShowItems(string prefix, int itemNumber) => $"{prefix}item {itemNumber}";

        /// <summary>
        /// <see cref="PocketWaifuModule.ShowCardImageAsync(ulong, bool)"/>.
        /// </summary>
        public static string ShowCardImage(string prefix, ulong cardId, bool showStats) => $"{prefix}card image {cardId}";

        /// <summary>
        /// <see cref="PocketWaifuModule.DestroyCardAsync(ulong[])"/>.
        /// </summary>
        public static string DestroyCard(string prefix, ulong cardId) => $"{prefix}destroy {cardId}";
    }
}
