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

        public string GetUserNameStatus()
        {
            if (Karma >= 2000)
            {
                return $"Papaj";
            }

            if (Karma >= 1600)
            {
                return $"Miłościwy kumpel";
            }

            if (Karma >= 1200)
            {
                return $"Oślepiony bugiem";
            }

            if (Karma >= 800)
            {
                return $"Pan pokoiku";
            }

            if (Karma >= 400)
            {
                return $"Błogosławiony rycerz";
            }

            if (Karma >= 200)
            {
                return $"Pionek buga";
            }

            if (Karma >= 100) return $"Sługa buga";
            if (Karma >= 50) return $"Biały koleś";
            if (Karma >= 10) return $"Pantofel";
            if (Karma >= 5) return $"Lizus";
            if (Karma <= -2000) return $"Mroczny panocek";
            if (Karma <= -1600) return $"Nienawistny koleżka";
            if (Karma <= -1200) return $"Mściwy ślepiec";
            if (Karma <= -800) return $"Pan wojenki";
            if (Karma <= -400) return $"Przeklęty rycerz";
            if (Karma <= -200) return $"Ciemny pionek";
            if (Karma <= -100) return $"Sługa mroku";
            if (Karma <= -50) return $"Murzynek";
            if (Karma <= -10) return $"Rzezimieszek";
            if (Karma <= -5) return $"Buntownik";
            return "Wieśniak";
        }

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
            var en = Wishes.FirstOrDefault(x => x.Type == WishlistObjectType.Character && x.ObjectId == id);
            if (en != null)
            {
                Wishes.Remove(en);
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

        public int LimitOfCardsOnExpedition() => 10;

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
            if (karmaDif < -6) karmaDif = -6;
            if (karmaDif > 6) karmaDif = 6;
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

        public string GetRankName(ulong experience)
        {
            switch (experience)
            {
                case var n when (n >= 17):
                    return "Konsul";

                case 16: return "Praetor";
                case 15: return "Legatus";
                case 14: return "Preafectus classis";
                case 13: return "Praefectus praetoria";
                case 12: return "Tribunus laticavius";
                case 11: return "Prefectus";
                case 10: return "Tribunus angusticlavius";
                case 9: return "Praefectus castorium";
                case 8: return "Primus pilus";
                case 7: return "Primi ordines";
                case 6: return "Centurio";
                case 5: return "Decurio";
                case 4: return "Tesserarius";
                case 3: return "Optio";
                case 2: return "Aquilifier";
                case 1: return "Signifer";

                default:
                    // (in ancient Rome) A common soldier; soldier of the ranks.
                    return "Miles gregarius";
            }
        }
    }
}
