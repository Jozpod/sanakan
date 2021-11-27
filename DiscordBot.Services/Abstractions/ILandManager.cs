using Discord;
using Sanakan.DAL.Models.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sanakan.DiscordBot.Services.Abstractions
{
    public interface ILandManager
    {
        UserLand? DetermineLand(IEnumerable<UserLand> lands, IEnumerable<ulong> roleIds, string? name);

        Task<IEnumerable<Embed>> GetMembersList(UserLand land, IGuild guild);
    }
}
