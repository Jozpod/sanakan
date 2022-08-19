using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Sanakan.DAL.Models
{
    public class CardTag
    {
        [StringLength(30)]
        [Required]
        public string Name { get; set; } = string.Empty;

        public ulong CardId { get; set; }

        [JsonIgnore]
        public virtual Card Card { get; set; } = null;
    }
}
