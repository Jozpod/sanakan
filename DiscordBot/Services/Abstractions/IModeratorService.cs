using Discord;
using Discord.WebSocket;
using Sanakan.DAL.Models.Configuration;
using Sanakan.DAL.Models.Management;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sanakan.DiscordBot.Services.Abstractions
{
    public interface IModeratorService
    {
        Task<PenaltyInfo> MuteUserAysnc(
           SocketGuildUser user,
           SocketRole muteRole,
           SocketRole muteModRole,
           SocketRole userRole,
           long duration,
           string reason = "nie podano",
           IEnumerable<ModeratorRoles>? modRoles = null);
        Task NotifyAboutPenaltyAsync(
           SocketGuildUser user,
           ITextChannel channel,
           PenaltyInfo info,
           string byWho = "automat");
    }
}
