﻿using Sanakan.ShindenApi.Models.Enums;
using System.Diagnostics.CodeAnalysis;

namespace Sanakan.ShindenApi
{
    [ExcludeFromCodeCoverage]
    public static class EnumsExtension
    {
        public static string ToQuery(this FavouriteType type, ulong id)
        {
            return $"{type.ToString().ToLower()}-{id}";
        }

        public static string ToQuery(this MangaRateType type)
        {
            return type switch
            {
                MangaRateType.Characters => "titlecahracters",
                MangaRateType.Story => "story",
                MangaRateType.Total => "total",
                MangaRateType.Art => "lines",
                _ => "total",
            };
        }

        public static string ToQuery(this ListType type)
        {
            return type switch
            {
                ListType.Skip => "skip",
                ListType.Hold => "hold",
                ListType.Plan => "plan",
                ListType.Dropped => "dropped",
                ListType.Completed => "completed",
                ListType.InProgress => "in progress",
                _ => "in progress",
            };
        }

        public static EpisodeType Parse(this EpisodeType type, string str)
        {
            switch(str)
            {
                case "ova": return EpisodeType.Ova;
                case "special": return EpisodeType.Special;
                case "standard": return EpisodeType.Standard;
                default: return EpisodeType.NotSpecified;
            }
        }

        public static StaffType Parse(this StaffType type, string str)
        {
            switch(str)
            {
                case "colaboration": return StaffType.Colaboration;
                case "company": return StaffType.Company;
                case "person": return StaffType.Person;
                default: return StaffType.NotSpecified;
            }
        }

        public static PictureType Parse(this PictureType type, string str)
        {
            switch(str)
            {
                case "image_picture": return PictureType.Image;
                default: return PictureType.NotSpecified;
            }
        }

        public static Gender Parse(this Gender gender, string str)
        {
            switch(str)
            {
                case "m": return Gender.Male;
                case "k": return Gender.Female;
                case "f": return Gender.Female;
                case "male": return Gender.Male;
                case "other": return Gender.Other;
                case "female": return Gender.Female;
                default: return Gender.NotSpecified;
            }
        }

        public static Language Parse(this Language lang, string str)
        {
            switch (str)
            {
                case "pl": return Language.Polish;
                case "kr": return Language.Korean;
                case "ko": return Language.Korean;
                case "cn": return Language.Chinese;
                case "en": return Language.English;
                case "jp": return Language.Japanese;
                default: return Language.NotSpecified;
            }
        }

        public static AnimeType Parse(this AnimeType type, string str)
        {
            switch (str)
            {
                case "tv":
                    return AnimeType.Tv;
                case "ona":
                    return AnimeType.Ona;
                case "ova":
                    return AnimeType.Ova;
                case "movie":
                    return AnimeType.Movie;
                case "music":
                    return AnimeType.Music;
                case "special":
                    return AnimeType.Special;
                default:
                    return AnimeType.NotSpecified;
            }
        }

        public static MangaType Parse(this MangaType type, string str)
        {
            switch (str)
            {
                case "light_novel":
                    return MangaType.LightNovel;
                case "doujinshi":
                    return MangaType.Doujinshi;
                case "novel":
                    return MangaType.LightNovel;
                case "one_shot":
                    return MangaType.OneShot;
                case "one shot":
                    return MangaType.OneShot;
                case "doujin":
                    return MangaType.Doujinshi;
                case "manhua":
                    return MangaType.Manhua;
                case "manhwa":
                    return MangaType.Manhwa;
                case "manga":
                    return MangaType.Manga;
                default:
                    return MangaType.NotSpecified;
            }
        }

        public static AnimeStatus Parse(this AnimeStatus status, string str)
        {
            switch (str)
            {
                case "currently airing":
                    return AnimeStatus.CurrentlyAiring;
                case "finished airing":
                    return AnimeStatus.FinishedAiring;
                case "not yet aired":
                    return AnimeStatus.NotYetAired;
                case "proposal":
                    return AnimeStatus.Proposal;
                default:
                    return AnimeStatus.NotSpecified;
            }
        }

        public static MangaStatus Parse(this MangaStatus status, string str)
        {
            switch (str)
            {
                case "finished":
                    return MangaStatus.Finished;
                case "publishing":
                    return MangaStatus.Publishing;
                case "not yet published":
                    return MangaStatus.NotYetPublished;
                default:
                    return MangaStatus.NotSpecified;
            }
        }

        public static MpaaRating Parse(this MpaaRating rating, string str)
        {
            switch (str)
            {
                case "g": return MpaaRating.G;
                case "r": return MpaaRating.R;
                case "pg": return MpaaRating.PG;
                case "rx": return MpaaRating.Rx;
                case "ry": return MpaaRating.Ry;
                case "r+": return MpaaRating.RPLUS;
                case "pg-13": return MpaaRating.PGThirteen;
                default: return MpaaRating.NotSpecified;
            }
        }

        public static AlternativeTitleType Parse(this AlternativeTitleType type, string str)
        {
            switch (str)
            {
                case "official": return AlternativeTitleType.Official;
                case "translated": return AlternativeTitleType.Translated;
                case "alternative": return AlternativeTitleType.Alternative;
                default: return AlternativeTitleType.NotSpecified;
            }
        }
    }
}
