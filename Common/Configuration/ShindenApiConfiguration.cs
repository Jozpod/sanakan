namespace Sanakan.Common.Configuration
{
    public class ShindenApiConfiguration
    {
        /// <summary>
        /// The Shinden API key.
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// The user agent.
        /// </summary>
        public string UserAgent { get; set; }

        /// <summary>
        /// The mysterious HTTP header used by Shinden API.
        /// </summary>
        public string? Marmolade { get; set; }
    }
}
