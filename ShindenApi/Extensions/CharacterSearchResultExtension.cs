using Shinden.API;
using Shinden.Models;
using Shinden.Models.Entities;
using System.Collections.Generic;

namespace Shinden.Extensions
{
    public static class CharacterSearchResultExtension
    {
        public static IPersonSearch ToModel(this CharacterSearchResult r)
        {
            ulong.TryParse(r?.Picture, out var pictureid);
            ulong.TryParse(r?.Id, out var userid);

            return new PersonSearch(userid, pictureid, r?.FirstName, r?.LastName, false);
        }

        public static List<IPersonSearch> ToModel(this List<CharacterSearchResult> rList)
        {
            var list = new List<IPersonSearch>();
            foreach (var item in rList) list.Add(item.ToModel());
            return list;
        }
    }
}