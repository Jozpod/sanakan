using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Sanakan.ShindenApi.Models
{
    public class SearchResultHighlight
    {
        [JsonPropertyName("titles")]
        public Titles Titles { get; set; } = null;

        [JsonPropertyName("material_titles")]
        public List<string> MaterialTitles { get; set; } = new();
    }
}
