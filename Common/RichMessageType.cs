namespace Sanakan.Configuration
{
    /// <summary>
    /// Describes rich message type.
    /// </summary>
    public enum RichMessageType : byte
    {
        /// <summary>
        /// Describes missing type
        /// </summary>
        None = 0,

        /// <summary>
        /// News channel
        /// </summary>
        News = 1,

        /// <summary>
        /// Review channel
        /// </summary>
        Review = 2,

        /// <summary>
        /// New epsiode channel
        /// </summary>
        NewEpisode = 3,

        /// <summary>
        /// User private message.
        /// </summary>
        UserNotify = 4,

        /// <summary>
        /// New epsiode channel
        /// </summary>
        NewEpisodePL = 5,

        /// <summary>
        /// Recommendation channel.
        /// </summary>
        Recommendation = 6,

        /// <summary>
        /// Wiadomość do kanału moderatorów
        /// </summary>
        ModNotify = 7,

        /// <summary>
        /// Wiadomość do kanału sezonów
        /// </summary>
        NewSesonalEpisode = 8,

        /// <summary>
        /// Wiadomość do kanału raportów
        /// </summary>
        AdminNotify = 9,
    }

}
