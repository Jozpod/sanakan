namespace Sanakan.Configuration
{
    /// <summary>
    /// Describes the rich message configuration.
    /// </summary>
    public class RichMessageConfig
    {
        /// <summary>
        /// The discord role identifier.
        /// </summary>
        public ulong RoleId { get; set; }

        /// <summary>
        /// The discord guild identifier.
        /// </summary>
        public ulong GuildId { get; set; }

        /// <summary>
        /// The discord channel identifier.
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
