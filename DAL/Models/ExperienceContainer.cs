using System.Text.Json.Serialization;

namespace Sanakan.DAL.Models
{
    public class ExperienceContainer
    {
        public ulong Id { get; set; }
        public double ExperienceCount { get; set; }
        public ExperienceContainerLevel Level { get; set; }

        public ulong GameDeckId { get; set; }

        [JsonIgnore]
        public virtual GameDeck GameDeck { get; set; }
    }
}
