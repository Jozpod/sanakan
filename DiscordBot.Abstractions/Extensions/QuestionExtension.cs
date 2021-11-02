using System;
using System.Collections.Generic;
using System.Linq;
using Sanakan.Common;
using Sanakan.DAL.Models;
using Sanakan.DiscordBot;
using Sanakan.DiscordBot.Abstractions;

namespace Sanakan.Extensions
{
    public static class QuestionExtension
    {
        private static IDictionary<int, Discord.IEmote> NumberEmoteMap = new Dictionary<int, Discord.IEmote>
        {
            { 0, Emojis.Zero },
        };

        private static Discord.IEmote GetEmote(int i)
        {
            switch (i)
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
