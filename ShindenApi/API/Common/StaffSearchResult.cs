using Newtonsoft.Json;

namespace Shinden.API
{
    public class StaffSearchResult
    {
        [JsonProperty("staff_id")]
        public string Id { get; set; }
        [JsonProperty("first_name")]
        public string FirstName { get; set; }
        [JsonProperty("last_name")]
        public string LastName { get; set; }
        [JsonProperty("picture")]
        public string Picture { get; set; }
        [JsonProperty("character_id")]
        public string CharacterId { get; set; }
        [JsonProperty("all_names")]
        public string Names { get; set; }
    }
}