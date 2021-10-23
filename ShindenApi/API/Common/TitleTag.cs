using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sanakan.ShindenApi.API.Common
{
    public class TitleTag
    {
        [JsonProperty("tag_id")]
        public string TagId { get; set; }

        [JsonProperty("relation")]
        public string Relation { get; set; }

        [JsonProperty("title_id")]
        public string TitleId { get; set; }

        [JsonProperty("title_tag_id")]
        public string TitleTagId { get; set; }
    }
}
