using System.Collections.Generic;

namespace Sanakan.Web.Models
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
        /// The list of expeditions
        /// </summary>
        public List<ExpeditionCard> Expeditions { get; set; } = null;

        /// <summary>
        /// The list of tags which user has in cards.
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
        /// The image position in user profile background.
        /// </summary>
        public int BackgroundPosition { get; set; }

        /// <summary>
        /// The image position in user profile foreground.
        /// </summary>
        public int ForegroundPosition { get; set; }

        /// <summary>
        /// The image in user profile background.
        /// </summary>
        public string? BackgroundImageUrl { get; set; }

        /// <summary>
        /// The character image in user profile foreground.
        /// </summary>
        public string? ForegroundImageUrl { get; set; }

        /// <summary>
        /// The main profile foreground color.
        /// </summary>
        public string? ForegroundColor { get; set; }

        /// <summary>
        /// The user karma.
        /// </summary>
        public double Karma { get; set; }
    }
}
