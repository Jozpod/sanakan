using System.Collections.Generic;

namespace Shinden.Models
{
    public interface ITagCategory
    {
        string Name { get; }
        List<ITag> Tags { get; }
    }
}