using Discord;
using Sanakan.Api.Models;
using Sanakan.Configuration;
using Sanakan.DiscordBot.Models;
using Sanakan.Extensions;
using Sanakan.Web.Models;
using System;
using System.Collections.Generic;

namespace Sanakan.Web
{
    public static class RichMessageExtension
    {
        public static Embed ToEmbed(this RichMessage msg)
        {
            var embed = new EmbedBuilder
            {
                Url = msg.Url ?? "",
                Title = (msg.Title ?? "").TrimToLength(EmbedBuilder.MaxTitleLength),
                Timestamp = msg.Timestamp,
                ImageUrl = msg.ImageUrl ?? "",
                Color = msg.MessageType.ToColor(),
                Description = (msg.Description ?? "").ConvertBBCodeToMarkdown().TrimToLength(1800),
                ThumbnailUrl = msg.ThumbnailUrl ?? "",
            };

            if (msg.ImageUrl != null)
            {
                embed.Description = embed.Description.Replace(msg.ImageUrl, "");
            }

            if (msg.Author != null)
            {
                embed.Author = new EmbedAuthorBuilder
                {
                    IconUrl = msg.Author.ImageUrl ?? "",
                    Url = msg.Author.NameUrl ?? "",
                    Name = (msg.Author.Name ?? "").TrimToLength(EmbedAuthorBuilder.MaxAuthorNameLength),
                };
            }

            if (msg.Footer != null)
            {
                embed.Footer = new EmbedFooterBuilder
                {
                    IconUrl = msg.Footer.ImageUrl ?? "",
                    Text = (msg.Footer.Text ?? "").TrimToLength(EmbedFooterBuilder.MaxFooterTextLength),
                };
            }

            if (msg.Fields != null)
            {
                int index = 0;
                foreach (var field in msg.Fields)
                {
                    if (++index >= EmbedBuilder.MaxFieldCount) break;

                    embed.AddField(new EmbedFieldBuilder
                    {
                        IsInline = field.IsInline,
                        Value = (field.Value ?? "").TrimToLength(EmbedFieldBuilder.MaxFieldValueLength),
                        Name = (field.Name ?? "").TrimToLength(EmbedFieldBuilder.MaxFieldNameLength),
                    });
                }
            }

            return embed.Build();
        }

        public static Color ToColor(this RichMessageType type)
        {
            switch (type)
            {
                case RichMessageType.NewEpisodePL:
                    return EMType.Error.Color();

                default:
                    return EMType.Info.Color();
            }
        }
    }
}
