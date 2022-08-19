using Sanakan.ShindenApi.Models;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Shinden.API
{
    public class TitleEpisodes
    {
        [JsonPropertyName("episodes")]
        public List<Episode> Episodes { get; set; } = new();

        [JsonPropertyName("connected_episodes")]
        public List<Episode> ConnectedEpisodes { get; set; } = new();
    }
}