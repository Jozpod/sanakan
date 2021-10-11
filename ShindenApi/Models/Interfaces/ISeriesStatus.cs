namespace Shinden.Models
{
    public interface ISeriesStatus
    {
         ulong? InProgress { get; }
         ulong? Completed { get; }
         ulong? Dropped { get; }
         ulong? Skipped { get; }
         ulong? InPlan { get; }
         ulong? OnHold { get; }
         ulong? Total { get; }
    }
}