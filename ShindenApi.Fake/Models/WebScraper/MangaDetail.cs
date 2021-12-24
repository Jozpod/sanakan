using System.Collections.Generic;

namespace Sanakan.ShindenApi.Fake.Models.WebScraper
{
    public record BasicMangaDetail(ulong Id, string Name, ulong? ImageId = null);
    public record MangaDetail(
        ulong Id,
        string Name,
        IllustrationType Type,
        IEnumerable<CharacterDetail> Characters,
        ulong? ImageId = null) : BasicMangaDetail(Id, Name, ImageId);
}
