using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using Sanakan.DiscordBot.Abstractions.Extensions;
using Sanakan.DiscordBot.Abstractions.Models;
using Sanakan.DiscordBot.Services;
using Sanakan.Extensions;
using Sanakan.Game.Services;
using Sanakan.Services.Session;
using Sanakan.Services.Session.Models;
using Sanakan.ShindenApi;
using Sanakan.TaskQueue;
using Shinden;

namespace Sanakan.Services
{
    public enum UrlParsingError
    {
        None,
        InvalidUrl,
        InvalidUrlForum
    }

    public class Shinden
    {
        private readonly IShindenClient _shindenClient;
        private readonly ISessionManager _session;
        private readonly IImageProcessor _imageProcessor;

        public Shinden(
            IShindenClient client,
            ISessionManager session,
            IImageProcessor imageProcessor)
        {
            _shindenClient = client;
            _session = session;
            _imageProcessor = imageProcessor;
        }

        public UrlParsingError ParseUrlToShindenId(string url, out ulong shindenId)
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
                if(splited[++toChek].Equals("user") || splited[toChek].Equals("animelist") || splited[toChek].Equals("mangalist"))
                {
                    var data = splited[++toChek].Split('-');
                    if (ulong.TryParse(data[0], out shindenId))
                    {
                        return UrlParsingError.None;
                    }
                }
            }

            if (splited[toChek].Equals("forum.shinden.pl") || splited[toChek].Equals("www.forum.shinden.pl"))
                return UrlParsingError.InvalidUrlForum;

            return UrlParsingError.InvalidUrl;
        }

        public string[] GetSearchResponse(IEnumerable<object> list, string title)
        {
            string temp = "";
            int messageNr = 0;
            var toSend = new string[10];
            toSend[0] = $"{title}\n```ini\n";
            int i = 0;

            foreach (var item in list)
            {
                temp += $"[{++i}] {item}\n";
                if (temp.Length > 1800)
                {
                    toSend[messageNr] += "\n```";
                    toSend[++messageNr] += $"```ini\n[{i}] {item}\n";
                    temp = "";
                }
                else toSend[messageNr] += $"[{i}] {item}\n";
            }
            toSend[messageNr] += "```\nNapisz `koniec`, aby zamknąć menu.";

            return toSend;
        }

        public async Task SendSearchInfoAsync(SocketCommandContext context, string title, QuickSearchType type)
        {
            if (title.Equals("fate/loli")) title = "Fate/kaleid Liner Prisma Illya";

            var session = new SearchSession(context.User, _shindenClient);
            
            if (_session.SessionExist(session))
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
                session.SList = list;
                await SendSearchResponseAsync(context, toSend, session);
            }
        }

        public async Task SendSearchResponseAsync(SocketCommandContext context, string[] toSend, SearchSession session)
        {
            var msg = new Discord.Rest.RestUserMessage[10];
            for (int index = 0; index < toSend.Length; index++)
                if (toSend[index] != null) msg[index] = await context.Channel.SendMessageAsync(toSend[index]);

            session.Messages = msg;
            await _session.TryAddSession(session);
        }

        public string GetResponseFromSearchCode(HttpStatusCode code)
        {
            switch (code)
            {
                case HttpStatusCode.NotFound:
                    return "Brak wyników!";

                default:
                    return $"Brak połączenia z Shindenem! ({code})";
            }
        }

        public async Task<Stream> GetSiteStatisticAsync(ulong shindenId, SocketGuildUser user)
        {
            var result = await _shindenClient.GetUserInfoAsync(shindenId);
            
            if (result.Value == null)
            {
                return null;
            }

            var shindenUser = result.Value;
            var resLR = await _shindenClient.GetLastReadedAsync(shindenId);
            var resLW = await _shindenClient.GetLastWatchedAsync(shindenId);
            var color = user.Roles.OrderByDescending(x => x.Position)
                .FirstOrDefault()?.Color ?? Discord.Color.DarkerGrey;

            using var image = await _imageProcessor.GetSiteStatisticAsync(
                shindenUser,
                color,
                null, //resLR.Value != null ? resLR.Value : null,
                null); //resLW.Value != null ? resLW.Value : null);

            return image.ToPngStream();
        }
    }
}