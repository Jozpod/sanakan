using Discord.WebSocket;
using Discord;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;

namespace Sanakan.DiscordBot.Services.Abstractions
{
    public interface IHelperService
    {
        IEnumerable<ModuleInfo> GetPublicModules();
        string GetCommandInfo(CommandInfo commandInfo, string? prefix = null);
        void AddPublicModuleInfo(IEnumerable<ModuleInfo> moduleInfos);
        void AddPrivateModuleInfo(params (string, ModuleInfo)[] moduleInfos);
        string GivePrivateHelp(string moduleName);
        string GivePublicHelp();
        string? GiveHelpAboutPrivateCommand(string moduleName, string command, string prefix, bool throwEx = true);
        IEmbed GetInfoAboutUser(IGuildUser user);
        Task<IEmbed> GetInfoAboutServerAsync(IGuild guild);
        IEmbed BuildRaportInfo(IMessage message, string reportAuthor, string reason, ulong reportId);
        string GiveHelpAboutPublicCommand(string command, string prefix, bool admin = false, bool dev = false);
    }
}
