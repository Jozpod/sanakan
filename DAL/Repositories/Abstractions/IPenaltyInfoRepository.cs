using Sanakan.DAL.Models.Management;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sanakan.DAL.Repositories.Abstractions
{
    public interface IPenaltyInfoRepository : 
        IRemoveRepository<PenaltyInfo>,
        ISaveRepository
    {
        Task<List<PenaltyInfo>> GetByGuildIdAsync(ulong id);
    }
}
