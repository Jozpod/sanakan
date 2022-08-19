using Discord;
using Sanakan.Common;
using Sanakan.DiscordBot.Abstractions.Models;
using System.Collections.Generic;
using System.Linq;

namespace Sanakan.DiscordBot.Abstractions.Extensions
{
    public static class DiscordExtensions
    {
        public static EmbedBuilder ToEmbedMessage(
            this string message,
            EMType type = EMType.Neutral,
            bool icon = false) => new EmbedBuilder()
                .WithColor(type.Color())
                .WithDescription($"{type.Emoji(!icon)}{message}");

        public static string GetLocalCreatedAtShortDateTime(this IMessage message)
           => message.CreatedAt.DateTime.ToLocalTime().ToString(Placeholders.ddMMyyyyHHmm);

        public static int CountEmotesTextLength(this IEnumerable<ITag> tags) => tags
                .Where(tag => tag.Type == TagType.Emoji)
                .Sum(x => x.Value.ToString().Length);

        public static string GetUserOrDefaultAvatarUrl(this IUser user, bool getFromGuild = false)
        {
            var avatar = user.GetAvatarUrl() ?? user.GetDefaultAvatarUrl();
            
            if (user is IGuildUser guildUser && getFromGuild)
            {
                var guildAvatar = guildUser.GetGuildAvatarUrl();

                if (guildAvatar != null)
                {
                    avatar = guildAvatar;
                }
            }
            
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

            if (user is IGuildUser guildUser)
            {
                builder.WithName($"{guildUser.Nickname ?? guildUser.Username}{id}");
            }
            else
            {
                builder.WithName($"{user.Username}{id}");
            }

            return builder.WithIconUrl(user.GetUserOrDefaultAvatarUrl());
        }
    }
}
