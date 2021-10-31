using Discord.WebSocket;
using DiscordBot.Services;
using Sanakan.DAL.Models;
using Sanakan.Game.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sanakan.DiscordBot.Services.Abstractions
{
    public interface IProfileService
    {
        Task RomoveUserColorAsync(SocketGuildUser user);
        bool HasSameColor(SocketGuildUser user, FColor color);
        Task<bool> SetUserColorAsync(SocketGuildUser user, ulong adminRole, FColor color);
        Task<SaveResult> SaveProfileImageAsync(
            string imgUrl,
            string path,
            int width = 0,
            int height = 0,
            bool streach = false);
        List<string> BuildListView(List<User> list, TopType type, SocketGuild guild);
        List<User> GetTopUsers(List<User> list, TopType type, DateTime date);
        Task<Stream> GetProfileImageAsync(SocketGuildUser discordUser, User botUser, long topPosition);
        Stream GetColorList(SCurrency currency);
    }
}
