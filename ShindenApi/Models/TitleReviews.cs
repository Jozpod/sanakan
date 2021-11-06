using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Shinden.API
{
    public class TitleReviews
    {
        [JsonPropertyName("reviews")]
        public List<Review> Reviews { get; set; }
    }
}