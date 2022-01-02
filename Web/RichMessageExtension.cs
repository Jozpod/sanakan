using Discord;
using Sanakan.Configuration;
using Sanakan.DiscordBot.Abstractions.Models;
using Sanakan.DiscordBot.Models;
using Sanakan.Extensions;
using System.Diagnostics.CodeAnalysis;

namespace Sanakan.Web
{
    [ExcludeFromCodeCoverage]
    public static class RichMessageExtension
    {
        public static Embed ToEmbed(this RichMessage message)
        {
            var embed = new EmbedBuilder
            {
                Url = message.Url ?? "",
                Title = (message.Title ?? "").ElipseTrimToLength(EmbedBuilder.MaxTitleLength),
                Timestamp = message.Timestamp,
                ImageUrl = message.ImageUrl ?? "",
                Color = message.MessageType.ToColor(),
                Description = (message.Description ?? "").ConvertBBCodeToMarkdown().ElipseTrimToLength(1800),
                ThumbnailUrl = message.ThumbnailUrl ?? "",
            };

            if (message.ImageUrl != null)
            {
                embed.Description = embed.Description.Replace(message.ImageUrl, "");
            }

            if (message.Author != null)
            {
                embed.Author = new EmbedAuthorBuilder
                {
                    IconUrl = message.Author.ImageUrl ?? "",
                    Url = message.Author.NameUrl ?? "",
                    Name = (message.Author.Name ?? "").ElipseTrimToLength(EmbedAuthorBuilder.MaxAuthorNameLength),
                };
            }

            if (message.Footer != null)
            {
                embed.Footer = new EmbedFooterBuilder
                {
                    IconUrl = message.Footer.ImageUrl ?? "",
                    Text = (message.Footer.Text ?? "").ElipseTrimToLength(EmbedFooterBuilder.MaxFooterTextLength),
                };
            }

            if (message.Fields != null)
            {
                int index = 0;
                foreach (var field in message.Fields)
                {
                    if (++index >= EmbedBuilder.MaxFieldCount)
                    {
                        break;
                    }

                    embed.AddField(new EmbedFieldBuilder
                    {
                        IsInline = field.IsInline,
                        Value = (field.Value ?? "").ElipseTrimToLength(EmbedFieldBuilder.MaxFieldValueLength),
                        Name = (field.Name ?? "").ElipseTrimToLength(EmbedFieldBuilder.MaxFieldNameLength),
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
