using System;
using System.Collections.Generic;

namespace Shinden.Models
{
    public interface ISimpleTitleInfo : IIndexable
    {
        string Title { get; }
        string CoverUrl { get; }
    }
}
