using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Microsoft.Extensions.DependencyInjection;
using Sanakan.DAL.Models;
using Sanakan.DiscordBot.Abstractions;
using Sanakan.DiscordBot.Abstractions.Configuration;
using Sanakan.Extensions;

namespace Sanakan.DiscordBot.Session
{
    public class ListSession<T> : InteractionSession
    {
        private readonly ListSessionPayload _payload;
        private static readonly StringBuilder _stringBuilder = new StringBuilder(500);
        private IIconConfiguration _iconConfiguration = null;

        public class ListSessionPayload
        {
            public bool Enumerable { get; set; }

            public IMessage Message { get; set; } = null;

            public int ItemsPerPage { get; set; } = 10;

            public List<T> ListItems { get; set; } = null;

            public EmbedBuilder Embed { get; set; } = null;

            public int CurrentPage { get; set; }

            public IUser Bot { get; set; } = null;
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

        public string CardToString(Card card)
        {
            var marks = new[]
            {
                card.InCage ? "[C]" : "",
                card.Active ? "[A]" : "",
                card.IsUnique ? (card.FromFigure ? "[F]" : "[U]") : "",
                card.Expedition != ExpeditionCardType.None ? "[W]" : "",
                card.IsBroken ? "[B]" : (card.IsUnusable ? "[N]" : ""),
            };

            var mark = marks.Any(x => x != "") ? $"**{string.Join("", marks)}** " : "";
            return $"{mark}{card.GetString(false, false, true)}";
        }

        public Embed BuildPage(int page)
        {
            int firstItem = page * _payload.ItemsPerPage;
            int lastItem = (_payload.ItemsPerPage - 1) + (page * _payload.ItemsPerPage);
            var exceededList = lastItem >= _payload.ListItems.Count;
            var embed = _payload.Embed;

            var footerText = $"{_payload.CurrentPage + 1} z {MaxPage()}";
            embed.Footer = new EmbedFooterBuilder().WithText(footerText);
            var count = exceededList ? (_payload.ListItems.Count - firstItem) : _payload.ItemsPerPage;
            var itemsOnPage = _payload.ListItems
                .GetRange(firstItem, count);

            lock (_stringBuilder)
            {
                for (var i = 0; i < itemsOnPage.Count; i++)
                {
                    var item = itemsOnPage[i];
                    var formattedItem = string.Empty;

                    if (item is Card card)
                    {
                        formattedItem = CardToString(card);
                    }
                    else
                    {
                        formattedItem = item.ToString();
                    }

                    if (_payload.Enumerable)
                    {
                        var index = (i + 1) + (page * _payload.ItemsPerPage);
                        _stringBuilder.AppendFormat("**{0}**: {1}\n", index, formattedItem);
                    }
                    else
                    {
                        _stringBuilder.AppendFormat("{0}\n", formattedItem);
                    }
                }

                embed.Description = _stringBuilder.ToString().ElipseTrimToLength(1800);
                _stringBuilder.Clear();
            }

            return embed.Build();
        }

        private int MaxPage()
        {
            var listItems = _payload.ListItems;
            var count = listItems.Count;

            return ((count % 10) == 0) ? (count / 10) : ((count / 10) + 1);
        }

        private int MaxPageReal() => MaxPage() - 1;

        public override async Task ExecuteAsync(
            SessionContext context,
            IServiceProvider serviceProvider,
            CancellationToken cancellationToken = default)
        {
            var message = _payload.Message;
            _iconConfiguration = serviceProvider.GetRequiredService<IIconConfiguration>();

            if (context.Message.Id != message.Id)
            {
                return;
            }

            var userMessage = await message.Channel.GetMessageAsync(message.Id) as IUserMessage;

            if (userMessage == null)
            {
                return;
            }

            var reaction = context.AddReaction ?? context.RemoveReaction;
            var emote = reaction.Emote;

            if (emote.Equals(_iconConfiguration.LeftwardsArrow))
            {
                if (--_payload.CurrentPage < 0)
                {
                    _payload.CurrentPage = MaxPageReal();
                }

                var embed = BuildPage(_payload.CurrentPage);
                await userMessage.ModifyAsync(x => x.Embed = embed);

                ResetExpiry();
                return;
            }

            if (emote.Equals(_iconConfiguration.RightwardsArrow))
            {
                if (++_payload.CurrentPage > MaxPageReal())
                {
                    _payload.CurrentPage = 0;
                }

                var embed = BuildPage(_payload.CurrentPage);
                await userMessage.ModifyAsync(x => x.Embed = embed);

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
                await userMessage.RemoveReactionsAsync(_payload.Bot, _iconConfiguration.LeftRightArrows);
            }

            _payload.Message = null;
            _payload.ListItems = null;
        }
    }
}