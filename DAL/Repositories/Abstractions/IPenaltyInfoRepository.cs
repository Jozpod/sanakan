﻿using Sanakan.DAL.Models.Management;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sanakan.DAL.Repositories.Abstractions
{
    public interface IPenaltyInfoRepository :
        ICreateRepository<PenaltyInfo>,
        IRemoveRepository<PenaltyInfo>,
        ISaveRepository
    {
        Task<IEnumerable<PenaltyInfo>> GetByGuildIdAsync(ulong id);

        Task<IEnumerable<PenaltyInfo>> GetMutedPenaltiesAsync(ulong discordGuildId);

        Task<PenaltyInfo?> GetPenaltyAsync(
            ulong discordUserId,
            ulong discordGuildId,
            PenaltyType penaltyType);

        Task<IEnumerable<PenaltyInfo>> GetCachedFullPenalties();
    }
}
