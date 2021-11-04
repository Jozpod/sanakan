namespace Sanakan.Configuration
{
    /// <summary>
    /// Describes rich message type.
    /// </summary>
    public enum RichMessageType
    {
        /// <summary>
        /// Oznaczenie brakujacego typu
        /// </summary>
        None = 0,
        /// <summary>
        /// Wiadomość do kanału newsów
        /// </summary>
        News = 1,
        /// <summary>
        /// Wiadomość do kanału recenzji
        /// </summary>
        Review = 2,
        /// <summary>
        /// Wiadomość do kanału nowych epizodów
        /// </summary>
        NewEpisode = 3,
        /// <summary>
        /// Wiadomość użytkownika na PW
        /// </summary>
        UserNotify = 4,
        /// <summary>
        /// Wiadomość do kanału nowych epizodów
        /// </summary>
        NewEpisodePL = 5,
        /// <summary>
        /// Wiadomość do kanału rekomendacji
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
