namespace Sanakan.Configuration
{
    /// <summary>
    /// Describes rich message type.
    /// </summary>
    public enum RichMessageType : byte
    {
        /// <summary>
        /// Describes missing type.
        /// </summary>
        None = 0,

        /// <summary>
        /// News channel.
        /// </summary>
        News = 1,

        /// <summary>
        /// Review channel.
        /// </summary>
        Review = 2,

        /// <summary>
        /// New epsiode channel.
        /// </summary>
        NewEpisode = 3,

        /// <summary>
        /// User private message.
        /// </summary>
        UserNotify = 4,

        /// <summary>
        /// New epsiode channel.
        /// </summary>
        NewEpisodePL = 5,

        /// <summary>
        /// Recommendation channel.
        /// </summary>
        Recommendation = 6,

        /// <summary>
        /// The message targeted at moderator channel.
        /// </summary>
        ModNotify = 7,

        /// <summary>
        /// The message targeted at season channel.
        /// </summary>
        NewSesonalEpisode = 8,

        /// <summary>
        /// The message targeted at report channel.
        /// </summary>
        AdminNotify = 9,
    }
}
