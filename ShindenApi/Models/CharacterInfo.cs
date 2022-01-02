using Sanakan.ShindenApi.Converters;
using Sanakan.ShindenApi.Models.Enums;
using Sanakan.ShindenApi.Utilities;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Sanakan.ShindenApi.Models
{
    public class CharacterInfo
    {
        [JsonPropertyName("character_id")]
        public ulong CharacterId { get; set; }

        [JsonPropertyName("first_name")]
        [JsonConverter(typeof(HtmlDecodeConverter))]
        public string FirstName { get; set; } = string.Empty;

        [JsonPropertyName("last_name")]
        [JsonConverter(typeof(HtmlDecodeConverter))]
        public string LastName { get; set; } = string.Empty;

        [JsonPropertyName("is_real")]
        public bool IsReal { get; set; }

        [JsonPropertyName("birth_date")]
        [JsonConverter(typeof(DateTimeyyyyMMddConverter))]
        public DateTime? BirthDate { get; set; }

        [JsonPropertyName("age")]
        [JsonConverter(typeof(HtmlDecodeConverter))]
        public string? Age { get; set; }

        [JsonPropertyName("death_date")]
        [JsonConverter(typeof(DateTimeyyyyMMddConverter))]
        public DateTime? DeathDate { get; set; }

        [JsonPropertyName("gender")]
        public Gender Gender { get; set; }

        [JsonPropertyName("bloodtype")]
        [JsonConverter(typeof(HtmlDecodeConverter))]
        public string Bloodtype { get; set; } = string.Empty;

        [JsonPropertyName("height")]
        [JsonConverter(typeof(HtmlDecodeConverter))]
        public string Height { get; set; } = string.Empty;

        [JsonPropertyName("weight")]
        [JsonConverter(typeof(HtmlDecodeConverter))]
        public string Weight { get; set; } = string.Empty;

        [JsonPropertyName("bust")]
        [JsonConverter(typeof(HtmlDecodeConverter))]
        public string Bust { get; set; } = string.Empty;

        [JsonPropertyName("waist")]
        [JsonConverter(typeof(HtmlDecodeConverter))]
        public string Waist { get; set; } = string.Empty;

        [JsonPropertyName("hips")]
        [JsonConverter(typeof(HtmlDecodeConverter))]
        public string Hips { get; set; } = string.Empty;

        [JsonPropertyName("picture_artifact_id")]
        public ulong? PictureId { get; set; }

        [JsonPropertyName("bio")]
        public CharacterBio Biography { get; set; } = null;

        [JsonPropertyName("fav_stats")]
        public CharacterFav FavStats { get; set; } = null;

        [JsonPropertyName("points")]
        public List<PointsForEdit> Points { get; set; } = new();

        [JsonPropertyName("relations")]
        public List<StaffInfoRelation> Relations { get; set; } = new();

        [JsonPropertyName("pictures")]
        public List<ImagePicture> Pictures { get; set; } = new();

        public string CharacterUrl => UrlHelpers.GetCharacterURL(CharacterId);

        public string PictureUrl => UrlHelpers.GetPersonPictureURL(PictureId);

        public override string ToString() => $"{FirstName} {LastName}";
    }
}