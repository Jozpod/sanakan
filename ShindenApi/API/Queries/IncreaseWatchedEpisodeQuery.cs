using System.Net.Http;
using Shinden.Extensions;
using System.Text;

namespace Shinden.API
{
    public class IncreaseWatchedEpisode : QueryPost<IncreaseWatched>
    {
        public IncreaseWatchedEpisode(ulong userId, ulong titleId)
        {
            UserId = userId;
            TitleId = titleId;
        }

        private ulong UserId { get; }
        private ulong TitleId { get; }

        // Query
        public override string QueryUri => $"{BaseUri}userlist/{UserId}/increase-watched/{TitleId}";
        public override string Uri => $"{QueryUri}?api_key={Token}";
    }
}
