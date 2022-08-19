namespace Sanakan.Common.Cache
{
    public static class CacheKeys
    {
        public const string Users = nameof(Users);

        public const string Cards = nameof(Cards);

        public const string TimeStatuses = nameof(TimeStatuses);

        public const string Penalties = nameof(Penalties);

        public const string Muted = nameof(Muted);

        public const string UsersLite = nameof(UsersLite);

        public const string Quizes = nameof(Quizes);

        public const string GameDeckUser = "gamedeck-user-{0}";

        public const string GameDecks = nameof(GameDecks);

        public const string QuizTemplate = "quiz-{0}";

        public const string GuildConfigTemplate = "config-{0}";

        public const string UserTemplate = "user-{0}";

        public static string Quiz(object id) => string.Format(QuizTemplate, id);

        public static string User(object id) => string.Format(UserTemplate, id);

        public static string GuildConfig(object id) => string.Format(GuildConfigTemplate, id);
    }
}
