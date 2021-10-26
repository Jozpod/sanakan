using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Sanakan.DAL.Models
{
    public class Question
    {
        public ulong Id { get; set; }
        public int Group { get; set; }
        public int Answer { get; set; }
        public int PointsWin { get; set; }
        public int PointsLose { get; set; }

        [StringLength(100)]
        public string Content { get; set; } = string.Empty;
        public TimeSpan TimeToAnswer { get; set; }

        public virtual ICollection<Answer> Answers { get; set; }
    }
}