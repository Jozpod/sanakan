namespace Sanakan.Web.Models
{
    /// <summary>
    /// Describes command attribute.
    /// </summary>
    public class CommandAttribute
    {
        /// <summary>
        /// The name of attribute.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// The description.
        /// </summary>
        public string Description { get; set; } = string.Empty;
    }
}