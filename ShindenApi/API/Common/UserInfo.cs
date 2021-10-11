using Newtonsoft.Json;

namespace Shinden.API
{
    public class UserInfo
    {
        [JsonProperty("manga_css")]
        public string MangaCss { get; set; }
        [JsonProperty("read_time")]
        public Time ReadTime { get; set; }
        [JsonProperty("anime_css")]
        public string AnimeCss { get; set; }
        [JsonProperty("about_me")]
        public string AboutMe { get; set; }
        [JsonProperty("avatar")]
        public string Avatar { get; set; }
        [JsonProperty("last_active")]
        public string LastActive { get; set; }
        [JsonProperty("email")]
        public string Email { get; set; }
        [JsonProperty("logo_img_id")]
        public string LogoImgId { get; set; }
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
        public string SkinId { get; set; }
        [JsonProperty("vb_id")]
        public string VbId { get; set; }
        [JsonProperty("register_date")]
        public string RegisterDate { get; set; }
        [JsonProperty("readed_status")]
        public ReadWatchStatuses ReadedStatus { get; set; }
        [JsonProperty("signature")]
        public string Signature { get; set; }
        [JsonProperty("total_points")]
        public string TotalPoints { get; set; }
        [JsonProperty("status")]
        public string Status { get; set; }
        [JsonProperty("user_id")]
        public string UserId { get; set; }
        [JsonProperty("watched_cnt")]
        public string WatchedCnt { get; set; }
        [JsonProperty("chapters_cnt")]
        public string ReadedCnt { get; set; }
        [JsonProperty("watch_time")]
        public Time WatchTime { get; set; }
        [JsonProperty("watched_more_than_1")]
        public string WatchedMoreThan1 { get; set; }
        [JsonProperty("watched_status")]
        public ReadWatchStatuses WatchedStatus { get; set; }
    }

    public class Time
    {
        [JsonProperty("h")]
        public string H { get; set; }
        [JsonProperty("n")]
        public string N { get; set; }
        [JsonProperty("d")]
        public string D { get; set; }
        [JsonProperty("m")]
        public string M { get; set; }
        [JsonProperty("t")]
        public string T { get; set; }
        [JsonProperty("y")]
        public string Y { get; set; }
    }

    public class MeanScore
    {
        [JsonProperty("mean_score")]
        public string OtherMeanScore { get; set; }
        [JsonProperty("scores_cnt")]
        public string ScoresCnt { get; set; }
    }

    public class ReadSpeed
    {
        [JsonProperty("manga_read_time")]
        public string MangaReadTime { get; set; }
        [JsonProperty("manga_proc")]
        public string MangaProc { get; set; }
        [JsonProperty("vn_proc")]
        public string VnProc { get; set; }
    }

    public class ReadWatchStatuses
    {
        [JsonProperty("hold")]
        public string Hold { get; set; }
        [JsonProperty("completed")]
        public string Completed { get; set; }
        [JsonProperty("_total")]
        public string Total { get; set; }
        [JsonProperty("dropped")]
        public string Dropped { get; set; }
        [JsonProperty("plan")]
        public string Plan { get; set; }
        [JsonProperty("in progress")]
        public string InProgress { get; set; }
        [JsonProperty("skip")]
        public string Skip { get; set; }
    }
}