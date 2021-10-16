using Discord;
using Discord.WebSocket;
using Sanakan.DAL.Models;
using Sanakan.Services.PocketWaifu;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiscordBot.Services.PocketWaifu.Abstractions
{
    public interface IWaifuService
    {
        bool GetEventSate();
        void SetEventState(bool state);
        void SetEventIds(List<ulong> ids);
        Embed GetItemList(SocketUser user, List<Item> items);
        Task<string> GetWaifuProfileImageAsync(Card card, ITextChannel trashCh);
        List<Card> GetListInRightOrder(IEnumerable<Card> list, HaremType type, string tag);
        Task<List<Card>> OpenBoosterPackAsync(IUser user, BoosterPack pack);
        Task<string> GetSafariViewAsync(SafariImage info, Card card, ITextChannel trashChannel);
        Task<string> GetSafariViewAsync(SafariImage info, ITextChannel trashChannel);
        Task<Embed> BuildCardImageAsync(Card card, ITextChannel trashChannel, SocketUser owner, bool showStats);
        Task<Embed> BuildCardViewAsync(Card card, ITextChannel trashChannel, SocketUser owner);
        Embed GetShopView(ItemWithCost[] items, string name = "Sklepik", string currency = "TC");
        Embed GetItemShopInfo(ItemWithCost item);
        Task<Embed> ExecuteShopAsync(
            ShopType type,
            IUser discordUser,
            int selectedItem,
            string specialCmd);
        SafariImage GetRandomSarafiImage();
        void DeleteCardImageIfExist(Card card);
        Task<string> GenerateAndSaveCardAsync(Card card, CardImageType type = CardImageType.Normal);
        Task<IEnumerable<Embed>> GetContentOfWishlist(List<ulong> cardsId, List<ulong> charactersId, List<ulong> titlesId);
        Task<IEnumerable<Card>> GetCardsFromWishlist(List<ulong> cardsId, List<ulong> charactersId, List<ulong> titlesId, List<Card> cards, IEnumerable<Card> userCards);
        Tuple<double, double> GetRealTimeOnExpeditionInMinutes(Card card, double karma);
        double GetProgressiveValueFromExpedition(double baseValue, double duration, double div);
    }
}
