﻿using Discord;
using DiscordBot.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sanakan.Common;
using Sanakan.DAL.Models;
using Sanakan.DiscordBot.Abstractions.Extensions;
using Sanakan.DiscordBot.Services.Abstractions;
using Sanakan.Extensions;
using Sanakan.Game.Extensions;
using Sanakan.Game.Models;
using Sanakan.Game.Services.Abstractions;
using Sanakan.ShindenApi;
using SixLabors.Primitives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Sanakan.DiscordBot.Services
{
    internal class ProfileService : IProfileService
    {
        private readonly IShindenClient _shindenClient;
        private readonly IImageProcessor _imageProcessor;
        private readonly IFileSystem _fileSystem;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private ILogger _logger;

        public ProfileService(
            IShindenClient shindenClient,
            IImageProcessor imageProcessor,
            IFileSystem fileSystem,
            IServiceScopeFactory serviceScopeFactory,
            ILogger<ProfileService> logger)
        {
            _shindenClient = shindenClient;
            _imageProcessor = imageProcessor;
            _fileSystem = fileSystem;
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
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

        public bool HasSameColor(IGuildUser user, FColor color)
        {
            if (user == null)
            {
                return false;
            }

            var colorNumeric = (uint)color;
            var guildRoles = user.Guild.Roles;

            var roles = guildRoles
                .Join(user.RoleIds, pr => pr.Id, pr => pr, (src, dst) => src);

            return roles.Any(x => x.Name == colorNumeric.ToString());
        }

        public async Task<bool> SetUserColorAsync(IGuildUser user, ulong adminRoleId, FColor color)
        {
            if (user == null)
            {
                return false;
            }

            var colorNumeric = (uint)color;
            var adminRole = user.Guild.GetRole(adminRoleId);

            if (adminRole == null)
            {
                return false;
            }

            var colorRole = user.Guild.Roles.FirstOrDefault(x => x.Name == colorNumeric.ToString());

            if (colorRole == null)
            {
                var dColor = new Color(colorNumeric);
                var createdRole = await user.Guild.CreateRoleAsync(colorNumeric.ToString(), GuildPermissions.None, dColor, false, false);
                await createdRole.ModifyAsync(x => x.Position = adminRole.Position + 1);
                await user.AddRoleAsync(createdRole);
                return true;
            }

            if (!user.RoleIds.Contains(colorRole.Id))
            {
                await user.AddRoleAsync(colorRole);
            }

            return true;
        }

      

        public List<User> GetTopUsers(IEnumerable<User> list, TopType type, DateTime date)
            => GetRangeMax(OrderUsersByTop(list, type, date), 50);

        private List<T> GetRangeMax<T>(List<T> list, int range)
            => list.GetRange(0, list.Count > range ? range : list.Count);

        private List<User> OrderUsersByTop(IEnumerable<User> list, TopType type, DateTime date)
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

        public async Task<List<string>> BuildListViewAsync(IEnumerable<User> userList, TopType type, IGuild guild)
        {
            var view = new List<string>();

            foreach (var user in userList)
            {
                var guildUser = await guild.GetUserAsync(user.Id);
                if (guildUser == null)
                {
                    continue;
                }

                view.Add($"{guildUser.Mention}: {user.GetViewValueForTop(type)}");
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

        public async Task<Stream> GetProfileImageAsync(IGuildUser discordUser, User botUser, long topPosition)
        {
            var isConnected = botUser.ShindenId.HasValue;

            var userResult = await _shindenClient.GetUserInfoAsync(botUser.ShindenId.Value);
            var user = userResult.Value;

            var guildRoles = discordUser.Guild.Roles;
            var roles = guildRoles
                .Join(discordUser.RoleIds, pr => pr.Id, pr => pr, (src, dst) => src);

            var roleColor = roles.OrderByDescending(x => x.Position)
                .FirstOrDefault()?.Color ?? Color.DarkerGrey;

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
            string? imageUrl,
            string path,
            int width = 0,
            int height = 0,
            bool streach = false)
        {
            if (imageUrl == null)
            {
                return SaveResult.BadUrl;
            }

            if (!imageUrl.IsURLToImage())
            {
                return SaveResult.BadUrl;
            }

            try
            {
                if (_fileSystem.Exists(path))
                {
                    _fileSystem.Delete(path);
                }

                await _imageProcessor.SaveImageFromUrlAsync(imageUrl, path, new Size(width, height), streach);
            }
            catch (Exception)
            {
                return SaveResult.Error;
            }

            return SaveResult.Success;
        }
    }
}