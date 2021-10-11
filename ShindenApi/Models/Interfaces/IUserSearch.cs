namespace Shinden.Models
{
    public interface IUserSearch : ISimpleUser
    {
        string Rank { get; }
    }
}