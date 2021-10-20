using System;
using System.Collections.Generic;
using System.Text;

namespace Sanakan.DAL.Repositories
{
    public class CardQueryOptions
    {
        public bool IncludeTagList { get; set; }
        public bool AsSingleQuery { get; set; }
        public bool IncludeArenaStats { get; set; }
        public bool IncludeGameDeck { get; set; }
        public bool AsNoTracking { get; set; }
    }
}
