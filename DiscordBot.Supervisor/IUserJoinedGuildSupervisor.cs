using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sanakan.DiscordBot.Supervisor
{
    public interface IUserJoinedGuildSupervisor
    {
        IEnumerable<ulong> GetUsersToBanCauseRaid(ulong guildId, string username, ulong userId);
        void Refresh();
    }
}
