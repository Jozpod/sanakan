﻿using System;
using System.Diagnostics.CodeAnalysis;

namespace Sanakan.ShindenApi
{
    [ExcludeFromCodeCoverage]
    public static class Constants
    {
        public const string ShindenUrl = "https://shinden.pl/";
        public const string ShindenCdnUrl = "http://cdn.shinden.eu/";
        public const string ApiShindenUrl = "https://api.shinden.pl/api/";

        public static Uri PlaceholderImageUrl = new Uri($"{ShindenCdnUrl}cdn1/other/placeholders/title/225x350.jpg");

        public static Uri GetPlaceholderUserImageUrl = new Uri($"{ShindenCdnUrl}cdn1/other/placeholders/user/100x100.jpg");
    }
}
