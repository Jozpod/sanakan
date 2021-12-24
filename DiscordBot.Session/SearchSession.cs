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
        private readonly IEnumerable<IMessage> _messages;
        private readonly List<QuickSearchResult> _animeMangaList;
        private readonly List<CharacterSearchResult> _characterList;

        public SearchSession(
            ulong ownerId,
            DateTime createdOn,
            IEnumerable<IMessage> messages,
            List<QuickSearchResult> animeMangaList = null,
            List<CharacterSearchResult> characterList = null) : base(
            ownerId,
            createdOn,
            TimeSpan.FromMinutes(2),
            Discord.Commands.RunMode.Async,
            SessionExecuteCondition.AllEvents)
        {
            _messages = messages;
            _animeMangaList = animeMangaList ?? new();
            _characterList = characterList ?? new();
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

            if (_animeMangaList.Any())
            {
                var list = _animeMangaList;
                if (number > 0 && list.Count >= number)
                {
                    var parameter = list[number - 1];
                    var animeMangaInfo = (await shindenClient.GetAnimeMangaInfoAsync(parameter.TitleId)).Value;
                    await channel.SendMessageAsync("", false, animeMangaInfo!.ToEmbed());
                    await message.DeleteAsync();
                    return;
                }
            }

            if (_characterList.Any())
            {
                var list = _characterList;
                if (number > 0 && list.Count >= number)
                {
                    var person = list[number - 1];
                    var characterInfo = (await shindenClient.GetCharacterInfoAsync(person.Id)).Value;

                    await channel.SendMessageAsync("", false, characterInfo!.ToEmbed());
                    await message.DeleteAsync();
                    return;
                }
            }

        }

        public override async ValueTask DisposeAsync()
        {
            foreach (var message in _messages)
            {
                var channelMessage = await message.Channel.GetMessageAsync(message.Id);

                if (channelMessage != null)
                {
                    await channelMessage.DeleteAsync();
                }
            }
        }
    }
}