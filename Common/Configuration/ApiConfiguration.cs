using Sanakan.Configuration;
using System;
using System.Collections.Generic;

namespace Sanakan.Common.Configuration
{
    public class ApiConfiguration
    {
        /// <summary>
        /// The list of API keys.
        /// </summary>
        public JwtConfiguration Jwt { get; set; } = new();

        /// <summary>
        /// The list of API keys.
        /// </summary>
        public List<SanakanApiKey> ApiKeys { get; set; } = new();
    }
}
