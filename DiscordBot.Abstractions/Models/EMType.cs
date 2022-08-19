using Discord;
using System.Collections.Generic;

namespace Sanakan.DiscordBot.Abstractions.Models
{
    public enum EMType : byte
    {
        Neutral = 0,
        Warning = 1,
        Success = 2,
        Error = 3,
        Info = 4,
        Bot = 5
    }

    public static class EMTypeExtensions
    {
        public static readonly IDictionary<EMType, Color> EMTypeColorMap = new Dictionary<EMType, Color>()
        {
            { EMType.Bot, new Color(158, 62, 211) },
            { EMType.Error, new Color(255, 0, 0) },
            { EMType.Info, new Color(0, 122, 204) },
            { EMType.Success, new Color(51, 255, 51) },
            { EMType.Warning, new Color(255, 255, 0) },
            { EMType.Neutral, new Color(128, 128, 128) },
        };

        public static string Emoji(this EMType type, bool hide = false)
        {
            if (hide)
            {
                return "";
            }

            return type switch
            {
                EMType.Bot => "🐙",
                EMType.Info => "ℹ",
                EMType.Error => "🚫",
                EMType.Success => "✅",
                EMType.Warning => "⚠",
                _ => "",
            };
        }

        public static Color Color(this EMType type) => EMTypeColorMap[type];
    }
}
