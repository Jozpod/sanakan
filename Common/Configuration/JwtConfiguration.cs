namespace Sanakan.Common.Configuration
{
    public class JwtConfiguration
    {
        /// <summary>
        /// The signing key.
        /// </summary>
        public string IssuerSigningKey { get; set; } = string.Empty;

        /// <summary>
        /// The JWT issuer
        /// </summary>
        public string Issuer { get; set; } = string.Empty;
    }
}
