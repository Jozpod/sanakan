using Microsoft.Extensions.Caching.Memory;

namespace Sanakan.Common
{
    public interface ICacheManager
    {
        void ExpireTag(params string[] tags);
        T? Get<T>(string key);
        void Add<T>(string key, T entity);
        void Add<T>(string key, T entity, MemoryCacheEntryOptions memoryCacheEntryOptions);
    }
}