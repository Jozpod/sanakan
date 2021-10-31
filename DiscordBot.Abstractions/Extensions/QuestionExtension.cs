using System;
using System.Collections.Generic;
using System.Linq;
using Sanakan.Common;
using Sanakan.DAL.Models;
using Sanakan.DiscordBot;

namespace Sanakan.Extensions
{
    public static class QuestionExtension
    {
        public static bool CheckAnswer(this Question q, int ans) => q.Answer == ans;

        public static string GetRightAnswer(this Question q)
            =>  $"Prawidłowa odpowiedź to: **{q.Answer}** - {q.Answers.First(x => x.Number == q.Answer).Content}";

        private static Discord.IEmote GetEmote(int i)
        {
            switch (i)
            {
                case 0:
                    return Emojis.Zero;
                case 1:
                    return Emojis.Zero;
                case 2:
                    return Emojis.Two;
                case 3:
                    return Emojis.Three;
                case 4:
                    return Emojis.Four;
                case 5:
                    return Emojis.Five;
                case 6:
                    return Emojis.Six;
                case 7:
                    return Emojis.Seven;
                case 8:
                    return Emojis.Eight;
                default:
                    return Emojis.Nine;
            };
        }

        public static void RandomizeAnswers(this Question q, IRandomNumberGenerator randomNumberGenerator)
        {
            var numbersColeration = new List<Tuple<int, int>>();
            var possibleAnswers = q.Answers.Select(x => x.Number).ToList();

            foreach (var answer in q.Answers)
            {
                var num = randomNumberGenerator.GetOneRandomFrom(possibleAnswers);
                possibleAnswers.Remove(num);

                numbersColeration.Add(new Tuple<int, int>(num, answer.Number));
                answer.Number = num;
            }

            q.Answer = numbersColeration.First(x => x.Item2 == q.Answer).Item1;
            q.Answers = q.Answers.OrderBy(x => x.Number).ToList();
        }

        public static string Get(this Question q)
        {
            string str = $"**{q.Content}**\n\n";
            foreach(var item in q.Answers)
            {
                str += $"**{item.Number}**: {item.Content}\n";
            }
            return str;
        }

        public static Discord.IEmote[] GetEmotes(this Question q)
        {
            List<Discord.IEmote> emo = new List<Discord.IEmote>();

            foreach (var qu in q.Answers)
            {
                emo.Add(GetEmote(qu.Number));
            }

            return emo.ToArray();
        }

        public static Discord.IEmote GetRightEmote(this Question q) => GetEmote(q.Answer);
    }
}
