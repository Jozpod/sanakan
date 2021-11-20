using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Sanakan.DiscordBot.Abstractions;
using Sanakan.Extensions;

namespace Sanakan.DiscordBot.Session
{
    public class ListSession<T> : InteractionSession
    {
        private readonly ListSessionPayload _payload;
        public class ListSessionPayload
        {
            public bool Enumerable { get; set; }
            public IMessage Message { get; set; }
            public int ItemsPerPage { get; set; } = 10;
            public List<T> ListItems { get; set; }
            public EmbedBuilder Embed { get; set; }
            public int CurrentPage { get; set; }
            public IUser Bot { get; set; }
        }

        public ListSession(
            ulong ownerId,
            DateTime createdOn,
            ListSessionPayload payload,
            SessionExecuteCondition executeCondition = SessionExecuteCondition.AllReactions) : base(
            ownerId,
            createdOn,
            TimeSpan.FromMinutes(1),
            Discord.Commands.RunMode.Sync,
            executeCondition)
        {
            _payload = payload;
        }

        public Embed BuildPage(int page)
        {
            int firstItem = page * _payload.ItemsPerPage;
            int lastItem = (_payload.ItemsPerPage - 1) + (page * _payload.ItemsPerPage);
            bool toMuch = lastItem >= _payload.ListItems.Count;

            _payload.Embed.Footer = new EmbedFooterBuilder().WithText($"{_payload.CurrentPage + 1} z {MaxPage()}");
            var itemsOnPage = _payload.ListItems.GetRange(firstItem, toMuch ? (_payload.ListItems.Count - firstItem) : _payload.ItemsPerPage);

            string pageString = "";
            for (int i = 0; i < itemsOnPage.Count; i++)
            {
                string enumerable = _payload.Enumerable ? $"**{(i + 1) + (page * _payload.ItemsPerPage)}**: " : "";
                pageString += $"{enumerable}{itemsOnPage[i]}\n";
            }

            _payload.Embed.Description = pageString.ElipseTrimToLength(1800);

            return _payload.Embed.Build();
        }

        private int MaxPage() => (((_payload.ListItems.Count % 10) == 0) ? (_payload.ListItems.Count / 10) : ((_payload.ListItems.Count / 10) + 1));

        private int MaxPageReal() => MaxPage() - 1;

        public override async Task ExecuteAsync(
            SessionContext context,
            IServiceProvider serviceProvider,
            CancellationToken cancellationToken = default)
        {
            if (context.Message.Id != _payload.Message.Id)
            {
                return;
            }

            var userMessage = await _payload.Message.Channel.GetMessageAsync(_payload.Message.Id) as IUserMessage;

            if (userMessage == null)
            {
                return;
            }

            var reaction = context.AddReaction ?? context.RemoveReaction;

            if (reaction.Emote.Equals(Emojis.LeftwardsArrow))
            {
                if (--_payload.CurrentPage < 0)
                {
                    _payload.CurrentPage = MaxPageReal();
                }
                await userMessage.ModifyAsync(x => x.Embed = BuildPage(_payload.CurrentPage));

                ResetExpiry();
                return;
            }
            
            if (reaction.Emote.Equals(Emojis.RightwardsArrow))
            {
                if (++_payload.CurrentPage > MaxPageReal())
                {
                    _payload.CurrentPage = 0;
                }

                await userMessage.ModifyAsync(x => x.Embed = BuildPage(_payload.CurrentPage));

                ResetExpiry();
            }
        }

        public override async ValueTask DisposeAsync()
        {
            if (_payload.Message == null)
            {
                return;
            }

            var userMessage = await _payload.Message.Channel.GetMessageAsync(_payload.Message.Id) as IUserMessage;

            if (userMessage == null)
            {
                _payload.Message = null;
                _payload.ListItems = null;
                return;
            }

            try
            {
                await userMessage.RemoveAllReactionsAsync();
            }
            catch (Exception)
            {
                var reactions = new IEmote[] {
                    Emojis.LeftwardsArrow,
                    Emojis.RightwardsArrow
                };

                await userMessage.RemoveReactionsAsync(_payload.Bot, reactions);
            }

            _payload.Message = null;
            _payload.ListItems = null;
        }
    }
}