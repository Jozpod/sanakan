using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Shinden.API
{
    public class TitleCharacters
    {
        [JsonPropertyName("relations")]
        public List<Relation> Relations { get; set; }
    }
}