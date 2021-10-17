using System;

namespace Sanakan.Web.Configuration
{
    /// <summary>
    /// 
    /// </summary>
    public class JwtConfig
    {
        /// <summary>
        /// 
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Issuer { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public TimeSpan ExpiresOn { get; set; } = TimeSpan.Parse("");
    }
}