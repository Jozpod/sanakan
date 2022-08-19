using Sanakan.Common;
using Sanakan.DAL.Models;
using Sanakan.DiscordBot;
using System;

namespace Sanakan.Extensions
{
    public static class TimeStatusExtension
    {
        public static Discord.IEmote Icon(this StatusType type) => Discord.Emote.Parse(type.GetEmoteString());

        public static string ToView(this TimeStatus status, DateTime dateTime)
        {
            if (status.Type.IsQuest())
            {
                var max = (uint)status.Type.ToComplete();
                var actualProgress = status.IsActive(dateTime) ? status.IntegerValue : 0;

                var progress = (actualProgress >= max) ? (status.BooleanValue ? Emotes.IconFull.ToString() : Emotes.IconEmpty.ToString())
                    : $"[{actualProgress}/{status.Type.ToComplete()}]";

                var reward = status.IsActive(dateTime)
                    && status.BooleanValue ? "" : $"\nNagroda: `{status.Type.GetRewardString()}`";

                return $"{status.Type.Icon()} **{status.Type.Name()}** {progress}{reward}";
            }

            var dateValue = status.EndsOn!.Value.ToString(Placeholders.ddMMyyyyHHmm);

            if (status.EndsOn < dateTime)
            {
                dateValue = "nieaktywne";
            }

            return $"{status.Type.Name()} do `{dateValue}`";
        }
    }
}
