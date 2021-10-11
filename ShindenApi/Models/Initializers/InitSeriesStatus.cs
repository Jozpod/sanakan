namespace Shinden.Models.Initializers
{
    public class InitSeriesStatus
    {
        public ulong? InProgress { get; set; }
        public ulong? Completed { get; set; }
        public ulong? Dropped { get; set; }
        public ulong? Skipped { get; set; }
        public ulong? InPlan { get; set; }
        public ulong? OnHold { get; set; }
    }
}