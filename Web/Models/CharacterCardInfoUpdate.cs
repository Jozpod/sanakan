namespace Sanakan.Web.Models
{
    /// <summary>
    /// Describes the card
    /// </summary>
    public class CharacterCardInfoUpdate
    {
        /// <summary>
        /// The image URL.
        /// </summary>
        public string? ImageUrl { get; set; }

        /// <summary>
        /// Imię i nazwisko na karcie
        /// </summary>

        public string CharacterName { get; set; } = string.Empty;

        /// <summary>
        /// Nazwa serii z której pochodzi karta
        /// </summary>
        public string CardSeriesTitle { get; set; } = string.Empty;
    }
}