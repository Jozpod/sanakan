using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Sanakan.ShindenApi.Models
{
    public class TitleReviews
    {
        [JsonPropertyName("reviews")]
        public List<Review> Reviews { get; set; } = new();
    }
}