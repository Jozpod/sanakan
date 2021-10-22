using System.Collections.Generic;
using Newtonsoft.Json;

namespace Shinden.API
{
    public class StaffInfo
    {
        [JsonProperty("staff_id")]
        public string StaffId { get; set; }

        [JsonProperty("first_name")]
        public string FirstName { get; set; }

        [JsonProperty("last_name")]
        public string LastName { get; set; }

        [JsonProperty("staff_type")]
        public string StaffType { get; set; }

        [JsonProperty("birth_date")]
        public string BirthDate { get; set; }

        [JsonProperty("birth_place")]
        public string BirthPlace { get; set; }

        [JsonProperty("death_date")]
        public string DeathDate { get; set; }

        [JsonProperty("gender")]
        public string Gender { get; set; }

        [JsonProperty("nationality")]
        public string Nationality { get; set; }

        [JsonProperty("picture_artifact_id")]
        public string PictureArtifactId { get; set; }

        [JsonProperty("bio")]
        public StaffBio Bio { get; set; }

        [JsonProperty("relations")]
        public List<Relation> Relations { get; set; }
    }

    public class StaffBio
    {
        [JsonProperty("staff_biography_id")]
        public string StaffBiographyId { get; set; }

        [JsonProperty("staff_id")]
        public string StaffId { get; set; }

        [JsonProperty("biography")]
        public string Biography { get; set; }

        [JsonProperty("lang")]
        public string Lang { get; set; }
    }

    public class Relation
    {
        [JsonProperty("many_id")]
        public ulong ManyId { get; set; }

        [JsonProperty("title_id")]
        public string TitleId { get; set; }

        [JsonProperty("staff_id")]
        public ulong StaffId { get; set; }

        [JsonProperty("staff_i18n_id")]
        public string StaffI18NId { get; set; }

        [JsonProperty("staff_detalis")]
        public string StaffDetalis { get; set; }

        [JsonProperty("character_id")]
        public ulong? CharacterId { get; set; }

        [JsonProperty("character_i18n_id")]
        public string CharacterI18NId { get; set; }

        [JsonProperty("seiyuu_lang")]
        public string SeiyuuLang { get; set; }
        [JsonProperty("first_name")]
        public string FirstName { get; set; }
        [JsonProperty("last_name")]
        public string LastName { get; set; }
        [JsonProperty("picture_artifact_id")]
        public string PictureArtifactId { get; set; }
        [JsonProperty("role")]
        public string Role { get; set; }
        [JsonProperty("c_order")]
        public string COrder { get; set; }
        [JsonProperty("s_first_name")]
        public string SFirstName { get; set; }
        [JsonProperty("s_last_name")]
        public string SLastName { get; set; }
        [JsonProperty("s_picture_artifact_id")]
        public string SPictureArtifactId { get; set; }
        [JsonProperty("position")]
        public string Position { get; set; }
        [JsonProperty("s_order")]
        public string SOrder { get; set; }
        [JsonProperty("dmca")]
        public string Dmca { get; set; }
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("rating_total_sum")]
        public string RatingTotalSum { get; set; }
        [JsonProperty("rating_total_cnt")]
        public string RatingTotalCnt { get; set; }
        [JsonProperty("rating_story_sum")]
        public string RatingStorySum { get; set; }
        [JsonProperty("rating_story_cnt")]
        public string RatingStoryCnt { get; set; }
        [JsonProperty("rating_design_sum")]
        public string RatingDesignSum { get; set; }
        [JsonProperty("rating_design_cnt")]
        public string RatingDesignCnt { get; set; }
        [JsonProperty("rating_titlecahracters_cnt")]
        public string RatingTitlecahractersCnt { get; set; }
        [JsonProperty("rating_titlecahracters_sum")]
        public string RatingTitlecahractersSum { get; set; }
        [JsonProperty("ranking_position")]
        public string RankingPosition { get; set; }
        [JsonProperty("ranking_rate")]
        public string RankingRate { get; set; }
        [JsonProperty("title_status")]
        public string TitleStatus { get; set; }
        [JsonProperty("add_date")]
        public string AddDate { get; set; }
        [JsonProperty("premiere_date")]
        public string PremiereDate { get; set; }
        [JsonProperty("premiere_precision")]
        public string PremierePrecision { get; set; }
        [JsonProperty("finish_date")]
        public string FinishDate { get; set; }
        [JsonProperty("finish_precision")]
        public string FinishPrecision { get; set; }
        [JsonProperty("mpaa_rating")]
        public string MpaaRating { get; set; }
        [JsonProperty("cover_artifact_id")]
        public string CoverArtifactId { get; set; }
        [JsonProperty("c_title_id")]
        public string CTitleId { get; set; }
        [JsonProperty("rating_graphics_sum")]
        public string RatingGraphicsSum { get; set; }
        [JsonProperty("rating_graphics_cnt")]
        public string RatingGraphicsCnt { get; set; }
        [JsonProperty("rating_music_sum")]
        public string RatingMusicSum { get; set; }
        [JsonProperty("rating_music_cnt")]
        public string RatingMusicCnt { get; set; }
        [JsonProperty("episodes")]
        public string Episodes { get; set; }
        [JsonProperty("episode_time")]
        public string EpisodeTime { get; set; }
        [JsonProperty("anime_type")]
        public string AnimeType { get; set; }
        [JsonProperty("rating_lines_sum")]
        public string RatingLinesSum { get; set; }
        [JsonProperty("rating_lines_cnt")]
        public string RatingLinesCnt { get; set; }
        [JsonProperty("volumes")]
        public string Volumes { get; set; }
        [JsonProperty("chapters")]
        public string Chapters { get; set; }
    }
}