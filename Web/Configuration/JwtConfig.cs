using System;

namespace Sanakan.Web.Configuration
{
    /// <summary>
    /// Describes JWT configuration.
    /// </summary>
    public class JwtConfig
    {
        /// <summary>
        /// The secret key.
        /// </summary>
        public string Key { get; set; } = string.Empty;

        /// <summary>
        /// The JWT issuer.
        /// </summary>
        public string Issuer { get; set; } = string.Empty;

        /// <summary>
        /// The time internval after which token expires
        /// </summary>
        public TimeSpan ExpiresOn { get; set; }
    }
}