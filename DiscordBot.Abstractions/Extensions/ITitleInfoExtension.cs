using Discord;
using Sanakan.DiscordBot.Abstractions.Models;
using Sanakan.ShindenApi.Models;
using Sanakan.ShindenApi.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sanakan.Extensions
{
    public static class ITitleInfoExtension
    {
        //public static Embed ToEmbed(this ITitleInfo info)
        //{
        //    if (info is IAnimeTitleInfo iam)
        //    {
        //        return iam.ToEmbed();
        //    }
        //    return (info as IMangaTitleInfo)?.ToEmbed();
        //}

        //public static Embed ToEmbed(this MangaTitleInfo info)
        //{
        //    return new EmbedBuilder()
        //    {
        //        Title = info.Title.TrimToLength(EmbedBuilder.MaxTitleLength),
        //        Description = info.Description.Content.TrimToLength(1000),
        //        ThumbnailUrl = info.CoverUrl,
        //        Color = EMType.Info.Color(),
        //        Fields = info.GetFields(),
        //        Footer = info.GetFooter(),
        //        Url = info.MangaUrl,
        //    }.Build();
        //}

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
            string start = "";
            string finish = "";

            if (info.Title.FinishDate.HasValue)
            {
                finish = info.Title.FinishDate.HasValue ? $" - {info.Title.FinishDate.Value.ToShortDateString()}" : "";
            }

            if (info.Title.StartDate.HasValue)
            {
                start = info.Title.StartDate.Value.ToShortDateString();
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

            if (info.Title.TitleOther.Any())
            {
                fields.Add(new EmbedFieldBuilder()
                {
                    Name = "Tytuły alternatywne",
                    Value = string.Join(", ", info.Title.TitleOther).ElipseTrimToLength(EmbedFieldBuilder.MaxFieldValueLength),
                    IsInline = false
                });
            }

            foreach (var tagType in info.Title.Tags)
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
                Value = info.Title.TitleId,
                IsInline = true
            });

            if (info.Title.TotalRating.HasValue)
            {
                if (info.Title.TotalRating > 0)
                {
                    fields.Add(new EmbedFieldBuilder()
                    {
                        Name = "Ocena ogólna",
                        Value = info.Title.TotalRating.Value.ToString("0.0"),
                        IsInline = true
                    });
                }
            }

            string typeVal = "--";
            string statVal = "--";
            //if (info is IAnimeTitleInfo aif)
            //{
            //    typeVal = aif.Type.ToName();
            //    statVal = aif.Status.ToName();

            //    if (aif.EpisodesCount.HasValue)
            //    {
            //        if (aif.EpisodesCount > 0)
            //        {
            //            fields.Add(new EmbedFieldBuilder()
            //            {
            //                Name = "Epizody",
            //                Value = aif.EpisodesCount,
            //                IsInline = true
            //            });
            //        }
            //    }
            //}
            //else if (info is IMangaTitleInfo mif)
            //{
            //    typeVal = mif.Type.ToName();
            //    statVal = mif.Status.ToName();

            //    if (mif.ChaptersCount.HasValue)
            //    {
            //        if (mif.ChaptersCount > 0)
            //        {
            //            fields.Add(new EmbedFieldBuilder()
            //            {
            //                Name = "Rozdziały",
            //                Value = mif.ChaptersCount,
            //                IsInline = true
            //            });
            //        }
            //    }
            //}

            fields.Add(new EmbedFieldBuilder()
            {
                Name = "Typ",
                Value = typeVal,
                IsInline = true
            });

            fields.Add(new EmbedFieldBuilder()
            {
                Name = "Status",
                Value = statVal,
                IsInline = true
            });

            return fields;
        }
    }
}