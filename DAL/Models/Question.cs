using System.Collections.Generic;

namespace Sanakan.DAL.Models
{
    public class Question
    {
        public ulong Id { get; set; }
        public int Group { get; set; }
        public int Answer { get; set; }
        public int PointsWin { get; set; }
        public int PointsLose { get; set; }
        public string Content { get; set; }
        public int TimeToAnswer { get; set; }

        public virtual ICollection<Answer> Answers { get; set; }
    }
}