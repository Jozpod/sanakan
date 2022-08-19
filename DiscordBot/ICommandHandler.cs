using System.Threading.Tasks;

namespace Sanakan.DiscordBot
{
    public interface ICommandHandler
    {
        Task InitializeAsync();
    }
}
