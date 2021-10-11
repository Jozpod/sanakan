namespace Shinden.Models
{
    public interface IStaffBiography : INationalText, IIndexable
    {
        ulong StaffId { get; }
    }
}