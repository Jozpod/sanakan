using Sanakan.ShindenApi.Models.Enums;
using System.Collections.Generic;

namespace Sanakan.ShindenApi.Converters
{
    public class LanguageConverter : EnumConverter<Language>
    {
        private static IDictionary<string, Language> _map = new Dictionary<string, Language>
        {
            { "pl", Language.Polish },
            { "kr", Language.Korean },
            { "ko", Language.Korean },
            { "cn", Language.Chinese },
            { "en", Language.English },
            { "jp", Language.Japanese },
        };

        public LanguageConverter()
            : base(_map, Language.NotSpecified)
        {
        }
    }
}