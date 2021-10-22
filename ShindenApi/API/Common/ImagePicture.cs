using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sanakan.ShindenApi.API.Common
{
    public class ImagePicture
    {
        [JsonProperty("artifact_type")]
        public string ArtifactType { get; set; }

        [JsonProperty("character_id")]
        public string CharacterId { get; set; }

        [JsonProperty("artifact_id")]
        public ulong ArtifactId { get; set; }

        [JsonProperty("is_accepted")]
        public bool IsAccepted { get; set; }

        [JsonProperty("is_18plus")]
        public bool Is18Plus { get; set; }

        [JsonProperty("user_id")]
        public string UserId { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }
    }
}
