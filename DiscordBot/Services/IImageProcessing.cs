using DiscordBot.Services;
using Sanakan.DAL.Models;
using Sanakan.Services;
using Sanakan.Services.PocketWaifu;
using Shinden.API;
using Shinden.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.Primitives;
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
            UserInfo? shindenUser,
            User botUser,
            string avatarUrl,
            long topPos,
            string nickname,
            Discord.Color color);
        Task<Image<Rgba32>> GetSiteStatisticAsync(
            UserInfo shindenInfo,
            Discord.Color color,
            List<ILastReaded> lastRead = null,
            List<ILastWatched> lastWatch = null);
        Task<Image<Rgba32>> GetLevelUpBadgeAsync(
            string name,
            long ulvl,
            string avatarUrl,
            Discord.Color color);
        Image<Rgba32> GetFColorsView(SCurrency currency);
        Task<Image<Rgba32>> GetWaifuInProfileCardAsync(Card card);
        Image<Rgba32> GetDuelCardImage(DuelInfo info, DuelImage image, Image<Rgba32> win, Image<Rgba32> los);
        Image<Rgba32> GetCatchThatWaifuImage(Image<Rgba32> card, string pokeImg, int xPos, int yPos);
        Task<Image<Rgba32>> GetWaifuCardAsync(string url, Card card);
        Task<Image<Rgba32>> GetWaifuCardAsync(Card card);
    }
}
