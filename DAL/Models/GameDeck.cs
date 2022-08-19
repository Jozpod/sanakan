using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;

namespace Sanakan.DAL.Models
{
    public class GameDeck
    {
        public GameDeck()
        {
            Cards = new Collection<Card>();
            Items = new Collection<Item>();
            BoosterPacks = new Collection<BoosterPack>();
            PvPStats = new Collection<CardPvPStats>();
            Wishes = new Collection<WishlistObject>();
            Figures = new Collection<Figure>();
        }

        /// <summary>
        /// The Discord user identifier and also <see cref="User"/> identifier.
        /// </summary>
        public ulong Id { get; set; }

        /// <summary>
        /// The amount of Cruelty Token points.
        /// </summary>
        public long CTCount { get; set; }

        /// <summary>
        /// The Shinden character identifier.
        /// </summary>
        public ulong? FavouriteWaifuId { get; set; }

        public double Karma { get; set; }

        public ulong ItemsDropped { get; set; }

        public bool WishlistIsPrivate { get; set; }

        public long PVPCoins { get; set; }

        public double DeckPower { get; set; }

        public long PVPWinStreak { get; set; }

        public long GlobalPVPRank { get; set; }

        public long SeasonalPVPRank { get; set; }

        public double MatchMakingRatio { get; set; }

        public ulong PVPDailyGamesPlayed { get; set; }

        public DateTime PVPSeasonBeginDate { get; set; }

        [StringLength(50)]
        public string? ExchangeConditions { get; set; }

        [StringLength(50)]
        public Uri? BackgroundImageUrl { get; set; }

        [StringLength(50)]
        public Uri? ForegroundImageUrl { get; set; }

        [StringLength(10)]
        public string? ForegroundColor { get; set; }

        public int ForegroundPosition { get; set; }

        public int BackgroundPosition { get; set; }

        public long MaxNumberOfCards { get; set; }

        public int CardsInGalleryCount { get; set; }

        public virtual ICollection<Card> Cards { get; set; }

        public virtual ICollection<Item> Items { get; set; }

        public virtual IList<BoosterPack> BoosterPacks { get; set; }

        public virtual ICollection<CardPvPStats> PvPStats { get; set; }

        public virtual ICollection<WishlistObject> Wishes { get; set; }

        public virtual ExperienceContainer ExperienceContainer { get; set; } = null!;

        public virtual ICollection<Figure> Figures { get; set; }

        /// <summary>
        /// The Discord user identifier.
        /// </summary>
        public ulong UserId { get; set; }

        [JsonIgnore]
        public virtual User User { get; set; } = null!;

        public int LimitOfCardsOnExpedition => 10;

        public string GetUserNameStatus() => Karma switch
        {
            var _ when Karma >= 2000 => "Papaj",
            var _ when Karma >= 1600 => "Miłościwy kumpel",
            var _ when Karma >= 1200 => "Oślepiony bugiem",
            var _ when Karma >= 800 => "Pan pokoiku",
            var _ when Karma >= 400 => "Błogosławiony rycerz",
            var _ when Karma >= 200 => "Pionek buga",
            var _ when Karma >= 100 => "Sługa buga",
            var _ when Karma >= 50 => "Biały koleś",
            var _ when Karma >= 10 => "Pantofel",
            var _ when Karma >= 5 => "Lizus",
            var _ when Karma <= -2000 => "Mroczny panocek",
            var _ when Karma <= -1600 => "Nienawistny koleżka",
            var _ when Karma <= -1200 => "Mściwy ślepiec",
            var _ when Karma <= -800 => "Pan wojenki",
            var _ when Karma <= -400 => "Przeklęty rycerz",
            var _ when Karma <= -200 => "Ciemny pionek",
            var _ when Karma <= -100 => "Sługa mroku",
            var _ when Karma <= -50 => "Murzynek",
            var _ when Karma <= -10 => "Rzezimieszek",
            var _ when Karma <= -5 => "Buntownik",
            _ => "Wieśniak",
        };

        public bool ReachedDailyMaxPVPCount() => PVPDailyGamesPlayed >= 10;

        public bool IsPVPSeasonalRankActive(DateTime date)
            => date.Month == PVPSeasonBeginDate.Month
                && date.Year == PVPSeasonBeginDate.Year;

