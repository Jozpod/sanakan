namespace Sanakan.DiscordBot.Services
{
    public enum EventIdsImporterState : byte
    {
        Ok = 0,
        InvalidStatusCode = 1,
        InvalidFileFormat = 2,
    }
}
