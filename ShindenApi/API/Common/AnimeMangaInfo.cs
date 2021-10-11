using Newtonsoft.Json;

namespace Shinden.API
{
    public class AnimeMangaInfo
    {
        [JsonProperty("title")]
        public TitleEntry Title { get; set; }

        public class TitleEntry
        {
            [JsonProperty("mpaa_rating")]
            public string MpaaRating { get; set; }
            [JsonProperty("rating_story_sum")]
            public string RatingStorySum { get; set; }
            [JsonProperty("description")]
            public Description Description { get; set; }
            [JsonProperty("anime")]
            public Anime Anime { get; set; }
            [JsonProperty("manga")]
            public Manga Manga { get; set; }
            [JsonProperty("add_date")]
            public string AddDate { get; set; }
            [JsonProperty("cover_artifact_id")]
            public string CoverArtifactId { get; set; }
            [JsonProperty("finish_date")]
            public string FinishDate { get; set; }
            [JsonProperty("dmca")]
            public string Dmca { get; set; }
            [JsonProperty("finish_precision")]
            public string FinishPrecision { get; set; }
            [JsonProperty("ranking_rate")]
            public string RankingRate { get; set; }
            [JsonProperty("premiere_precision")]
            public string PremierePrecision { get; set; }
            [JsonProperty("premiere_date")]
            public string PremiereDate { get; set; }
            [JsonProperty("ranking_position")]
            public string RankingPosition { get; set; }
            [JsonProperty("rating_design_sum")]
            public string RatingDesignSum { get; set; }
            [JsonProperty("rating_design_cnt")]
            public string RatingDesignCnt { get; set; }
            [JsonProperty("rating_story_cnt")]
            public string RatingStoryCnt { get; set; }
            [JsonProperty("rating_total_sum")]
            public string RatingTotalSum { get; set; }
            [JsonProperty("rating_titlecahracters_sum")]
            public string RatingTitlecahractersSum { get; set; }
            [JsonProperty("rating_titlecahracters_cnt")]
            public string RatingTitlecahractersCnt { get; set; }
            [JsonProperty("rating_total_cnt")]
            public string RatingTotalCnt { get; set; }
            [JsonProperty("title")]
            public string OtherTitle { get; set; }
            [JsonProperty("title_other")]
            public TitleOther[] TitleOther { get; set; }
            [JsonProperty("tags")]
            public Tags Tags { get; set; }
            [JsonProperty("title_id")]
            public string TitleId { get; set; }
            [JsonProperty("title_status")]
            public string TitleStatus { get; set; }
            [JsonProperty("type")]
            public string Type { get; set; }
        }

        public class Description
        {
            [JsonProperty("description_id")]
            public string DescriptionId { get; set; }
            [JsonProperty("description")]
            public string OtherDescription { get; set; }
            [JsonProperty("lang_code")]
            public string LangCode { get; set; }
            [JsonProperty("title_id")]
            public string TitleId { get; set; }
        }

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
            public string TitleId { get; set; }
        }

        public class Manga
        {
            [JsonProperty("rating_lines_cnt")]
            public string RatingLinesCnt { get; set; }
            [JsonProperty("title_id")]
            public string TitleId { get; set; }
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
