﻿using Discord;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using Sanakan.Common;
using Sanakan.Common.Cache;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.DiscordBot.Abstractions.Extensions;
using Sanakan.DiscordBot.Abstractions.Models;
using Sanakan.DiscordBot.Resources;
using Sanakan.DiscordBot.Session;
using Sanakan.DiscordBot.Session.Abstractions;
using Sanakan.Extensions;
using Sanakan.Game.Services.Abstractions;
using Sanakan.Preconditions;
using Sanakan.ShindenApi;
using Sanakan.ShindenApi.Models.Enums;
using Sanakan.ShindenApi.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Sanakan.DiscordBot.Modules
{
    [Name("Shinden"), RequireUserRole]
    public class ShindenModule : SanakanModuleBase
    {
        private readonly IShindenClient _shindenClient;
        private readonly ISessionManager _sessionManager;
        private readonly ICacheManager _cacheManager;
        private readonly IUserRepository _userRepository;
        private readonly ISystemClock _systemClock;
        private readonly IImageProcessor _imageProcessor;
        private readonly ITaskManager _taskManager;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IServiceScope _serviceScope;

        public ShindenModule(
            IShindenClient shindenClient,
            ISessionManager sessionManager,
            ICacheManager cacheManager,
            ISystemClock systemClock,
            ITaskManager taskManager,
            IImageProcessor imageProcessor,
            IServiceScopeFactory serviceScopeFactory)
        {
            _shindenClient = shindenClient;
            _sessionManager = sessionManager;
            _cacheManager = cacheManager;
            _systemClock = systemClock;
            _taskManager = taskManager;
            _imageProcessor = imageProcessor;
            _serviceScopeFactory = serviceScopeFactory;

            _serviceScope = _serviceScopeFactory.CreateScope();
            var serviceProvider = _serviceScope.ServiceProvider;
            _userRepository = serviceProvider.GetRequiredService<IUserRepository>();
        }

        public override void Dispose()
        {
            _serviceScope.Dispose();
        }

        [Command("odcinki", RunMode = RunMode.Async)]
        [Alias("episodes")]
        [Summary("wyświetla nowo dodane epizody")]
        [Remarks(""), RequireCommandChannel]
        public async Task ShowNewEpisodesAsync()
        {
            var response = await _shindenClient.GetNewEpisodesAsync();
            Embed embed;

            if (response.Value == null)
            {
                embed = Strings.CouldNotGetEpsiodes.ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            var episodes = response.Value;
            var user = Context.User;

            if (episodes.Any())
            {
                embed = string.Format(Strings.SentDM, user.Mention).ToEmbedMessage(EMType.Success).Build();
                var message = await ReplyAsync(embed: embed);

                try
                {
                    var delay = TimeSpan.FromMilliseconds(500);
                    var dmChannel = await TryCreateDMChannelAsync(user);
                    var embeds = episodes.Select(pr => pr.ToEmbed());

                    foreach (var embedIt in embeds)
                    {
                        await dmChannel.SendMessageAsync(embed: embedIt);
                        await _taskManager.Delay(delay);
                    }

                    await (dmChannel as IDMChannel)?.CloseAsync();
                }
                catch (Exception)
                {
                    embed = string.Format(Strings.CouldNotSendDM, user.Mention).ToEmbedMessage(EMType.Error).Build();
                    await message.ModifyAsync(x => x.Embed = embed);
                }
            }
        }

        [Command("anime", RunMode = RunMode.Async)]
        [Alias("bajka")]
        [Summary("wyświetla informacje o anime")]
        [Remarks("Soul Eater")]
        public async Task SearchAnimeAsync([Summary("tytuł")][Remainder] string title) =>
            await SendSearchInfoAsync(Context, title, QuickSearchType.Anime);

        [Command("manga", RunMode = RunMode.Async)]
        [Alias("komiks")]
        [Summary("wyświetla informacje o mandze")]
        [Remarks("Gintama")]
        public async Task SearchMangaAsync(
            [Summary(ParameterInfo.Title)][Remainder] string title) =>
            await SendSearchInfoAsync(Context, title, QuickSearchType.Manga);

        [Command("postać", RunMode = RunMode.Async)]
        [Alias("postac", "character")]
        [Summary("wyświetla informacje o postaci")]
        [Remarks("Gintoki")]
        public async Task SearchCharacterAsync(
            [Summary("imie")][Remainder] string name)
        {
            var discordUserId = Context.User.Id;

            if (_sessionManager.Exists<SearchSession>(discordUserId))
            {
                return;
            }

            var searchResult = await _shindenClient.SearchCharacterAsync(name);

            if (searchResult.Value == null)
            {
                var content = GetResponseFromSearchCode(searchResult.StatusCode)
                    .ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: content);
                return;
            }

            var list = searchResult.Value;
            var toSend = GetSearchResponse(list, Strings.SelectCharacterPrompt);

            if (list.Count == 1)
            {
                var firstCharacter = list.First();
                var characterInfo = (await _shindenClient.GetCharacterInfoAsync(firstCharacter.Id)).Value;

                var embed = new EmbedBuilder()
                {
                    Title = $"{characterInfo} ({characterInfo.CharacterId})".ElipseTrimToLength(EmbedBuilder.MaxTitleLength),
                    Description = characterInfo?.Biography?.Biography?.ElipseTrimToLength(1000),
                    Color = EMType.Info.Color(),
                    ImageUrl = characterInfo.PictureUrl,
                    Fields = characterInfo.GetFields(),
                    Url = characterInfo.CharacterUrl,
                }.Build();

                await ReplyAsync("", false, embed);
                return;
            }

            var messages = await SendMessagesAsync(toSend);

            var session = new SearchSession(
                discordUserId,
                _systemClock.UtcNow,
                messages,
                characterList: list);

            _sessionManager.Add(session);
        }

        [Command("strona", RunMode = RunMode.Async)]
        [Alias("ile", "otaku", "site", "mangozjeb")]
        [Summary("wyświetla statystyki użytkownika z strony")]
        [Remarks("karna")]
        public async Task ShowSiteStatisticAsync(
            [Summary(ParameterInfo.UserOptional)] IGuildUser? guildUser = null)
        {
            var user = guildUser ?? Context.User as IGuildUser;

            if (user == null)
            {
                return;
            }

            var databaseUser = await _userRepository.GetCachedAsync(user.Id);

            if (databaseUser == null)
            {
                await ReplyAsync(embed: Strings.UserDoesNotExistInDatabase.ToEmbedMessage(EMType.Error).Build());
                return;
            }

            var shindenId = databaseUser.ShindenId;

            if (!shindenId.HasValue)
            {
                await ReplyAsync(embed: Strings.UserNotConnectedToShinden.ToEmbedMessage(EMType.Error).Build());
                return;
            }

            using var stream = await GetSiteStatisticAsync(shindenId.Value, user);

            if (stream == null)
            {
                await ReplyAsync(embed: $"Brak połączenia z Shindenem!".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            var profileUrl = UrlHelpers.GetProfileURL(shindenId.Value);

            await Context.Channel.SendFileAsync(stream, $"{user.Id}.png", $"{profileUrl}");
        }

        [Command("połącz")]
        [Alias("connect", "polacz", "połacz", "polącz")]
        [Summary("łączy funkcje bota, z kontem na stronie")]
        [Remarks("https://shinden.pl/user/123-user-1")]
        public async Task ConnectAsync([Summary("adres do profilu")] Uri url)
        {
            Embed embed;

            switch (UrlHelpers.ParseUrlToShindenId(url, out var shindenId))
            {
                case UrlParsingError.InvalidUrlForum:
                    embed = "Wygląda na to, że podałeś link do forum zamiast strony.".ToEmbedMessage(EMType.Error).Build();
                    await ReplyAsync(embed: embed);
                    return;

                default:
                case UrlParsingError.None:
                    break;
            }

            var userResult = await _shindenClient.GetUserInfoAsync(shindenId);

            if (userResult.Value == null)
            {
                embed = $"Wystapil blad podczas polaczenia konta. {userResult.StatusCode}"
                    .ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            var user = userResult.Value;
            var invokingUser = Context.User;
            var userNameInDiscord = (invokingUser as IGuildUser).Nickname ?? invokingUser.Username;

            if (!user.Name.Equals(userNameInDiscord))
            {
                embed = Strings.ImpersonationAttempt.ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            if (await _userRepository.ExistsByShindenIdAsync(shindenId))
            {
                embed = Strings.UserAlreadyConnected.ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            var databaseUser = await _userRepository.GetUserOrCreateAsync(invokingUser.Id);
            databaseUser.ShindenId = shindenId;

            await _userRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(CacheKeys.User(databaseUser.Id));

            embed = Strings.UserConnected.ToEmbedMessage(EMType.Success).Build();
            await ReplyAsync(embed: embed);
        }

        private IEnumerable<string> GetSearchResponse(IEnumerable<object> list, string title)
        {
            var temp = new StringBuilder(2000);
            int messageNr = 0;
            var toSend = new string[10];
            toSend[0] = $"{title}\n```ini\n";
            var index = 0;

            foreach (var item in list)
            {
                temp.AppendFormat("[{0}] {1}\n", ++index, item);
                if (temp.Length > 1800)
                {
                    toSend[messageNr] += "\n```";
                    toSend[++messageNr] += $"```ini\n[{index}] {item}\n";
                    temp.Clear();
                }
                else
                {
                    toSend[messageNr] += $"[{index}] {item}\n";
                }
            }

            toSend[messageNr] += "```\nNapisz `koniec`, aby zamknąć menu.";

            return toSend;
        }

        private async Task SendSearchInfoAsync(ICommandContext context, string title, QuickSearchType type)
        {
            if (title.Equals("fate/loli"))
            {
                title = "Fate/kaleid Liner Prisma Illya";
            }

            var discordUser = context.User;

            if (_sessionManager.Exists<SearchSession>(discordUser.Id))
            {
                return;
            }

            var searchResult = await _shindenClient.QuickSearchAsync(title, type);

            if (searchResult.Value == null)
            {
                await context.Channel.SendMessageAsync("", false, GetResponseFromSearchCode(searchResult.StatusCode)
                    .ToEmbedMessage(EMType.Error).Build());
                return;
            }

            var list = searchResult.Value;
            var toSend = GetSearchResponse(list, "Wybierz tytuł, który chcesz wyświetlić poprzez wpisanie numeru odpowiadającemu mu na liście.");

            if (list.Count == 1)
            {
                var first = list.First();
                var info = (await _shindenClient.GetAnimeMangaInfoAsync(first.TitleId)).Value;
                await context.Channel.SendMessageAsync("", false, info!.ToEmbed());
                return;
            }

            var messages = await SendMessagesAsync(toSend);

            var session = new SearchSession(
                discordUser.Id,
                _systemClock.UtcNow,
                messages,
                animeMangaList: list);

            _sessionManager.Add(session);
        }

        private async Task<IEnumerable<IUserMessage>> SendMessagesAsync(IEnumerable<string?> toSend)
        {
            var messages = new List<IUserMessage>(10);
            var channel = Context.Channel;

            foreach (var item in toSend)
            {
                if (item == null)
                {
                    continue;
                }

                var message = await channel.SendMessageAsync(item);
                messages.Add(message);
            }

            return messages;
        }

        private string GetResponseFromSearchCode(HttpStatusCode code)
        {
            switch (code)
            {
                case HttpStatusCode.NotFound:
                    return Strings.NoItems;

                default:
                    return string.Format(Strings.CouldNotConnectToShinden, code);
            }
        }

        private async Task<Stream?> GetSiteStatisticAsync(ulong shindenUserId, IGuildUser user)
        {
            var result = await _shindenClient.GetUserInfoAsync(shindenUserId);

            if (result.Value == null)
            {
                return null;
            }

            var shindenUser = result.Value;
            var resLR = await _shindenClient.GetLastReadAsync(shindenUserId);
            var resLW = await _shindenClient.GetLastWatchedAsync(shindenUserId);

            var color = user.Guild.Roles
                .Join(user.RoleIds, pr => pr.Id, pr => pr, (src, dst) => src)
                .OrderByDescending(pr => pr.Position)
                .FirstOrDefault()?.Color ?? Discord.Color.DarkerGrey;

            using var image = await _imageProcessor.GetSiteStatisticAsync(
                shindenUser,
                color,
                resLR.Value,
                resLW.Value);

            return image.ToPngStream();
        }
    }
}