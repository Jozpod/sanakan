using System.Reflection;

namespace Shinden.Extensions
{
    public static class AuthExtension
    {
        public static string GetUserAgent(this Auth auth)
        {
            return $"{auth.UserAgent} (Shinden.NET/{Assembly.GetAssembly(typeof(ShindenClient)).GetName().Version})";
        }
    }
}