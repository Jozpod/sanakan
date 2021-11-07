using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Sanakan.ShindenApi.Converters;
using Sanakan.ShindenApi.Models.Enums;
using Sanakan.ShindenApi.Utilities;

namespace Sanakan.ShindenApi.Models
{
    public class CharacterInfo
    {
        [JsonPropertyName("character_id")]
        public ulong CharacterId { get; set; }

        [JsonPropertyName("first_name")]
        [JsonConverter(typeof(HtmlDecodeConverter))]
        public string FirstName { get; set; }

        [JsonPropertyName("last_name")]
        [JsonConverter(typeof(HtmlDecodeConverter))]
        public string LastName { get; set; }

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
        public string Bloodtype { get; set; }

        [JsonPropertyName("height")]
        [JsonConverter(typeof(HtmlDecodeConverter))]
        public string Height { get; set; }

        [JsonPropertyName("weight")]
        [JsonConverter(typeof(HtmlDecodeConverter))]
        public string Weight { get; set; }

        [JsonPropertyName("bust")]
        [JsonConverter(typeof(HtmlDecodeConverter))]
        public string Bust { get; set; }

        [JsonPropertyName("waist")]
        [JsonConverter(typeof(HtmlDecodeConverter))]
        public string Waist { get; set; }

        [JsonPropertyName("hips")]
        [JsonConverter(typeof(HtmlDecodeConverter))]
        public string Hips { get; set; }

        [JsonPropertyName("picture_artifact_id")]
        public ulong? PictureId { get; set; }

        [JsonPropertyName("bio")]
        public CharacterBio Biography { get; set; }

        [JsonPropertyName("fav_stats")]
        public CharacterFav FavStats { get; set; }

        [JsonPropertyName("points")]
        public List<PointsForEdit>? Points { get; set; }

        [JsonPropertyName("relations")]
        public List<StaffInfoRelation>? Relations { get; set; }

        [JsonPropertyName("pictures")]
        public List<ImagePicture> Pictures { get; set; }

        public string CharacterUrl => UrlHelpers.GetCharacterURL(CharacterId);
        public string PictureUrl => UrlHelpers.GetPersonPictureURL(PictureId);

        public override string ToString() => $"{FirstName} {LastName}";
    }
}