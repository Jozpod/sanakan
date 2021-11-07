using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using Sanakan.Common.Configuration;
using Sanakan.ShindenApi;
using Sanakan.ShindenApi.Models;
using Shinden.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Sanakan.ShindenApi.Tests
{
    [TestClass]
    public class GetReviewsAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Return_Reviews()
        {
            MockHttpOk("reviews-result.json", HttpMethod.Get);

            var review = new TitleReviews
            {
                Reviews = new List<Review>
                {
                    new Review
                    {
                        ReviewId = 1,
                        EnterCnt = 1,
                        RateCount = 1,
                        UserId = 1,
                        ReadCnt = 1,
                        Avatar = 1,
                        Name = string.Empty,
                        IsAbstract = true,
                        Rating = 1,
                        ReviewContent = string.Empty,
                    }
                }
            };

            var reviewId = 1ul;
            var result = await _shindenClient.GetReviewsAsync(reviewId);
            result.Value.Should().BeEquivalentTo(review);
        }
    }
}
