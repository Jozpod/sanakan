using System.Text.Json.Serialization;

namespace Sanakan.ShindenApi.Models
{
    public class TitleStatus
    {
        [JsonPropertyName("watched_status_id")]
        public string WatchedStatusId { get; set; }

        [JsonPropertyName("user_id")]
        public string UserId { get; set; }

        [JsonPropertyName("title_id")]
        public string TitleId { get; set; }

        [JsonPropertyName("watch_status")]
        public string WatchStatus { get; set; }

        [JsonPropertyName("is_favourite")]
        public string IsFavourite { get; set; }

        [JsonPropertyName("fav_order")]
        public string FavOrder { get; set; }

        [JsonPropertyName("priority")]
        public string Priority { get; set; }

        [JsonPropertyName("recommend")]
        public string Recommend { get; set; }
    }
}
