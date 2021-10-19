using Discord;
using Discord.WebSocket;
using DiscordBot.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sanakan.DAL.Models;
using Sanakan.DiscordBot.Services;
using Sanakan.Extensions;
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
    public class Profile
    {
        private readonly DiscordSocketClient _client;
        private readonly IShindenClient _shClient;
        private readonly IImageProcessing _img;
        private ILogger _logger;
        private Timer _timer;

        public Profile(
            DiscordSocketClient client,
            IShindenClient shClient,
            IImageProcessing img,
            ILogger<Profile> logger)
        {
            _shClient = shClient;
            _client = client;
            _logger = logger;
            _img = img;

            _timer = new Timer(async _ =>
            {
                try
                {
                    await CyclicCheckAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError($"in profile check: {ex}", ex);
                }
            },
            null,
            TimeSpan.FromMinutes(1),
            TimeSpan.FromMinutes(1));
        }

        private async Task CyclicCheckAsync()
        {
            var subs = context.TimeStatuses
                .AsNoTracking()
                .FromCache(new[] { "users" })
                .Where(x => x.Type.IsSubType());

            foreach (var sub in subs)
            {
                if (sub.IsActive())
                    continue;

                var guild = _client.GetGuild(sub.Guild);
                switch (sub.Type)
                {
                    case StatusType.Globals:
                        var guildConfig = await guildContext.GetCachedGuildFullConfigAsync(sub.Guild);
                        await RemoveRoleAsync(guild, guildConfig?.GlobalEmotesRole ?? 0, sub.UserId);
                        break;

                    case StatusType.Color:
                        await RomoveUserColorAsync(guild.GetUser(sub.UserId));
                        break;

                    default:
                        break;
                }
            }
        }

        private async Task RemoveRoleAsync(SocketGuild guild, ulong roleId, ulong userId)
        {
            var role = guild.GetRole(roleId);
            if (role == null) return;

            var user = guild.GetUser(userId);
            if (user == null) return;

            await user.RemoveRoleAsync(role);
        }

        public bool HasSameColor(SocketGuildUser user, FColor color)
        {
            if (user == null) return false;

            var colorNumeric = (uint)color;
            return user.Roles.Any(x => x.Name == colorNumeric.ToString());
        }

        public async Task<bool> SetUserColorAsync(SocketGuildUser user, ulong adminRole, FColor color)
        {
            if (user == null) return false;

            var colorNumeric = (uint)color;
            var aRole = user.Guild.GetRole(adminRole);
            if (aRole == null) return false;

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
                await user.AddRoleAsync(cRole);

            return true;
        }

        public async Task RomoveUserColorAsync(SocketGuildUser user)
        {
            if (user == null) return;

            foreach(uint color in Enum.GetValues(typeof(FColor)))
            {
                var cR = user.Roles.FirstOrDefault(x => x.Name == color.ToString());
                if (cR != null)
                {
                    if (cR.Members.Count() == 1)
                    {
                        await cR.DeleteAsync();
                        return;
                    }
                    await user.RemoveRoleAsync(cR);
                }
            }
        }

        public List<User> GetTopUsers(List<User> list, TopType type)
            => GetRangeMax(OrderUsersByTop(list, type), 50);

        private List<T> GetRangeMax<T>(List<T> list, int range)
            => list.GetRange(0, list.Count > range ? range : list.Count);

        private List<User> OrderUsersByTop(List<User> list, TopType type)
        {
            switch (type)
            {
                default:
                case TopType.Level:
                    return list.OrderByDescending(x => x.ExpCnt).ToList();

                case TopType.ScCnt:
                    return list.OrderByDescending(x => x.ScCnt).ToList();

                case TopType.TcCnt:
                    return list.OrderByDescending(x => x.TcCnt).ToList();

                case TopType.AcCnt:
                    return list.OrderByDescending(x => x.AcCnt).ToList();

                case TopType.PcCnt:
                    return list.OrderByDescending(x => x.GameDeck.PVPCoins).ToList();

                case TopType.Posts:
                    return list.OrderByDescending(x => x.MessagesCnt).ToList();

                case TopType.PostsMonthly:
                    return list.Where(x => x.IsCharCounterActive()).OrderByDescending(x => x.MessagesCnt - x.MessagesCntAtDate).ToList();

                case TopType.PostsMonthlyCharacter:
                    return list.Where(x => x.IsCharCounterActive() && x.SendAnyMsgInMonth()).OrderByDescending(x => x.CharacterCntFromDate / (x.MessagesCnt - x.MessagesCntAtDate)).ToList();

                case TopType.Commands:
                    return list.OrderByDescending(x => x.CommandsCnt).ToList();

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
                    return list.Where(x => x.GameDeck.GlobalPVPRank > 0).OrderByDescending(x => x.GameDeck.GlobalPVPRank).ToList();

                case TopType.PvpSeason:
                    return list.Where(x => x.IsPVPSeasonalRankActive() && x.GameDeck.SeasonalPVPRank > 0).OrderByDescending(x => x.GameDeck.SeasonalPVPRank).ToList();
            }
        }

        public List<string> BuildListView(List<User> list, TopType type, SocketGuild guild)
        {
            var view = new List<string>();

            foreach (var user in list)
            {
                var bUsr = guild.GetUser(user.Id);
                if (bUsr == null) continue;

                view.Add($"{bUsr.Mention}: {user.GetViewValueForTop(type)}");
            }

            return view;
        }

        public Stream GetColorList(SCurrency currency)
        {
            using (var image = _img.GetFColorsView(currency))
            {
                return image.ToPngStream();
            }
        }

        public async Task<Stream> GetProfileImageAsync(SocketGuildUser user, User botUser, long topPosition)
        {
            bool isConnected = botUser.Shinden != 0;
            var response = _shClient.User.GetAsync(botUser.Shinden);
            var roleColor = user.Roles.OrderByDescending(x => x.Position)
                .FirstOrDefault()?.Color ?? Discord.Color.DarkerGrey;

            using var image = await _img.GetUserProfileAsync(
                isConnected ? (await response).Body : null,
                botUser,
                user.GetUserOrDefaultAvatarUrl(),
                topPosition,
                user.Nickname ?? user.Username,
                roleColor);
            return image.ToPngStream();
        }

        public async Task<SaveResult> SaveProfileImageAsync(string imgUrl, string path, int width = 0, int height = 0, bool streach = false)
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

                await _img.SaveImageFromUrlAsync(imgUrl, path, new Size(width, height), streach);
            }
            catch (Exception)
            {
                return SaveResult.Error;
            }

            return SaveResult.Success;
        }
    }
}