using Sanakan.ShindenApi.Models.Enums;
using Shinden.Models;

namespace Sanakan.ShindenApi
{
    public static class EnumsExtension
    {
        public static string ToQuery(this FavouriteType type, ulong id)
        {
            return $"{type.ToString().ToLower()}-{id}";
        }

        public static string ToQuery(this MangaRateType type)
        {
            switch (type)
            {
                case MangaRateType.Characters: return "titlecahracters";
                case MangaRateType.Story: return "story";
                case MangaRateType.Total: return "total";
                case MangaRateType.Art: return "lines";
                default: return "total";
            }
        }

        public static string ToQuery(this AnimeRateType type)
        {
            switch (type)
            {
                case AnimeRateType.Characters: return "titlecahracters";
                case AnimeRateType.Graphic: return "graphics";
                case AnimeRateType.Music: return "music";
                case AnimeRateType.Stroy: return "story";
                case AnimeRateType.Total: return "total";
                default: return "total";
            }
        }

        public static string ToQuery(this ListType type)
        {
            switch (type)
            {
                case ListType.Skip: return "skip";
                case ListType.Hold: return "hold";
                case ListType.Plan: return "plan";
                case ListType.Dropped: return "dropped";
                case ListType.Completed: return "completed";
                case ListType.InProgress: return "in progress";
                default: return "in progress";
            }
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

        public static Sex Parse(this Sex gender, string str)
        {
            switch(str)
            {
                case "m": return Sex.Male;
                case "k": return Sex.Female;
                case "f": return Sex.Female;
                case "male": return Sex.Male;
                case "other": return Sex.Other;
                case "female": return Sex.Female;
                default: return Sex.NotSpecified;
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
                case "tv": return AnimeType.Tv;
                case "ona": return AnimeType.Ona;
                case "ova": return AnimeType.Ova;
                case "movie": return AnimeType.Movie;
                case "music": return AnimeType.Music;
                case "special": return AnimeType.Special;
                default: return AnimeType.NotSpecified;
            }
        }

        public static MangaType Parse(this MangaType type, string str)
        {
            switch (str)
            {
                case "light_novel": return MangaType.LightNovel;
                case "doujinshi": return MangaType.Doujinshi;
                case "novel": return MangaType.LightNovel;
                case "one_shot": return MangaType.OneShot;
                case "one shot": return MangaType.OneShot;
                case "doujin": return MangaType.Doujinshi;
                case "manhua": return MangaType.Manhua;
                case "manhwa": return MangaType.Manhwa;
                case "manga": return MangaType.Manga;
                default: return MangaType.NotSpecified;
            }
        }

        public static AnimeStatus Parse(this AnimeStatus status, string str)
        {
            switch (str)
            {
                case "currently airing": return AnimeStatus.CurrentlyAiring;
                case "finished airing": return AnimeStatus.FinishedAiring;
                case "not yet aired": return AnimeStatus.NotYetAired;
                case "proposal": return AnimeStatus.Proposal;
                default: return AnimeStatus.NotSpecified;
            }
        }

        public static MangaStatus Parse(this MangaStatus status, string str)
        {
            switch (str)
            {
                case "finished": return MangaStatus.Finished;
                case "publishing": return MangaStatus.Publishing;
                case "not yet published": return MangaStatus.NotYetPublished;
                default: return MangaStatus.NotSpecified;
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

        public static UserStatus Parse(this UserStatus status, string str)
        {
            switch (str)
            {
                case "active": return UserStatus.Active;
                default: return UserStatus.NotSpecified;
            }
        }
    }
}
