using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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

        string GiveHelpAboutPublicCommand(string command, string prefix, bool isAdmin = false, bool isDev = false);

        Version GetVersion();
    }
}
