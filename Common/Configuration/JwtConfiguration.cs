using System;

namespace Sanakan.Common.Configuration
{
    public class JwtConfiguration
    {
        /// <summary>
        /// The signing key.
        /// </summary>
        public string IssuerSigningKey { get; set; } = string.Empty;

        /// <summary>
        /// The JWT issuer.
        /// </summary>
        public string Issuer { get; set; } = string.Empty;

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
