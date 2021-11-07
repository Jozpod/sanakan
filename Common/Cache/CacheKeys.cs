using System;

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
        public const string GuilConfig = "config-{0}";
        public const string User = "user-{0}";
    }
}
