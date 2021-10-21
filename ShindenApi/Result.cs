using System;
using System.Collections.Generic;
using System.Text;

namespace Sanakan.ShindenApi
{
    public class Result<T>
    {
        public T? Value { get; set; }
    }
}
