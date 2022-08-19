using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Sanakan.Common.Cache;
using Sanakan.Common.Configuration;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Sanakan.Common
{
    internal class MSCacheManager : ICacheManager
    {
        private readonly IMemoryCache _cache;
        private readonly MemoryCacheOptions _options;
        private readonly MemoryCacheEntryOptions _cacheEntryOptions;
        private readonly IDictionary<string, ISet<string>> _cacheKeys;
        private readonly object _syncRoot = new object();

        public MSCacheManager(IOptions<MSCacheManagerOptions> options)
        {
            _options = new MemoryCacheOptions();
            _cache = new MemoryCache(_options);
            _cacheEntryOptions = new MemoryCacheEntryOptions()
            {
                SlidingExpiration = options.Value.SlidingExpiration,
                AbsoluteExpirationRelativeToNow = options.Value.AbsoluteExpirationRelativeToNow,
            };
            _cacheKeys = new ConcurrentDictionary<string, ISet<string>>();
        }

        public void Add<T>(string key, T value, string? parentKey = null)
        {
            lock (_syncRoot)
            {
                if (parentKey != null)
                {
                    if (!_cacheKeys.TryGetValue(parentKey, out var subKeys))
                    {
                        subKeys = new HashSet<string>();
                        _cacheKeys[parentKey] = subKeys;
                    }

                    subKeys.Add(key);
                }

                var entry = new MemoryCacheEntry<T>
                {
                    Value = value,
                };

                _cache.Set(key, entry);
            }
        }

        public void Add<T>(string key, T value, MemoryCacheEntryOptions memoryCacheEntryOptions)
        {
            lock (_syncRoot)
            {
                var entry = new MemoryCacheEntry<T>
                {
                    Value = value,
                };

                _cache.Set(key, entry, memoryCacheEntryOptions);
            }
        }

        public void ExpireTag(params string[] cacheKeys)
        {
            lock (_syncRoot)
            {
                foreach (var cacheKey in cacheKeys)
                {
                    if (_cacheKeys.TryGetValue(cacheKey, out var subKeys))
                    {
                        foreach (var subKey in subKeys)
                        {
                            _cache.Remove(subKey);
                        }

                        subKeys.Clear();
                    }

                    _cache.Remove(cacheKey);
                }
            }
        }

        public MemoryCacheEntry<T>? Get<T>(string key)
        {
            var item = _cache.Get<MemoryCacheEntry<T>>(key);
            return item;
        }
    }
}
