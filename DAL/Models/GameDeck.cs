﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Sanakan.DAL.Models
{
    public class GameDeck
    {
        public ulong Id { get; set; }
        public long CTCnt { get; set; }
        public ulong Waifu { get; set; }
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
        public string? BackgroundImageUrl { get; set; }

        [StringLength(50)]
        public string? ForegroundImageUrl { get; set; }

        [StringLength(50)]
        public string? ForegroundColor { get; set; }
        public int ForegroundPosition { get; set; }
        public int BackgroundPosition { get; set; }

        public long MaxNumberOfCards { get; set; }
        public int CardsInGallery { get; set; }

        public virtual ICollection<Card> Cards { get; set; }
        public virtual ICollection<Item> Items { get; set; }
        public virtual ICollection<BoosterPack> BoosterPacks { get; set; }
        public virtual ICollection<CardPvPStats> PvPStats { get; set; }
        public virtual ICollection<WishlistObject> Wishes { get; set; }

        public virtual ExpContainer ExpContainer { get; set; }

        public virtual ICollection<Figure> Figures { get; set; }

        public ulong UserId { get; set; }
        [JsonIgnore]
        public virtual User User { get; set; }
    }
}
