using System.Collections.Generic;
using System.Threading.Tasks;
using Sanakan.ShindenApi;
using Shinden.API;
using Shinden.Extensions;
using Shinden.Models;

namespace Shinden.Modules
{
    public class SearchModule
    {
        private readonly RequestManager _manager;

        public SearchModule(RequestManager manager)
        {
            _manager = manager;
        }

    }
}