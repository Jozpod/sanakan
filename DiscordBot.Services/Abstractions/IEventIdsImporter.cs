using System.Threading.Tasks;

namespace Sanakan.DiscordBot.Services.Abstractions
{
    public interface IEventIdsImporter
    {
        Task<EventIdsImporterResult> RunAsync(string url);
    }
}
