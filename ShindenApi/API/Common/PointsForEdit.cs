using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sanakan.ShindenApi.API.Common
{
    public class PointsForEdit
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("user_id")]
        public string UserId { get; set; }

        [JsonProperty("points")]
        public string Points { get; set; }

        [JsonProperty("avatar")]
        public string Avatar { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("rank")]
        public string Rank { get; set; }
    }
}
