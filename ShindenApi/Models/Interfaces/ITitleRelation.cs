namespace Shinden.Models
{
    public interface ITitleRelation : IIndexable
    {
        string Title { get; }
        string RelationType { get; }
    }
}