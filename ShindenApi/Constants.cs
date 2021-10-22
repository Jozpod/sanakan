using System;
using System.Collections.Generic;
using System.Text;

namespace Sanakan.ShindenApi
{
    public static class Constants
    {
        public const string ShindenUrl = "https://shinden.pl/";
        public const string ShindenCdnUrl = "http://cdn.shinden.eu/";
        public const string ApiShindenUrl = "https://api.shinden.pl/api/";
        public static string GetPlaceholderImageUrl = $"{ShindenCdnUrl}cdn1/other/placeholders/title/225x350.jpg";
        public static string GetPlaceholderUserImageUrl = $"{ShindenCdnUrl}cdn1/other/placeholders/user/100x100.jpg";
    }
}
