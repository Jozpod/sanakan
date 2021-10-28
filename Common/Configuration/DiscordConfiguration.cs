namespace Sanakan.Common.Configuration
{
    public class DiscordConfiguration
    {
        /// <summary>
        /// Runs commands on Discord only when they start with given prefix.
        /// </summary>
        public string Prefix { get; set; } = string.Empty;

        /// <summary>
        /// The Discord bot token.
        /// </summary>
        public string BotToken { get; set; } = string.Empty;
    }
}
