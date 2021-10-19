using System;
using System.Collections.Generic;
using System.Text;

namespace Sanakan.DAL.Repositories
{
    public class UserQueryOptions
    {
        public bool IncludeGameDeck { get; set; }
        public bool IncludeWishes { get; set; }
        public bool IncludeCards { get; set; }
    }
}
