using System;
using System.Text.Json.Serialization;

namespace Sanakan.ShindenApi.API.Common
{
    public class ShindenUser
    {
        [JsonPropertyName("user_id")]
        public ulong UserId { get; set; }

        [JsonPropertyName("vb_id")]
        public ulong VbId { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("last_active")]
        public DateTime? LastActive { get; set; }
        //DateTime.ParseExact(info.LoggedUser.LastActive, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);

        [JsonPropertyName("register_date")]
        [JsonConverter()]
        public DateTime? RegisterDate { get; set; }
        //DateTime.ParseExact(info.RegisterDate, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);

        [JsonPropertyName("rank")]
        public string Rank { get; set; }

        [JsonPropertyName("portal_lang")]
        public string PortalLang { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("avatar")]
        public ulong Avatar { get; set; }

        [JsonPropertyName("anime_css")]
        public string AnimeCss { get; set; }

        [JsonPropertyName("manga_css")]
        public string MangaCss { get; set; }

        [JsonPropertyName("total_points")]
        public long TotalPoints { get; set; }

        [JsonPropertyName("birthdate")]
        public DateTime? Birthdate { get; set; }
        // DateTime.ParseExact(info.LoggedUser.Birthdate, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);

        [JsonPropertyName("sex")]
        public string Sex { get; set; }

        [JsonPropertyName("signature")]
        public string Signature { get; set; }

        [JsonPropertyName("about_me")]
        public string AboutMe { get; set; }
    }
}
