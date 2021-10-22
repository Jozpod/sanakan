using System.Collections.Generic;
using Newtonsoft.Json;
using Sanakan.ShindenApi;
using Sanakan.ShindenApi.API.Common;

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
        public string IsReal { get; set; }

        [JsonProperty("birth_date")]
        public string BirthDate { get; set; }

        [JsonProperty("age")]
        public string Age { get; set; }

        [JsonProperty("death_date")]
        public string DeathDate { get; set; }

        [JsonProperty("gender")]
        public string Gender { get; set; }

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
        public ulong? PictureArtifactId { get; set; }

        [JsonProperty("bio")]
        public CharacterBio Bio { get; set; }

        [JsonProperty("fav_stats")]
        public CharacterFav FavStats { get; set; }

        [JsonProperty("points")]
        public List<PointsForEdit>? Points { get; set; }

        [JsonProperty("relations")]
        public List<Relation>? Relations { get; set; }

        [JsonProperty("pictures")]
        public List<ImagePicture> Pictures { get; set; }

        public override string ToString() => $"{FirstName} {LastName}";
    }
}