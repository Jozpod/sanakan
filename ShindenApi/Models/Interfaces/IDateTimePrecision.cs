using System;

namespace Shinden.Models
{
    public interface IDateTimePrecision
    {
        DateTime Date { get; }
        ulong? Precision { get; }
        bool IsSpecified { get; }
    }
}