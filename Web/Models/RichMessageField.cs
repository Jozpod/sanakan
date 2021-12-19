namespace Sanakan.Web.Models
{
    /// <summary>
    /// Dodatkowe pole wiadomości
    /// </summary>
    public partial class RichMessageField
    {
        /// <summary>
        /// Nagłówek dodatkowego pola
        /// </summary>
        public string Name { get; set; } = null;

        /// <summary>
        /// Tekst dodatkowego pola
        /// </summary>
        public string Value { get; set; } = null;

        /// <summary>
        /// Czy pole może zostać wyświetlne w jednej lini z innymi
        /// </summary>
        public bool IsInline { get; set; }
    }
}