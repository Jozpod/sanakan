using System.Text.Json.Serialization;
using System;

namespace Sanakan.DAL.Models
{

    public class TimeStatus
    {
        [JsonConstructor]
        private TimeStatus() { }

        public TimeStatus(StatusType statusType, ulong? guildId = null)
        {
            Type = statusType;
            GuildId = guildId;
        }

        public ulong Id { get; set; }

        public StatusType Type { get; set; }

        public DateTime? EndsOn { get; set; }

        /// <summary>
        /// The value might indicate:
        /// The acquired daily card bundles before <see cref="EndsOn"/>.
        /// </summary>
        public ulong IntegerValue { get; set; }

        public bool BooleanValue { get; set; }

        public ulong? GuildId { get; set; }

        public ulong UserId { get; set; }

        [JsonIgnore]
        public virtual User User { get; set; }

        public void Reset()
        {
            IntegerValue = 0;
            BooleanValue = false;
            EndsOn = null;
        }

        public bool IsClaimed(DateTime dateTime)
            => IsActive(dateTime) && BooleanValue;

        public bool CanClaim(DateTime dateTime)
            => IsActive(dateTime) && !BooleanValue
            && Type.IsQuest() && IntegerValue >= (uint)Type.ToComplete();

        public TimeSpan RemainingTime(DateTime currentTime) => EndsOn.HasValue ? EndsOn.Value - currentTime : TimeSpan.Zero;

        public void Count(DateTime currentDate, uint times = 1)
        {
            if (!Type.IsQuest())
            {
                return;
            }

            if (IsActive(currentDate) && !BooleanValue)
            {
                IntegerValue += times;
            }

            else if (!IsActive(currentDate))
            {
                IntegerValue = times;
                BooleanValue = false;

                if (Type.IsDailyQuestType())
                {
                    EndsOn = currentDate.Date.AddDays(1);
                }

                if (Type.IsWeeklyQuestType())
                {
                    EndsOn = currentDate.Date.AddDays(7 - (int)currentDate.DayOfWeek);
                }
            }

            var max = (ulong)Type.ToComplete();
            if (max > 0 && IntegerValue > max)
            {
                IntegerValue = max;
            }
        }

        public void Claim(User user)
        {
            if (BooleanValue)
            {
                return;
            }

            BooleanValue = true;

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

        public bool IsActive(DateTime dateTime) => EndsOn.HasValue && EndsOn.Value < dateTime;
    }
}
