using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sanakan.ShindenApi.API.Common
{
    public class CharacterBio
    {
        [JsonProperty("character_biography_id")]
        public string CharacterBiographyId { get; set; }

        [JsonProperty("character_id")]
        public string CharacterId { get; set; }

        [JsonProperty("biography")]
        public string Biography { get; set; }

        [JsonProperty("lang")]
        public string Lang { get; set; }
    }
}
