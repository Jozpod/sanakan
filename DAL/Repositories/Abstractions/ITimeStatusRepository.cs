using Sanakan.DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sanakan.DAL.Repositories.Abstractions
{
    public interface ITimeStatusRepository : 
        IRemoveRepository<TimeStatus>,
        ISaveRepository
    {
        Task<List<TimeStatus>> GetByGuildIdAsync(ulong discordGuildId);
        Task<List<TimeStatus>> GetBySubTypeAsync();
    }
}
