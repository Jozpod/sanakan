using Discord;
using Discord.WebSocket;
using Sanakan.DAL.Models;
using Sanakan.Services.PocketWaifu;
using Sanakan.Services.PocketWaifu.Fight;
using Sanakan.ShindenApi.Models;
using Shinden.API;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Item = Sanakan.DAL.Models.Item;

namespace DiscordBot.Services.PocketWaifu.Abstractions
{
    public interface IWaifuService
    {
        Card GenerateNewCard(IUser user, CharacterInfo character, Rarity rarity);
        Card GenerateNewCard(IUser? user, CharacterInfo character);
        Card GenerateNewCard(IUser user, CharacterInfo character, List<Rarity> rarityExcluded);
        Embed GetBoosterPackList(IUser user, List<BoosterPack> packs);
        double GetExpToUpgrade(Card toUp, Card toSac);
        ItemType RandomizeItemFromMarket();
        Quality RandomizeItemQualityFromMarket();
        ItemType RandomizeItemFromBlackMarket();
        Embed GetActiveList(IEnumerable<Card> list);
        string EndExpedition(User user, Card card, bool showStats = false);
        int GetDefenceAfterLevelUp(Rarity oldRarity, int oldDef);
        int GetAttactAfterLevelUp(Rarity oldRarity, int oldAtk);
        bool EventState { get; set; }
        void SetEventIds(List<ulong> ids);
        Task<CharacterInfo> GetRandomCharacterAsync();
        Embed GetItemList(IUser user, List<Item> items);
        Task<string> GetWaifuProfileImageAsync(Card card, IMessageChannel trashCh);
        List<Card> GetListInRightOrder(IEnumerable<Card> list, HaremType type, string tag);
        Task<List<Card>> OpenBoosterPackAsync(IUser user, BoosterPack pack);
        Task<string> GetSafariViewAsync(SafariImage info, Card card, ITextChannel trashChannel);
        Task<string> GetSafariViewAsync(SafariImage info, ITextChannel trashChannel);
        Task<Embed> BuildCardImageAsync(Card card, ITextChannel trashChannel, IUser owner, bool showStats);
        Task<Embed> BuildCardViewAsync(Card card, ITextChannel trashChannel, IUser owner);
        FightHistory MakeFightAsync(List<PlayerInfo> players, bool oneCard = false);
        string GetDeathLog(FightHistory fight, List<PlayerInfo> players);
        Embed GetShopView(ItemWithCost[] items, string name = "Sklepik", string currency = "TC");
        Embed GetItemShopInfo(ItemWithCost item);
        Task<IEnumerable<Embed>> GetWaifuFromCharacterSearchResult(
            string title,
            IEnumerable<Card> cards,
            IDiscordClient client,
            bool mention);
        Task<IEnumerable<Embed>> GetWaifuFromCharacterTitleSearchResult(
            IEnumerable<Card> cards,
            IDiscordClient client,
            bool mention);
        Task<Embed> ExecuteShopAsync(
            ShopType type,
            IUser discordUser,
            int selectedItem,
            string specialCmd);
        Task<SafariImage?> GetRandomSarafiImage();
        void DeleteCardImageIfExist(Card card);
        Task<string> GenerateAndSaveCardAsync(Card card, CardImageType type = CardImageType.Normal);
        Task<IEnumerable<Embed>> GetContentOfWishlist(
            List<ulong> cardsId,
            List<ulong> charactersId,
            List<ulong> titlesId);
        Task<IEnumerable<Card>> GetCardsFromWishlist(
            List<ulong> cardsId,
            List<ulong> charactersId,
            List<ulong> titlesId,
            List<Card> cards,
            IEnumerable<Card> userCards);
        Tuple<double, double> GetRealTimeOnExpeditionInMinutes(Card card, double karma);
        double GetProgressiveValueFromExpedition(double baseValue, double duration, double div);
    }
}
