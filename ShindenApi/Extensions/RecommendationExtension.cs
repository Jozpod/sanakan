using Shinden.Models;
using Shinden.Models.Entities;
using Shinden.Models.Initializers;
using System;
using System.Collections.Generic;
using System.Web;

namespace Shinden.Extensions
{
    public static class RecommendationExtension
    {
        public static List<IRecommendation> ToModel(this API.TitleRecommendation rec)
        {
            var list = new List<IRecommendation>();
            foreach(var item in rec.Recommendations) list.Add(item.ToModel());
            return list;
        }

        public static DateTime GetDate(this API.Recommendation info)
        {
            if (info?.AddDate == null) return DateTime.MinValue;

            return DateTime.ParseExact(info.AddDate, "yyyy-MM-dd HH:mm:ss",
                System.Globalization.CultureInfo.InvariantCulture);
        }

        public static ISimpleUser ToUserModel(this API.Recommendation info)
        {
            ulong.TryParse(info?.UserId, out var uID);
            ulong.TryParse(info?.Avatar, out var avatar);

            return new SimpleUser(uID, info?.Name,  avatar);
        }

        public static ISimpleTitleInfo ToTitleModel(this API.Recommendation info)
        {
            ulong.TryParse(info?.RtitleId, out var tID);
            ulong.TryParse(info?.CoverArtifactId, out var cID);

            return new SimpleTitleInfo(tID, info?.Title, cID);
        }

        public static IRecommendation ToModel(this API.Recommendation info)
        {
            ulong.TryParse(info?.RecommendationId, out var rID);
            ulong.TryParse(info?.RecTitleId, out var tID);

            long.TryParse(info?.Rating, out var rating);
            long.TryParse(info?.RateCount, out var rCount);

            return new Recommendation(new InitRecommendation()
            {
                Id = rID,
                Rating = rating,
                BaseTitleId = tID,
                RateCount = rCount,
                AddDate = info.GetDate(),
                Author = info.ToUserModel(),
                RecommendedTitle = info.ToTitleModel(),
                Content = HttpUtility.HtmlDecode(info?.RecommendationContent),
            });
        }
    }
}
