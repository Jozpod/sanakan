using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Sanakan.ShindenApi.Models
{
    public class TitleRelations
    {
        [JsonPropertyName("related_titles")]
        public List<TitleRelation> RelatedTitles { get; set; } = new();
    }
}