using System.Collections.Generic;

namespace Sanakan.Api.Models
{
    /// <summary>
    /// Describes user profile in a website.
    /// </summary>
    public class UserSiteProfile
    {
        /// <summary>
        /// The aggregated number of cards grouped by rarity.
        /// </summary>
        public IDictionary<string, long> CardsCount { get; set; } = null;

        /// <summary>
        /// The wallet content.
        /// </summary>
        public IDictionary<string, long> Wallet { get; set; } = null;

        /// <summary>
        /// Waifu
        /// </summary>
        public CardFinalView Waifu { get; set; } = null;

        /// <summary>
        /// Galeria
        /// </summary>
        public List<CardFinalView> Gallery { get; set; } = null;

        /// <summary>
        /// Lista wypraw
        /// </summary>
        public List<ExpeditionCard> Expeditions { get; set; } = null;

        /// <summary>
        /// Lista tagów jakie ma użytkownik na kartach
        /// </summary>
        public List<string> TagList { get; set; } = null;

        /// <summary>
        /// User exchange rules.
        /// </summary>
        public string ExchangeConditions { get; set; } = null;

        /// <summary>
        /// The user title.
        /// </summary>
        public string? UserTitle { get; set; }

        /// <summary>
        /// Pozycja obrazku tła profilu użytkownika
        /// </summary>
        public int BackgroundPosition { get; set; }

        /// <summary>
        /// Pozycja obrazka postaci na tle profilu użytkownika
        /// </summary>
        public int ForegroundPosition { get; set; }

        /// <summary>
        /// Obrazek tła profilu użytkownika
        /// </summary>
        public string? BackgroundImageUrl { get; set; }

        /// <summary>
        /// Obrazek postaci na tle profilu użytkownika
        /// </summary>
        public string? ForegroundImageUrl { get; set; }

        /// <summary>
        /// Główny kolor profilu użytkownika
        /// </summary>
        public string? ForegroundColor { get; set; }

        /// <summary>
        /// Karma użytkownika
        /// </summary>
        public double Karma { get; set; }
    }
}
