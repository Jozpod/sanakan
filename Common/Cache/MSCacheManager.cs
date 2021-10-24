using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Sanakan.Common.Configuration;
using System;

namespace Sanakan.Common
{
    internal class MSCacheManager : ICacheManager
    {
        private readonly IMemoryCache _cache;
        private readonly MemoryCacheOptions _options;
        private readonly MemoryCacheEntryOptions _cacheEntryOptions;

        public MSCacheManager(IOptions<MSCacheManagerOptions> options)
        {
            _options = new MemoryCacheOptions();
            _cache = new MemoryCache(_options);
            _cacheEntryOptions = new MemoryCacheEntryOptions()
            {
                SlidingExpiration = options.Value.SlidingExpiration,
                AbsoluteExpirationRelativeToNow = options.Value.AbsoluteExpirationRelativeToNow,
            };
        }

        public void Add<T>(string key, T entity)
        {
            _cache.Set(key, entity);
        }

        public void ExpireTag(params string[] tags)
        {
            foreach (var tag in tags)
            {
                _cache.Remove(tag);
            }
        }

        public T? Get<T>(string key)
        {
            var item = _cache.Get<T>(key);
            return item;
        }
    }
}
