using System.Collections.Generic;

namespace Sanakan.ShindenApi.Fake.Models.WebScraper
{
    public record BasicAnimeDetail(ulong Id, string Name, ulong? ImageId = null);

    public record AnimeDetail(
        ulong Id,
        string Name,
        IllustrationType Type,
        IEnumerable<CharacterDetail> Characters,
        ulong? ImageId = null) : BasicAnimeDetail(Id, Name, ImageId);
}
