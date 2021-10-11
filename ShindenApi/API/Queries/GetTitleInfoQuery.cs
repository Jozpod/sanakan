using Newtonsoft.Json;

namespace Shinden.API
{
    public class GetTitleInfoQuery : QueryGet<AnimeMangaInfo>
    {
        public GetTitleInfoQuery(ulong id)
        {
            Id = id;
        }

        private ulong Id { get; }

        // Query
        public override string QueryUri => $"{BaseUri}title/{Id}/info";
        public override string Uri => $"{QueryUri}?api_key={Token}&lang=pl&decode=1";
        public override AnimeMangaInfo Parse(string json)
        {
            return JsonConvert.DeserializeObject<AnimeMangaInfo>(json.Replace("\"tags\":[],", ""));
        }
    }
}
