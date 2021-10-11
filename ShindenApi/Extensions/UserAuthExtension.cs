using System.Collections.Generic;

namespace Shinden.Extensions
{
    public static class UserAuthExtension
    {
        public static string Build(this UserAuth auth)
        {
            var list = new List<string>();
            list.AppendPOST("username", auth.Nickname);
            list.AppendPOST("password", auth.Password);
            return string.Join("&", list);
        }
    }
}