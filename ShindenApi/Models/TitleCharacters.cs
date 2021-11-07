using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Sanakan.ShindenApi.Models
{
    public class TitleCharacters
    {
        [JsonPropertyName("relations")]
        public List<StaffInfoRelation> Relations { get; set; }
    }
}