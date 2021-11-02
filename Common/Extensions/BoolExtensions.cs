using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sanakan.Common.Extensions
{
    public static class BoolExtensions
    {
        public static string GetYesNo(this bool b) => b ? "Tak" : "Nie";
    }
}
