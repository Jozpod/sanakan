using System.Collections.Generic;
using Newtonsoft.Json;

namespace Shinden.API
{
    public class TitleCharacters
    {
        [JsonProperty("relations")]
        public List<Relation> Relations { get; set; }
    }
}