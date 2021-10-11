namespace Shinden.API
{
    public class GetStaffInfoQuery : QueryGet<StaffInfo>
    {
        public GetStaffInfoQuery(ulong id)
        {
            Id = id;
        }

        private ulong Id { get; }

        // Query
        public override string QueryUri => $"{BaseUri}staff/{Id}";
        public override string Uri => $"{QueryUri}?api_key={Token}";
    }
}
