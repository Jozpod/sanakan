using Sanakan.ShindenApi.Converters;
using Sanakan.ShindenApi.Models.Enums;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Sanakan.ShindenApi.Models
{
    public class StaffInfo
    {
        [JsonPropertyName("staff_id")]
        public ulong StaffId { get; set; }

        [JsonPropertyName("first_name")]
        [JsonConverter(typeof(HtmlDecodeConverter))]
        public string FirstName { get; set; } = string.Empty;

        [JsonPropertyName("last_name")]
        [JsonConverter(typeof(HtmlDecodeConverter))]
        public string LastName { get; set; } = string.Empty;

        [JsonPropertyName("staff_type")]
        public StaffType StaffType { get; set; }

        [JsonPropertyName("birth_date")]
        [JsonConverter(typeof(DateTimeyyyyMMddConverter))]
        public DateTime? BirthDate { get; set; }

        [JsonPropertyName("birth_place")]
        [JsonConverter(typeof(HtmlDecodeConverter))]
        public string BirthPlace { get; set; } = string.Empty;

        [JsonPropertyName("death_date")]
        [JsonConverter(typeof(DateTimeyyyyMMddConverter))]
        public DateTime? DeathDate { get; set; }

        [JsonPropertyName("gender")]
        public Gender Gender { get; set; }

        [JsonPropertyName("nationality")]
        public Language Nationality { get; set; }

        [JsonPropertyName("picture_artifact_id")]
        public ulong PictureArtifactId { get; set; }

        [JsonPropertyName("bio")]
        public StaffBio Biography { get; set; } = null;

        [JsonPropertyName("relations")]
        public List<StaffInfoRelation> Relations { get; set; } = new();
    }
}