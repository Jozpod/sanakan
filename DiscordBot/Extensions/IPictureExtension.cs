using System.Collections.Generic;
using Shinden.Models;

namespace Sanakan.Extensions
{
    public static class IPictureExtension
    {
        public static string GetStr(this IPicture p)
        {
            if (p == null || p.Is18Plus)
                return null;

            return Shinden.API.Url.GetPersonPictureURL(p.PictureId);
        }
    }
}
