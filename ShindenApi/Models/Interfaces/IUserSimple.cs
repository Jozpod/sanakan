namespace Shinden.Models
{
    public interface ISimpleUser : IIndexable
    {
        string Name { get; }
        string AvatarUrl { get; }
    }
}