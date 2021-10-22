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
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        private readonly BuildDatabaseContext _dbContext;
        private readonly ISystemClock _systemClock;
        private readonly ICacheManager _cacheManager;

        public UserRepository(
            BuildDatabaseContext dbContext,
            ISystemClock systemClock,
            ICacheManager cacheManager) : base(dbContext)
        {
            _dbContext = dbContext;
            _systemClock = systemClock;
            _cacheManager = cacheManager;
        }
        public Task<User?> GetBaseUserAndDontTrackAsync(ulong userId)
        {
            return _dbContext.Users
                .AsQueryable()
                .AsNoTracking()
                .AsSplitQuery()
                .FirstOrDefaultAsync(x => x.Id == userId);
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
                    user = new User(userId, _systemClock.StartOfMonth);
                    await _dbContext.Users.AddAsync(user);
                }

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

        public Task<User> GetUserAndDontTrackAsync(ulong userId)
        {
            var result = _dbContext
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

        public Task<List<User>> GetByShindenIdExcludeDiscordIdAsync(ulong shindenUserId, ulong discordUserId)
        {
            return _dbContext.Users
                .Where(x => x.Shinden == shindenUserId
                    && x.Id != discordUserId)
                .ToListAsync();
        }

        public Task<List<ulong>> GetByExcludedDiscordIdsAsync(IEnumerable<ulong> discordUserIds)
        {
            return _dbContext.Users
                .AsSplitQuery()
                .Where(x => !discordUserIds.Any(id => id == x.Id))
                .Select(x => x.Id)
                .ToListAsync();

        }
    }
}
