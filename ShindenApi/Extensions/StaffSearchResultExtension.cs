using Shinden.API;
using Shinden.Models;
using Shinden.Models.Entities;
using System.Collections.Generic;

namespace Shinden.Extensions
{
    public static class StaffSearchResultExtension
    {
        public static IPersonSearch ToModel(this StaffSearchResult r)
        {
            ulong.TryParse(r?.Picture, out var pictureid);
            ulong.TryParse(r?.Id, out var userid);

            return new PersonSearch(userid, pictureid, r?.FirstName, r?.LastName, true);
        }

        public static List<IPersonSearch> ToModel(this List<StaffSearchResult> rList)
        {
            var list = new List<IPersonSearch>();
            foreach (var item in rList) list.Add(item.ToModel());
            return list;
        }
    }
}