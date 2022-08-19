using Discord;

namespace Sanakan.DiscordBot.Abstractions.Configuration
{
    public class DefaultIconConfiguration : IIconConfiguration
    {
        public IEmote GiveawayCardParticipate => Emojis.Checked;

        public IEmote Accept => Emojis.Checked;

        public IEmote Decline => Emojis.CrossMark;

        public IEmote HandSign => Emojis.HandSign;

        public IEmote Psyduck => Emotes.Psyduck;

        public IEmote InboxTray => Emojis.InboxTray;

        public IEmote CrossMark => Emojis.CrossMark;

        public IEmote OneEmote => Emojis.OneEmote;

        public IEmote TwoEmote => Emojis.TwoEmote;

        public IEmote LeftwardsArrow => Emojis.LeftwardsArrow;

        public IEmote RightwardsArrow => Emojis.RightwardsArrow;

        public IEmote[] ExchangeOneTwo => new[] { OneEmote, TwoEmote };

        public IEmote[] AcceptDecline => new[] { Accept, Decline };

        public IEmote[] LeftRightArrows => new[] { LeftwardsArrow, RightwardsArrow };
    }
}
