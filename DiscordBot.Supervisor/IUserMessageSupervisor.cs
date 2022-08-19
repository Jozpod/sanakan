using System.Threading.Tasks;

namespace Sanakan.DiscordBot.Supervisor
{
    public interface IUserMessageSupervisor
    {
        Task<SupervisorAction> MakeDecisionAsync(ulong guildId, ulong userId, string content, bool lessSeverePunishment);

        void Refresh();
    }
}
