using Discord;
using DiscordBot.Services;
using Sanakan.DAL.Models;
using Sanakan.Game.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Sanakan.DiscordBot.Services.Abstractions
{
    public interface IProfileService
    {
        Task RomoveUserColorAsync(IGuildUser user);

        bool HasSameColor(IGuildUser user, FColor color);

        Task<bool> SetUserColorAsync(IGuildUser user, ulong adminRoleId, FColor color);

        Task<SaveResult> SaveProfileImageAsync(
            string imgUrl,
            string path,
            int width = 0,
            int height = 0,
            bool streach = false);
        Task<List<string>> BuildListViewAsync(IEnumerable<User> userList, TopType topType, IGuild guild);

        List<User> GetTopUsers(IEnumerable<User> userList, TopType topType, DateTime date);

        Task<Stream> GetProfileImageAsync(IGuildUser discordUser, User databaseUser, long topPosition);

        Stream GetColorList(SCurrency currency);
    }
}
