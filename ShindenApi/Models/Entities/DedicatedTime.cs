using Shinden.Models.Initializers;

namespace Shinden.Models.Entities
{
    public class DedicatedTime : IDedicatedTime
    {
        public DedicatedTime(InitDedicatedTime Init)
        {
            Days = Init.Days;
            Years = Init.Years;
            Hours = Init.Hours;
            Months = Init.Months;
            Minutes = Init.Minutes;
        }

        // IDedicatedTime
        public ulong? Days { get; }
        public ulong? Years { get; }
        public ulong? Hours { get; }
        public ulong? Months { get; }
        public ulong? Minutes { get; }

        public ulong? TotalMinutes =>
            Minutes + (Hours * 60) + (Days * 24 * 60) + 
            (Months * 31 * 24 * 60) + (Years * 12 * 31 * 24 * 60);

        public override string ToString() => $"{TotalMinutes}";
    }
}
