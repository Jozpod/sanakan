using Discord.Commands;
using Discord.WebSocket;
using Sanakan.Extensions;
using Sanakan.Preconditions;
using System;
using System.Linq;
using System.Threading.Tasks;
using Sanakan.Common;
using Sanakan.ShindenApi;
using Sanakan.DAL.Repositories.Abstractions;
using Discord;
using Sanakan.ShindenApi.Utilities;
using Sanakan.DiscordBot.Abstractions.Models;
using Sanakan.DiscordBot.Abstractions.Extensions;
using System.Net;
using System.Collections.Generic;
using System.IO;
using Sanakan.ShindenApi.Models.Enums;
using Sanakan.Game.Services.Abstractions;
using System.Text;
using Sanakan.Common.Cache;
using Sanakan.DiscordBot.Session;
using Microsoft.Extensions.DependencyInjection;
using Sanakan.DiscordBot.Resources;

namespace Sanakan.DiscordBot.Modules
{
    [Name("Shinden"), RequireUserRole]
    public class ShindenModule : SanakanModuleBase
    {
        public enum UrlParsingError
        {
            None,
            InvalidUrl,
            InvalidUrlForum
        }

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

            if (response.Value == null)
            {
                await ReplyAsync(embed: "Nie udało się pobrać listy odcinków.".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            var episodes = response.Value;
            var user = Context.User;

            if (episodes?.Count > 0)
            {
                var message = await ReplyAsync(embed: "Lista poszła na PW!".ToEmbedMessage(EMType.Success).Build());

                try
                {
                    var dmChannel = await user.GetOrCreateDMChannelAsync();
                    foreach (var episode in episodes)
                    {
                        var embed = episode.ToEmbed();
                        await dmChannel.SendMessageAsync("", false, embed);
                        await _taskManager.Delay(TimeSpan.FromMilliseconds(500));
                    }

                    await dmChannel.CloseAsync();
                }
                catch (Exception ex)
                {
                    await message.ModifyAsync(x => x.Embed = $"{user.Mention} nie udało się wyłać PW! ({ex.Message})"
                        .ToEmbedMessage(EMType.Error).Build());
                }

                return;
            }
        }

        [Command("anime", RunMode = RunMode.Async)]
        [Alias("bajka")]
        [Summary("wyświetla informacje o anime")]
        [Remarks("Soul Eater")]
        public async Task SearchAnimeAsync([Summary("tytuł")][Remainder]string title)
        {
            await SendSearchInfoAsync(Context, title, QuickSearchType.Anime);
        }

        [Command("manga", RunMode = RunMode.Async)]
        [Alias("komiks")]
        [Summary("wyświetla informacje o mandze")]
        [Remarks("Gintama")]
        public async Task SearchMangaAsync(
            [Summary("tytuł")][Remainder]string title)
        {
            await SendSearchInfoAsync(Context, title, QuickSearchType.Manga);
        }

        [Command("postać", RunMode = RunMode.Async)]
        [Alias("postac", "character")]
        [Summary("wyświetla informacje o postaci")]
        [Remarks("Gintoki")]
        public async Task SearchCharacterAsync(
            [Summary("imie")][Remainder]string name)
        {
            var discordUserId = Context.User.Id;
            var payload = new SearchSession.SearchSessionPayload();

            var session = new SearchSession(discordUserId, _systemClock.UtcNow, payload);
            
            if (_sessionManager.Exists<SearchSession>(discordUserId))
            {
                return;
            }

            var searchResult = await _shindenClient.SearchCharacterAsync(name);

            if (searchResult.Value == null)
            {
                var content = GetResponseFromSearchCode(System.Net.HttpStatusCode.BadRequest)
                    .ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: content);
                return;
            }

            var list = searchResult.Value;
            var toSend = GetSearchResponse(list, "Wybierz postać, którą chcesz wyświetlić poprzez wpisanie numeru odpowiadającemu jej na liście.");

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
            }
            else
            {
                payload.PList = list;
                await SendSearchResponseAsync(Context, toSend, session, payload);
            }
        }

