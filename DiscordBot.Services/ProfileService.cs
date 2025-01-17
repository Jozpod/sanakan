﻿using Discord;
using DiscordBot.Services;
using Microsoft.EntityFrameworkCore;
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
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Color = Discord.Color;

namespace Sanakan.DiscordBot.Services
{
    internal class ProfileService : IProfileService
    {
        private readonly IShindenClient _shindenClient;
        private readonly IImageProcessor _imageProcessor;
        private readonly IFileSystem _fileSystem;
        private ILogger _logger;

        public ProfileService(
            IShindenClient shindenClient,
            IImageProcessor imageProcessor,
            IFileSystem fileSystem,
            ILogger<ProfileService> logger)
        {
            _shindenClient = shindenClient;
            _imageProcessor = imageProcessor;
            _fileSystem = fileSystem;
            _logger = logger;
        }

        public async Task RemoveUserColorAsync(IGuildUser? user, FColor ignored = FColor.None)
        {
            if (user == null)
            {
                return;
            }

            var colors = FColorExtensions.FColors;
            var guild = user.Guild;
            var roles = guild.Roles;
            var userRoles = roles
                .Join(user.RoleIds, pr => pr.Id, pr => pr, (src, dst) => src)
                .ToList();

            foreach (uint color in colors)
            {
                if (color == (uint)ignored)
                {
                    continue;
                }

                var role = userRoles
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

            var guild = user.Guild;
            var colorNumeric = (uint)color;
            var adminRole = guild.GetRole(adminRoleId);

            if (adminRole == null)
            {
                return false;
            }

            var colorRole = guild.Roles.FirstOrDefault(x => x.Name == colorNumeric.ToString());

            if (colorRole == null)
            {
                var dColor = new Color(colorNumeric);
                var createdRole = await guild.CreateRoleAsync(colorNumeric.ToString(), GuildPermissions.None, dColor, false, false);
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

        public IEnumerable<User> GetTopUsers(IEnumerable<User> list, TopType type, DateTime date)
            => GetRangeMax(OrderUsersByTop(list, type, date), 50);

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

        public async Task<Stream> GetProfileImageAsync(IGuildUser discordUser, User databaseUser, long topPosition)
        {
            var shindenId = databaseUser.ShindenId;
            var isConnected = shindenId.HasValue;

            var userResult = await _shindenClient.GetUserInfoAsync(shindenId!.Value);
            var user = userResult.Value;

            var guildRoles = discordUser.Guild.Roles;
            var roles = guildRoles
                .Join(discordUser.RoleIds, pr => pr.Id, pr => pr, (src, dst) => src);

            var roleColor = roles.OrderByDescending(x => x.Position)
                .FirstOrDefault()?.Color ?? Color.DarkerGrey;

            var avatarUrl = discordUser.GetUserOrDefaultAvatarUrl();

            using var image = await _imageProcessor.GetUserProfileAsync(
                isConnected ? user : null,
                databaseUser,
                avatarUrl,
                topPosition,
                discordUser.Nickname ?? discordUser.Username,
                roleColor);
            return image.ToPngStream();
        }

        public async Task<SaveResult> SaveProfileImageAsync(
            string? imageUrl,
            string filePath,
            int width = 0,
            int height = 0,
            bool stretch = false)
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
                if (_fileSystem.Exists(filePath))
                {
                    _fileSystem.Delete(filePath);
                }

                await _imageProcessor.SaveImageFromUrlAsync(imageUrl, filePath, new Size(width, height), stretch);
            }
            catch (Exception)
            {
                return SaveResult.Error;
            }

            return SaveResult.Success;
        }

        private static IEnumerable<User> OrderUsersByTop(IEnumerable<User> list, TopType type, DateTime date)
        {
            return type switch
            {
                TopType.ScCount => list.OrderByDescending(x => x.ScCount).ToList(),
                TopType.TcCount => list.OrderByDescending(x => x.TcCount).ToList(),
                TopType.AcCount => list.OrderByDescending(x => x.AcCount).ToList(),
                TopType.PcCount => list.OrderByDescending(x => x.GameDeck.PVPCoins).ToList(),
                TopType.Posts => list.OrderByDescending(x => x.MessagesCount).ToList(),
                TopType.PostsMonthly => list.Where(x => x.IsCharCounterActive(date))
                    .OrderByDescending(x => x.MessagesCount - x.MessagesCountAtDate).ToList(),
                TopType.PostsMonthlyCharacter => list
                    .Where(x => x.IsCharCounterActive(date) && x.HasSentAnyMessagesInMonth())
                    .OrderByDescending(x => x.CharacterCountFromDate / (x.MessagesCount - x.MessagesCountAtDate))
                    .ToList(),
                TopType.Commands => list.OrderByDescending(x => x.CommandsCount).ToList(),
                TopType.Card => list.OrderByDescending(x => x.GameDeck.GetStrongestCardPower()).ToList(),
                TopType.Cards => list.OrderByDescending(x => x.GameDeck.Cards.Count).ToList(),
                TopType.CardsPower => list.OrderByDescending(x => x.GameDeck.Cards.Sum(c => c.CardPower)).ToList(),
                TopType.Karma => list.OrderByDescending(x => x.GameDeck.Karma).ToList(),
                TopType.KarmaNegative => list.OrderBy(x => x.GameDeck.Karma).ToList(),
                TopType.Pvp => list.Where(x => x.GameDeck.GlobalPVPRank > 0)
                    .OrderByDescending(x => x.GameDeck.GlobalPVPRank)
                    .ToList(),
                TopType.PvpSeason => list
                    .Where(x => x.IsPVPSeasonalRankActive(date)
                        && x.GameDeck.SeasonalPVPRank > 0)
                    .OrderByDescending(x => x.GameDeck.SeasonalPVPRank)
                    .ToList(),
                _ => list.OrderByDescending(x => x.ExperienceCount).ToList(),
            };
        }

        private static IEnumerable<T> GetRangeMax<T>(IEnumerable<T> list, int range)
        {
            var length = list.Count();
            return list.Take(length > range ? range : length);
        }
    }
}