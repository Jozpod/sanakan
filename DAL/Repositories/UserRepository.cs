﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Sanakan.Common;
using Sanakan.Common.Cache;
using Sanakan.DAL.Models;
using Sanakan.DAL.Repositories.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sanakan.DAL.Repositories
{
    internal class UserRepository : BaseRepository<User>, IUserRepository
    {
        private readonly SanakanDbContext _dbContext;
        private readonly ISystemClock _systemClock;
        private readonly ICacheManager _cacheManager;

        public UserRepository(
            SanakanDbContext dbContext,
            ISystemClock systemClock,
            ICacheManager cacheManager)
            : base(dbContext)
        {
            _dbContext = dbContext;
            _systemClock = systemClock;
            _cacheManager = cacheManager;
        }

        public Task<List<ulong>> GetShindenUserIdsByHavingCharacterAsync(ulong characterId)
        {
            var result = _dbContext.Cards
               .Include(x => x.GameDeck)
                   .ThenInclude(x => x.User)
               .Where(x => x.CharacterId == characterId
                   && x.GameDeck.User.ShindenId.HasValue)
               .AsNoTracking()
               .Select(x => x.GameDeck.User.ShindenId!.Value)
               .Distinct()
               .ToListAsync();

            return result;
        }

        public async Task<User?> GetWithWaifuProfileAsync(ulong shindenUserId)
        {
            var result = await _dbSet
               .Include(x => x.GameDeck)
                   .ThenInclude(x => x.Cards)
                   .ThenInclude(x => x.ArenaStats)
               .Include(x => x.GameDeck)
                   .ThenInclude(x => x.Cards)
                   .ThenInclude(x => x.Tags)
                .Where(x => x.ShindenId == shindenUserId)
               .AsNoTracking()
               .AsQueryable()
               .AsSplitQuery()
               .FirstOrDefaultAsync();

            return result;
        }

        public Task<User?> GetBasicAsync(ulong discordUserId)
        {
            return _dbSet
                .AsNoTracking()
                .AsSplitQuery()
                .FirstOrDefaultAsync(x => x.Id == discordUserId)!;
        }

        public async Task<User?> GetCachedAsync(ulong discordUserId)
        {
            var key = CacheKeys.User(discordUserId);

            var cacheResult = _cacheManager.Get<User>(key);

            if (cacheResult != null)
            {
                return cacheResult.Value;
            }

            var result = await Get().FirstOrDefaultAsync(x => x.Id == discordUserId);

            _cacheManager.Add(key, result);

            return result;
        }

        public async Task<User?> GetCachedByShindenIdAsync(ulong shindenUserId)
        {
            var key = CacheKeys.User(shindenUserId);

            var cacheResult = _cacheManager.Get<User>(key);

            if (cacheResult != null)
            {
                return cacheResult.Value;
            }

            var result = await Get().FirstOrDefaultAsync(x => x.ShindenId == shindenUserId);

            _cacheManager.Add(key, result);

            return result;
        }

        public async Task<List<User>> GetCachedAllUsersLiteAsync()
        {
            var cacheResult = _cacheManager.Get<List<User>>(CacheKeys.UsersLite);

            if (cacheResult != null)
            {
                return cacheResult.Value ?? new();
            }

            var result = await _dbSet
                .AsNoTracking()
                .AsSplitQuery()
                .ToListAsync();

            _cacheManager.Add(CacheKeys.UsersLite, result, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = Durations.Hour
            });

            return result;
        }

        public Task<User?> GetByShindenIdAsync(ulong shindenUserId)
        {
            var user = _dbSet.FirstOrDefaultAsync(x => x.ShindenId == shindenUserId);

            return user!;
        }

        public async Task<User?> GetUserOrCreateAsync(ulong discordUserId)
        {
            var user = await Get().FirstOrDefaultAsync(x => x.Id == discordUserId);

            if (user == null)
            {
                user = new User(discordUserId, _systemClock.StartOfMonth);
                _dbContext.Users.Add(user);
            }

            return user;
        }

        public async Task<IEnumerable<User>> GetAllCachedAsync()
        {
            var cacheResult = _cacheManager.Get<List<User>>(CacheKeys.Users);

            if (cacheResult != null)
            {
                return cacheResult.Value ?? new List<User>();
            }

            var result = await Get().ToListAsync();

            _cacheManager.Add(CacheKeys.Users, result, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(6)
            });

            return result;
        }

        public Task<bool> ExistsByDiscordIdAsync(ulong discordUserId)
            => _dbContext.Users.AnyAsync(x => x.Id == discordUserId);

        public Task<User?> GetByDiscordIdAsync(ulong discordUserId)
            => _dbContext.Users.FirstOrDefaultAsync(x => x.Id == discordUserId)!;

        public Task<User?> GetByShindenIdAsync(ulong userShindenId, UserQueryOptions userQueryOptions)
        {
            var query = _dbContext
                .Users
                .AsQueryable()
                .AsSplitQuery()
                .Where(x => x.ShindenId == userShindenId);

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
                .FirstOrDefaultAsync()!;
        }

        public Task<bool> ExistsByShindenIdAsync(ulong userShindenId)
            => _dbContext.Users.AnyAsync(x => x.ShindenId == userShindenId);

        public Task<User?> GetUserAndDontTrackAsync(ulong discordUserId)
        {
            var result = Get().FirstOrDefaultAsync(x => x.Id == discordUserId);

            return result!;
        }

        public Task<List<User>> GetByShindenIdExcludeDiscordIdAsync(ulong shindenUserId, ulong discordUserId)
        {
            return _dbContext.Users
                .Where(x => x.ShindenId == shindenUserId
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

        private IQueryable<User> Get()
        {
            return _dbSet
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
                    .ThenInclude(x => x.ExperienceContainer)
                .Include(x => x.GameDeck)
                    .ThenInclude(x => x.BoosterPacks)
                    .ThenInclude(x => x.Characters)
                .Include(x => x.GameDeck)
                    .ThenInclude(x => x.BoosterPacks)
                    .ThenInclude(x => x.RarityExcludedFromPack)
                .Include(x => x.GameDeck)
                    .ThenInclude(x => x.Cards)
                    .ThenInclude(x => x.Tags)
                .Include(x => x.GameDeck)
                    .ThenInclude(x => x.Figures)
                .AsNoTracking()
                .AsSplitQuery();
        }
    }
}
