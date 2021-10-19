using Sanakan.DAL.Models.Configuration;
using Sanakan.DAL.Repositories.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sanakan.DAL.Repositories
{
    public class GuildConfigRepository : IGuildConfigRepository
    {
        public Task<GuildOptions> GetCachedGuildFullConfigAsync(ulong guildId)
        {
            throw new NotImplementedException();
        }

        public Task<GuildOptions> GetGuildConfigOrCreateAsync(ulong guildId)
        {
            throw new NotImplementedException();
        }
    }
}
