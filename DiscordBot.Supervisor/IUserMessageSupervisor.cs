using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sanakan.DiscordBot.Supervisor
{
    public interface IUserMessageSupervisor
    {
        SupervisorAction MakeDecision(ulong guildId, ulong userId, string content, bool lessSeverePunishment);
        void Refresh();
    }
}
