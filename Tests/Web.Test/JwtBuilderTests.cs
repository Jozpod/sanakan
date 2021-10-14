using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sanakan.Web.Test
{
    [TestClass]
    public class JwtBuilderTests
    {
        private readonly IJwtBuilder _jwtBuilder;

        public JwtBuilderTests()
        {
            _jwtBuilder = new JwtBuilder();
        }

        
        [TestMethod]
        public async Task Should_Generate_Token()
        {
            var userId = 0;
            var tokenData = await _jwtBuilder.Build(userId);
        }
    }
}
