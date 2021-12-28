using Discord;
using Discord.Commands;
using DiscordBot.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Sanakan.Common;
using Sanakan.Common.Cache;
using Sanakan.Common.Configuration;
using Sanakan.Common.Models;
using Sanakan.DAL.Models;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.DiscordBot.Abstractions.Configuration;
using Sanakan.DiscordBot.Abstractions.Extensions;
using Sanakan.DiscordBot.Abstractions.Models;
using Sanakan.DiscordBot.Resources;
using Sanakan.DiscordBot.Services;
using Sanakan.DiscordBot.Services.Abstractions;
using Sanakan.DiscordBot.Session;
using Sanakan.DiscordBot.Session.Abstractions;
using Sanakan.Extensions;
using Sanakan.Game;
using Sanakan.Game.Models;
using Sanakan.Preconditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sanakan.DiscordBot.Modules
{
    [Name("Profil"), RequireUserRole]
    public class ProfileModule : SanakanModuleBase
    {
        private readonly IIconConfiguration _iconConfiguration;
        private readonly ImagingConfiguration _imagingConfiguration;
        private readonly IProfileService _profileService;
        private readonly ISessionManager _sessionManager;
        private readonly ICacheManager _cacheManager;
        private readonly IGuildConfigRepository _guildConfigRepository;
        private readonly IGameDeckRepository _gameDeckRepository;
        private readonly IUserRepository _userRepository;
        private readonly ISystemClock _systemClock;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IServiceScope _serviceScope;

        public ProfileModule(
            IIconConfiguration iconConfiguration,
            IOptionsMonitor<ImagingConfiguration> imagingConfiguration,
            IProfileService profileService,
            ISessionManager sessionManager,
            ICacheManager cacheManager,
            ISystemClock systemClock,
            IServiceScopeFactory serviceScopeFactory)
        {
            _iconConfiguration = iconConfiguration;
            _imagingConfiguration = imagingConfiguration.CurrentValue;
            _profileService = profileService;
            _sessionManager = sessionManager;
            _cacheManager = cacheManager;
            _systemClock = systemClock;
            _serviceScopeFactory = serviceScopeFactory;

            _serviceScope = _serviceScopeFactory.CreateScope();
            var serviceProvider = _serviceScope.ServiceProvider;
            _guildConfigRepository = serviceProvider.GetRequiredService<IGuildConfigRepository>();
            _gameDeckRepository = serviceProvider.GetRequiredService<IGameDeckRepository>();
            _userRepository = serviceProvider.GetRequiredService<IUserRepository>();
        }

        public override void Dispose()
        {
            _serviceScope.Dispose();
        }

        [Command("portfel", RunMode = RunMode.Async)]
        [Alias("wallet")]
        [Summary("wyświetla portfel użytkownika")]
        [Remarks("")]
        public async Task ShowWalletAsync(
            [Summary("użytkownik (opcjonalne)")] IUser? user = null)
        {
            var effectiveUser = user ?? Context.User;

            if (effectiveUser == null)
            {
                return;
            }

            var databaseUser = await _userRepository.GetCachedFullUserAsync(effectiveUser.Id);

            if (databaseUser == null)
            {
                await ReplyAsync(embed: Strings.UserDoesNotExistInDatabase.ToEmbedMessage(EMType.Error).Build());
                return;
            }

            var parameters = new object?[]
            {
                effectiveUser.Mention,
                databaseUser.ScCount,
                databaseUser.TcCount,
                databaseUser.AcCount,
                databaseUser.GameDeck?.CTCount,
                databaseUser.GameDeck?.PVPCoins,
            };

            var walletInfo = string.Format(Strings.WalletInfo, parameters);

            var content = walletInfo
                .ToEmbedMessage(EMType.Info)
                .Build();

            await ReplyAsync(embed: content);
        }

        [Command("subskrypcje", RunMode = RunMode.Async)]
        [Alias("sub")]
        [Summary("wyświetla daty zakończenia subskrypcji")]
        [Remarks(""), RequireCommandChannel]
        public async Task ShowSubscriptionsAsync()
        {
            var user = Context.User;
            var databaseUser = await _userRepository.GetCachedFullUserAsync(user.Id);
            var timeStatuses = databaseUser.TimeStatuses.Where(x => x.Type.IsSubType());

            var stringBuilder = new StringBuilder($"**Subskrypcje** {user.Mention}:\n\n", 50);
            var utcNow = _systemClock.UtcNow;

            if (timeStatuses.Any())
            {
                foreach (var timeStatus in timeStatuses)
                {
                    stringBuilder.AppendFormat("{0}\n", timeStatus.ToView(utcNow));
                }
            }
            else
            {
                stringBuilder.Append("brak");
            }

            var embed = stringBuilder.ToString().ElipseTrimToLength(1950)
                .ToEmbedMessage(EMType.Info)
                .Build();
            await ReplyAsync(embed: embed);
        }

        [Command("przyznaj role", RunMode = RunMode.Async)]
        [Alias("add role")]
        [Summary("dodaje samo zarządzaną role")]
        [Remarks("newsy"), RequireCommandChannel]
        public async Task AddRoleAsync([Summary("nazwa roli z wypisz role")] string name)
        {
            var user = Context.User as IGuildUser;

            if (user == null)
            {
                return;
            }

            var guild = Context.Guild;
            var config = await _guildConfigRepository.GetCachedGuildFullConfigAsync(guild.Id);
            var selfRole = config.SelfRoles.FirstOrDefault(x => x.Name == name);
            var gRole = guild.GetRole(selfRole?.RoleId ?? 0);

            if (gRole == null)
            {
                await ReplyAsync(embed: $"Nie odnaleziono roli `{name}`".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            if (!user.RoleIds.Contains(gRole.Id))
            {
                await user.AddRoleAsync(gRole);
            }

            var content = $"{user.Mention} przyznano role: `{name}`".ToEmbedMessage(EMType.Success).Build();
            await ReplyAsync(embed: content);
        }

        [Command("zdejmij role", RunMode = RunMode.Async)]
        [Alias("remove role")]
        [Summary("zdejmuje samo zarządzaną role")]
        [Remarks("newsy"), RequireCommandChannel]
        public async Task RemoveRoleAsync([Summary("nazwa roli z wypisz role")] string name)
        {
            var user = Context.User as IGuildUser;

            if (user == null)
            {
                return;
            }

            var config = await _guildConfigRepository.GetCachedGuildFullConfigAsync(Context.Guild.Id);
            var selfRole = config.SelfRoles.FirstOrDefault(x => x.Name == name);
            var guildRole = Context.Guild.GetRole(selfRole?.RoleId ?? 0);

            if (guildRole == null)
            {
                await ReplyAsync(embed: $"Nie odnaleziono roli `{name}`".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            if (user.RoleIds.Contains(guildRole.Id))
            {
                await user.RemoveRoleAsync(guildRole);
            }

            var content = $"{user.Mention} zdjęto role: `{name}`".ToEmbedMessage(EMType.Success).Build();
            await ReplyAsync(embed: content);
        }

        [Command("wypisz role", RunMode = RunMode.Async)]
        [Summary("wypisuje samo zarządzane role")]
        [Remarks(""), RequireCommandChannel]
        public async Task ShowRolesAsync()
        {
            var guild = Context.Guild;
            var config = await _guildConfigRepository.GetCachedGuildFullConfigAsync(guild.Id);

            if (config.SelfRoles.Count < 1)
            {
                await ReplyAsync(embed: "Nie odnaleziono roli.".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            var summary = new StringBuilder("**Dostępne role:**\n");
            foreach (var selfRole in config.SelfRoles)
            {
                var gRole = guild.GetRole(selfRole?.RoleId ?? 0);
                summary.AppendFormat(" `{0}` ", selfRole.Name);
            }

            summary.Append($"\n\nUżyj `s.przyznaj role [nazwa]` aby dodać lub `s.zdejmij role [nazwa]` odebrać sobie role.");

            await ReplyAsync(summary.ToString());
        }

        [Command("statystyki", RunMode = RunMode.Async)]
        [Alias("stats")]
        [Summary("wyświetla statystyki użytkownika")]
        [Remarks("karna")]
        public async Task ShowUserStatsAsync(
            [Summary("użytkownik (opcjonalne)")] IUser? user = null)
        {
            var effectiveUser = user ?? Context.User;
            Embed embed;

            if (effectiveUser == null)
            {
                return;
            }

            var databaseUser = await _userRepository.GetCachedFullUserAsync(effectiveUser.Id);

            if (databaseUser == null)
            {
                embed = Strings.UserDoesNotExistInDatabase.ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            var userStats = databaseUser.Stats;

            var parameters = new object[]
            {
                effectiveUser.Mention,
                databaseUser.MessagesCount,
                databaseUser.CommandsCount,
                userStats.WastedTcOnCards,
                userStats.WastedTcOnCookies,
                userStats.ScLost,
                userStats.IncomeInSc,
                userStats.SlotMachineGames,
                userStats.Tail = userStats.Head,
                userStats.Hit,
                userStats.Misd,
                userStats.OpenedBoosterPacksActivity,
                userStats.OpenedBoosterPacks
            };

            var summary = string.Format(Strings.ProfileUserStats, parameters);

            var embedBuilder = new EmbedBuilder
            {
                Color = EMType.Info.Color(),
                Description = summary.ElipseTrimToLength(1950),
            };

            await ReplyAsync(embed: embedBuilder.Build());
        }

        [Command("idp", RunMode = RunMode.Async)]
        [Alias("iledopoziomu", "howmuchtolevelup", "hmtlup")]
        [Summary("wyświetla ile pozostało punktów doświadczenia do następnego poziomu")]
        [Remarks("karna")]
        public async Task ShowHowMuchToLevelUpAsync(
            [Summary("użytkownik(opcjonalne)")] IUser? user = null)
        {
            var effectiveUser = user ?? Context.User;

            if (effectiveUser == null)
            {
                return;
            }

            var databaseUser = await _userRepository.GetByDiscordIdAsync(effectiveUser.Id);

            if (databaseUser == null)
            {
                await ReplyAsync(embed: Strings.UserDoesNotExistInDatabase.ToEmbedMessage(EMType.Error).Build());
                return;
            }

            var nextLevel = databaseUser.Level + 1;
            var experience = ExperienceUtils.CalculateExpForLevel(nextLevel);
            var remainingExperience = databaseUser.GetRemainingExp(experience);

            var embed = $"{effectiveUser.Mention} potrzebuje **{remainingExperience}** punktów doświadczenia do następnego poziomu."
                .ToEmbedMessage(EMType.Info).Build();
            await ReplyAsync(embed: embed);
        }

        [Command("topka", RunMode = RunMode.Async)]
        [Alias("top")]
        [Summary("wyświetla topke użytkowników")]
        [Remarks(""), RequireAnyCommandChannel]
        public async Task ShowTopAsync(
            [Summary("rodzaj topki (poziom/sc/tc/pc/ac/posty(m/ms)/kart(a/y/ym)/karma(-))/pvp(s)")] TopType topType = TopType.Level)
        {
            var utcNow = _systemClock.UtcNow;
            var discordUserId = Context.User.Id;
            
            _sessionManager.RemoveIfExists<ListSession>(discordUserId);

            var building = await ReplyAsync(embed: $"🔨 Trwa budowanie topki...".ToEmbedMessage(EMType.Bot).Build());
            var users = await _userRepository.GetCachedAllUsersAsync();
            var topUsers = _profileService.GetTopUsers(users, topType, utcNow);
            var items = await _profileService.BuildListViewAsync(topUsers, topType, Context.Guild);

            var embedBuilder = new EmbedBuilder
            {
                Color = EMType.Info.Color(),
                Title = $"Topka {topType.Name()}"
            };

            await building.DeleteAsync();
            var embed = ListSessionUtils.BuildPage(embedBuilder, items);
            var message = await ReplyAsync(embed: embed);
            await message.AddReactionsAsync(_iconConfiguration.LeftRightArrows);
            var bot = Context.Client.CurrentUser;

            var session = new ListSession(
                discordUserId,
                utcNow,
                items,
                bot,
                message,
                embedBuilder);
            _sessionManager.Add(session);
        }

        [Command("widok waifu")]
        [Alias("waifu view")]
        [Summary("przełącza widoczność waifu na pasku bocznym profilu użytkownika")]
        [Remarks(""), RequireAnyCommandChannel]
        public async Task ToggleWaifuViewInProfileAsync()
        {
            var user = Context.User;
            var databaseUser = await _userRepository.GetUserOrCreateAsync(user.Id);
            databaseUser.ShowWaifuInProfile = !databaseUser.ShowWaifuInProfile;

            var result = databaseUser.ShowWaifuInProfile ? "załączony" : "wyłączony";

            await _userRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(CacheKeys.User(databaseUser.Id), CacheKeys.Users);

            var content = $"Podgląd waifu w profilu {user.Mention} został {result}."
                .ToEmbedMessage(EMType.Success).Build();

            await ReplyAsync(embed: content);
        }

        [Command("profil", RunMode = RunMode.Async)]
        [Alias("profile")]
        [Summary("wyświetla profil użytkownika")]
        [Remarks("karna")]
        public async Task ShowUserProfileAsync(
            [Summary("użytkownik (opcjonalne)")] IGuildUser? guildUser = null)
        {
            var user = guildUser ?? Context.User as IGuildUser;

            if (user == null)
            {
                return;
            }

            var allUsers = await _userRepository.GetCachedAllUsersLiteAsync();
            var databaseUser = allUsers.FirstOrDefault(x => x.Id == user.Id);

            if (databaseUser == null)
            {
                await ReplyAsync(embed: Strings.UserDoesNotExistInDatabase.ToEmbedMessage(EMType.Error).Build());
                return;
            }

            var gameDeck = await _gameDeckRepository.GetCachedUserGameDeckAsync(user.Id);
            databaseUser.GameDeck = gameDeck!;
            var topPosition = allUsers
                .OrderByDescending(x => x.ExperienceCount)
                .ToList()
                .IndexOf(databaseUser) + 1;

            using var stream = await _profileService
                .GetProfileImageAsync(user, databaseUser, topPosition);

            await Context.Channel.SendFileAsync(stream, $"{user.Id}.png");
        }

        [Command("misje")]
        [Alias("quest")]
        [Summary("wyświetla postęp misji użytkownika")]
        [Remarks("tak"), RequireAnyCommandChannel]
        public async Task ShowUserQuestsProgressAsync(
            [Summary("czy odebrać nagrody?")] bool claimGifts = false)
        {
            var user = Context.User;
            var databaseUser = await _userRepository.GetUserOrCreateAsync(user.Id);
            var weeklyQuests = databaseUser.CreateOrGetAllWeeklyQuests();
            var dailyQuests = databaseUser.CreateOrGetAllDailyQuests();
            var utcNow = _systemClock.UtcNow;

            Embed embed;

            if (claimGifts)
            {
                var rewards = new List<string>();
                var allClaimedBefore = dailyQuests.Count(x => x.IsClaimed(utcNow)) == dailyQuests.Count;
                foreach (var dailyQuest in dailyQuests)
                {
                    if (dailyQuest.CanClaim(utcNow))
                    {
                        dailyQuest.Claim(databaseUser);
                        rewards.Add(dailyQuest.Type.GetRewardString());
                    }
                }

                if (!allClaimedBefore && dailyQuests.Count(x => x.IsClaimed(utcNow)) == dailyQuests.Count)
                {
                    databaseUser.AcCount += 10;
                    rewards.Add("10 AC");
                }

                foreach (var weeklyQuest in weeklyQuests)
                {
                    if (weeklyQuest.CanClaim(utcNow))
                    {
                        weeklyQuest.Claim(databaseUser);
                        rewards.Add(weeklyQuest.Type.GetRewardString());
                    }
                }

                if (rewards.Any())
                {
                    _cacheManager.ExpireTag(CacheKeys.User(databaseUser.Id), CacheKeys.Users);

                    embed = $"**Odebrane nagrody:**\n\n{string.Join("\n", rewards)}"
                        .ToEmbedMessage(EMType.Success)
                        .WithUser(user)
                        .Build();

                    await ReplyAsync(embed: embed);
                    await _userRepository.SaveChangesAsync();
                    return;
                }

                embed = "Nie masz nic do odebrania.".ToEmbedMessage(EMType.Error)
                    .WithUser(user)
                    .Build();

                await ReplyAsync(embed: embed);
                return;
            }

            var parameters = new object[]
            {
                string.Join("\n", dailyQuests.Select(x => x.ToView(utcNow))),
                string.Join("\n", weeklyQuests.Select(x => x.ToView(utcNow)))
            };

            embed = string.Format(Strings.UserQuestsProgress, parameters)
                .ToEmbedMessage(EMType.Bot)
                .WithUser(Context.User).Build();
            await ReplyAsync(embed: embed);
        }

        [Command("styl")]
        [Alias("style")]
        [Summary("zmienia styl profilu (koszt 3000 SC/1000 TC)")]
        [Remarks("1 https://i.imgur.com/8UK8eby.png"), RequireCommandChannel]
        public async Task ChangeStyleAsync(
            [Summary("typ stylu (statystyki(0), obrazek(1), brzydkie(2), karcianka(3))")] ProfileType profileType,
            [Summary("bezpośredni adres do obrazka gdy wybrany styl 1 lub 2 (325 x 272)")] string? imageUrl = null,
            [Summary("waluta (SC/TC)")] SCurrency currency = SCurrency.Sc)
        {
            var scCost = 3000;
            var tcCost = 1000;
            var user = Context.User;
            var mention = user.Mention;
            var databaseUser = await _userRepository.GetUserOrCreateAsync(user.Id);
            Embed embed;

            if (databaseUser.ScCount < scCost && currency == SCurrency.Sc)
            {
                embed = $"{mention} nie posiadasz wystarczającej liczby SC!".ToEmbedMessage(EMType.Error)
                    .Build();
                await ReplyAsync(embed: embed);
                return;
            }
            if (databaseUser.TcCount < tcCost && currency == SCurrency.Tc)
            {
                embed = string.Format(Strings.NoEnoughTC, mention).ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            switch (profileType)
            {
                case ProfileType.Image:
                case ProfileType.StatisticsWithImage:
                    var filePath = $"{Paths.SavedData}/SR{databaseUser.Id}.png";
                    var saveResult = await _profileService.SaveProfileImageAsync(
                        imageUrl!,
                        filePath,
                        _imagingConfiguration.ProfileImageWidth,
                        _imagingConfiguration.ProfileImageHeight);

                    if (saveResult == SaveResult.Success)
                    {
                        databaseUser.StatsReplacementProfileUri = $"{Paths.SavedData}/SR{databaseUser.Id}.png";
                        break;
                    }
                    else if (saveResult == SaveResult.BadUrl)
                    {
                        embed = Strings.InvalidImageProvideCorrectUrl.ToEmbedMessage(EMType.Error).Build();
                        await ReplyAsync(embed: embed);
                        return;
                    }

                    embed = "Coś poszło nie tak, prawdopodobnie nie mam uprawnień do zapisu!"
                        .ToEmbedMessage(EMType.Error).Build();
                    await ReplyAsync(embed: embed);
                    return;

                default:
                    break;
            }

            if (currency == SCurrency.Sc)
            {
                databaseUser.ScCount -= scCost;
            }
            else
            {
                databaseUser.TcCount -= tcCost;
            }
            databaseUser.ProfileType = profileType;

            await _userRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(CacheKeys.User(databaseUser.Id), CacheKeys.Users);

            embed = $"Zmieniono styl profilu użytkownika: {mention}!".ToEmbedMessage(EMType.Success).Build();
            await ReplyAsync(embed: embed);
        }

        [Command("tło")]
        [Alias("tlo", "bg", "background")]
        [Summary("zmienia obrazek tła profilu (koszt 5000 SC/2500 TC)")]
        [Remarks("https://i.imgur.com/LjVxiv8.png"), RequireCommandChannel]
        public async Task ChangeBackgroundAsync(
            [Summary("bezpośredni adres do obrazka (450 x 145)")] string imageUrl,
            [Summary("waluta (SC/TC)")] SCurrency currency = SCurrency.Sc)
        {
            var tcCost = 2500;
            var scCost = 5000;
            var user = Context.User;
            var mention = user.Mention;
            Embed embed;

            var databaseUser = await _userRepository.GetUserOrCreateAsync(user.Id);
            
            if (databaseUser.ScCount < scCost && currency == SCurrency.Sc)
            {
                embed = $"{mention} nie posiadasz wystarczającej liczby SC!".ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }
            if (databaseUser.TcCount < tcCost && currency == SCurrency.Tc)
            {
                embed = string.Format(Strings.NoEnoughTC, mention).ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            var savePath = $"{Paths.SavedData}/BG{databaseUser.Id}.png";
            var saveResult = await _profileService.SaveProfileImageAsync(imageUrl, savePath, 450, 145, true);

            if (saveResult == SaveResult.Success)
            {
                databaseUser.BackgroundProfileUri = $"{Paths.SavedData}/BG{databaseUser.Id}.png";
            }
            else if (saveResult == SaveResult.BadUrl)
            {
                embed = Strings.InvalidImageProvideCorrectUrl.ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }
            else
            {
                embed = "Coś poszło nie tak, prawdopodobnie nie mam uprawnień do zapisu!".ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            if (currency == SCurrency.Sc)
            {
                databaseUser.ScCount -= scCost;
            }
            else
            {
                databaseUser.TcCount -= tcCost;
            }

            await _userRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(CacheKeys.User(databaseUser.Id), CacheKeys.Users);

            embed = $"Zmieniono tło profilu użytkownika: {mention}!".ToEmbedMessage(EMType.Success).Build();
            await ReplyAsync(embed: embed);
        }

        [Command("globalki")]
        [Alias("global")]
        [Summary("nadaje na miesiąc rangę od globalnych emotek (1000 TC)")]
        [Remarks(""), RequireCommandChannel]
        public async Task AddGlobalEmotesAsync()
        {
            var cost = 1000;
            var user = Context.User as IGuildUser;
            var mention = user.Mention;
            Embed embed;

            if (user == null)
            {
                return;
            }

            var databaseUser = await _userRepository.GetUserOrCreateAsync(user.Id);
            if (databaseUser.TcCount < cost)
            {
                embed = string.Format(Strings.NoEnoughTC, mention).ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            var guild = Context.Guild;
            var guildid = guild.Id;
            var guildConfig = await _guildConfigRepository.GetCachedGuildFullConfigAsync(guildid);
            var globalRole = guild.GetRole(guildConfig.GlobalEmotesRoleId);

            if (globalRole == null)
            {
                embed = "Serwer nie ma ustawionej roli globalnych emotek.".ToEmbedMessage(EMType.Bot).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            var global = databaseUser.TimeStatuses
                .FirstOrDefault(x => x.Type == StatusType.Globals
                    && x.GuildId == guildid);

            if (global == null)
            {
                global = new TimeStatus(StatusType.Globals, guildid);
                global.EndsOn = _systemClock.UtcNow;
                databaseUser.TimeStatuses.Add(global);
            }

            if (!user.RoleIds.Contains(globalRole.Id))
            {
                await user.AddRoleAsync(globalRole);
            }

            global.EndsOn = global.EndsOn!.Value.AddMonths(1);
            databaseUser.TcCount -= cost;

            await _userRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(CacheKeys.User(databaseUser.Id));

            embed = $"{mention} wykupił miesiąc globalnych emotek!".ToEmbedMessage(EMType.Success)
                .Build();
            await ReplyAsync(embed: embed);
        }

        [Command("kolor")]
        [Alias("color", "colour")]
        [Summary("zmienia kolor użytkownika (koszt TC/SC na liście)")]
        [Remarks("pink"), RequireCommandChannel]
        public async Task ToggleColorRoleAsync(
            [Summary("kolor z listy (none - lista)")] FColor color = FColor.None,
            [Summary("waluta (SC/TC)")] SCurrency currency = SCurrency.Tc)
        {
            var user = Context.User as IGuildUser;
            var guild = Context.Guild;
            var guildId = guild.Id;
            Embed embed;

            if (user == null)
            {
                return;
            }

            if (color == FColor.None)
            {
                using var imageList = _profileService.GetColorList(currency);
                await Context.Channel.SendFileAsync(imageList, "list.png");
                return;
            }

            var databaseUser = await _userRepository.GetUserOrCreateAsync(user.Id);
            var points = currency == SCurrency.Tc ? databaseUser.TcCount : databaseUser.ScCount;

            if (points < color.Price(currency))
            {
                embed = $"{user.Mention} nie posiadasz wystarczającej liczby {currency.ToString().ToUpper()}!"
                    .ToEmbedMessage(EMType.Error)
                    .Build();
                await ReplyAsync(embed: embed);
                return;
            }

            var colorTimeStatus = databaseUser.TimeStatuses
                .FirstOrDefault(x => x.Type == StatusType.Color
                    && x.GuildId == guildId);

            if (colorTimeStatus == null)
            {
                colorTimeStatus = new TimeStatus(StatusType.Color, guildId);
                databaseUser.TimeStatuses.Add(colorTimeStatus);
            }

            var utcNow = _systemClock.UtcNow;

            if (color == FColor.CleanColor)
            {
                colorTimeStatus.EndsOn = utcNow;
                await _profileService.RemoveUserColorAsync(user, color);
            }
            else
            {
                if (_profileService.HasSameColor(user, color) && colorTimeStatus.IsActive(utcNow))
                {
                    colorTimeStatus.EndsOn = colorTimeStatus.EndsOn!.Value.AddMonths(1);
                }
                else
                {
                    await _profileService.RemoveUserColorAsync(user);
                    colorTimeStatus.EndsOn = utcNow.AddMonths(1);
                }

                var gConfig = await _guildConfigRepository.GetCachedGuildFullConfigAsync(guild.Id);
                var adminRoleId = gConfig.AdminRoleId!.Value;

                if (!await _profileService.SetUserColorAsync(user, adminRoleId, color))
                {
                    await ReplyAsync(embed: $"Coś poszło nie tak!".ToEmbedMessage(EMType.Error).Build());
                    return;
                }

                if (currency == SCurrency.Tc)
                {
                    databaseUser.TcCount -= color.Price(currency);
                }
                else
                {
                    databaseUser.ScCount -= color.Price(currency);
                }
            }

            await _userRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(CacheKeys.User(databaseUser.Id), CacheKeys.Users);

            embed = $"{user.Mention} wykupił kolor!".ToEmbedMessage(EMType.Success).Build();
            await ReplyAsync(embed: embed);
        }
    }
}