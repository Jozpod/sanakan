using Sanakan.Configuration;
using Sanakan.Web.Models;
using System;
using System.Collections.Generic;

namespace Sanakan.DiscordBot.Models
{
    /// <summary>
    /// Embed message generated from Discord, One of the fields must be provided.
    /// </summary>
    public class RichMessage
    {
        /// <summary>
        /// The url.
        /// </summary>
        public string Url { get; set; } = string.Empty;

        /// <summary>
        /// The message title.
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// The image url.
        /// </summary>
        public string ImageUrl { get; set; } = null;

        /// <summary>
        /// The message description.
        /// </summary>
        public string Description { get; set; } = null;

        /// <summary>
        /// The thumbnail image url.
        /// </summary>
        public string ThumbnailUrl { get; set; } = null;

        /// <summary>
        /// The message author.
        /// </summary>
        public RichMessageAuthor Author { get; set; } = null;

        /// <summary>
        /// The message footer.
        /// </summary>
        public RichMessageFooter Footer { get; set; } = null;

        /// <summary>
        /// The message timestamp.
        /// </summary>
        public DateTimeOffset? Timestamp { get; set; }

        /// <summary>
        /// Additional fields.
        /// </summary>
        public List<RichMessageField> Fields { get; set; } = new();

        /// <summary>
        /// The messgage type.
        /// </summary>
        public RichMessageType MessageType { get; set; }

        /// <summary>
        /// The additional message content.
        /// </summary>
        public string Content { get; set; } = null;
    }
}