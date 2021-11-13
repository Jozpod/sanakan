using System;

namespace Sanakan.Common.Configuration
{
    public class SupervisorConfiguration
    {
        /// <summary>
        /// The maximum amount of messages user is allowed to send in a short time frame.
        /// </summary>
        public uint MessagesLimit { get; set; }

        /// <summary>
        /// The maximum amount of the same message user is allowed to send in a short time frame.
        /// </summary>
        public uint MessageLimit { get; set; }

        /// <summary>
        /// Additional amount of messages user can send when message is command.
        /// </summary>
        public uint MessageCommandLimit { get; set; }

        /// <summary>
        /// The maximum amount of users which can join guild with the same username.
        /// </summary>
        public uint SameUsernameLimit { get; set; }

        /// <summary>
        /// Specifies amount of time after which user message won't be analyzer nor be involved in decision mechanism.
        /// </summary>
        public TimeSpan MessageExpiry { get; set; }

        /// <summary>
        /// Allowed time frame between sent messages by user.
        /// </summary>
        public TimeSpan TimeIntervalBetweenMessages { get; set; }

        /// <summary>
        /// Allowed time frame between events where user joins guild.
        /// </summary>
        public TimeSpan TimeIntervalBetweenUserGuildJoins { get; set; }
    }
}
