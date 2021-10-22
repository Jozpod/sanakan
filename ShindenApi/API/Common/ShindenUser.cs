using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sanakan.ShindenApi.API.Common
{
    public class ShindenUser
    {
        [JsonProperty("user_id")]
        public ulong UserId { get; set; }

        [JsonProperty("vb_id")]
        public ulong VbId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("last_active")]
        public DateTime? LastActive { get; set; }
        //DateTime.ParseExact(info.LoggedUser.LastActive, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);

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
        public ulong Avatar { get; set; }

        [JsonProperty("anime_css")]
        public string AnimeCss { get; set; }

        [JsonProperty("manga_css")]
        public string MangaCss { get; set; }

        [JsonProperty("total_points")]
        public long TotalPoints { get; set; }

        [JsonProperty("birthdate")]
        public DateTime? Birthdate { get; set; }
        // DateTime.ParseExact(info.LoggedUser.Birthdate, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);

        [JsonProperty("sex")]
        public string Sex { get; set; }

        [JsonProperty("signature")]
        public string Signature { get; set; }

        [JsonProperty("about_me")]
        public string AboutMe { get; set; }
    }
}
