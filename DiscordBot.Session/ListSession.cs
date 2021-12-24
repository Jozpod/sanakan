using Discord;
using Microsoft.Extensions.DependencyInjection;
using Sanakan.DAL.Models;
using Sanakan.DiscordBot.Abstractions.Configuration;
using Sanakan.DiscordBot.Abstractions.Models;
using Sanakan.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sanakan.DiscordBot.Session
{
    public static class ListSessionUtils
    {
        public static Embed BuildPage(
           EmbedBuilder embedBuilder,
           IEnumerable<string> items,
           int currentPage = 0,
           int itemsPerPage = 10,
           bool isEnumerable = false)
        {
            var itemsCount = items.Count();
            var maxPage = ((itemsCount % itemsPerPage) == 0) ?
                (itemsCount / itemsPerPage) : ((itemsCount / itemsPerPage) + 1);

            var stringBuilder = new StringBuilder(500);
            int firstItem = currentPage * itemsPerPage;
            int lastItem = itemsPerPage - 1 + (currentPage * itemsPerPage);

            var exceededList = lastItem >= itemsCount;

            var footerText = $"{currentPage + 1} z {maxPage}";
            embedBuilder.Footer = new EmbedFooterBuilder().WithText(footerText);
            var count = exceededList ? (itemsCount - firstItem) : itemsPerPage;
            var itemsOnPage = items.Skip(firstItem).Take(count);
            var index = 0;

            foreach (var item in itemsOnPage)
            {
                if (isEnumerable)
                {
                    var itemPosition = index + 1 + (currentPage * itemsPerPage);
                    stringBuilder.AppendFormat("**{0}**: {1}\n", itemPosition, item);
                }
                else
                {
                    stringBuilder.AppendFormat("{0}\n", item);
                }

                index++;
            }

            embedBuilder.Description = stringBuilder.ToString().ElipseTrimToLength(1800);
            stringBuilder.Clear();

            return embedBuilder.Build();
        }

        public static int MaxPage<T>(
            IEnumerable<T> items,
            int itemsPerPage)
        {
            var count = items.Count();

            return ((count % itemsPerPage) == 0) ? (count / itemsPerPage) : ((count / itemsPerPage) + 1);
        }
    }

    public class ListSession : InteractionSession
    {
        private IIconConfiguration _iconConfiguration = null;
        private bool _isEnumerable;
        private readonly int _itemsPerPage;
        private IEnumerable<string> _items;
        private readonly EmbedBuilder _embedBuilder;
        private int _currentPage;
        private readonly IUser _bot;
        private readonly IUserMessage _userMessage;

        public ListSession(
            ulong ownerId,
            DateTime createdOn,
            IEnumerable<string> items,
            IUser bot,
            IUserMessage userMessage,
            EmbedBuilder embedBuilder,
            int itemsPerPage = 10,
            bool isEnumerable = false,
            SessionExecuteCondition executeCondition = SessionExecuteCondition.AllReactions) : base(
            ownerId,
            createdOn,
            TimeSpan.FromMinutes(1),
            Discord.Commands.RunMode.Sync,
            executeCondition)
        {
            _items = items;
            _bot = bot;
            _currentPage = 0;
            _itemsPerPage = itemsPerPage;
            _isEnumerable = isEnumerable;
            _userMessage = userMessage;
            _embedBuilder = embedBuilder;
        }

        private int MaxPageReal() => ListSessionUtils.MaxPage(_items, _itemsPerPage) - 1;

        public override async Task ExecuteAsync(
            SessionContext context,
            IServiceProvider serviceProvider,
            CancellationToken cancellationToken = default)
        {
            if (context.Message.Id != _userMessage.Id)
            {
                return;
            }

            _iconConfiguration = serviceProvider.GetRequiredService<IIconConfiguration>();
            var reaction = context.AddReaction ?? context.RemoveReaction;
            var emote = reaction.Emote;

            if (emote.Equals(_iconConfiguration.LeftwardsArrow))
            {
                if (--_currentPage < 0)
                {
                    _currentPage = MaxPageReal();
                }

                var embed = ListSessionUtils.BuildPage(_embedBuilder, _items, _currentPage, _itemsPerPage, _isEnumerable);
                await _userMessage.ModifyAsync(x => x.Embed = embed);

                ResetExpiry();
                return;
            }

            if (emote.Equals(_iconConfiguration.RightwardsArrow))
            {
                if (++_currentPage > MaxPageReal())
                {
                    _currentPage = 0;
                }

                var embed = ListSessionUtils.BuildPage(_embedBuilder, _items, _currentPage, _itemsPerPage, _isEnumerable);
                await _userMessage.ModifyAsync(x => x.Embed = embed);

                ResetExpiry();
            }
        }

        public override async ValueTask DisposeAsync()
        {
            try
            {
                await _userMessage.RemoveAllReactionsAsync();
            }
            catch (Exception)
            {
                _iconConfiguration ??= ServiceProvider.GetRequiredService<IIconConfiguration>();

                await _userMessage.RemoveReactionsAsync(_bot, _iconConfiguration.LeftRightArrows);
            }

            _iconConfiguration = null;
            _items = null;
        }
    }
}