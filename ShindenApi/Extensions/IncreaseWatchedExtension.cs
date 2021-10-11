using Shinden.Models;
using Shinden.Models.Entities;

namespace Shinden.Extensions
{
    public static class IncreaseWatchedExtension
    {
        public static IEmptyResponse ToModel(this API.IncreaseWatched mod)
        {
            return new EmptyResponse(mod.Status == "ok" ? $"Update successful. EP: {mod.EpisodeAdded}" : "Update unsuccessful.");
        }
    }
}
