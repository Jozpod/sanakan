using Discord;
using DiscordBot.Services.PocketWaifu;
using Sanakan.DAL.Models;
using Sanakan.Game.Models;
using Sanakan.ShindenApi.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Item = Sanakan.DAL.Models.Item;

namespace Sanakan.Game.Services.Abstractions
{
    public interface IWaifuService
    {
        Card GenerateNewCard(ulong? discordUserId, CharacterInfo character, Rarity rarity);

        Card GenerateNewCard(ulong? discordUserId, CharacterInfo character);

        Card GenerateNewCard(ulong? discordUserId, CharacterInfo character, IEnumerable<Rarity> rarityExcluded);

        Embed GetBoosterPackList(IUser user, List<BoosterPack> packs);

        double GetExperienceToUpgrade(Card toUp, Card toSac);

        Embed GetActiveList(IEnumerable<Card> list);

        string EndExpedition(User user, Card card, bool showStats = false);

        int GetDefenceAfterLevelUp(Rarity oldRarity, int oldDefence);

        int GetAttactAfterLevelUp(Rarity oldRarity, int oldAttack);

        bool EventState { get; set; }

        void SetEventIds(IEnumerable<ulong> ids);

        Task<CharacterInfo?> GetRandomCharacterAsync();

        Embed GetItemList(IUser user, IEnumerable<Item> items);

        Task<string> GetWaifuProfileImageUrlAsync(Card card, IMessageChannel trashCh);

        List<Card> GetListInRightOrder(IEnumerable<Card> list, HaremType type, string tag);

        Task<List<Card>> OpenBoosterPackAsync(ulong? discordUserId, BoosterPack pack);

        Task<string> GetSafariViewAsync(SafariImage info, Card card, IMessageChannel trashChannel);

        Task<string> SendAndGetSafariImageUrlAsync(SafariImage info, IMessageChannel trashChannel); // GetSafariViewAsync

        Task<Embed> BuildCardImageAsync(Card card, ITextChannel trashChannel, IUser owner, bool showStats);

        Task<Embed> BuildCardViewAsync(Card card, ITextChannel trashChannel, IUser owner);

        FightHistory MakeFightAsync(IEnumerable<PlayerInfo> players, bool oneCard = false);

        string GetDeathLog(FightHistory fight, IEnumerable<PlayerInfo> players);

        Embed GetShopView(ItemWithCost[] items, string name = "Sklepik", string currency = "TC");

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
            string specialCommand);

        Task<SafariImage?> GetRandomSarafiImage();

        void DeleteCardImageIfExist(Card card);

        Task<string> GenerateAndSaveCardAsync(Card card, CardImageType type = CardImageType.Normal);

        Task<IEnumerable<Embed>> GetContentOfWishlist(
            List<ulong> cardsId,
            List<ulong> charactersId,
            List<ulong> titlesId);

        Task<IEnumerable<Card>> GetCardsFromWishlist(
            IEnumerable<ulong> cardsId,
            IEnumerable<ulong> charactersId,
            IEnumerable<ulong> titlesId,
            List<Card> allCards,
            IEnumerable<Card> userCards);

        (TimeSpan, TimeSpan) GetRealTimeOnExpedition(Card card, double karma);

        double GetProgressiveValueFromExpedition(double baseValue, double duration, double div);
    }
}