        public DeckPowerStatus CanFightPvP(double minDeckPower, double maxDeckPower)
        {
            DeckPower = CalculateDeckPower();

            if (DeckPower > maxDeckPower)
            {
                return DeckPowerStatus.TooHigh;
            }

            if (DeckPower < minDeckPower)
            {
                return DeckPowerStatus.TooLow;
            }

            return DeckPowerStatus.Ok;
        }

        public string GetCardCountStats()
        {
            var stats = string.Empty;

            foreach (var rarity in RarityExtensions.Rarities)
            {
                var count = Cards.Count(x => x.Rarity == rarity);
                if (count > 0)
                {
                    stats += $"**{rarity.ToString().ToUpper()}**: {count} ";
                }
            }

            return stats;
        }

        public bool RemoveCharacterFromWishList(ulong id)
        {
            var wishlistObject = Wishes.FirstOrDefault(x => x.Type == WishlistObjectType.Character && x.ObjectId == id);

            if (wishlistObject != null)
            {
                Wishes.Remove(wishlistObject);
                return true;
            }

            return false;
        }

        public void RemoveFromWaifu(Card card)
        {
            if (FavouriteWaifuId == card.CharacterId)
            {
                card.Affection -= 25;
                FavouriteWaifuId = 0;
            }
        }

        public void RemoveCardFromWishList(ulong cardId)
        {
            var cardWishObject = Wishes.FirstOrDefault(x => x.Type == WishlistObjectType.Card
                && x.ObjectId == cardId);

            if (cardWishObject != null)
            {
                Wishes.Remove(cardWishObject);
            }
        }

        public bool CanFightPvPs(double minDeckPower, double maxDeckPower) => CanFightPvP(minDeckPower, maxDeckPower) == DeckPowerStatus.Ok;

        public double GetDeckPower() => DeckPower;

        public bool NeedToSetDeckAgain() => DeckPower == -1;

        public double CalculateDeckPower()
            => Cards.Where(x => x.Active).Sum(x => x.CalculateCardPower());

        public bool CanCreateDemon() => Karma <= -2000;

        public bool CanCreateAngel() => Karma >= 2000;

        public bool IsMarketDisabled() => Karma <= -400;

        public bool IsBlackMarketDisabled() => Karma > -400;

        public bool IsEvil() => Karma <= -10;

        public bool IsGood() => Karma >= 10;

        public bool IsNeutral() => Karma > -10 && Karma < 10;

        public double AffectionFromKarma()
        {
            var karmaDif = Karma / 150d;

            switch (karmaDif)
            {
                case var _ when karmaDif < -6:
                    karmaDif = -6;
                    break;
                case var _ when karmaDif > 6:
                    karmaDif = 6;
                    break;
            }

            return karmaDif;
        }

        public double GetStrongestCardPower()
        {
            return Cards
                .OrderByDescending(x => x.CardPower)
                .FirstOrDefault()?.CardPower ?? 0;
        }

        public IEnumerable<ulong> GetTitlesWishList()
        {
            return Wishes
                .Where(x => x.Type == WishlistObjectType.Title)
                .Select(x => x.ObjectId)
                .ToList();
        }

        public IEnumerable<ulong> GetCardsWishList()
        {
            return Wishes
                .Where(x => x.Type == WishlistObjectType.Card)
                .Select(x => x.ObjectId)
                .ToList();
        }

        public IEnumerable<ulong> GetCharactersWishList()
        {
            return Wishes
                .Where(x => x.Type == WishlistObjectType.Character)
                .Select(x => x.ObjectId)
                .ToList();
        }

        public string GetRankName(ulong experience) => experience switch
        {
            var n when n >= 17 => "Konsul",
            16 => "Praetor",
            15 => "Legatus",
            14 => "Preafectus classis",
            13 => "Praefectus praetoria",
            12 => "Tribunus laticavius",
            11 => "Prefectus",
            10 => "Tribunus angusticlavius",
            9 => "Praefectus castorium",
            8 => "Primus pilus",
            7 => "Primi ordines",
            6 => "Centurio",
            5 => "Decurio",
            4 => "Tesserarius",
            3 => "Optio",
            2 => "Aquilifier",
            1 => "Signifer",
            _ => "Miles gregarius", // (in ancient Rome) A common soldier; soldier of the ranks.
        };
    }
}
