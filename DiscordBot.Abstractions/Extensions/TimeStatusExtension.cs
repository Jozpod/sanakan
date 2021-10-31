using System;
using System.Collections.Generic;
using Sanakan.DAL.Models;

namespace Sanakan.Extensions
{
    public static class TimeStatusExtension
    {
        private static Discord.Emote _toClaim = Discord.Emote.Parse("<:icon_empty:829327739034402817>");
        private static Discord.Emote _claimed = Discord.Emote.Parse("<:icon_full:829327738233421875>");

        public static Discord.IEmote Icon(this StatusType type) => Discord.Emote.Parse(type.GetEmoteString());

        public static bool IsQuest(this StatusType type) => type.IsWeeklyQuestType() || type.IsDailyQuestType();

        public static void Count(this TimeStatus status, DateTime currentDate, uint times = 1)
        {
            if (status.Type.IsQuest())
            {
                if (status.IsActive(currentDate) && !status.BValue)
                {
                    status.IValue += times;
                }
                else if (!status.IsActive(currentDate))
                {
                    status.IValue = times;
                    status.BValue = false;

                    if (status.Type.IsDailyQuestType())
                    {
                        status.EndsAt = currentDate.Date.AddDays(1);
                    }

                    if (status.Type.IsWeeklyQuestType())
                    {
                        status.EndsAt = currentDate.Date.AddDays(7 - (int)currentDate.DayOfWeek);
                    }
                }
            }

            var max = (ulong)status.Type.ToComplete();
            if (max > 0 && status.IValue > max)
            {
                status.IValue = max;
            }
        }

        public static void Claim(this TimeStatus status, User user)
        {
            if (status.BValue)
            {
                return;
            }

            status.BValue = true;

            switch(status.Type)
            {
                case StatusType.DExpeditions:
                case StatusType.DUsedItems:
                    user.AcCount += 1;
                    break;

                case StatusType.DHourly:
                    user.ScCount += 100;
                    break;

                case StatusType.DPacket:
                case StatusType.DMarket:
                    user.AcCount += 2;
                    break;

                case StatusType.DPvp:
                    user.GameDeck.PVPCoins += 200;
                    break;

                case StatusType.WCardPlus:
                    user.AcCount += 50;
                    break;

                case StatusType.WDaily:
                    user.ScCount += 1000;
                    user.AcCount += 10;
                    break;

                default:
                    break;
            }
        }

        public static bool IsClaimed(this TimeStatus status, DateTime dateTime)
            => status.IsActive(dateTime) && status.BValue;

        public static bool CanClaim(this TimeStatus status, DateTime dateTime)
            => status.IsActive(dateTime) && !status.BValue
            && status.Type.IsQuest() && status.IValue >= (uint)status.Type.ToComplete();

        public static double RemainingMinutes(this TimeStatus status, DateTime dateTime)
            => (status.EndsAt.Value - dateTime).TotalMinutes;

        public static double RemainingSeconds(this TimeStatus status, DateTime dateTime)
            => (status.EndsAt.Value - dateTime).TotalSeconds;

        public static string ToView(this TimeStatus status, DateTime dateTime)
        {
            if (status.Type.IsQuest())
            {
                var max = (uint)status.Type.ToComplete();
                var actualProgress = status.IsActive(dateTime) ? status.IValue : 0;

                var progress = (actualProgress >= max) ? (status.BValue ? _claimed.ToString() : _toClaim.ToString())
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
