namespace Sanakan.Configuration
{
    public class ExperienceConfiguration
    {
        /// <summary>
        /// character count for one experience point.
        /// </summary>
        public double CharPerPoint { get; set; }

        /// <summary>
        /// The minimum amount of experience from one discord message.
        /// </summary>
        public double MinPerMessage { get; set; }

        /// <summary>
        /// The maximum amount of experience from one discord message.
        /// </summary>
        public double MaxPerMessage { get; set; }
    }
}