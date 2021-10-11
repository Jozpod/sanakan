using Newtonsoft.Json;

namespace Shinden.API
{
    public class TitleStatusAfterChange
    {
        [JsonProperty("status")]
        public string Status { get; set; }
        [JsonProperty("series_status")]
        public TitleStatus TitleStatus { get; set; }
    }

    public class TitleStatus
    {
        [JsonProperty("watched_status_id")]
        public string WatchedStatusId { get; set; }
        [JsonProperty("user_id")]
        public string UserId { get; set; }
        [JsonProperty("title_id")]
        public string TitleId { get; set; }
        [JsonProperty("watch_status")]
        public string WatchStatus { get; set; }
        [JsonProperty("is_favourite")]
        public string IsFavourite { get; set; }
        [JsonProperty("fav_order")]
        public string FavOrder { get; set; }
        [JsonProperty("priority")]
        public string Priority { get; set; }
        [JsonProperty("recommend")]
        public string Recommend { get; set; }
    }
}