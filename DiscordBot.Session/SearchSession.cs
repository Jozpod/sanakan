using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Microsoft.Extensions.DependencyInjection;
using Sanakan.Extensions;
using Sanakan.ShindenApi;
using Sanakan.ShindenApi.Models;

namespace Sanakan.DiscordBot.Session
{
    public class SearchSession : InteractionSession
    {
        private readonly SearchSessionPayload _payload;
        public class SearchSessionPayload
        {
            public IEnumerable<IMessage> Messages { get; set; }
            public List<QuickSearchResult> SList { get; set; }
            public List<CharacterSearchResult> PList { get; set; }
        }

        public SearchSession(
            ulong ownerId,
            DateTime createdOn,
            SearchSessionPayload payload) : base(
            ownerId,
            createdOn,
            TimeSpan.FromMinutes(2),
            Discord.Commands.RunMode.Async,
            SessionExecuteCondition.AllEvents)
        {
            _payload = payload;
        }

        public override async Task ExecuteAsync(
          SessionContext sessionContext,
          IServiceProvider serviceProvider,
          CancellationToken cancellationToken = default)
        {
            var content = sessionContext.Message?.Content;

            if (content == null)
            {
                return;
            }

            if (content.ToLower() == "koniec")
            {
                return;
            }

            if (!int.TryParse(content, out var number))
            {
                return;
            }

            var shindenClient = serviceProvider.GetRequiredService<IShindenClient>();

            if (_payload.SList != null)
            {
                if (number > 0 && _payload.SList.Count >= number)
                {
                    var parameter = _payload.SList[number - 1];
                    var animeMangaInfo = (await shindenClient.GetAnimeMangaInfoAsync(parameter.TitleId)).Value;
                    await sessionContext.Channel.SendMessageAsync("", false, animeMangaInfo.ToEmbed());
                    await sessionContext.Message.DeleteAsync();
                    return;
                }
            }
            if (_payload.PList != null)
            {
                if (number > 0 && _payload.PList.Count >= number)
                {
                    var person = _payload.PList.ToArray()[number - 1];
                    var characterInfo = (await shindenClient.GetCharacterInfoAsync(person.Id)).Value;

                    await sessionContext.Channel.SendMessageAsync("", false, characterInfo.ToEmbed());
                    await sessionContext.Message.DeleteAsync();
                    return;
                }
            }

            return;
        }

        private async Task DisposeAction()
        {
            if (_payload.Messages == null)
            {
                return;
            }

            foreach (var message in _payload.Messages)
            {
                var discordMessage = await message.Channel.GetMessageAsync(message.Id);
                
                if (discordMessage != null)
                {
                    await discordMessage.DeleteAsync();
                }
            }

            _payload.Messages = null;

            _payload.SList = null;
            _payload.PList = null;
        }
    }
}