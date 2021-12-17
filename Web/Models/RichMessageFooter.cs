namespace Sanakan.Web.Models
{
    /// <summary>
    /// Describes message footer.
    /// </summary>
    public partial class RichMessageFooter
    {
        /// <summary>
        /// The text.
        /// </summary>
        public string Text { get; set; } = null;

        /// <summary>
        /// Last line image link.
        /// </summary>
        public string ImageUrl { get; set; } = null;
    }
}