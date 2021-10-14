using Sanakan.DAL.Models;
using Sanakan.Services;
using Shinden.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sanakan.DiscordBot.Services
{
    public interface IImageProcessing
    {
        Task SaveImageFromUrlAsync(string url, string path);
        Task SaveImageFromUrlAsync(string url, string path, Size size, bool strech = false);
        Task<Image<Rgba32>> GetUserProfileAsync(
            IUserInfo shindenUser,
            User botUser,
            string avatarUrl,
            long topPos,
            string nickname,
            Discord.Color color);
        Task<Image<Rgba32>> GetSiteStatisticAsync(
            IUserInfo shindenInfo,
            Discord.Color color,
            List<ILastReaded> lastRead = null,
            List<ILastWatched> lastWatch = null);
        Task<Image<Rgba32>> GetLevelUpBadgeAsync(
            string name,
            long ulvl,
            string avatarUrl,
            Discord.Color color);
        Image<Rgba32> GetFColorsView(SCurrency currency);
    }
}
