using System;
using Sanakan.DAL.Models;
using Sanakan.DiscordBot;

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
                var actualProgress = status.IsActive(dateTime) ? status.IValue : 0;

                var progress = (actualProgress >= max) ? (status.BValue ? Emotes.IconFull.ToString() : Emotes.IconEmpty.ToString())
                    : $"[{actualProgress}/{status.Type.ToComplete()}]";

                var reward = status.IsActive(dateTime)
                    && status.BValue ? "" : $"\nNagroda: `{status.Type.GetRewardString()}`";

                return $"{status.Type.Icon()} **{status.Type.Name()}** {progress}{reward}";
            }

            var dateValue = status.EndsAt.Value.ToString("dd/MM/yyyy HH:mm");

            if (status.EndsAt < dateTime)
            {
                dateValue = "nieaktywne";
            }

            return $"{status.Type.Name()} do `{dateValue}`";
        }
    }
}
