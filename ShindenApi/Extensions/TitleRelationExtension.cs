using Shinden.Models;
using Shinden.Models.Entities;
using Shinden.Models.Initializers;
using System.Collections.Generic;
using System.Web;

namespace Shinden.Extensions
{
    public static class TitleRelationExtension
    {
        public static List<ITitleRelation> ToModel(this API.TitleRelations rev)
        {
            var list = new List<ITitleRelation>();
            foreach(var item in rev.RelatedTitles) list.Add(item.ToModel());
            return list;
        }

        public static ITitleRelation ToModel(this API.TitleRelation info)
        {
            ulong.TryParse(info?.RTitleId, out var rID);

            return new TitleRelation(rID, HttpUtility.HtmlDecode(info?.Title), info?.Name);
        }
    }
}
