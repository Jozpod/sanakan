using Discord;

namespace Sanakan.DiscordBot.Abstractions
{
    public static class Emojis
    {
        public static Emoji Checked = new Emoji("✅");
        public static Emoji Zero = new Emoji("0⃣");
        public static Emoji One = new Emoji("1⃣");
        public static Emoji Two = new Emoji("2⃣");
        public static Emoji Three = new Emoji("3⃣");
        public static Emoji Four = new Emoji("4⃣");
        public static Emoji Five = new Emoji("5⃣");
        public static Emoji Six = new Emoji(" 6⃣");
        public static Emoji Seven = new Emoji("7⃣");
        public static Emoji Eight = new Emoji("8⃣");
        public static Emoji Nine = new Emoji("9⃣");
        public static Emoji HandSign = new Emoji("👌");
        public static Emoji RaisedHand = new Emoji("🖐");
        public static Emoji LeftwardsArrow = new Emoji("⬅");
        public static Emoji RightwardsArrow = new Emoji("➡");

        public static Emote DeclineEmote = Emote.Parse("<:redcross:581152766655856660>");

        public static Emoji InboxTray = new Emoji("📥");
        public static Emoji CrossMark = new Emoji("❌");
        public static Emoji OutboxTray = new Emoji("📤");

        public static Emoji OneEmote = new Emoji("\u0031\u20E3");
        public static Emoji TwoEmote = new Emoji("\u0032\u20E3");
    }
}
