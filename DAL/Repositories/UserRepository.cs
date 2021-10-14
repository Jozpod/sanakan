﻿using DAL.Repositories.Abstractions;
using Microsoft.EntityFrameworkCore;
using Sanakan.Common;
using Sanakan.DAL;
using Sanakan.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    internal class UserRepository : IUserRepository
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

            _cacheManager.AddTag(new string[] { $"user-{userId}", "users" });
        }

        public async Task<User> GetCachedFullUserByShindenIdAsync(ulong userId)
        {
            var key = $"user-{userId}";

            var cached = _cacheManager.Get<User>(key);

            if (cached == null)
            {
                return cached;
            }

            var result = (await _dbContext.Users
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

            _cacheManager.AddTag(new string[] { key, "users" });
        }

        public async Task<List<User>> GetCachedAllUsersLiteAsync()
        {
            return (await _dbContext.Users
                .AsQueryable()
                .AsNoTracking()
                .AsSplitQuery()
                .FromCacheAsync(new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1) }))
                .ToList();
        }


    }
}
