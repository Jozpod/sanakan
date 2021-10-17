namespace Sanakan.Configuration
{
    /// <summary>
    /// Enum z typem wiadomości
    /// </summary>
    public enum RichMessageType
    {
        /// <summary>
        /// Oznaczenie brakujacego typu
        /// </summary>
        None,
        /// <summary>
        /// Wiadomość do kanału newsów
        /// </summary>
        News,
        /// <summary>
        /// Wiadomość do kanału recenzji
        /// </summary>
        Review,
        /// <summary>
        /// Wiadomość do kanału nowych epizodów
        /// </summary>
        NewEpisode,
        /// <summary>
        /// Wiadomość użytkownika na PW
        /// </summary>
        UserNotify,
        /// <summary>
        /// Wiadomość do kanału nowych epizodów
        /// </summary>
        NewEpisodePL,
        /// <summary>
        /// Wiadomość do kanału rekomendacji
        /// </summary>
        Recommendation,
        /// <summary>
        /// Wiadomość do kanału moderatorów
        /// </summary>
        ModNotify,
        /// <summary>
        /// Wiadomość do kanału sezonów
        /// </summary>
        NewSesonalEpisode,
        /// <summary>
        /// Wiadomość do kanału raportów
        /// </summary>
        AdminNotify,
    }

}
