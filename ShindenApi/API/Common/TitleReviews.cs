using System.Collections.Generic;
using Newtonsoft.Json;

namespace Shinden.API
{
    public class TitleReviews
    {
        [JsonProperty("reviews")]
        public List<Review> Reviews { get; set; }
    }
}