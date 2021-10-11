using System;
using System.Text.RegularExpressions;

namespace Shinden.Extensions
{
    public static class StringExtension
    {
        public static string TrimToLength(this string s, int length)
        {
            if (s.Length <= length) return s;

            var charAr = s.ToCharArray();
            charAr[length] = '\0';

            return new string(charAr, 0, Array.IndexOf(charAr, '\0'));
        }

        public static string RemoveBBCode(this string s)
        {
            return new Regex(@"\[(.*?)\]", RegexOptions.Compiled | RegexOptions.IgnoreCase).Replace(s, "");
        }
    }
}
