using System;
using System.Text.Json.Serialization;

namespace Sanakan.ShindenApi.Models
{
    public class UserInfo
    {
        [JsonPropertyName("vb_id")]
        public ulong? Id { get; set; }

        [JsonPropertyName("manga_css")]
        public string MangaCss { get; set; } = null;

        [JsonPropertyName("read_time")]
        public Time? ReadTime { get; set; }

        [JsonPropertyName("anime_css")]
        public string AnimeCss { get; set; } = null;

        [JsonPropertyName("about_me")]
        public string AboutMe { get; set; } = null;

        [JsonPropertyName("avatar")]
        public ulong? AvatarId { get; set; }

        [JsonPropertyName("last_active")]
        public DateTime LastActive { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; } = null;

        [JsonPropertyName("logo_img_id")]
        public ulong? LogoImgId { get; set; }

        [JsonPropertyName("portal_lang")]
        public string PortalLang { get; set; } = null;

        [JsonPropertyName("mean_manga_score")]
        public MeanScore MeanMangaScore { get; set; } = null;

        [JsonPropertyName("mean_anime_score")]
        public MeanScore MeanAnimeScore { get; set; } = null;

        [JsonPropertyName("name")]
        public string Name { get; set; } = null;

        [JsonPropertyName("read_more_than_1")]
        public string ReadMoreThan1 { get; set; } = null;

        [JsonPropertyName("rank")]
        public string Rank { get; set; } = null;

        [JsonPropertyName("read_speed")]
        public ReadSpeed ReadSpeed { get; set; } = null;

        [JsonPropertyName("skin_id")]
        public ulong? SkinId { get; set; }

        [JsonPropertyName("register_date")]
        public DateTime RegisterDate { get; set; }

        [JsonPropertyName("readed_status")]
        public ReadWatchStatuses? ReadedStatus { get; set; }

        [JsonPropertyName("signature")]
        public string Signature { get; set; } = null;

        [JsonPropertyName("total_points")]
        public long TotalPoints { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; } = null;

        [JsonPropertyName("user_id")]
        public ulong? UserId { get; set; }

        [JsonPropertyName("watched_cnt")]
        public ulong? WatchedCnt { get; set; }

        [JsonPropertyName("chapters_cnt")]
        public ulong? ReadedCnt { get; set; }

        [JsonPropertyName("watch_time")]
        public Time WatchTime { get; set; } = null;

        [JsonPropertyName("watched_more_than_1")]
        public string WatchedMoreThan1 { get; set; } = null;

        [JsonPropertyName("watched_status")]
        public ReadWatchStatuses? WatchedStatus { get; set; }
    }
}