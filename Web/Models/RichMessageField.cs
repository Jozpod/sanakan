namespace Sanakan.Web.Models
{
    /// <summary>
    /// Describes the rich message field.
    /// </summary>
    public partial class RichMessageField
    {
        /// <summary>
        /// The rich field header.
        /// </summary>
        public string Name { get; set; } = null;

        /// <summary>
        /// The rich field text.
        /// </summary>
        public string Value { get; set; } = null;

        /// <summary>
        /// Specifies whether the field can be inlined.
        /// </summary>
        public bool IsInline { get; set; }
    }
}