        [Command("strona", RunMode = RunMode.Async)]
        [Alias("ile", "otaku", "site", "mangozjeb")]
        [Summary("wyświetla statystyki użytkownika z strony")]
        [Remarks("karna")]
        public async Task ShowSiteStatisticAsync(
            [Summary("użytkownik (opcjonalne)")]IGuildUser? guildUser = null)
        {
            var user = guildUser ?? Context.User as IGuildUser;

            if (user == null)
            {
                return;
            }

            var databaseUser = await _userRepository.GetCachedFullUserAsync(user.Id);

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
        [Remarks("https://shinden.pl/user/136-mo0nisi44")]
        public async Task ConnectAsync([Summary("adres do profilu")]string url)
        {
            switch (ParseUrlToShindenId(url, out var shindenId))
            {
                case UrlParsingError.InvalidUrl:
                    await ReplyAsync(embed: "Wygląda na to, że podałeś niepoprawny link.".ToEmbedMessage(EMType.Error).Build());
                    return;

                case UrlParsingError.InvalidUrlForum:
                    await ReplyAsync(embed: "Wygląda na to, że podałeś link do forum zamiast strony.".ToEmbedMessage(EMType.Error).Build());
                    return;

                default:
                case UrlParsingError.None:
                    break;
            }

            var userResult = await _shindenClient.GetUserInfoAsync(shindenId);

            if (userResult.Value == null)
            {
                await ReplyAsync(embed: $"Wystapil blad podczas polaczenia konta".ToEmbedMessage(EMType.Error).Build());
                //await ReplyAsync(embed: $"Brak połączenia z Shindenem! ({response.Code})".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            var user = userResult.Value;
            var userNameInDiscord = (Context.User as IGuildUser).Nickname ?? Context.User.Username;

            if (!user.Name.Equals(userNameInDiscord))
            {
                await ReplyAsync(embed: "Wykryto próbę podszycia się. Nieładnie!".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            if (await _userRepository.ExistsByShindenIdAsync(shindenId))
            {
                await ReplyAsync(embed: "Wygląda na to, że ktoś już połączył się z tym kontem.".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            var databaseUser = await _userRepository.GetUserOrCreateAsync(Context.User.Id);
            databaseUser.ShindenId = shindenId;

            await _userRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(CacheKeys.User(databaseUser.Id));

            await ReplyAsync(embed: "Konta zostały połączone.".ToEmbedMessage(EMType.Success).Build());
            return;
            
        }

        private UrlParsingError ParseUrlToShindenId(string url, out ulong shindenId)
        {
            shindenId = 0;
            var splited = url.Split('/');
            bool http = splited[0].Equals("https:") || splited[0].Equals("http:");
            int toChek = http ? 2 : 0;

            if (splited.Length < (toChek == 2 ? 5 : 3))
            {
                return UrlParsingError.InvalidUrl;
            }

            if (splited[toChek].Equals("shinden.pl") || splited[toChek].Equals("www.shinden.pl"))
            {
                if (splited[++toChek].Equals("user") || splited[toChek].Equals("animelist") || splited[toChek].Equals("mangalist"))
                {
                    var data = splited[++toChek].Split('-');
                    if (ulong.TryParse(data[0], out shindenId))
                    {
                        return UrlParsingError.None;
                    }
                }
            }

            if (splited[toChek].Equals("forum.shinden.pl")
                || splited[toChek].Equals("www.forum.shinden.pl"))
            {
                return UrlParsingError.InvalidUrlForum;
            }

            return UrlParsingError.InvalidUrl;
        }

        private string[] GetSearchResponse(IEnumerable<object> list, string title)
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

            var payload = new SearchSession.SearchSessionPayload();

            var session = new SearchSession(discordUser.Id, _systemClock.UtcNow, payload);

            if (_sessionManager.Exists<SearchSession>(discordUser.Id))
            {
                return;
            }

            var searchResult = await _shindenClient.QuickSearchAsync(title, type);

            if (searchResult.Value == null)
            {
                await context.Channel.SendMessageAsync("", false, GetResponseFromSearchCode(HttpStatusCode.BadRequest)
                    .ToEmbedMessage(EMType.Error).Build());
                return;
            }

            var list = searchResult.Value;
            var toSend = GetSearchResponse(list, "Wybierz tytuł, który chcesz wyświetlić poprzez wpisanie numeru odpowiadającemu mu na liście.");

            if (list.Count == 1)
            {
                var first = list.First();
                var info = (await _shindenClient.GetAnimeMangaInfoAsync(first.TitleId)).Value;
                await context.Channel.SendMessageAsync("", false, info.ToEmbed());
            }
            else
            {
                payload.SList = list;
                await SendSearchResponseAsync(context, toSend, session, payload);
            }
        }

        private async Task SendSearchResponseAsync(
            ICommandContext context,
            IEnumerable<string?> toSend,
            SearchSession session,
            SearchSession.SearchSessionPayload payload)
        {
            var messages = new List<IUserMessage>(10);
            foreach (var item in toSend)
            {
                if (item == null)
                {
                    continue;

                }

                var message = await context.Channel.SendMessageAsync(item);
                messages.Add(message);
            }

            payload.Messages = messages;
            _sessionManager.Add(session);
        }

        private string GetResponseFromSearchCode(HttpStatusCode code)
        {
            switch (code)
            {
                case HttpStatusCode.NotFound:
                    return "Brak wyników!";

                default:
                    return $"Brak połączenia z Shindenem! ({code})";
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