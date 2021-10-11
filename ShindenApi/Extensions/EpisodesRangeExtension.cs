using Shinden.Models;
using Shinden.Models.Entities;

namespace Shinden.Extensions
{
    public static class EpisodesRangeExtension
    {
        public static IEpisodesRange ToModel(this API.EpisodesRange rang, ulong id)
        {
            long.TryParse(rang?.MaxNo, out var max);
            long.TryParse(rang?.MinNo, out var min);

            return new EpisodesRange(min, max, id);
        }
    }
}
