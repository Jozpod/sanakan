using Discord;
using Sanakan.DAL.Models.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sanakan.DiscordBot.Services.Abstractions
{
    public interface ILandManager
    {
        MyLand? DetermineLand(IEnumerable<MyLand> lands, IEnumerable<ulong> roleIds, string? name);

        Task<IEnumerable<Embed>> GetMembersList(MyLand land, IGuild guild);
    }
}
