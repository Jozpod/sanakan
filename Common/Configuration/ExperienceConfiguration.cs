namespace Sanakan.Common.Configuration
{
    public class ExperienceConfiguration
    {
        /// <summary>
        /// The amount of characters needed for one card packet.
        /// </summary>
        public ulong CharPerPacket { get; set; }

        /// <summary>
        /// The character count per one experience point ratio.
        /// </summary>
        public double CharPerPoint { get; set; }

        /// <summary>
        /// The minimum amount of experience from one Discord message.
        /// </summary>
        public double MinPerMessage { get; set; }

        /// <summary>
        /// The maximum amount of experience from one Discord message.
        /// </summary>
        public double MaxPerMessage { get; set; }
    }
}