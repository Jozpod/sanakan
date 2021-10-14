using DAL.Repositories.Abstractions;
using Microsoft.EntityFrameworkCore;
using Sanakan.Common;
using Sanakan.DAL;
using Sanakan.DAL.Models;
using Sanakan.DAL.Models.Configuration;
using Sanakan.DAL.Models.Management;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    internal class Repository : IRepository
    {
        private readonly BuildDatabaseContext _dbContext;
        private readonly ICacheManager _cacheManager;

        public Repository(
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

            var result = (await _dbContext
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
                .FromCacheAsync(new string[] {  }))
                .FirstOrDefault(x => x.Id == guildId);
        }

        public async Task<IEnumerable<PenaltyInfo>> GetCachedFullPenalties()
        {
            var result = (await _dbContext
                .Penalties
                .AsQueryable()
                .Include(x => x.Roles)
                .AsNoTracking()
                .AsSplitQuery()
                .FromCacheAsync(new string[] { $"mute" }))
                .ToList();
        }

        public async Task<GameDeck> GetCachedUserGameDeckAsync(ulong userId)
        {
            var result = (await _dbContext
                .GameDecks
                .AsQueryable()
                .Where(x => x.UserId == userId)
                .Include(x => x.Cards)
                .AsNoTracking()
                .AsSplitQuery()
                .FromCacheAsync(new string[] { $"user-{userId}", "users" }))
                .FirstOrDefault();
        }

        public async Task<List<User>> GetCachedAllUsersAsync()
        {
            var result = (await _dbContext.Users
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
                .FromCacheAsync(new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(6) })).ToList();
        }

        public async Task<List<GameDeck>> GetCachedPlayersForPVP(ulong ignore = 1)
        {
            var result = await _dbContext
                .GameDecks
                .AsQueryable()
                .Where(x => x.DeckPower > UserExtension.MIN_DECK_POWER
                    && x.DeckPower < UserExtension.MAX_DECK_POWER
                    && x.UserId != ignore)
                .AsNoTracking()
                .AsSplitQuery()
                .FromCacheAsync(new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2) })
                .ToListAsync();
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
                user = user.Default(userId);
                await _dbContext.Users.AddAsync(user);
            }

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
        }

        public async Task<List<Question>> GetCachedAllQuestionsAsync()
        {
            var result = (await _dbContext
                .Questions
                .AsQueryable()
                .Include(x => x.Answers)
                .AsNoTracking()
                .AsSplitQuery()
                .FromCacheAsync(new string[] { $"quiz" }))
                .ToList();
        }

        public async Task<Question> GetCachedQuestionAsync(ulong id)
        {
            var result = (await _dbContext.Questions
                .AsQueryable()
                .Include(x => x.Answers)
                .AsNoTracking()
                .AsSplitQuery()
                .FromCacheAsync(new string[] { $"quiz" }))
                .FirstOrDefault(x => x.Id == id);

            return result;
        }

    }
}
