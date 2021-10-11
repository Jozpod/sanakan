namespace Shinden.Models
{
    public interface IAlternativeTitle : INationalText, IIndexable
    {
        AlternativeTitleType Type { get; }
    }
}