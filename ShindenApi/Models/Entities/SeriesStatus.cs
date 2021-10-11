using Shinden.Models.Initializers;

namespace Shinden.Models.Entities
{
    public class SeriesStatus : ISeriesStatus
    {
        public SeriesStatus(InitSeriesStatus Init)
        {
            InPlan = Init.InPlan;
            OnHold = Init.OnHold;
            Dropped = Init.Dropped;
            Skipped = Init.Skipped;
            Completed = Init.Completed;
            InProgress = Init.InProgress;
        }

        // ISeriesStatus
        public ulong? InProgress { get; }
        public ulong? Completed { get; }
        public ulong? Dropped { get; }
        public ulong? Skipped { get; }
        public ulong? InPlan { get; }
        public ulong? OnHold { get; }

        public ulong? Total => InProgress + Completed + Dropped + Skipped + InPlan + OnHold;

        public override string ToString() => $"{Total}";
    }
}
