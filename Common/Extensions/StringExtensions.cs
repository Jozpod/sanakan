using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Sanakan.Common.Configuration;
using Sanakan.Configuration;

namespace Sanakan.Extensions
{
    public static class StringExtension
    {
        private static Regex _hexRegex = new ("^#(?:[0-9a-fA-F]{3}){1,2}$", RegexOptions.Compiled);
        public static Regex? CommandRegex;
        private static Regex _linkRegex = new ("(http|ftp|https)://[\\w-]+(\\.[\\w-]+)+([\\w.,@?^=%&:/~+#-]*[\\w@?^=%&/~+#-])?", RegexOptions.Compiled);
        private static Regex _quotedTextLengthRegex = new(@"(^>[ ][^\n]*\n)|(\n>[ ][^\n]*\n)|(\n>[ ][^\n]*$)", RegexOptions.Compiled);
        private static Regex _isEmotikunEmoteRegex = new(@"\B-\w+", RegexOptions.Compiled);
        private static Regex _replaceUrlRegex = new (@"\[url=['""]?([^\['""]+)['""]?\]([^\[]+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static readonly string[] _bbCodes =
      {
            "list", "quote", "code", "spoiler", "chk", "size", "color", "bg", "center", "right",
            "left", "font", "align", "mail", "img", "small", "sub", "sup", "p", "gvideo", "bull",
            "copyright", "registered", "tm", "indent", "iframe", "url", "youtube", "i", "b", "s",
            "u", "color", "size"
        };
        private static Regex _replaceBBCodesRegex = new($@"\[/?({string.Join('|', _bbCodes)})(=[^\]]*)?\]", RegexOptions.Compiled | RegexOptions.IgnoreCase);

       

        public static string ElipseTrimToLength(this string str, int length)
        {
            if (str == null)
            {
                return string.Empty;
            }
            if (str.Length <= length)
            {
                return str;
            }

            var charAr = str.ToCharArray();
            for (int i = 1; i < 4; i++)
            {
                charAr[length - i] = '.';
            }
            
            charAr[length] = '\0';

            return new (charAr, 0, Array.IndexOf(charAr, '\0'));
        }

        public static string ConvertBBCodeToMarkdown(this string str)
        {
            var stringBuilder = new StringBuilder(str);

            stringBuilder = stringBuilder
                .Replace("[*]", "— ")
                .Replace("[i]", "*")
                .Replace("[/i]", "*")
                .Replace("[b]", "**")
                .Replace("[/b]", "**")
                .Replace("[u]", "__")
                .Replace("[/u]", "__")
                .Replace("[s]", "~~")
                .Replace("[/s]", "~~")
                .Replace("[code]", "```")
                .Replace("[/code]", "```")
                .Replace("[youtube]", "https://www.youtube.com/watch?v=");

            str = _replaceUrlRegex.Replace(stringBuilder.ToString(), "[$2]($1)");

            return _replaceBBCodesRegex.Replace(str, "");
        }

        public static bool IsURLToImage(this string s)
        {
            var http = s.Split(':').FirstOrDefault();
            bool hasHttp = !(http == null || (!http.Equals("http") && !http.Equals("https")));

            bool hasRightExt = false;
            var ext = s.Split('.').LastOrDefault();
            var extensions = new string[] { "png", "jpg", "jpeg", "gif"};
            if (ext != null)
            {
                ext = ext.ToLower();
                hasRightExt = extensions.Any(x => x.Equals(ext));
            }

            return hasHttp && hasRightExt;
        }

        public static string GetQMarksIfEmpty(this string s)
        {
            if (string.IsNullOrEmpty(s))
                return "??";

            if (string.IsNullOrWhiteSpace(s))
                return "??";

            return s;
        }

      

        public static bool IsEmotikunEmote(this string message) => _isEmotikunEmoteRegex.Matches(message).Count > 0;

        public static int CountQuotedTextLength(this string message) => _quotedTextLengthRegex.Matches(message).Sum(x => x.Length);

        public static int CountLinkTextLength(this string message) => _linkRegex.Matches(message).Sum(x => x.Length);
        public static bool IsHexTriplet(this string message) => _hexRegex.IsMatch(message);
        public static bool IsCommand(this DiscordConfiguration sanakanConfiguration, string message)
        {
            if (CommandRegex == null)
            {
                var prefix = sanakanConfiguration.Prefix.Replace(".", @"\.").Replace("?", @"\?");
                CommandRegex = new Regex($@"^{prefix}\w+", RegexOptions.Compiled);
            }

            return CommandRegex.Matches(message).Count > 0;
        }
    }
}
