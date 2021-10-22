using System.Collections.Generic;
using Newtonsoft.Json;

namespace Shinden.API
{
    public class TitleEpisodes
    {
        [JsonProperty("episodes")]
        public List<Episode> Episodes { get; set; }

        [JsonProperty("connected_episodes")]
        public List<Episode> ConnectedEpisodes { get; set; }
    }
}