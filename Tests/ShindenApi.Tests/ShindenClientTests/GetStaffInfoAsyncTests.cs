using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sanakan.ShindenApi.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Sanakan.ShindenApi.Tests
{
    [TestClass]
    public class GetStaffInfoAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Return_Staff_Info()
        {
            MockHttpOk("staff-info-result.json", HttpMethod.Get);

            var expected = new StaffInfo
            {
                StaffId = 1,
                StaffType = Models.Enums.StaffType.NotSpecified,
                FirstName = string.Empty,
                LastName = string.Empty,
                BirthPlace = string.Empty,
                PictureArtifactId = 1,
                Relations = new List<StaffInfoRelation>(),
                Biography = new StaffBio
                {
                    Biography = string.Empty,
                    Lang = string.Empty,
                    StaffBiographyId = string.Empty,
                    StaffId = string.Empty,
                }
            };

            var staffId = 1ul;
            var result = await _shindenClient.GetStaffInfoAsync(staffId);
            result.Value.Should().BeEquivalentTo(expected);
        }
    }
}
