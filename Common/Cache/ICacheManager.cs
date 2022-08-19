using Microsoft.Extensions.Caching.Memory;

namespace Sanakan.Common.Cache
{
    public interface ICacheManager
    {
        void ExpireTag(params string[] tags);

        MemoryCacheEntry<T>? Get<T>(string key);

        void Add<T>(string key, T value, string? parentKey = null);

        void Add<T>(string key, T value, MemoryCacheEntryOptions memoryCacheEntryOptions);
    }
}