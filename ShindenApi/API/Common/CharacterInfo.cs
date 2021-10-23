using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Sanakan.ShindenApi;
using Sanakan.ShindenApi.API.Common;
using Sanakan.ShindenApi.Utilities;
using Shinden.Models;

namespace Shinden.API
{
    public class CharacterInfo
    {
        [JsonProperty("character_id")]
        public ulong CharacterId { get; set; }

        [JsonProperty("first_name")]
        public string FirstName { get; set; }

        [JsonProperty("last_name")]
        public string LastName { get; set; }

        [JsonProperty("is_real")]
        public bool IsReal { get; set; }

        [JsonProperty("birth_date")]
        public string BirthDate { get; set; }

        [JsonProperty("age")]
        public string Age { get; set; }

        [JsonProperty("death_date")]
        public string DeathDate { get; set; }

        [JsonProperty("gender")]
        [JsonConverter(typeof(StringEnumConverter))]
        public Sex Gender { get; set; } // TO-DO

        [JsonProperty("bloodtype")]
        public string Bloodtype { get; set; }

        [JsonProperty("height")]
        public string Height { get; set; }

        [JsonProperty("weight")]
        public string Weight { get; set; }

        [JsonProperty("bust")]
        public string Bust { get; set; }

        [JsonProperty("waist")]
        public string Waist { get; set; }

        [JsonProperty("hips")]
        public string Hips { get; set; }

        [JsonProperty("picture_artifact_id")]
        public ulong? PictureId { get; set; }

        [JsonProperty("bio")]
        public CharacterBio Biography { get; set; }

        [JsonProperty("fav_stats")]
        public CharacterFav FavStats { get; set; }

        [JsonProperty("points")]
        public List<PointsForEdit>? Points { get; set; }

        [JsonProperty("relations")]
        public List<Relation>? Relations { get; set; }

        [JsonProperty("pictures")]
        public List<ImagePicture> Pictures { get; set; }

        public string CharacterUrl => UrlHelpers.GetCharacterURL(CharacterId);
        public string PictureUrl => UrlHelpers.GetPersonPictureURL(PictureId);

        public override string ToString() => $"{FirstName} {LastName}";
    }
}