using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Sanakan.ShindenApi.Models
{
    public class SearchResult
    {
        [JsonPropertyName("per_page")]
        public long? PerPage { get; set; }

        [JsonPropertyName("offset")]
        public long? Offset { get; set; }

        [JsonPropertyName("total_items")]
        public long? TotalItems { get; set; }

        [JsonPropertyName("result_type")]
        public string ResultType { get; set; } = string.Empty;

        [JsonPropertyName("items")]
        public List<SearchResultItem> Items { get; set; } = new();
    }

    public class Titles
    {
        [JsonPropertyName("1")]
        public string The1 { get; set; } = string.Empty;
    }
}
