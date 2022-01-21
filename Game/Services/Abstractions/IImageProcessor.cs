using Sanakan.DAL.Models;
using Sanakan.Game.Models;
using Sanakan.ShindenApi.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sanakan.Game.Services.Abstractions
{
    public interface IImageProcessor
    {
        Task SaveImageFromUrlAsync(string imageUrl, string filePath);

        Task SaveImageFromUrlAsync(
            string imageUrl,
            string filePath,
            Size size,
            bool stretch = false);

        Task<Image> GetUserProfileAsync(
            UserInfo? shindenUser,
            User botUser,
            string avatarUrl,
            long topPosition,
            string nickname,
            Discord.Color color);

        public Task<Image> GetSiteStatisticAsync(
             UserInfo shindenInfo,
             Discord.Color color,
             List<LastWatchedRead>? lastRead = null,
             List<LastWatchedRead>? lastWatch = null);

        Task<Image> GetLevelUpBadgeAsync(
            string name,
            ulong userLevel,
            string avatarUrl,
            Discord.Color color);

        Image GetFColorsView(IEnumerable<(string, uint)> colours);

        Task<Image> GetWaifuInProfileCardAsync(Card card);

        Task<Image> GetDuelCardImageAsync(
            DuelInfo info,
            DuelImage image,
            Image winImage,
            Image losImage);

        Task<Image> GetCatchThatWaifuImageAsync(
            Image card,
            string imageUrl,
            int xPosition,
            int yPosition);

        Task<Image> GetWaifuCardImageAsync(Card card);
    }
}
