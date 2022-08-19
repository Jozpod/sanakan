namespace Sanakan.Configuration
{
    /// <summary>
    /// Describes the rich message configuration.
    /// </summary>
    public class RichMessageConfig
    {
        /// <summary>
        /// The Discord role identifier which is used to mention.
        /// </summary>
        public ulong RoleId { get; set; }

        /// <summary>
        /// The Discord guild (server) identifier of the guild (server) where to post messages.
        /// </summary>
        public ulong GuildId { get; set; }

        /// <summary>
        /// The Discord channel identifier of the channel where to post messages.
        /// </summary>
        public ulong ChannelId { get; set; }

        /// <summary>
        /// The rich message type.
        /// </summary>
        public RichMessageType Type { get; set; }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"Serwer: {GuildId}\nRola: {RoleId}\nKanał: {ChannelId}\nTyp: {Type}";
        }
    }
}
