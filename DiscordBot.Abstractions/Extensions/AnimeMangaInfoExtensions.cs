using Discord;
using Sanakan.DiscordBot.Abstractions.Models;
using Sanakan.ShindenApi.Models;
using Sanakan.ShindenApi.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sanakan.Extensions
{
    public static class AnimeMangaInfoExtensions
    {
        public static Embed ToEmbed(this AnimeMangaInfo info)
        {
            return new EmbedBuilder()
            {
                Title = info.Title.Title.ElipseTrimToLength(EmbedBuilder.MaxTitleLength),
                Description = info.Title.Description.OtherDescription.ElipseTrimToLength(1000),
                ThumbnailUrl = info.Title.CoverUrl,
                Color = EMType.Info.Color(),
                Fields = info.GetFields(),
                Footer = info.GetFooter(),
                Url = info.Title.AnimeUrl,
            }.Build();
        }

        public static EmbedFooterBuilder GetFooter(this AnimeMangaInfo info)
        {
            var start = "";
            var finish = "";
            var finishDate = info.Title.FinishDate;
            var startDate = info.Title.StartDate;

            if (finishDate.HasValue)
            {
                finish = finishDate.HasValue ? $" - {finishDate.Value.ToShortDateString()}" : "";
            }

            if (startDate.HasValue)
            {
                start = startDate.Value.ToShortDateString();
            }

            return new EmbedFooterBuilder()
            {
                Text = $"{start}{finish}",
            };
        }

        public static string GetPrecisonDate(this DateTime date, ulong precision)
        {
            switch (precision)
            {
                case 1: return date.ToString("yyyy");
                case 2: return date.ToString("yyyy - MM");
                default: return date.ToShortDateString();
            }
        }

        public static string ToName(this AnimeStatus status)
        {
            switch (status)
            {
                case AnimeStatus.CurrentlyAiring: return "Wychodzi";
                case AnimeStatus.FinishedAiring: return "Zakończone";
                case AnimeStatus.Proposal:
                case AnimeStatus.NotYetAired: return "Deklaracja";
                default: return "Niesprecyzowane";
            }
        }

        public static string ToName(this MangaStatus status)
        {
            switch (status)
            {
                case MangaStatus.Publishing: return "Wychodzi";
                case MangaStatus.Finished: return "Zakończone";
                case MangaStatus.NotYetPublished: return "Deklaracja";
                default: return "Niesprecyzowane";
            }
        }

        public static string ToName(this AnimeType type)
        {
            switch (type)
            {
                case AnimeType.Movie: return "Film";
                case AnimeType.Music: return "Teledysk";
                case AnimeType.Ona: return "ONA";
                case AnimeType.Ova: return "OVA";
                case AnimeType.Tv: return "TV";
                case AnimeType.Special: return "Odcinek specjalny";
                default: return "Niesprecyzowane";
            }
        }

        public static string ToName(this MangaType type)
        {
            switch (type)
            {
                case MangaType.LightNovel: return "Light novel";
                case MangaType.Doujinshi: return "Doujinshi";
                case MangaType.Manga: return "Manga";
                case MangaType.Manhua: return "Manhua";
                case MangaType.Manhwa: return "Manhwa";
                case MangaType.OneShot: return "One shot";
                default: return "Niesprecyzowane";
            }
        }

        public static List<EmbedFieldBuilder> GetFields(this AnimeMangaInfo info)
        {
            var fields = new List<EmbedFieldBuilder>();
            var entry = info.Title;

            if (entry.TitleOther.Any())
            {
                fields.Add(new EmbedFieldBuilder()
                {
                    Name = "Tytuły alternatywne",
                    Value = string.Join(", ", entry.TitleOther).ElipseTrimToLength(EmbedFieldBuilder.MaxFieldValueLength),
                    IsInline = false
                });
            }

            foreach (var tagType in entry.Tags)
            {
                fields.Add(new EmbedFieldBuilder()
                {
                    Name = tagType.Name.ElipseTrimToLength(EmbedFieldBuilder.MaxFieldNameLength),
                    Value = string.Join(", ", tagType.Items.Select(pr => pr.TagName))
                        .ElipseTrimToLength(EmbedFieldBuilder.MaxFieldValueLength),
                    IsInline = false
                });
            }

            fields.Add(new EmbedFieldBuilder()
            {
                Name = "Id",
                Value = entry.TitleId,
                IsInline = true
            });

            if (entry.TotalRating.HasValue)
            {
                if (entry.TotalRating > 0)
                {
                    fields.Add(new EmbedFieldBuilder()
                    {
                        Name = "Ocena ogólna",
                        Value = entry.TotalRating.Value.ToString("0.0"),
                        IsInline = true
                    });
                }
            }

            var typeHumanized = "--";
            var statusHumanized = "--";

            if(entry.Type == IllustrationType.Anime)
            {
                typeHumanized = entry.Anime.AnimeType.ToName();
                statusHumanized = entry.AnimeStatus.ToName();
                var episodesCount = entry.Anime.EpisodesCount;

                if (episodesCount.HasValue)
                {
                    if (episodesCount.Value > 0)
                    {
                        fields.Add(new EmbedFieldBuilder()
                        {
                            Name = "Epizody",
                            Value = episodesCount,
                            IsInline = true
                        });
                    }
                }
            }

            if(entry.Type == IllustrationType.Manga)
            {
                typeHumanized = entry.MangaType.ToName();
                statusHumanized = entry.MangaStatus.ToName();
                var chaptersCount = entry.Manga.ChaptersCount;

                if (chaptersCount.HasValue)
                {
                    if (chaptersCount > 0)
                    {
                        fields.Add(new EmbedFieldBuilder()
                        {
                            Name = "Rozdziały",
                            Value = chaptersCount,
                            IsInline = true
                        });
                    }
                }
            }

            fields.Add(new EmbedFieldBuilder()
            {
                Name = "Typ",
                Value = typeHumanized,
                IsInline = true
            });

            fields.Add(new EmbedFieldBuilder()
            {
                Name = "Status",
                Value = statusHumanized,
                IsInline = true
            });

            return fields;
        }
    }
}