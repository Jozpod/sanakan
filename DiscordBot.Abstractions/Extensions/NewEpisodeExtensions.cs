using System.Collections.Generic;
using Discord;
using Sanakan.DiscordBot.Abstractions.Models;
using Shinden.API;
using Shinden.Models;

namespace Sanakan.Extensions
{
    public static class NewEpisodeExtensions
    {
        public static Embed ToEmbed(this NewEpisode episode)
        {
            return new EmbedBuilder()
            {
                Title = episode.Title.ElipseTrimToLength(EmbedBuilder.MaxTitleLength),
                ThumbnailUrl = episode.AnimeCoverUrl,
                Color = EMType.Info.Color(),
                Fields = episode.GetFields(),
                Url = episode.AnimeUrl,
            }.Build();
        }

        public static List<EmbedFieldBuilder> GetFields(this NewEpisode ep)
        {
            return new List<EmbedFieldBuilder>
            {
                new EmbedFieldBuilder
                {
                    Name = "Numer epizodu",
                    Value = ep.EpisodeNumber,
                    IsInline = true
                },
                new EmbedFieldBuilder
                {
                     Name = "Czas trwania",
                     Value = ep.EpisodeLength,
                     IsInline = true
                },
                new EmbedFieldBuilder
                {
                    Name = "Język napisów",
                    Value = ep.SubtitlesLanguage.ToName(),
                    IsInline = true
                },
                new EmbedFieldBuilder
                {
                    Name = "Link",
                    Value = ep.EpisodeUrl,
                    IsInline = false
                },
                new EmbedFieldBuilder
                {
                    Name = "Data dodania",
                    Value = ep.AddDate.ToShortDateString(),
                    IsInline = false
                }
            };
        }
    }
}