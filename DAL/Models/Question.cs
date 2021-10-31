using Sanakan.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

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

        public bool CheckAnswer(int ans) => Answer == ans;

        public string GetRightAnswer()
            => $"Prawidłowa odpowiedź to: **{Answer}** - {Answers.First(x => x.Number == Answer).Content}";

        public void RandomizeAnswers(IRandomNumberGenerator randomNumberGenerator)
        {
            var numbersColeration = new List<Tuple<int, int>>();
            var possibleAnswers = Answers.Select(x => x.Number).ToList();

            foreach (var answer in Answers)
            {
                var num = randomNumberGenerator.GetOneRandomFrom(possibleAnswers);
                possibleAnswers.Remove(num);

                numbersColeration.Add(new Tuple<int, int>(num, answer.Number));
                answer.Number = num;
            }

            Answer = numbersColeration.First(x => x.Item2 == Answer).Item1;
            Answers = Answers.OrderBy(x => x.Number).ToList();
        }

    }
}