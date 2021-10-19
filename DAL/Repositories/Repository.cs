using DAL.Repositories.Abstractions;
using Microsoft.EntityFrameworkCore;
using Sanakan.Common;
using Sanakan.Common.Models;
using Sanakan.DAL;
using Sanakan.DAL.Models;
using Sanakan.DAL.Models.Analytics;
using Sanakan.DAL.Models.Configuration;
using Sanakan.DAL.Models.Management;
using Sanakan.DAL.Repositories.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sanakan.DAL.Repositories
{
    public class AllRepository : IAllRepository
    {
        private readonly BuildDatabaseContext _dbContext;
        private readonly ICacheManager _cacheManager;

        public AllRepository(
            BuildDatabaseContext dbContext,
            ICacheManager cacheManager)
        {
            _dbContext = dbContext;
            _cacheManager = cacheManager;
        }

        public async Task<GuildOptions> GetGuildConfigOrCreateAsync(ulong guildId)
        {
            var config = await _dbContext.Guilds
                .AsQueryable()
                .Include(x => x.IgnoredChannels)
                .Include(x => x.ChannelsWithoutExp)
                .Include(x => x.ChannelsWithoutSupervision)
                .Include(x => x.CommandChannels)
                .Include(x => x.SelfRoles)
                .Include(x => x.Lands)
                .Include(x => x.ModeratorRoles)
                .Include(x => x.RolesPerLevel)
                .Include(x => x.WaifuConfig)
                    .ThenInclude(x => x.CommandChannels)
                .Include(x => x.Raports)
                .Include(x => x.WaifuConfig)
                    .ThenInclude(x => x.FightChannels)
                .AsSplitQuery()
                .FirstOrDefaultAsync(x => x.Id == guildId);

            if (config == null)
            {
                config = new GuildOptions
                {
                    Id = guildId,
                    SafariLimit = 50
                };

                await _dbContext.Guilds.AddAsync(config);
            }

            return config;
        }

        public async Task<GuildOptions> GetCachedGuildFullConfigAsync(ulong guildId)
        {
            var key = $"config-{guildId}";

            var cached = _cacheManager.Get<GuildOptions>(key);

            if(cached != null)
            {
                return cached;
            }

            var result = await _dbContext
                .Guilds
                .AsQueryable()
                .Include(x => x.IgnoredChannels)
                .Include(x => x.ChannelsWithoutExp)
                .Include(x => x.ChannelsWithoutSupervision)
                .Include(x => x.CommandChannels)
                .Include(x => x.SelfRoles)
                .Include(x => x.Lands)
                .Include(x => x.ModeratorRoles)
                .Include(x => x.RolesPerLevel)
                .Include(x => x.WaifuConfig)
                    .ThenInclude(x => x.CommandChannels)
                    .Include(x => x.Raports)
                .Include(x => x.WaifuConfig)
                    .ThenInclude(x => x.FightChannels)
                .AsNoTracking()
                .AsSplitQuery()
                .FirstOrDefaultAsync(x => x.Id == guildId);

            return result;
        }

        public async Task<IEnumerable<PenaltyInfo>> GetCachedFullPenalties()
        {
            var key = $"mute";

            var cached = _cacheManager.Get<IEnumerable<PenaltyInfo>>(key);

            if (cached != null)
            {
                return cached;
            }

            var result = await _dbContext
                .Penalties
                .AsQueryable()
                .Include(x => x.Roles)
                .AsNoTracking()
                .AsSplitQuery()
                .ToListAsync();

            _cacheManager.Add(key, result);

            return result;
        }

        public async Task<GameDeck> GetCachedUserGameDeckAsync(ulong userId)
        {
            var key = $"user-{userId}";

            var cached = _cacheManager.Get<GameDeck>(key);

            if (cached != null)
            {
                return cached;
            }

            var result = await _dbContext
                .GameDecks
                .AsQueryable()
                .Where(x => x.UserId == userId)
                .Include(x => x.Cards)
                .AsNoTracking()
                .AsSplitQuery()
                //.FromCacheAsync(new string[] { $"user-{userId}", "users" }))
                .FirstOrDefaultAsync();

            return result;
        }

        public async Task<List<User>> GetCachedAllUsersAsync()
        {
            var result = await _dbContext.Users
                .AsQueryable()
                .Include(x => x.Stats)
                .Include(x => x.SMConfig)
                .Include(x => x.TimeStatuses)
                .Include(x => x.GameDeck)
                    .ThenInclude(x => x.PvPStats)
                .Include(x => x.GameDeck)
                    .ThenInclude(x => x.Wishes)
                .Include(x => x.GameDeck)
                    .ThenInclude(x => x.Items)
                .Include(x => x.GameDeck)
                    .ThenInclude(x => x.ExpContainer)
                .Include(x => x.GameDeck)
                    .ThenInclude(x => x.Cards)
                    .ThenInclude(x => x.ArenaStats)
                .Include(x => x.GameDeck)
                    .ThenInclude(x => x.BoosterPacks)
                    .ThenInclude(x => x.Characters)
                .Include(x => x.GameDeck)
                    .ThenInclude(x => x.BoosterPacks)
                    .ThenInclude(x => x.RarityExcludedFromPack)
                .Include(x => x.GameDeck)
                    .ThenInclude(x => x.Cards)
                    .ThenInclude(x => x.TagList)
                .Include(x => x.GameDeck)
                    .ThenInclude(x => x.Figures)
                .AsNoTracking()
                .AsSplitQuery()
                //.FromCacheAsync(new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(6) }))
                .ToListAsync();

            return result;
        }

        public const double MAX_DECK_POWER = 800;
        public const double MIN_DECK_POWER = 200;

        public async Task<List<GameDeck>> GetCachedPlayersForPVP(ulong ignore = 1)
        {
            var result = await _dbContext
                .GameDecks
                .AsQueryable()
                .Where(x => x.DeckPower > MIN_DECK_POWER
                    && x.DeckPower < MAX_DECK_POWER
                    && x.UserId != ignore)
                .AsNoTracking()
                .AsSplitQuery()
                //.FromCacheAsync(new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2) })
                .ToListAsync();

            return result;
        }

        public async Task<User> GetUserOrCreateAsync(ulong userId)
        {
            var user = await _dbContext.Users
                .AsQueryable()
                .Where(x => x.Id == userId)
                .Include(x => x.Stats)
                .Include(x => x.SMConfig)
                .Include(x => x.TimeStatuses)
                .Include(x => x.GameDeck)
                    .ThenInclude(x => x.PvPStats)
                .Include(x => x.GameDeck)
                    .ThenInclude(x => x.Wishes)
                .Include(x => x.GameDeck)
                    .ThenInclude(x => x.Items)
                .Include(x => x.GameDeck)
                    .ThenInclude(x => x.Cards)
                    .ThenInclude(x => x.ArenaStats)
                .Include(x => x.GameDeck)
                    .ThenInclude(x => x.ExpContainer)
                .Include(x => x.GameDeck)
                    .ThenInclude(x => x.BoosterPacks)
                .ThenInclude(x => x.Characters)
                    .Include(x => x.GameDeck)
                    .ThenInclude(x => x.BoosterPacks)
                    .ThenInclude(x => x.RarityExcludedFromPack)
                .Include(x => x.GameDeck)
                    .ThenInclude(x => x.Cards)
                    .ThenInclude(x => x.TagList)
                .Include(x => x.GameDeck)
                    .ThenInclude(x => x.Figures)
                .AsSplitQuery()
                .FirstOrDefaultAsync();

            if (user == null)
            {
                user = CreateUser(userId);
                await _dbContext.Users.AddAsync(user);
            }

            return user;
        }

        public static User CreateUser(ulong id)
        {
            var user = new User
            {
                Id = id,
                Level = 1,
                AcCnt = 0,
                TcCnt = 0,
                ScCnt = 100,
                ExpCnt = 10,
                Shinden = 0,
                Warnings = 0,
                MessagesCnt = 0,
                CommandsCnt = 0,
                MessagesCntAtDate = 0,
                IsBlacklisted = false,
                CharacterCntFromDate = 0,
                ShowWaifuInProfile = false,
                ProfileType = ProfileType.Stats,
                StatsReplacementProfileUri = "none",
                TimeStatuses = new List<TimeStatus>(),
                BackgroundProfileUri = $"./Pictures/defBg.png",
                MeasureDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1),
                GameDeck = new GameDeck
                {
                    Id = id,
                    Waifu = 0,
                    CTCnt = 0,
                    Karma = 0,
                    PVPCoins = 0,
                    DeckPower = 0,
                    PVPWinStreak = 0,
                    ItemsDropped = 0,
                    GlobalPVPRank = 0,
                    SeasonalPVPRank = 0,
                    CardsInGallery = 10,
                    MatachMakingRatio = 0,
                    ForegroundColor = null,
                    ForegroundPosition = 0,
                    BackgroundPosition = 35,
                    PVPDailyGamesPlayed = 0,
                    MaxNumberOfCards = 1000,
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
                    PVPSeasonBeginDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1),
                    ExpContainer = new ExpContainer
                    {
                        Id = id,
                        ExpCount = 0,
                        Level = ExpContainerLevel.Disabled
                    }
                },
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
                },
                SMConfig = new SlotMachineConfig
                {
                    PsayMode = 0,
                    Beat = SlotMachineBeat.b1,
                    Rows = SlotMachineSelectedRows.r1,
                    Multiplier = SlotMachineBeatMultiplier.x1,
                }
            };

            user.GameDeck.BoosterPacks.Add(new BoosterPack
            {
                CardCnt = 5,
                MinRarity = Rarity.A,
                Name = "Startowy pakiet",
                IsCardFromPackTradable = true
            });

            return user;
        }

        public async Task<User> GetBaseUserAndDontTrackAsync(ulong userId)
        {
            return await _dbContext.Users
                .AsQueryable()
                .AsNoTracking()
                .AsSplitQuery()
                .FirstOrDefaultAsync(x => x.Id == userId);
        }

        public async Task<User> GetUserAndDontTrackAsync(ulong userId)
        {
            var result = await _dbContext
                .Users
                .AsQueryable()
                .Include(x => x.Stats)
                .Include(x => x.SMConfig)
                .Include(x => x.TimeStatuses)
                .Include(x => x.GameDeck)
                    .ThenInclude(x => x.PvPStats)
                .Include(x => x.GameDeck)
                    .ThenInclude(x => x.Wishes)
                .Include(x => x.GameDeck)
                    .ThenInclude(x => x.Items)
                .Include(x => x.GameDeck)
                    .ThenInclude(x => x.Cards)
                    .ThenInclude(x => x.ArenaStats)
                .Include(x => x.GameDeck)
                    .ThenInclude(x => x.ExpContainer)
                .Include(x => x.GameDeck)
                    .ThenInclude(x => x.BoosterPacks)
                    .ThenInclude(x => x.Characters)
                .Include(x => x.GameDeck)
                    .ThenInclude(x => x.BoosterPacks)
                    .ThenInclude(x => x.RarityExcludedFromPack)
                .Include(x => x.GameDeck)
                    .ThenInclude(x => x.Cards)
                    .ThenInclude(x => x.TagList)
                .Include(x => x.GameDeck)
                    .ThenInclude(x => x.Figures)
                .AsNoTracking()
                .AsSplitQuery()
                .FirstOrDefaultAsync(x => x.Id == userId);

            return result;
        }

        public async Task<List<Question>> GetCachedAllQuestionsAsync()
        {
            var key = $"quiz";

            var cached = _cacheManager.Get<List<Question>>(key);

            if (cached != null)
            {
                return cached;
            }

            var result = await _dbContext
                .Questions
                .AsQueryable()
                .Include(x => x.Answers)
                .AsNoTracking()
                .AsSplitQuery()
                .ToListAsync();

            _cacheManager.Add(key, result);

            return result;
        }

        public async Task<Question> GetCachedQuestionAsync(ulong id)
        {
            var key = $"quiz-{id}";

            var cached = _cacheManager.Get<Question>(key);

            if (cached != null)
            {
                return cached;
            }

            var result = await _dbContext
                .Questions
                .AsQueryable()
                .Include(x => x.Answers)
                .AsNoTracking()
                .AsSplitQuery()
                .FirstOrDefaultAsync(x => x.Id == id);

            _cacheManager.Add(key, result);

            return result;
        }

        public async Task<Card> GetCardAsync(ulong wid)
        {
            var result = await _dbContext
                .Cards
                .Include(x => x.GameDeck)
                .Include(x => x.ArenaStats)
                .Include(x => x.TagList)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == wid);

            return result;
        }

        public async Task<IEnumerable<ulong>> GetUserShindenIdsByHavingCharacterAsync(ulong id)
        {
            var result = await _dbContext.Cards
               .Include(x => x.GameDeck)
                   .ThenInclude(x => x.User)
               .Where(x => x.Character == id
                   && x.GameDeck.User.Shinden != 0)
               .AsNoTracking()
               .Select(x => x.GameDeck.User.Shinden)
               .Distinct()
               .ToListAsync();

            return result;
        }

        public async Task AddQuestionAsync(Question question)
        {
            _dbContext.Questions.Add(question);
            await _dbContext.SaveChangesAsync();
        }

        public async Task RemoveQuestionAsync(Question question)
        {
            _dbContext.Questions.Remove(question);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<Question> GetQuestionAsync(ulong id)
        {
            var result = await _dbContext
                .Questions
                .Include(x => x.Answer)
                .FirstOrDefaultAsync(x => x.Id == id);

            return result;
        }

        public async Task<User?> GetUserCardsAsync(ulong id)
        {
            var result = await _dbContext
                .Users
               .AsQueryable()
               .Where(x => x.Shinden == id)
               .Include(x => x.GameDeck)
                   .ThenInclude(x => x.Cards)
                   .ThenInclude(x => x.ArenaStats)
               .Include(x => x.GameDeck)
                  .ThenInclude(x => x.Cards)
                  .ThenInclude(x => x.TagList)
               .AsNoTracking()
               .AsSplitQuery()
               .FirstOrDefaultAsync();

            return result;
        }

        public async Task AddSystemAnalyticsAsync(SystemAnalytics record)
        {
            _dbContext.SystemData.Add(record);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<User> GetUsersCardsByShindenIdWithOffsetAndFilterAsync1(ulong id)
        {
            var result = await _dbContext.Users.AsQueryable()
                .Where(x => x.Shinden == id)
                .Include(x => x.GameDeck)
                .AsNoTracking()
                .AsSplitQuery()
                .FirstOrDefaultAsync();

            return result;
        }

        public async Task<IEnumerable<Card>> GetUsersCardsByShindenIdWithOffsetAndFilterAsync2(ulong id)
        {
            var result = await _dbContext.Cards
               .AsQueryable()
               .AsSplitQuery()
               .Where(x => x.GameDeckId == id)
               .Include(x => x.ArenaStats)
               .Include(x => x.TagList)
               .AsNoTracking()
               .ToListAsync();

            return result;
        }

        public async Task<List<Card>> GetCardsWithTagAsync(string tag)
        {
            var result = await _dbContext
                .Cards
               .Include(x => x.ArenaStats)
               .Include(x => x.TagList)
               .Where(x => x.TagList
                   .Any(c => c.Name.Equals(tag, StringComparison.CurrentCultureIgnoreCase)))
               .AsNoTracking()
               .ToListAsync();

            return result;
        }

        public async Task<List<Card>> GetCardsByCharacterIdAsync(ulong id)
        {
            var cards = await _dbContext
                .Cards
                .AsQueryable()
                .AsSplitQuery()
                .Where(x => x.Character == id)
                .ToListAsync();

            return cards;
        }

        public async Task SaveChangesAsync()
        {
            await _dbContext.SaveChangesAsync();
        }

        public async Task<User?> GetUserWaifuProfileAsync(ulong id)
        {
            var result = await _dbContext.Users
               .AsQueryable()
               .AsSplitQuery()
               .Where(x => x.Shinden == id)
               .Include(x => x.GameDeck)
                   .ThenInclude(x => x.Cards)
                   .ThenInclude(x => x.ArenaStats)
               .Include(x => x.GameDeck)
                   .ThenInclude(x => x.Cards)
                   .ThenInclude(x => x.TagList)
               .AsNoTracking()
               .FirstOrDefaultAsync();

            return result;
        }

        public async Task AddTransferAnalyticsAsync(TransferAnalytics record)
        {
            _dbContext.TransferData.Add(record);
            await dbc.SaveChangesAsync();
        }
    }
}
