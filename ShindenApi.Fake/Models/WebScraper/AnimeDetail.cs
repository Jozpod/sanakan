using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sanakan.ShindenApi.Fake.Models.WebScraper
{
    public record BasicAnimeDetail(ulong Id, string Name);
    public record AnimeDetail(ulong Id, string Name, IEnumerable<CharacterDetail> Characters, ulong? ImageId = null) : BasicAnimeDetail(Id, Name);
}
