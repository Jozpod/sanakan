using System.Collections.Generic;
using Newtonsoft.Json;

namespace Shinden.API
{
    public class CharacterInfo
    {
        [JsonProperty("character_id")]
        public string CharacterId { get; set; }
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
        public string PictureArtifactId { get; set; }
        [JsonProperty("bio")]
        public CharacterBio Bio { get; set; }
        [JsonProperty("fav_stats")]
        public CharacterFav FavStats { get; set; }
        [JsonProperty("points")]
        public List<PointsForEdit> Points { get; set; }
        [JsonProperty("relations")]
        public List<Relation> Relations { get; set; }
        [JsonProperty("pictures")]
        public List<ImagePicture> Pictures { get; set; }
    }

    public class CharacterFav
    {
        [JsonProperty("fav")]
        public string Fav { get; set; }
        [JsonProperty("unfav")]
        public string Unfav { get; set; }
        [JsonProperty("avg_pos")]
        public string AvgPos { get; set; }
        [JsonProperty("1_pos")]
        public string OnePos { get; set; }
        [JsonProperty("under_3_pos")]
        public string Under3Pos { get; set; }
        [JsonProperty("under_10_pos")]
        public string Under10Pos { get; set; }
        [JsonProperty("under_50_pos")]
        public string Under50Pos { get; set; }
    }

    public class PointsForEdit
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("user_id")]
        public string UserId { get; set; }
        [JsonProperty("points")]
        public string Points { get; set; }
        [JsonProperty("avatar")]
        public string Avatar { get; set; }
        [JsonProperty("email")]
        public string Email { get; set; }
        [JsonProperty("rank")]
        public string Rank { get; set; }
    }

    public class CharacterBio
    {
        [JsonProperty("character_biography_id")]
        public string CharacterBiographyId { get; set; }
        [JsonProperty("character_id")]
        public string CharacterId { get; set; }
        [JsonProperty("biography")]
        public string Biography { get; set; }
        [JsonProperty("lang")]
        public string Lang { get; set; }
    }

    public class ImagePicture
    {
        [JsonProperty("artifact_type")]
        public string ArtifactType { get; set; }
        [JsonProperty("character_id")]
        public string CharacterId { get; set; }
        [JsonProperty("artifact_id")]
        public string ArtifactId { get; set; }
        [JsonProperty("is_accepted")]
        public string IsAccepted { get; set; }
        [JsonProperty("is_18plus")]
        public string Is18Plus { get; set; }
        [JsonProperty("user_id")]
        public string UserId { get; set; }
        [JsonProperty("title")]
        public string Title { get; set; }
    }
}