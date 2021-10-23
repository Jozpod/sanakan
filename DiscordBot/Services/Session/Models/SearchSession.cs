using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using DiscordBot.Services.Session;
using Sanakan.Extensions;
using Sanakan.ShindenApi;
using Shinden;
using Shinden.API;
using Shinden.Models;

namespace Sanakan.Services.Session.Models
{
    public class SearchSession : Session
    {
        public IMessage[] Messages { get; set; }
        public List<QuickSearchResult> SList { get; set; }
        public List<CharacterSearchResult> PList { get; set; }
        public readonly IShindenClient _shindenClient;

        public SearchSession(
            IUser owner,
            IShindenClient client) : base(owner)
        {
            Event = ExecuteOn.Message;
            RunMode = RunMode.Async;
            _shindenClient = client;
            TimeoutMs = 120000;

            Messages = null;
            SList = null;
            PList = null;

            OnExecute = ExecuteAction;
            OnDispose = DisposeAction;
        }

        private async Task<bool> ExecuteAction(SessionContext context, Session session)
        {
            var content = context.Message?.Content;

            if (content == null) {
                return false;
            }

            if (content.ToLower() == "koniec")
            {
                return true;
            }

            if (int.TryParse(content, out int number))
            {
                if (SList != null)
                {
                    if (number > 0 && SList.Count >= number)
                    {
                        var parameter = SList[number - 1];
                        var animeMangaInfo = (await _shindenClient.GetAnimeMangaInfoAsync(parameter.TitleId)).Value;
                        await context.Channel.SendMessageAsync("", false, animeMangaInfo.ToEmbed());
                        await context.Message.DeleteAsync();
                        return true;
                    }
                }
                if (PList != null)
                {
                    if (number > 0 && PList.Count >= number)
                    {
                        var person = PList.ToArray()[number - 1];
                        var characterInfo = (await _shindenClient.GetCharacterInfoAsync(person.Id)).Value;

                        await context.Channel.SendMessageAsync("", false, characterInfo.ToEmbed());
                        await context.Message.DeleteAsync();
                        return true;
                    }
                }
            }

            return false;
        }

        private async Task DisposeAction()
        {
            if (Messages != null)
            {
                foreach (var message in Messages)
                {
                    var msg = await message.Channel.GetMessageAsync(message.Id);
                    if (msg != null) await msg.DeleteAsync();
                }

                Messages = null;
            }

            SList = null;
            PList = null;
        }
    }
}