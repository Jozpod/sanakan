using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Sanakan.DAL.Models
{
    public class Answer
    {
        public ulong Id { get; set; }
        public int Number { get; set; }

        [StringLength(50)]
        public string Content { get; set; } = string.Empty;

        public ulong QuestionId { get; set; }
        [JsonIgnore]
        public virtual Question Question { get; set; } = new();
    }
}