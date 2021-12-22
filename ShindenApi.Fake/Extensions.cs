using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Sanakan.ShindenApi.Fake
{
    public static class Extensions
    {
        private static Regex _idInLinkPattern = new Regex(@"\/(\d+).*", RegexOptions.Compiled);

        public static async Task<HtmlNode?> GetDocumentAsync(this HttpClient httpClient, string requireUri)
        {
            var response = await httpClient.GetAsync(requireUri);

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var stream = await response.Content.ReadAsStreamAsync();
            var htmlDocument = new HtmlDocument();
            htmlDocument.Load(stream);
            return htmlDocument.DocumentNode;
        }

        public static ulong? GetIdFromLink(this string link)
        {
            var match = _idInLinkPattern.Match(link);

            if (!match.Success)
            {
                return null;
            }

            var idStr = match.Groups[1].Value;
            ulong.TryParse(idStr, out var id);
            return id;
        }
    }
}
