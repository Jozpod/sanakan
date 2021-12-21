using System;

namespace Sanakan.Common.Configuration
{
    public class ShindenApiConfiguration
    {
        /// <summary>
        /// The option which specifies whether to use fake in-memory API.
        /// </summary>
        public bool UseFake { get; set; }

        /// <summary>
        /// The Shinden API base url.
        /// </summary>
        public Uri BaseUrl { get; set; } = null;

        /// <summary>
        /// The Shinden API key.
        /// </summary>
        public string Token { get; set; } = null;

        /// <summary>
        /// The user agent.
        /// </summary>
        public string UserAgent { get; set; } = null;

        /// <summary>
        /// The mysterious HTTP header used by Shinden API.
        /// </summary>
        public string? Marmolade { get; set; }

        /// <summary>
        /// The the interval of session once agent has logged in.
        /// </summary>
        public TimeSpan SessionExiry { get; set; }
    }
}
