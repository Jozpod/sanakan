using Discord;
using Microsoft.Extensions.DependencyInjection;
using Sanakan.Extensions;
using Sanakan.ShindenApi;
using Sanakan.ShindenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Sanakan.DiscordBot.Session
{
    public class SearchSession : InteractionSession
    {
        private readonly SearchSessionPayload _payload;
        public class SearchSessionPayload
        {
            public IEnumerable<IMessage> Messages { get; set; } = Enumerable.Empty<IMessage>();
            public List<QuickSearchResult> AnimeMangaList { get; set; } = new();
            public List<CharacterSearchResult> CharacterList { get; set; } = new();
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
            var message = sessionContext.Message;
            var content = message?.Content;

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
            var channel = sessionContext.Channel;

            if (_payload.AnimeMangaList.Any())
            {
                var list = _payload.AnimeMangaList;
                if (number > 0 && list.Count >= number)
                {
                    var parameter = list[number - 1];
                    var animeMangaInfo = (await shindenClient.GetAnimeMangaInfoAsync(parameter.TitleId)).Value;
                    await channel.SendMessageAsync("", false, animeMangaInfo!.ToEmbed());
                    await message.DeleteAsync();
                    return;
                }
            }
            if (_payload.CharacterList.Any())
            {
                var list = _payload.CharacterList;
                if (number > 0 && list.Count >= number)
                {
                    var person = list[number - 1];
                    var characterInfo = (await shindenClient.GetCharacterInfoAsync(person.Id)).Value;

                    await channel.SendMessageAsync("", false, characterInfo!.ToEmbed());
                    await message.DeleteAsync();
                    return;
                }
            }

            return;
        }

        public override async ValueTask DisposeAsync()
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

            _payload.AnimeMangaList = null;
            _payload.CharacterList = null;
        }
    }
}