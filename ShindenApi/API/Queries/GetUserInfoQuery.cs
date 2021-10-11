namespace Shinden.API
{
    public class GetUserInfoQuery : QueryGet<UserInfo>
    {
        public GetUserInfoQuery(ulong id)
        {
            Id = id;
        }

        private ulong Id { get; }

        // Query
        public override string QueryUri => $"{BaseUri}user/{Id}/info";
        public override string Uri => $"{QueryUri}?api_key={Token}";
    }
}
