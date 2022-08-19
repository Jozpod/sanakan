using Discord;
using Sanakan.DiscordBot.Modules;

namespace Sanakan.DiscordBot.Integration.Tests.CommandBuilders
{
    /// <summary>
    /// Provides methods to build commands in <see cref="ShindenModule"/>.
    /// </summary>
    public static class ShindenCommandBuilder
    {
        /// <summary>
        /// <see cref="ShindenModule.ConnectAsync(string)"/>.
        /// </summary>
        public static string Connect(string prefix, string url) => $"{prefix}connect {url}";

        /// <summary>
        /// <see cref="ShindenModule.SearchAnimeAsync(string)"/>.
        /// </summary>
        public static string SearchAnime(string prefix, string title) => $"{prefix}anime {title}";

        /// <summary>
        /// <see cref="ShindenModule.SearchMangaAsync(string)"/>.
        /// </summary>
        public static string SearchManga(string prefix, string title) => $"{prefix}manga {title}";

        /// <summary>
        /// <see cref="ShindenModule.SearchCharacterAsync(string)"/>.
        /// </summary>
        public static string SearchCharacter(string prefix, string title) => $"{prefix}character {title}";

        /// <summary>
        /// <see cref="ShindenModule.ShowNewEpisodesAsync"/>.
        /// </summary>
        public static string ShowNewEpisodes(string prefix) => $"{prefix}episodes";

        /// <summary>
        /// <see cref="ShindenModule.ShowSiteStatisticAsync(IGuildUser)"/>.
        /// </summary>
        public static string GetSiteStatistic(string prefix, string mention) => $"{prefix}site {mention}";
    }
}
