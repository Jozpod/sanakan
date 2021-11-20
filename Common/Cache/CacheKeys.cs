namespace Sanakan.Common.Cache
{
    public static class CacheKeys
    {
        public const string Users = nameof(Users);
        public const string Muted = nameof(Muted);
        public const string UsersLite = nameof(UsersLite);
        public const string Quiz = nameof(Quiz);
        public const string GameDeckUser = "gamedeck-user-{0}";
        public const string GameDecks = nameof(GameDecks);
        public const string GuildConfigTemplate = "config-{0}";
        public const string UserTemplate = "user-{0}";

        public static string User(object id) => string.Format(UserTemplate, id);
        public static string GuildConfig(object id) => string.Format(GuildConfigTemplate, id);
    }
}
