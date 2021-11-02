using Newtonsoft.Json;
using System;

namespace Sanakan.DAL.Models
{

    public class TimeStatus
    {
        private TimeStatus() { }

        public TimeStatus(StatusType statusType, ulong? guildId = null)
        {
            Type = statusType;
            GuildId = guildId;
        }

        public ulong Id { get; set; }
        public StatusType Type { get; set; }
        public DateTime? EndsAt { get; set; }

        public ulong IValue { get; set; }
        public bool BValue { get; set; }

        public ulong? GuildId { get; set; }
        public ulong UserId { get; set; }

        [JsonIgnore]
        public virtual User User { get; set; }

        public void Reset()
        {
            IValue = 0;
            BValue = false;
            EndsAt = null;
        }

        public bool IsClaimed(DateTime dateTime)
            => IsActive(dateTime) && BValue;

        public bool CanClaim(DateTime dateTime)
            => IsActive(dateTime) && !BValue
            && Type.IsQuest() && IValue >= (uint)Type.ToComplete();

        public TimeSpan RemainingTime(DateTime currentTime) => EndsAt.Value - currentTime;

        public void Count(DateTime currentDate, uint times = 1)
        {
            if (!Type.IsQuest())
            {
                return;
            }

            if (IsActive(currentDate) && !BValue)
            {
                IValue += times;
            }

            else if (!IsActive(currentDate))
            {
                IValue = times;
                BValue = false;

                if (Type.IsDailyQuestType())
                {
                    EndsAt = currentDate.Date.AddDays(1);
                }

                if (Type.IsWeeklyQuestType())
                {
                    EndsAt = currentDate.Date.AddDays(7 - (int)currentDate.DayOfWeek);
                }
            }

            var max = (ulong)Type.ToComplete();
            if (max > 0 && IValue > max)
            {
                IValue = max;
            }
        }

        public void Claim(User user)
        {
            if (BValue)
            {
                return;
            }

            BValue = true;

            switch (Type)
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

        public bool IsActive(DateTime dateTime) => EndsAt.HasValue && EndsAt.Value < dateTime;
    }
}
