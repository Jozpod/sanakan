using Shinden.Models;
using Shinden.Models.Entities;
using Shinden.Models.Initializers;
using System.Collections.Generic;
using System.Web;

namespace Shinden.Extensions
{
    public static class ReviewsExtension
    {
        public static List<IReview> ToModel(this API.TitleReviews rev, ulong titleId)
        {
            var list = new List<IReview>();
            foreach(var item in rev.Reviews) list.Add(item.ToModel(titleId));
            return list;
        }

        public static ISimpleUser ToUserModel(this API.Review info)
        {
            ulong.TryParse(info?.UserId, out var uID);
            ulong.TryParse(info?.Avatar, out var avatar);

            return new SimpleUser(uID, info?.Name,  avatar);
        }

        public static IReview ToModel(this API.Review info, ulong titleId)
        {
            ulong.TryParse(info?.ReviewId, out var rID);

            long.TryParse(info?.Rating, out var rating);
            long.TryParse(info?.EnterCnt, out var eCount);
            long.TryParse(info?.ReadCnt, out var reCount);
            long.TryParse(info?.RateCount, out var rCount);

            bool.TryParse(info?.IsAbstract, out var abs);

            return new Review(new InitReview()
            {
                Id = rID,
                Rating = rating,
                IsAbstract = abs,
                TitleId = titleId,
                RateCount = rCount,
                ReadCount = reCount,
                EnterCount = eCount,
                Author = info.ToUserModel(),
                Content = HttpUtility.HtmlDecode(info?.ReviewContent),
            });
        }
    }
}
