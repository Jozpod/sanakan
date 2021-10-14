using System.Collections.Generic;

namespace Sanakan.Web.Configuration
{
    public class ConfigModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string Prefix { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string BotToken { get; set; }

        /// <summary>
        /// Enables flood/spam supervision
        /// </summary>
        public bool Supervision { get; set; }

        /// <summary>
        /// Exits app if it detects discord timeout.
        /// </summary>
        public bool Demonization { get; set; }

        /// <summary>
        ///  Generates cards from user messages.
        /// </summary>
        public bool SafariEnabled { get; set; }

        /// <summary>
        /// Character for one cards packet.
        /// </summary>
        public long CharPerPacket { get; set; }
        public ConfigShinden Shinden { get; set; }
        public ConfigExp Exp { get; set; }
        public List<ulong> Dev { get; set; }
        public JwtConfig Jwt { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public List<SanakanApiKey> ApiKeys { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public List<RichMessageConfig> RMConfig { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<ulong> BlacklistedGuilds { get; set; }
    }
}