using Discord;

namespace Sanakan.DiscordBot.Abstractions.Configuration
{
    public interface IIconConfiguration
    {
        /// <summary>
        /// Used in lottery.
        /// </summary>
        public IEmote GiveawayCardParticipate { get; }

        /// <summary>
        /// Used for Accept/Decline interaction with bot.
        /// </summary>
        public IEmote Accept { get; }

        /// <summary>
        /// Used for Accept/Decline interaction with bot.
        /// </summary>
        public IEmote Decline { get; }

        /// <summary>
        /// Used 
        /// </summary>
        public IEmote HandSign { get; }

        /// <summary>
        /// Used in slot machine.
        /// </summary>
        public IEmote Psyduck { get; }

        /// <summary>
        /// Used in crafting session.
        /// </summary>
        public IEmote InboxTray { get; }

        /// <summary>
        /// Used in crafting session.
        /// </summary>
        public IEmote CrossMark { get; }

        /// <summary>
        /// Used in exchange session.
        /// </summary>
        public IEmote OneEmote { get; }

        /// <summary>
        /// Used in exchange session.
        /// </summary>
        public IEmote TwoEmote { get; }

        /// <summary>
        /// Used for interaction with bot which involves paging
        /// </summary>
        public IEmote LeftwardsArrow { get; }

        /// <summary>
        /// Used for interaction with bot which involves paging
        /// </summary>
        public IEmote RightwardsArrow { get; }

        /// <summary>
        /// Used for Accept/Decline interaction with bot.
        /// </summary>
        public IEmote[] AcceptDecline { get; }

        /// <summary>
        /// Used for interaction with bot which involves paging
        /// </summary>
        public IEmote[] LeftRightArrows { get; }
    }
}
