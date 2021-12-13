using Sanakan.Common;
using Sanakan.Common.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;

namespace Sanakan.DAL.Models
{
    public class User
    {
        [JsonConstructor]
        public User()
        {
            TimeStatuses = new Collection<TimeStatus>();
        }

        public User(ulong discordUserId, DateTime datetime)
        {
            Id = discordUserId;
            Level = 1;
            AcCount = 0;
            TcCount = 0;
            ScCount = Constants.ScCount;
            ExperienceCount = 10;
            WarningsCount = 0;
            MessagesCount = 0;
            CommandsCount = 0;
            MessagesCountAtDate = 0;
            IsBlacklisted = false;
            CharacterCountFromDate = 0;
            ShowWaifuInProfile = false;
            ProfileType = ProfileType.Statistics;
            TimeStatuses = new List<TimeStatus>();
            BackgroundProfileUri = Paths.DefaultBackgroundPicture;
            MeasuredOn = datetime;
            GameDeck = new GameDeck
            {
                Id = discordUserId,
                FavouriteWaifuId = 0,
                CTCount = 0,
                Karma = 0,
                PVPCoins = 0,
                DeckPower = 0,
                PVPWinStreak = 0,
                ItemsDropped = 0,
                GlobalPVPRank = 0,
                SeasonalPVPRank = 0,
                CardsInGalleryCount = Constants.CardsInGallery,
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
                ExperienceContainer = new ExperienceContainer
                {
                    Id = discordUserId,
                    ExperienceCount = 0,
                    Level = ExperienceContainerLevel.Disabled,
                },
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
                UpgradedCardsCount = 0,
                YamiUpgrades = 0,
                YatoUpgrades = 0,
                RaitoUpgrades = 0,
                ReleasedCards = 0,
                TournamentsWon = 0,
                UpgradedToSSS = 0,
                UnleashedCardsCount = 0,
                SacrificedCardsCount = 0,
                DestroyedCardsCount = 0,
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

        /// <summary>
        /// The amount of Ac coins.
        /// </summary>
        public long AcCount { get; set; }

        /// <summary>
        /// The amount of Tc coins.
        /// </summary>
        public long TcCount { get; set; }

        /// <summary>
        /// The amount of Sc coins.
        /// </summary>
        public long ScCount { get; set; }

        /// <summary>
        /// The level of user.
        /// </summary>
        public ulong Level { get; set; }

        /// <summary>
        /// The amount of experience accumulated by user.
        /// </summary>
        public ulong ExperienceCount { get; set; }

        /// <summary>
        /// Describes the type of profile.
        /// </summary>
        public ProfileType ProfileType { get; set; }

        [StringLength(50)]
        public string BackgroundProfileUri { get; set; } = string.Empty;

        [StringLength(50)]
        public string? StatsReplacementProfileUri { get; set; }

        public ulong MessagesCount { get; set; }

        public ulong CommandsCount { get; set; }

        /// <summary>
        /// The datetime when experience was measured.
        /// </summary>
        public DateTime MeasuredOn { get; set; }
        public ulong MessagesCountAtDate { get; set; }
        public ulong CharacterCountFromDate { get; set; }
        public bool ShowWaifuInProfile { get; set; }

        /// <summary>
        /// The number of warnings issued for this user in Discord.
        /// </summary>
        public long WarningsCount { get; set; }

        public virtual UserStats Stats { get; set; } = null!;

        public virtual GameDeck GameDeck { get; set; } = null!;

        public virtual SlotMachineConfig SMConfig { get; set; } = null!;

        public virtual ICollection<TimeStatus> TimeStatuses { get; set; }

        public void StoreExpIfPossible(double experience)
        {
            var maxToTransfer = GameDeck.ExperienceContainer.Level.GetMaxExpTransferToChest();
            if (maxToTransfer != -1)
            {
                experience = Math.Floor(experience);
                var diff = maxToTransfer - GameDeck.ExperienceContainer.ExperienceCount;
                if (diff <= experience)
                {
                    experience = Math.Floor(diff);
                }
                if (experience < 0)
                {
                    experience = 0;
                }
            }
            GameDeck.ExperienceContainer.ExperienceCount += experience;
        }
        public List<TimeStatus> CreateOrGetAllWeeklyQuests()
        {
            var quests = new List<TimeStatus>();
            foreach (var type in StatusTypeExtensions.WeeklyQuestTypes)
            {
                var mission = TimeStatuses.FirstOrDefault(x => x.Type == type);
                if (mission == null)
                {
                    mission = new TimeStatus(type);
                    TimeStatuses.Add(mission);
                }
                quests.Add(mission);
            }
            return quests;
        }

        public List<TimeStatus> CreateOrGetAllDailyQuests()
        {
            var quests = new List<TimeStatus>();
            foreach (var type in StatusTypeExtensions.DailyQuestTypes)
            {
                var mission = TimeStatuses.FirstOrDefault(x => x.Type == type);
                if (mission == null)
                {
                    mission = new TimeStatus(type);
                    TimeStatuses.Add(mission);
                }
                quests.Add(mission);
            }
            return quests;
        }

        public bool IsCharCounterActive(DateTime date)
            => date.Month == MeasuredOn.Month
           && date.Year == MeasuredOn.Year;

        public bool IsPVPSeasonalRankActive(DateTime date)
            => GameDeck.IsPVPSeasonalRankActive(date);

        public bool SendAnyMsgInMonth()
            => (MessagesCount - MessagesCountAtDate) > 0;

        public ulong GetRemainingExp(ulong nextLevelExperience)
        {
            var experienceLeft = nextLevelExperience - ExperienceCount;

            if (experienceLeft < 1)
            {
                experienceLeft = 1;
            }

            return experienceLeft;
        }

    }
}