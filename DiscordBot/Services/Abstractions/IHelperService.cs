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
        IEnumerable<ModuleInfo> PublicModulesInfo { get; }
        string GivePrivateHelp(string moduleName);
        string GivePublicHelp();
        string GiveHelpAboutPrivateCmd(string moduleName, string command, string prefix, bool throwEx = true);
        IEmbed GetInfoAboutUser(SocketGuildUser user);
        IEmbed GetInfoAboutServer(SocketGuild guild);
        IEmbed BuildRaportInfo(IMessage message, string reportAuthor, string reason, ulong reportId);
        string GiveHelpAboutPublicCmd(string command, string prefix, bool admin = false, bool dev = false);
    }
}
