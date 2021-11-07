using Sanakan.ShindenApi.Converters;
using Sanakan.ShindenApi.Models.Enums;
using System;
using System.Text.Json.Serialization;

namespace Sanakan.ShindenApi.Models
{
    public class ShindenUser
    {
        [JsonPropertyName("user_id")]
        public ulong UserId { get; set; }

        [JsonPropertyName("vb_id")]
        public ulong Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("last_active")]
        [JsonConverter(typeof(DateTimeyyyyMMddHHmmssConverter))]
        public DateTime? LastActive { get; set; }

        [JsonPropertyName("register_date")]
        [JsonConverter(typeof(DateTimeyyyyMMddHHmmssConverter))]
        public DateTime? RegisterDate { get; set; }

        [JsonPropertyName("rank")]
        public string Rank { get; set; }

        [JsonPropertyName("portal_lang")]
        public Language? PortalLang { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("status")]
        public UserStatus Status { get; set; }

        [JsonPropertyName("avatar")]
        public ulong Avatar { get; set; }

        [JsonPropertyName("anime_css")]
        public string AnimeCss { get; set; }

        [JsonPropertyName("manga_css")]
        public string MangaCss { get; set; }

        [JsonPropertyName("total_points")]
        public long? TotalPoints { get; set; }

        [JsonPropertyName("birthdate")]
        [JsonConverter(typeof(DateTimeyyyyMMddConverter))]
        public DateTime? Birthdate { get; set; }

        [JsonPropertyName("sex")]
        public Gender Sex { get; set; }

        [JsonPropertyName("signature")]
        public string Signature { get; set; }

        [JsonPropertyName("about_me")]
        public string AboutMe { get; set; }
    }
}
