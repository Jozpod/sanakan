using Sanakan.DAL.Models;
using Sanakan.Services.PocketWaifu;
using Sanakan.ShindenApi.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.Primitives;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sanakan.Game.Services.Abstractions
{
    public interface IImageProcessor
    {
        Task SaveImageFromUrlAsync(string imageUrl, string filePath);

        Task SaveImageFromUrlAsync(string imageUrl, string filePath, Size size, bool strech = false);

        Task<Image<Rgba32>> GetUserProfileAsync(
            UserInfo? shindenUser,
            User botUser,
            string avatarUrl,
            long topPosition,
            string nickname,
            Discord.Color color);

        public Task<Image<Rgba32>> GetSiteStatisticAsync(
             UserInfo shindenInfo,
             Discord.Color color,
             List<LastWatchedRead>? lastRead = null,
             List<LastWatchedRead>? lastWatch = null);

        Task<Image<Rgba32>> GetLevelUpBadgeAsync(
            string name,
            ulong userLevel,
            string avatarUrl,
            Discord.Color color);

        Image<Rgba32> GetFColorsView(IEnumerable<(string, uint)> colours);

        Task<Image<Rgba32>> GetWaifuInProfileCardAsync(Card card);

        Image<Rgba32> GetDuelCardImage(DuelInfo info, DuelImage image, Image<Rgba32> win, Image<Rgba32> los);

        Image<Rgba32> GetCatchThatWaifuImage(Image<Rgba32> card, string imageUrl, int xPos, int yPos);

        Task<Image<Rgba32>> GetWaifuCardAsync(string url, Card card);

        Task<Image<Rgba32>> GetWaifuCardImageAsync(Card card);
    }
}
