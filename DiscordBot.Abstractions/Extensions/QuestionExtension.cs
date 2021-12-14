using Sanakan.DAL.Models;
using Sanakan.DiscordBot.Abstractions;
using System.Linq;

namespace Sanakan.Extensions
{
    public static class QuestionExtension
    {
        private static Discord.IEmote GetEmote(int index)
        {
            switch (index)
            {
                case 0:
                    return Emojis.Zero;
                case 1:
                    return Emojis.One;
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

        public static Discord.IEmote[] GetEmotes(this Question question)
        {
            var count = question.Answers.Count;
            var emotes = new Discord.IEmote[count];
            var answers = question.Answers;

            for (var index = 0; index < count; index++)
            {
                emotes[index] = GetEmote(answers[index].Number);
            }

            return emotes.ToArray();
        }

        public static Discord.IEmote GetRightEmote(this Question question) => GetEmote(question.AnswerNumber);
    }
}
