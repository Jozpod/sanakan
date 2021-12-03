using System.Text.Json.Serialization;

namespace Sanakan.ShindenApi.Models
{
    public class StaffInfoRelation
    {
        [JsonPropertyName("many_id")]
        public ulong ManyId { get; set; }

        [JsonPropertyName("title_id")]
        public string TitleId { get; set; }

        [JsonPropertyName("staff_id")]
        public ulong StaffId { get; set; }

        [JsonPropertyName("staff_i18n_id")]
        public string StaffI18NId { get; set; }

        [JsonPropertyName("staff_detalis")]
        public string StaffDetalis { get; set; }

        [JsonPropertyName("character_id")]
        public ulong? CharacterId { get; set; }

        [JsonPropertyName("character_i18n_id")]
        public string CharacterI18NId { get; set; }

        [JsonPropertyName("seiyuu_lang")]
        public string SeiyuuLang { get; set; }

        [JsonPropertyName("first_name")]
        public string FirstName { get; set; }

        [JsonPropertyName("last_name")]
        public string LastName { get; set; }

        [JsonPropertyName("picture_artifact_id")]
        public string PictureArtifactId { get; set; }

        [JsonPropertyName("role")]
        public string Role { get; set; }

        [JsonPropertyName("c_order")]
        public string COrder { get; set; }

        [JsonPropertyName("s_first_name")]
        public string SFirstName { get; set; }

        [JsonPropertyName("s_last_name")]
        public string SLastName { get; set; }

        [JsonPropertyName("s_picture_artifact_id")]
        public string SPictureArtifactId { get; set; }

        [JsonPropertyName("position")]
        public string Position { get; set; }
        [JsonPropertyName("s_order")]
        public string SOrder { get; set; }
        [JsonPropertyName("dmca")]
        public string Dmca { get; set; }
        [JsonPropertyName("title")]
        public string Title { get; set; }
        [JsonPropertyName("type")]
        public string Type { get; set; }
        [JsonPropertyName("rating_total_sum")]
        public string RatingTotalSum { get; set; }

        [JsonPropertyName("rating_total_cnt")]
        public string RatingTotalCnt { get; set; }

        [JsonPropertyName("rating_story_sum")]
        public string RatingStorySum { get; set; }

        [JsonPropertyName("rating_story_cnt")]
        public string RatingStoryCnt { get; set; }

        [JsonPropertyName("rating_design_sum")]
        public string RatingDesignSum { get; set; }

        [JsonPropertyName("rating_design_cnt")]
        public string RatingDesignCnt { get; set; }

        [JsonPropertyName("rating_titlecahracters_cnt")]
        public string RatingTitlecahractersCnt { get; set; }

        [JsonPropertyName("rating_titlecahracters_sum")]
        public string RatingTitlecahractersSum { get; set; }

        [JsonPropertyName("ranking_position")]
        public ulong RankingPosition { get; set; }

        [JsonPropertyName("ranking_rate")]
        public string RankingRate { get; set; }

        [JsonPropertyName("title_status")]
        public string TitleStatus { get; set; }

        [JsonPropertyName("add_date")]
        public string AddDate { get; set; }

        [JsonPropertyName("premiere_date")]
        public string PremiereDate { get; set; }

        [JsonPropertyName("premiere_precision")]
        public string PremierePrecision { get; set; }

        [JsonPropertyName("finish_date")]
        public string FinishDate { get; set; }

        [JsonPropertyName("finish_precision")]
        public string FinishPrecision { get; set; }

        [JsonPropertyName("mpaa_rating")]
        public string MpaaRating { get; set; }
        [JsonPropertyName("cover_artifact_id")]
        public string CoverArtifactId { get; set; }
        [JsonPropertyName("c_title_id")]
        public string CTitleId { get; set; }

        [JsonPropertyName("rating_graphics_sum")]
        public string RatingGraphicsSum { get; set; }

        [JsonPropertyName("rating_graphics_cnt")]
        public string RatingGraphicsCnt { get; set; }

        [JsonPropertyName("rating_music_sum")]
        public string RatingMusicSum { get; set; }

        [JsonPropertyName("rating_music_cnt")]
        public string RatingMusicCnt { get; set; }

        [JsonPropertyName("episodes")]
        public string? Episodes { get; set; }

        [JsonPropertyName("episode_time")]
        public string? EpisodeTime { get; set; }

        [JsonPropertyName("anime_type")]
        public string? AnimeType { get; set; }

        [JsonPropertyName("rating_lines_sum")]
        public double RatingLinesSum { get; set; }

        [JsonPropertyName("rating_lines_cnt")]
        public double RatingLinesCnt { get; set; }

        [JsonPropertyName("volumes")]
        public string? Volumes { get; set; }

        [JsonPropertyName("chapters")]
        public string Chapters { get; set; }
    }
}
