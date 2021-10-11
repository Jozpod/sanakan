namespace Shinden.API
{
    public class RemoveTitleFromListQuery : QueryDelete<TitleStatusAfterChange>
    {
        public RemoveTitleFromListQuery(ulong userId, ulong titleId)
        {
            UserId = userId;
            TitleId = titleId;
        }

        private ulong UserId { get; }
        private ulong TitleId {  get; }

        // Query
        public override string QueryUri => $"{BaseUri}userlist/{UserId}/series/{TitleId}";
        public override string Uri => $"{QueryUri}?api_key={Token}";
    }
}
