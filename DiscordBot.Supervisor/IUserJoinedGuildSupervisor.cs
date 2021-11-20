using System.Collections.Generic;

namespace Sanakan.DiscordBot.Supervisor
{
    public interface IUserJoinedGuildSupervisor
    {
        IEnumerable<ulong> GetUsersToBanCauseRaid(ulong guildId, string username, ulong userId);
        void Refresh();
    }
}
