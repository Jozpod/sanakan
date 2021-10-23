using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sanakan.ShindenApi.API.Common
{
    public class AnimeMangaInfoDescription
    {
        [JsonProperty("description_id")]
        public ulong DescriptionId { get; set; }

        [JsonProperty("description")]
        public string OtherDescription { get; set; } // HttpUtility.HtmlDecode(desc?.OtherDescription)?.RemoveBBCode(),

        [JsonProperty("lang_code")]
        public string LangCode { get; set; }

        [JsonProperty("title_id")]
        public ulong TitleId { get; set; }
    }
}
