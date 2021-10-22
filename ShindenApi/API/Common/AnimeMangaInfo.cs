using Newtonsoft.Json;
using Sanakan.ShindenApi.API.Common;

namespace Shinden.API
{
    public class AnimeMangaInfo
    {
        [JsonProperty("title")]
        public TitleEntry Title { get; set; }

        public class Anime
        {
            [JsonProperty("rating_graphics_cnt")]
            public string RatingGraphicsCnt { get; set; }

            [JsonProperty("episode_time")]
            public string EpisodeTime { get; set; }

            [JsonProperty("anime_type")]
            public string AnimeType { get; set; }

            [JsonProperty("episodes")]
            public string Episodes { get; set; }

            [JsonProperty("rating_music_cnt")]
            public string RatingMusicCnt { get; set; }

            [JsonProperty("rating_graphics_sum")]
            public string RatingGraphicsSum { get; set; }

            [JsonProperty("rating_music_sum")]
            public string RatingMusicSum { get; set; }

            [JsonProperty("title_id")]
            public ulong? TitleId { get; set; }
        }

        public class Manga
        {
            [JsonProperty("rating_lines_cnt")]
            public string RatingLinesCnt { get; set; }

            [JsonProperty("title_id")]
            public ulong? TitleId { get; set; }

            [JsonProperty("chapters")]
            public string Chapters { get; set; }

            [JsonProperty("rating_lines_sum")]
            public string RatingLinesSum { get; set; }

            [JsonProperty("volumes")]
            public string Volumes { get; set; }
        }

        public class TitleOther
        {
            [JsonProperty("lang")]
            public string Lang { get; set; }

            [JsonProperty("title_id")]
            public string TitleId { get; set; }

            [JsonProperty("is_accepted")]
            public string IsAccepted { get; set; }

            [JsonProperty("title")]
            public string Title { get; set; }

            [JsonProperty("title_other_id")]
            public string TitleOtherId { get; set; }

            [JsonProperty("title_type")]
            public string TitleType { get; set; }
        }

        public class Tags
        {
            [JsonProperty("genre")]
            public Entity Genre { get; set; }

            [JsonProperty("source")]
            public Entity Source { get; set; }

            [JsonProperty("entity")]
            public Entity Entity { get; set; }

            [JsonProperty("place")]
            public Entity Place { get; set; }

            [JsonProperty("studio")]
            public Entity Studio { get; set; }

            [JsonProperty("tag")]
            public Entity Tag { get; set; }
        }

        public class Entity
        {
            [JsonProperty("items")]
            public Item[] Items { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }
        }

        public class Item
        {
            [JsonProperty("tag_id")]
            public string TagId { get; set; }

            [JsonProperty("national_name")]
            public string NationalName { get; set; }

            [JsonProperty("is_accepted")]
            public string IsAccepted { get; set; }

            [JsonProperty("parent_id")]
            public string ParentId { get; set; }

            [JsonProperty("tag_type")]
            public string TagType { get; set; }

            [JsonProperty("tag_name")]
            public string TagName { get; set; }

            [JsonProperty("title_tag")]
            public TitleTag TitleTag { get; set; }
        }

        public class TitleTag
        {
            [JsonProperty("tag_id")]
            public string TagId { get; set; }

            [JsonProperty("relation")]
            public string Relation { get; set; }

            [JsonProperty("title_id")]
            public string TitleId { get; set; }

            [JsonProperty("title_tag_id")]
            public string TitleTagId { get; set; }
        }
    }
}
