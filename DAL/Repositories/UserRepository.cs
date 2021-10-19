using DAL.Repositories.Abstractions;
using Microsoft.EntityFrameworkCore;
using Sanakan.Common;
using Sanakan.Common.Models;
using Sanakan.DAL;
using Sanakan.DAL.Models;
using Sanakan.DAL.Repositories.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sanakan.DAL.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly BuildDatabaseContext _dbContext;
        private readonly ICacheManager _cacheManager;

        public UserRepository(
            BuildDatabaseContext dbContext,
            ICacheManager cacheManager)
        {
            _dbContext = dbContext;
            _cacheManager = cacheManager;
        }

        public async Task<User> GetCachedFullUserAsync(ulong userId)
        {
            var key = $"user-{userId}";

            var cached = _cacheManager.Get<User>(key);

            if (cached != null)
            {
                return cached;
            }

            var result = await _dbContext.Users
                .AsQueryable()
                .Where(x => x.Id == userId)
                .Include(x => x.Stats)
                .Include(x => x.SMConfig)
                .Include(x => x.TimeStatuses)
                .Include(x => x.GameDeck)
                    .ThenInclude(x => x.PvPStats)
                .Include(x => x.GameDeck)
                    .ThenInclude(x => x.Items)
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
                    .ThenInclude(x => x.ExpContainer)
                .Include(x => x.GameDeck)
                    .ThenInclude(x => x.Wishes)
                .Include(x => x.GameDeck)
                    .ThenInclude(x => x.Cards)
                    .ThenInclude(x => x.TagList)
                .Include(x => x.GameDeck)
                .ThenInclude(x => x.Figures)
                .AsNoTracking()
                .AsSplitQuery()
                .FirstOrDefaultAsync();

            _cacheManager.Add(key, result);

            return result;
        }

        public async Task<User> GetCachedFullUserByShindenIdAsync(ulong userId)
        {
            var key = $"user-{userId}";

            var cached = _cacheManager.Get<User>(key);

            if (cached == null)
            {
                return cached;
            }

            var result = await _dbContext.Users
                .AsQueryable()
                .Where(x => x.Shinden == userId)
                .Include(x => x.Stats)
                .Include(x => x.SMConfig)
                .Include(x => x.TimeStatuses)
                .Include(x => x.GameDeck)
                    .ThenInclude(x => x.PvPStats)
                .Include(x => x.GameDeck)
                    .ThenInclude(x => x.Items)
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
                    .ThenInclude(x => x.ExpContainer)
                .Include(x => x.GameDeck)
                    .ThenInclude(x => x.Wishes)
                .Include(x => x.GameDeck)
                    .ThenInclude(x => x.Cards)
                    .ThenInclude(x => x.TagList)
                .Include(x => x.GameDeck)
                    .ThenInclude(x => x.Figures)
                .AsNoTracking()
                .AsSplitQuery()
                .FirstOrDefaultAsync();

            _cacheManager.Add(key, result);

            return result;
        }

        public async Task<List<User>> GetCachedAllUsersLiteAsync()
        {
            var result = await _dbContext.Users
                .AsQueryable()
                .AsNoTracking()
                .AsSplitQuery()
                //.FromCacheAsync(new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1) }))
                .ToListAsync();

            return result;
        }

        public Task<User?> GetByShindenIdAsync(ulong userShindenId)
        {
            var user = _dbContext.Users
                .FirstOrDefaultAsync(x => x.Shinden == userShindenId);

            return user;
        }

        public async Task SaveChangesAsync()
        {
            await _dbContext.SaveChangesAsync();
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

        public User CreateUser(ulong id)
        {
            var utcNow = DateTime.UtcNow.Date;
            var firstDayOfMonth = utcNow.AddDays(1 - utcNow.Day);

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
                MeasureDate = firstDayOfMonth,
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
                    PVPSeasonBeginDate = firstDayOfMonth,
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

        public Task<List<User>> GetCachedAllUsersAsync()
        {
            throw new NotImplementedException();
        }

        public Task<bool> ExistsByDiscordIdAsync(ulong discordUserId)
            => _dbContext.Users.AnyAsync(x => x.Id == discordUserId);

        public Task<User?> GetByDiscordIdAsync(ulong discordUserId)
            => _dbContext.Users.FirstOrDefaultAsync(x => x.Id == discordUserId);

        public Task<User?> GetByShindenIdAsync(ulong userShindenId, UserQueryOptions userQueryOptions)
        {
            //_dbContext.Users.FirstOrDefaultAsync(x => x.Id == discordUserId);
            var query = _dbContext
                .Users
               .AsQueryable()
               .AsSplitQuery()
               .Where(x => x.Shinden == userShindenId);

            //await db.Users
            //   .AsQueryable()
            //   .AsSplitQuery()
            //   .Where(x => x.Shinden == id)
            //   .Include(x => x.GameDeck)
            //       .ThenInclude(x => x.Wishes)
            //   .AsNoTracking()
            //   .FirstOrDefaultAsync();

            //var bUser = await db.Users.AsQueryable()
            //.Where(x => x.Shinden == id)
            //.Include(x => x.GameDeck)
            //.ThenInclude(x => x.Cards)
            //.AsNoTracking()
            //.AsSplitQuery()
            //.FirstOrDefaultAsync();

            if (userQueryOptions.IncludeGameDeck)
            {
                var includeQuery = query.Include(x => x.GameDeck);

                if (userQueryOptions.IncludeCards)
                {
                    var thenIncludeQuery = includeQuery.ThenInclude(x => x.Cards);
                    query = thenIncludeQuery;
                }
                else
                {
                    query = includeQuery;
                }

                if (userQueryOptions.IncludeWishes)
                {
                    var thenIncludeQuery = includeQuery.ThenInclude(x => x.Wishes);
                    query = thenIncludeQuery;
                }
                else
                {
                    query = includeQuery;
                }
            }

            return query
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }

        public Task<bool> ExistsByShindenIdAsync(ulong userShindenId)
            => _dbContext.Users.AnyAsync(x => x.Shinden == userShindenId);
    }
}
