using Shinden.API;
using Shinden.Models;
using Shinden.Models.Entities;
using Shinden.Models.Initializers;
using System.Collections.Generic;

namespace Shinden.Extensions
{
    public static class UserSearchResultExtension
    {
        public static IUserSearch ToModel(this UserSearchResult r)
        {
            ulong.TryParse(r?.Avatar, out var avatarid);
            ulong.TryParse(r?.Id, out var userid);

            return new UserSearch(new InitUserSearch()
            {
                Id = userid,
                Name = r?.Name,
                Rank = r?.Rank,
                AvatarId = avatarid,
            });
        }

        public static List<IUserSearch> ToModel(this List<UserSearchResult> rList)
        {
            var list = new List<IUserSearch>();
            foreach (var item in rList) list.Add(item.ToModel());
            return list;
        }
    }
}