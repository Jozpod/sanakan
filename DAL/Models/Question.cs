using Sanakan.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Sanakan.DAL.Models
{
    public class Question
    {
        public Question()
        {
            Answers = new List<Answer>();
        }

        public ulong Id { get; set; }

        public int Group { get; set; }

        public int AnswerNumber { get; set; }

        public int PointsWin { get; set; }

        public int PointsLose { get; set; }

        [StringLength(100)]
        public string Content { get; set; } = string.Empty;

        public TimeSpan TimeToAnswer { get; set; }

        public virtual IList<Answer> Answers { get; set; }

        public bool CheckAnswer(int answerNumber) => AnswerNumber == answerNumber;

        public string GetRightAnswer()
            => $"Prawidłowa odpowiedź to: **{AnswerNumber}** - {Answers.First(x => x.Number == AnswerNumber).Content}";

        public string Get()
        {
            var str = $"**{Content}**\n\n";
            foreach (var answer in Answers)
            {
                str += $"**{answer.Number}**: {answer.Content}\n";
            }

            return str;
        }

        public void RandomizeAnswers(IRandomNumberGenerator randomNumberGenerator)
        {
            var numbersColeration = new List<(int, int)>();
            var possibleAnswers = Answers.Select(x => x.Number).ToList();

            foreach (var answer in Answers)
            {
                var num = randomNumberGenerator.GetOneRandomFrom(possibleAnswers);
                possibleAnswers.Remove(num);

                numbersColeration.Add((num, answer.Number));
                answer.Number = num;
            }

            AnswerNumber = numbersColeration.First(x => x.Item2 == AnswerNumber).Item1;
            Answers = Answers.OrderBy(x => x.Number).ToList();
        }
    }
}