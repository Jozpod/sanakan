using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Shinden.API
{
    public class GetNewEpisodesQuery : QueryGet<List<NewEpisode>>
    {
        // Query
        public override string QueryUri => $"{BaseUri}episode/new";
        public override string Uri => $"{QueryUri}?api_key={Token}";
        public override List<NewEpisode> Parse(string json)
        {
            var list = new List<NewEpisode>();
            var jsonObj = JObject.Parse(json);
            foreach (var item in jsonObj["lastonline"].Children())
            {
                list.Add(JsonConvert.DeserializeObject<NewEpisode>(item.ToString()));
            }
            return list;
        }
    }
}
