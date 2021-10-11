namespace Shinden.Models
{
    public interface IDedicatedTime
    {
         ulong? Days { get; }
         ulong? Years { get; }
         ulong? Hours { get; }
         ulong? Months { get; }
         ulong? Minutes { get; }
         ulong? TotalMinutes { get; }
    }
}