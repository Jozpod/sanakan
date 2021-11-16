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

        /// <summary>
        /// The time span after JWT expires.
        /// </summary>
        public TimeSpan TokenExpiry { get; set; }

        /// <summary>
        /// The time span after JWT with user expires.
        /// </summary>
        public TimeSpan UserWithTokenExpiry { get; set; }
    }
}
