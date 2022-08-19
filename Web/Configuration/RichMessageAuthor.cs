namespace Sanakan.Configuration
{
    /// <summary>
    /// Describes message author.
    /// </summary>
    public partial class RichMessageAuthor
    {
        /// <summary>
        /// The first line text.
        /// </summary>
        public string Name { get; set; } = null;

        /// <summary>
        /// The image URL.
        /// </summary>
        public string ImageUrl { get; set; } = null;

        /// <summary>
        /// The author link URL.
        /// </summary>
        public string NameUrl { get; set; } = null;
    }
}