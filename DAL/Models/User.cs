using Sanakan.Common;
using Sanakan.Common.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Sanakan.DAL.Models
{
    public class User
    {
        private User() { }

        public User(ulong discordUserId, DateTime datetime)
        {
            Id = discordUserId;
            Level = 1;
            AcCnt = 0;
            TcCnt = 0;
            ScCnt = 100;
            ExpCnt = 10;
            ShindenId = 0;
            Warnings = 0;
            MessagesCnt = 0;
            CommandsCnt = 0;
            MessagesCntAtDate = 0;
            IsBlacklisted = false;
            CharacterCntFromDate = 0;
            ShowWaifuInProfile = false;
            ProfileType = ProfileType.Stats;
            StatsReplacementProfileUri = "none";
            TimeStatuses = new List<TimeStatus>();
            BackgroundProfileUri = Paths.DefaultBackgroundPicture;
            MeasureDate = datetime;
            GameDeck = new GameDeck
            {
                Id = discordUserId,
                Waifu = 0,
                CTCnt = 0,
                Karma = 0,
                PVPCoins = 0,
                DeckPower = 0,
                PVPWinStreak = 0,
                ItemsDropped = 0,
                GlobalPVPRank = 0,
                SeasonalPVPRank = 0,
                CardsInGallery = Constants.CardsInGallery,
                MatchMakingRatio = 0,
                ForegroundColor = null,
                ForegroundPosition = 0,
                BackgroundPosition = Constants.BackgroundPosition,
                PVPDailyGamesPlayed = 0,
                MaxNumberOfCards = Constants.MaxNumberOfCards,
                Items = new List<Item>(),
                Cards = new List<Card>(),
                ExchangeConditions = null,
                BackgroundImageUrl = null,
                ForegroundImageUrl = null,
                WishlistIsPrivate = false,
                Figures = new List<Figure>(),
                Wishes = new List<WishlistObject>(),
                PvPStats = new List<CardPvPStats>(),
                BoosterPacks = new List<BoosterPack>(),
                PVPSeasonBeginDate = datetime,
                ExpContainer = new ExpContainer
                {
                    Id = discordUserId,
                    ExpCount = 0,
                    Level = ExpContainerLevel.Disabled
                }
            };
            Stats = new UserStats
            {
                Hit = 0,
                Head = 0,
                Misd = 0,
                Tail = 0,
                ScLost = 0,
                IncomeInSc = 0,
                RightAnswers = 0,
                TotalAnswers = 0,
                UpgaredCards = 0,
                YamiUpgrades = 0,
                YatoUpgrades = 0,
                RaitoUpgrades = 0,
                ReleasedCards = 0,
                TurnamentsWon = 0,
                UpgradedToSSS = 0,
                UnleashedCards = 0,
                SacraficeCards = 0,
                DestroyedCards = 0,
                WastedTcOnCards = 0,
                SlotMachineGames = 0,
                WastedTcOnCookies = 0,
                OpenedBoosterPacks = 0,
                WastedPuzzlesOnCards = 0,
                WastedPuzzlesOnCookies = 0,
                OpenedBoosterPacksActivity = 0,
            };
            SMConfig = new SlotMachineConfig
            {
                PsayMode = 0,
                Beat = SlotMachineBeat.b1,
                Rows = SlotMachineSelectedRows.r1,
                Multiplier = SlotMachineBeatMultiplier.x1,
            };

            GameDeck.BoosterPacks.Add(new BoosterPack
            {
                CardCount = 5,
                MinRarity = Rarity.A,
                Name = "Startowy pakiet",
                IsCardFromPackTradable = true
            });
        }

        /// <summary>
        /// The user identifer in Discord.
        /// </summary>
        public ulong Id { get; set; }

        /// <summary>
        /// The user identifer in Shinden.
        /// </summary>
        public ulong? ShindenId { get; set; }

        /// <summary>
        /// Specifies whether the user is blacklisted in Discord.
        /// </summary>
        public bool IsBlacklisted { get; set; }
        public long AcCnt { get; set; }
        public long TcCnt { get; set; }
        public long ScCnt { get; set; }
        public long Level { get; set; }
        public long ExpCnt { get; set; }
        public ProfileType ProfileType { get; set; }

        [StringLength(50)]
        public string BackgroundProfileUri { get; set; }

        [StringLength(50)]
        public string StatsReplacementProfileUri { get; set; }
        public ulong MessagesCnt { get; set; }
        public ulong CommandsCnt { get; set; }
        public DateTime MeasureDate { get; set; }
        public ulong MessagesCntAtDate { get; set; }
        public ulong CharacterCntFromDate { get; set; }
        public bool ShowWaifuInProfile { get; set; }
        public long Warnings { get; set; }

        public virtual UserStats Stats { get; set; }
        public virtual GameDeck GameDeck { get; set; }
        public virtual SlotMachineConfig SMConfig { get; set; }

        public virtual ICollection<TimeStatus> TimeStatuses { get; set; }
    }
}
