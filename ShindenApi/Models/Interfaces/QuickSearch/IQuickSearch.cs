namespace Shinden.Models
{
    public interface IQuickSearch : IIndexable
    {
         string Title { get; }
         string CoverUrl { get; }
    }
}