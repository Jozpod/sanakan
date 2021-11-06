using System.Collections.Generic;
using System.Text.Json.Serialization;
using Sanakan.ShindenApi;
using Sanakan.ShindenApi.API.Common;
using Sanakan.ShindenApi.Converters;
using Sanakan.ShindenApi.Utilities;
using Shinden.Models;

namespace Sanakan.ShindenApi.Models
{
    public class CharacterInfo
    {
        [JsonPropertyName("character_id")]
        public ulong CharacterId { get; set; }

        [JsonPropertyName("first_name")]
        public string FirstName { get; set; }

        [JsonPropertyName("last_name")]
        public string LastName { get; set; }

        [JsonPropertyName("is_real")]
        public bool IsReal { get; set; }

        [JsonPropertyName("birth_date")]
        public string BirthDate { get; set; }
         //return DateTime.ParseExact(info.BirthDate, "yyyy-MM-dd",
         //       System.Globalization.CultureInfo.InvariantCulture);

        [JsonPropertyName("age")]
        public string Age { get; set; }

        [JsonPropertyName("death_date")]
        public string DeathDate { get; set; }
        //return DateTime.ParseExact(info.DeathDate, "yyyy-MM-dd",
        //        System.Globalization.CultureInfo.InvariantCulture);

        [JsonPropertyName("gender")]
        [JsonConverter(typeof(EnumConverter<>))]
        public Sex Gender { get; set; } // TO-DO

        [JsonPropertyName("bloodtype")]
        public string Bloodtype { get; set; }

        [JsonPropertyName("height")]
        public string Height { get; set; }

        [JsonPropertyName("weight")]
        public string Weight { get; set; }

        [JsonPropertyName("bust")]
        public string Bust { get; set; }

        [JsonPropertyName("waist")]
        public string Waist { get; set; }

        [JsonPropertyName("hips")]
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
        public List<Relation>? Relations { get; set; }

        [JsonPropertyName("pictures")]
        public List<ImagePicture> Pictures { get; set; }

        public string CharacterUrl => UrlHelpers.GetCharacterURL(CharacterId);
        public string PictureUrl => UrlHelpers.GetPersonPictureURL(PictureId);

        public override string ToString() => $"{FirstName} {LastName}";
    }
}