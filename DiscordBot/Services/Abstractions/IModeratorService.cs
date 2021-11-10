using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.Services;
using Sanakan.DAL.Models.Configuration;
using Sanakan.DAL.Models.Management;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sanakan.DiscordBot.Services.Abstractions
{
    public interface IModeratorService
    {
        Task<EmbedBuilder> GetConfigurationAsync(GuildOptions config, SocketCommandContext context, ConfigType type);
        Task NotifyUserAsync(IUser user, string reason);
        Embed BuildTodo(IMessage message, SocketGuildUser who);
        Task<Embed> GetMutedListAsync(SocketCommandContext context);
        Task UnmuteUserAsync(
            SocketGuildUser user,
            SocketRole muteRole,
            SocketRole muteModRole);

        Task<PenaltyInfo> MuteUserAysnc(
           IGuildUser user,
           IRole? muteRole,
           IRole? muteModRole,
           IRole? userRole,
           TimeSpan duration,
           string reason = "nie podano",
           IEnumerable<ModeratorRoles>? modRoles = null);

        Task<PenaltyInfo> BanUserAysnc(
           SocketGuildUser user,
           TimeSpan duration,
           string reason = "nie podano");
        Task NotifyAboutPenaltyAsync(
           IGuildUser user,
           IMessageChannel channel,
           PenaltyInfo penaltyInfo,
           string byWho = "automat");
    }
}
