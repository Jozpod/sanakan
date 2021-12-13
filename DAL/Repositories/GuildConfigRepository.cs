﻿using Microsoft.EntityFrameworkCore;
using Sanakan.Common;
using Sanakan.Common.Cache;
using Sanakan.DAL.Models.Configuration;
using Sanakan.DAL.Repositories.Abstractions;
using System.Threading.Tasks;

namespace Sanakan.DAL.Repositories
{
    internal class GuildConfigRepository : BaseRepository<GuildOptions>, IGuildConfigRepository
    {
        private readonly SanakanDbContext _dbContext;
        private readonly ICacheManager _cacheManager;

        public GuildConfigRepository(
            SanakanDbContext dbContext,
            ICacheManager cacheManager) : base(dbContext)
        {
            _dbContext = dbContext;
            _cacheManager = cacheManager;
        }

        public async Task<GuildOptions?> GetCachedGuildFullConfigAsync(ulong guildId)
        {
            var key = CacheKeys.GuildConfig(guildId);

            var cacheResult = _cacheManager.Get<GuildOptions>(key);

            if (cacheResult != null)
            {
                return cacheResult.Value;
            }

            var result = await _dbContext
                .Guilds
                .AsQueryable()
                .Include(x => x.IgnoredChannels)
                .Include(x => x.ChannelsWithoutExperience)
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

        public async Task<GuildOptions?> GetGuildConfigOrCreateAsync(ulong guildId)
        {
            var config = await _dbContext.Guilds
                .AsQueryable()
                .Include(x => x.IgnoredChannels)
                .Include(x => x.ChannelsWithoutExperience)
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
                config = new GuildOptions(guildId, Constants.SafariLimit);
                _dbContext.Guilds.Add(config);
                await _dbContext.SaveChangesAsync();
            }

            return config;
        }
    }
}
