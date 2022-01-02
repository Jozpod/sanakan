namespace Sanakan.Web.Models
{
    /// <summary>
    /// Describes the card.
    /// </summary>
    public class CharacterCardInfoUpdate
    {
        /// <summary>
        /// The image URL.
        /// </summary>
        public string? ImageUrl { get; set; }

        /// <summary>
        /// The full card name.
        /// </summary>
        public string CharacterName { get; set; } = string.Empty;

        /// <summary>
        /// The card series title.
        /// </summary>
        public string CardSeriesTitle { get; set; } = string.Empty;
    }
}