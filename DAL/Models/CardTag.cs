using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace Sanakan.DAL.Models
{
    public class CardTag
    {
        public ulong Id { get; set; }

        [StringLength(50)]
        public string Name { get; set; }

        public ulong CardId { get; set; }

        [JsonIgnore]
        public virtual Card Card { get; set; }
    }
}
