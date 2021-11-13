using Discord;
using Discord.WebSocket;
using DiscordBot.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sanakan.Common;
using Sanakan.DAL.Models;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.DiscordBot.Abstractions.Extensions;
using Sanakan.DiscordBot.Services;
using Sanakan.DiscordBot.Services.Abstractions;
using Sanakan.Extensions;
using Sanakan.Game.Extensions;
using Sanakan.Game.Models;
using Sanakan.Game.Services;
using Sanakan.Game.Services.Abstractions;
using Sanakan.ShindenApi;
using SixLabors.Primitives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Sanakan.Services
{
    internal class ProfileService : IProfileService
    {
        private readonly DiscordSocketClient _client;
        private readonly IShindenClient _shindenClient;
        private readonly IImageProcessor _imageProcessor;
        private readonly IFileSystem _fileSystem;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private ILogger _logger;

        public ProfileService(
            DiscordSocketClient client,
            IShindenClient shindenClient,
            IImageProcessor imageProcessor,
            IFileSystem fileSystem,
            IServiceScopeFactory serviceScopeFactory,
            ILogger<ProfileService> logger)
        {
            _shindenClient = shindenClient;
            _client = client;
            _logger = logger;
            _fileSystem = fileSystem;
            _serviceScopeFactory = serviceScopeFactory;
            _imageProcessor = imageProcessor;
        }

        public async Task RomoveUserColorAsync(IGuildUser user)
        {
            if (user == null)
            {
                return;
            }

            var colors = FColorExtensions.FColors;
            var guild = user.Guild;
            var roles = guild.Roles;
            foreach (uint color in colors)
            {
                var role = roles
                    .Join(user.RoleIds, pr => pr.Id, pr => pr, (src, dst) => src)
                    .FirstOrDefault(x => x.Name == color.ToString());
                if (role == null)
                {
                    continue;
                }

                var members = (await guild.GetUsersAsync())
                    .Where(x => x.RoleIds.Any(id => id == role.Id));

                if (members.Count() == 1)
                {
                    await role.DeleteAsync();
                    return;
                }
                await user.RemoveRoleAsync(role);
            }
        }

        public bool HasSameColor(SocketGuildUser user, FColor color)
        {
            if (user == null)
            {
                return false;
            }

            var colorNumeric = (uint)color;
            return user.Roles.Any(x => x.Name == colorNumeric.ToString());
        }

        public async Task<bool> SetUserColorAsync(SocketGuildUser user, ulong adminRole, FColor color)
        {
            if (user == null)
            {
                return false;
            }

            var colorNumeric = (uint)color;
            var aRole = user.Guild.GetRole(adminRole);

            if (aRole == null)
            {
                return false;
            }

            var cRole = user.Guild.Roles.FirstOrDefault(x => x.Name == colorNumeric.ToString());

            if (cRole == null)
            {
                var dColor = new Color(colorNumeric);
                var createdRole = await user.Guild.CreateRoleAsync(colorNumeric.ToString(), GuildPermissions.None, dColor, false, false);
                await createdRole.ModifyAsync(x => x.Position = aRole.Position + 1);
                await user.AddRoleAsync(createdRole);
                return true;
            }

            if (!user.Roles.Contains(cRole))
            {
                await user.AddRoleAsync(cRole);
            }

            return true;
        }

      

        public List<User> GetTopUsers(List<User> list, TopType type, DateTime date)
            => GetRangeMax(OrderUsersByTop(list, type, date), 50);

        private List<T> GetRangeMax<T>(List<T> list, int range)
            => list.GetRange(0, list.Count > range ? range : list.Count);

        private List<User> OrderUsersByTop(List<User> list, TopType type, DateTime date)
        {
            switch (type)
            {
                default:
                case TopType.Level:
                    return list.OrderByDescending(x => x.ExperienceCount).ToList();

                case TopType.ScCnt:
                    return list.OrderByDescending(x => x.ScCount).ToList();

                case TopType.TcCnt:
                    return list.OrderByDescending(x => x.TcCount).ToList();

                case TopType.AcCnt:
                    return list.OrderByDescending(x => x.AcCount).ToList();

                case TopType.PcCnt:
                    return list.OrderByDescending(x => x.GameDeck.PVPCoins).ToList();

                case TopType.Posts:
                    return list.OrderByDescending(x => x.MessagesCount).ToList();

                case TopType.PostsMonthly:
                    return list.Where(x => x.IsCharCounterActive(date))
                        .OrderByDescending(x => x.MessagesCount - x.MessagesCountAtDate).ToList();

                case TopType.PostsMonthlyCharacter:
                    return list.Where(x => x.IsCharCounterActive(date) && x.SendAnyMsgInMonth())
                        .OrderByDescending(x => x.CharacterCountFromDate / (x.MessagesCount - x.MessagesCountAtDate)).ToList();

                case TopType.Commands:
                    return list.OrderByDescending(x => x.CommandsCount).ToList();

                case TopType.Card:
                    return list.OrderByDescending(x => x.GameDeck.GetStrongestCardPower()).ToList();

                case TopType.Cards:
                    return list.OrderByDescending(x => x.GameDeck.Cards.Count).ToList();

                case TopType.CardsPower:
                    return list.OrderByDescending(x => x.GameDeck.Cards.Sum(c => c.CardPower)).ToList();

                case TopType.Karma:
                    return list.OrderByDescending(x => x.GameDeck.Karma).ToList();

                case TopType.KarmaNegative:
                    return list.OrderBy(x => x.GameDeck.Karma).ToList();

                case TopType.Pvp:
                    return list.Where(x => x.GameDeck.GlobalPVPRank > 0)
                        .OrderByDescending(x => x.GameDeck.GlobalPVPRank).ToList();

                case TopType.PvpSeason:
                    return list.Where(x => x.IsPVPSeasonalRankActive(date)
                        && x.GameDeck.SeasonalPVPRank > 0)
                        .OrderByDescending(x => x.GameDeck.SeasonalPVPRank).ToList();
            }
        }

        public async Task<List<string>> BuildListViewAsync(List<User> list, TopType type, IGuild guild)
        {
            var view = new List<string>();

            foreach (var user in list)
            {
                var socketGuildUser = await guild.GetUserAsync(user.Id);
                if (socketGuildUser == null)
                {
                    continue;
                }

                view.Add($"{socketGuildUser.Mention}: {user.GetViewValueForTop(type)}");
            }

            return view;
        }

        public Stream GetColorList(SCurrency currency)
        {
            var colours = FColorExtensions.FColors;
            var coloursSummary = colours
                .Select(colour => ($"{colour} ({colour.Price(currency)} {currency.ToString().ToUpper()})", (uint)colour));

            using var image = _imageProcessor.GetFColorsView(coloursSummary);
            return image.ToPngStream();
        }

        public async Task<Stream> GetProfileImageAsync(SocketGuildUser discordUser, User botUser, long topPosition)
        {
            var isConnected = botUser.ShindenId.HasValue;

            var userResult = await _shindenClient.GetUserInfoAsync(botUser.ShindenId.Value);
            var user = userResult.Value;

            var roleColor = discordUser.Roles.OrderByDescending(x => x.Position)
                .FirstOrDefault()?.Color ?? Discord.Color.DarkerGrey;

            using var image = await _imageProcessor.GetUserProfileAsync(
                isConnected ? user : null,
                botUser,
                discordUser.GetUserOrDefaultAvatarUrl(),
                topPosition,
                discordUser.Nickname ?? discordUser.Username,
                roleColor);
            return image.ToPngStream();
        }

        public async Task<SaveResult> SaveProfileImageAsync(
            string imgUrl,
            string path,
            int width = 0,
            int height = 0,
            bool streach = false)
        {
            if (imgUrl == null)
                return SaveResult.BadUrl;

            if (!imgUrl.IsURLToImage())
                return SaveResult.BadUrl;

            try
            {
                if (_fileSystem.Exists(path))
                {
                    _fileSystem.Delete(path);
                }

                await _imageProcessor.SaveImageFromUrlAsync(imgUrl, path, new Size(width, height), streach);
            }
            catch (Exception)
            {
                return SaveResult.Error;
            }

            return SaveResult.Success;
        }
    }
}