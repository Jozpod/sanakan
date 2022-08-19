namespace Sanakan.ShindenApi.Fake.Models.WebScraper
{
    public record CharacterDetail(
        ulong Id,
        string Name,
        string? Biography = null,
        ulong? ImageId = null);
}
