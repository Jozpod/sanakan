using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sanakan.Common.Cache
{
    public class MemoryCacheEntry<T>
    {
        public T? Value { get; set; }
    }
}
