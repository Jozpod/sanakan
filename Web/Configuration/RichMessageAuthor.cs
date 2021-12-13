namespace Sanakan.Configuration
{
    /// <summary>
    /// Autor wiadomości - pierwsza linia
    /// </summary>
    public partial class RichMessageAuthor
    {
        /// <summary>
        /// Tekst pierwszej lini
        /// </summary>
        public string Name { get; set; } = null;

        /// <summary>
        /// Obrazek przy pierwszej lini
        /// </summary>
        public string ImageUrl { get; set; } = null;

        /// <summary>
        /// Adres do którego prowadzi imię autora
        /// </summary>
        public string NameUrl { get; set; } = null;
    }
}