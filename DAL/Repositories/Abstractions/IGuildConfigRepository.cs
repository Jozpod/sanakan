﻿using Sanakan.DAL.Models.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sanakan.DAL.Repositories.Abstractions
{
    public interface IGuildConfigRepository : 
        ICreateRepository<GuildOptions>,
        IRemoveRepository<GuildOptions>,
        ISaveRepository
    {
        Task<GuildOptions> GetGuildConfigOrCreateAsync(ulong guildId);
        Task<GuildOptions> GetCachedGuildFullConfigAsync(ulong guildId);
    }
}