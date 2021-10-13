using Newtonsoft.Json;

namespace Sanakan.DAL.Models
{
    public class Answer
    {
        public ulong Id { get; set; }
        public int Number { get; set; }
        public string Content { get; set; }

        public ulong QuestionId { get; set; }
        [JsonIgnore]
        public virtual Question Question { get; set; }
    }
}