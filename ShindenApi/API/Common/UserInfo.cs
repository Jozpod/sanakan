using Newtonsoft.Json;
using Sanakan.ShindenApi.API.Common;
using System;

namespace Shinden.API
{
    public class UserInfo
    {
        [JsonProperty("vb_id")]
        public ulong? Id { get; set; }

        [JsonProperty("manga_css")]
        public string MangaCss { get; set; }

        [JsonProperty("read_time")]
        public Time? ReadTime { get; set; }

        [JsonProperty("anime_css")]
        public string AnimeCss { get; set; }

        [JsonProperty("about_me")]
        public string AboutMe { get; set; }

        [JsonProperty("avatar")]
        public ulong? AvatarId { get; set; }

        [JsonProperty("last_active")]
        public DateTime LastActive { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("logo_img_id")]
        public ulong? LogoImgId { get; set; }

        [JsonProperty("portal_lang")]
        public string PortalLang { get; set; }

        [JsonProperty("mean_manga_score")]
        public MeanScore MeanMangaScore { get; set; }

        [JsonProperty("mean_anime_score")]
        public MeanScore MeanAnimeScore { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("read_more_than_1")]
        public string ReadMoreThan1 { get; set; }

        [JsonProperty("rank")]
        public string Rank { get; set; }

        [JsonProperty("read_speed")]
        public ReadSpeed ReadSpeed { get; set; }

        [JsonProperty("skin_id")]
        public ulong? SkinId { get; set; }

        [JsonProperty("register_date")]
        public DateTime RegisterDate { get; set; }

        [JsonProperty("readed_status")]
        public ReadWatchStatuses? ReadedStatus { get; set; }

        [JsonProperty("signature")]
        public string Signature { get; set; }

        [JsonProperty("total_points")]
        public long TotalPoints { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("user_id")]
        public ulong? UserId { get; set; }

        [JsonProperty("watched_cnt")]
        public ulong? WatchedCnt { get; set; }

        [JsonProperty("chapters_cnt")]
        public ulong? ReadedCnt { get; set; }

        [JsonProperty("watch_time")]
        public Time WatchTime { get; set; }

        [JsonProperty("watched_more_than_1")]
        public string WatchedMoreThan1 { get; set; }

        [JsonProperty("watched_status")]
        public ReadWatchStatuses? WatchedStatus { get; set; }
    }
   
}