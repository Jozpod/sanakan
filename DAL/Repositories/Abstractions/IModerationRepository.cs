using Sanakan.DAL.Models.Configuration;
using Sanakan.DAL.Models.Management;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sanakan.DAL.Repositories.Abstractions
{
    public interface IModerationRepository
    {
        Task<List<PenaltyInfo>> GetMutedPenaltiesAsync(ulong discordGuildId);
        Task<PenaltyInfo> GetPenaltyAsync(
            ulong discordUserId,
            ulong discordGuildId,
            PenaltyType penaltyType);
        Task<IEnumerable<PenaltyInfo>> GetCachedFullPenalties();
        Task RemovePenaltyAsync(PenaltyInfo penalty);
        Task AddPenaltyAsync(PenaltyInfo info);
    }
}
