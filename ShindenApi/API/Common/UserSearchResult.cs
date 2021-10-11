using Newtonsoft.Json;

namespace Shinden.API
{
    public class UserSearchResult
    {
        [JsonProperty("user_id")]
        public string Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("rank")]
        public string Rank { get; set; }
        [JsonProperty("avatar")]
        public string Avatar { get; set; }
    }
}