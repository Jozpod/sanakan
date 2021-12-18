using Discord;
using Discord.Commands;
using Sanakan.DAL.Models.Configuration;
using Sanakan.DAL.Models.Management;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sanakan.DiscordBot.Services.Abstractions
{
    public interface IModeratorService
    {
        Task<EmbedBuilder> GetConfigurationAsync(GuildOptions config, IGuild guild, ConfigType type);

        Task NotifyUserAsync(IUser user, string reason);

        Embed BuildTodo(IMessage message, IGuildUser who);

        Task<Embed> GetMutedListAsync(IGuild guild);

        Task UnmuteUserAsync(
            IGuildUser user,
            IRole muteRole,
            IRole muteModRole);

        Task<PenaltyInfo> MuteUserAsync(
           IGuildUser user,
           IRole? muteRole,
           IRole? muteModRole,
           IRole? userRole,
           TimeSpan duration,
           string reason = Constants.NoReason,
           IEnumerable<ModeratorRoles>? modRoles = null);

        Task<PenaltyInfo> BanUserAysnc(
           IGuildUser user,
           TimeSpan duration,
           string reason = Constants.NoReason);

        Task NotifyAboutPenaltyAsync(
           IGuildUser user,
           IMessageChannel channel,
           PenaltyInfo penaltyInfo,
           string byWho = Constants.Automatic);
    }
}
