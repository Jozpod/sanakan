namespace Shinden.Models
{
    public interface IPersonSearch : IIndexable
    {
        string LastName { get; }
        string FirstName { get; }
        string PersonUrl { get; }
        string PictureUrl { get; }
    }
}