﻿using Discord;
using Discord.WebSocket;
using Sanakan.DAL.Models;
using Sanakan.Services.PocketWaifu;
using Sanakan.Services.PocketWaifu.Fight;
using Shinden.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiscordBot.Services.PocketWaifu.Abstractions
{
    public interface IWaifuService
    {
        string EndExpedition(User user, Card card, bool showStats = false);
        bool GetEventSate();
        int GetDefenceAfterLevelUp(Rarity oldRarity, int oldDef);
        int GetAttactAfterLevelUp(Rarity oldRarity, int oldAtk);
        void SetEventState(bool state);
        void SetEventIds(List<ulong> ids);
        Task<ICharacterInfo> GetRandomCharacterAsync();
        Embed GetItemList(SocketUser user, List<Item> items);
        Task<string> GetWaifuProfileImageAsync(Card card, ITextChannel trashCh);
        List<Card> GetListInRightOrder(IEnumerable<Card> list, HaremType type, string tag);
        Task<List<Card>> OpenBoosterPackAsync(IUser user, BoosterPack pack);
        Task<string> GetSafariViewAsync(SafariImage info, Card card, ITextChannel trashChannel);
        Task<string> GetSafariViewAsync(SafariImage info, ITextChannel trashChannel);
        Task<Embed> BuildCardImageAsync(Card card, ITextChannel trashChannel, SocketUser owner, bool showStats);
        Task<Embed> BuildCardViewAsync(Card card, ITextChannel trashChannel, SocketUser owner);
        FightHistory MakeFightAsync(List<PlayerInfo> players, bool oneCard = false);
        string GetDeathLog(FightHistory fight, List<PlayerInfo> players);
        Card GenerateNewCard(IUser user, ICharacterInfo character);
        Card GenerateNewCard(IUser user, ICharacterInfo character, List<Rarity> rarityExcluded);
        Embed GetShopView(ItemWithCost[] items, string name = "Sklepik", string currency = "TC");
        Embed GetItemShopInfo(ItemWithCost item);
        List<Embed> GetWaifuFromCharacterTitleSearchResult(
            IEnumerable<Card> cards,
            DiscordSocketClient client,
            bool mention);
        Task<Embed> ExecuteShopAsync(
            ShopType type,
            IUser discordUser,
            int selectedItem,
            string specialCmd);
        Task<SafariImage?> GetRandomSarafiImage();
        void DeleteCardImageIfExist(Card card);
        Task<string> GenerateAndSaveCardAsync(Card card, CardImageType type = CardImageType.Normal);
        Task<IEnumerable<Embed>> GetContentOfWishlist(List<ulong> cardsId, List<ulong> charactersId, List<ulong> titlesId);
        Task<IEnumerable<Card>> GetCardsFromWishlist(List<ulong> cardsId, List<ulong> charactersId, List<ulong> titlesId, List<Card> cards, IEnumerable<Card> userCards);
        Tuple<double, double> GetRealTimeOnExpeditionInMinutes(Card card, double karma);
        double GetProgressiveValueFromExpedition(double baseValue, double duration, double div);
    }
}
