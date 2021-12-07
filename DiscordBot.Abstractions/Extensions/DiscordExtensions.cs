using System.Collections.Generic;
using System.Linq;
using Discord;
using Discord.WebSocket;
using Sanakan.Common;
using Sanakan.DiscordBot.Abstractions.Models;

namespace Sanakan.DiscordBot.Abstractions.Extensions
{
    public static class DiscordExtensions
    {
        public static EmbedBuilder ToEmbedMessage(
            this string message,
            EMType type = EMType.Neutral,
            bool icon = false)
        {
            return new EmbedBuilder().WithColor(type.Color())
                .WithDescription($"{type.Emoji(!icon)}{message}");
        }

        public static string GetLocalCreatedAtShortDateTime(this IMessage message)
           => message.CreatedAt.DateTime.ToLocalTime().ToString("dd/MM/yyyy HH:mm");

        public static int CountEmotesTextLength(this IReadOnlyCollection<Discord.ITag> tags)
        {
            return tags.Where(tag => tag.Type == TagType.Emoji).Sum(x => x.Value.ToString().Length);
        }

        public static string GetUserOrDefaultAvatarUrl(this IUser user)
        {
            var avatar = user.GetAvatarUrl() ?? user.GetDefaultAvatarUrl();
            return avatar.Split("?")[0];
        }

        public static EmbedBuilder WithUser(this EmbedBuilder builder, IUser user, bool includeId = false)
        {
            return builder.WithAuthor(new EmbedAuthorBuilder().WithUser(user, includeId));
        }

        public static EmbedAuthorBuilder WithUser(this EmbedAuthorBuilder builder, IUser user, bool includeId = false)
        {
            if (user == null)
            {
                return builder.WithName(Placeholders.Undefined);
            }

            var id = includeId ? $" ({user.Id})" : "";

            if (user is IGuildUser sUser)
            {
                builder.WithName($"{sUser.Nickname ?? sUser.Username}{id}");
            }
            else
            {
                builder.WithName($"{user.Username}{id}");
            }

            return builder.WithIconUrl(user.GetUserOrDefaultAvatarUrl());
        }
    }
}
