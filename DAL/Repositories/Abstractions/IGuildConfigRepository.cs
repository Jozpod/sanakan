using Sanakan.DAL.Models.Configuration;
using System.Threading.Tasks;

namespace Sanakan.DAL.Repositories.Abstractions
{
    public interface IGuildConfigRepository :
        ICreateRepository<GuildOptions>,
        IRemoveRepository<GuildOptions>,
        ISaveRepository
    {
        Task<GuildOptions?> GetOrCreateAsync(ulong guildId);

        Task<GuildOptions?> GetCachedById(ulong guildId);
    }
}
