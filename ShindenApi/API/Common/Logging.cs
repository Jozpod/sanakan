using Newtonsoft.Json;

namespace Shinden.API
{
    public class Logging
    {
        [JsonProperty("logged_user")]
        public LoggedUser LoggedUser { get; set; }
        [JsonProperty("hash")]
        public string Hash { get; set; }
        [JsonProperty("session")]
        public Session Session { get; set; }
    }

    public class LoggedUser
    {
        [JsonProperty("user_id")]
        public string UserId { get; set; }
        [JsonProperty("vb_id")]
        public string VbId { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("last_active")]
        public string LastActive { get; set; }
        [JsonProperty("register_date")]
        public string RegisterDate { get; set; }
        [JsonProperty("rank")]
        public string Rank { get; set; }
        [JsonProperty("portal_lang")]
        public string PortalLang { get; set; }
        [JsonProperty("email")]
        public string Email { get; set; }
        [JsonProperty("status")]
        public string Status { get; set; }
        [JsonProperty("avatar")]
        public string Avatar { get; set; }
        [JsonProperty("anime_css")]
        public string AnimeCss { get; set; }
        [JsonProperty("manga_css")]
        public string MangaCss { get; set; }
        [JsonProperty("total_points")]
        public string TotalPoints { get; set; }
        [JsonProperty("birthdate")]
        public string Birthdate { get; set; }
        [JsonProperty("sex")]
        public string Sex { get; set; }
        [JsonProperty("signature")]
        public string Signature { get; set; }
        [JsonProperty("about_me")]
        public string AboutMe { get; set; }
    }

    public class Session
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("id")]
        public string Id { get; set; }
    }
}