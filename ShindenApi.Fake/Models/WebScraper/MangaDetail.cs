using System.Collections.Generic;

namespace Sanakan.ShindenApi.Fake.Models.WebScraper
{
    public record BasicMangaDetail(ulong Id, string Name);
    public record MangaDetail(ulong Id, string Name, IEnumerable<CharacterDetail> Characters, ulong? ImageId = null) : BasicAnimeDetail(Id, Name);
}
