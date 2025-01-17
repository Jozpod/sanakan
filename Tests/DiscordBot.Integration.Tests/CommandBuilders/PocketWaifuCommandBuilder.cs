﻿using Sanakan.DAL.Models;
using Sanakan.DiscordBot.Modules;

namespace Sanakan.DiscordBot.Integration.Tests.CommandBuilders
{
    /// <summary>
    /// Provides methods to build commands in <see cref="PocketWaifuModule"/>.
    /// </summary>
    public static class PocketWaifuCommandBuilder
    {
        /// <summary>
        /// <see cref="PocketWaifuModule.AddToWishlistAsync(WishlistObjectType, ulong)"/>.
        /// </summary>
        public static string AddToWishlist(string prefix, string wishlistObjectType, ulong id) => $"{prefix}wadd {wishlistObjectType} {id}";

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
        public static string SendCardToExpedition(string prefix, ulong wid, string expeditionCardType) => $"{prefix}expedition {wid} {expeditionCardType}";

        /// <summary>
        /// <see cref="PocketWaifuModule.ShowItemsAsync(int)"/>.
        /// </summary>
        public static string ShowItems(string prefix, int itemNumber) => $"{prefix}item {itemNumber}";

        /// <summary>
        /// <see cref="PocketWaifuModule.ShowCardImageAsync(ulong, bool)"/>.
        /// </summary>
        public static string ShowCardImage(string prefix, ulong cardId, bool showStats) => $"{prefix}card image {cardId}";

        /// <summary>
        /// <see cref="PocketWaifuModule.ShowCardsAsync(Game.Models.HaremType, string?)"/>.
        /// </summary>
        public static string ShowCards(string prefix, string haremType) => $"{prefix}cards {haremType}";

        /// <summary>
        /// <see cref="PocketWaifuModule.DestroyCardAsync(ulong[])"/>.
        /// </summary>
        public static string DestroyCard(string prefix, ulong cardId) => $"{prefix}destroy {cardId}";

        /// <summary>
        /// <see cref="PocketWaifuModule.CraftCardAsync"/>.
        /// </summary>
        public static string CraftCard(string prefix) => $"{prefix}crafting";

        /// <summary>
        /// <see cref="PocketWaifuModule.GetFreeCardAsync"/>.
        /// </summary>
        public static string GetFreeCard(string prefix) => $"{prefix}karta+";

        /// <summary>
        /// <see cref="PocketWaifuModule.UpdateCardAsync(ulong, bool)"/>.
        /// </summary>
        public static string UpdateCard(string prefix, ulong cardId) => $"{prefix}update {cardId}";

        /// <summary>
        /// <see cref="PocketWaifuModule.UseItemAsync(int, ulong, string)"/>.
        /// </summary>
        public static string UseItem(string prefix, int itemNumber = 0, ulong cardId = 0)
        {
            var command = $"{prefix}use";

            if(itemNumber != 0)
            {
                command += $" {itemNumber}";
            }

            if (cardId != 0)
            {
                command += $" {cardId}";
            }

            return command;
        }

        /// <summary>
        /// <see cref="PocketWaifuModule.OpenPacketAsync(int, int, bool)"/>.
        /// </summary>
        public static string OpenPacket(string prefix, int numberOfPack = 0, int count = 1, bool checkWishlists = false)
        {
            var command = $"{prefix}pakiet";

            if(numberOfPack != 0)
            {
                command += $" {numberOfPack}";
            }

            if (count != 1)
            {
                command += $" {count}";
            }

            if (checkWishlists)
            {
                command += " tak";
            }

            return command;
        }
    }
}
