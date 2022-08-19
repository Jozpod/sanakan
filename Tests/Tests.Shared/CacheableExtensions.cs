using Discord;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Sanakan.Tests.Shared
{
    public static class CacheableExtensions
    {
        public static Cacheable<TEntity, TId> CreateCacheable<TEntity, TId>(TEntity value, TId id)
            where TEntity : IEntity<TId>
            where TId : IEquatable<TId>
        {
            var bindingAttr = BindingFlags.NonPublic | BindingFlags.Instance;
            var types = new[]
            {
                typeof(TEntity),
                typeof(TId),
                typeof(bool),
                typeof(Func<Task<TEntity>>),
            };

            var cacheableCtor = typeof(Cacheable<TEntity, TId>).GetConstructor(bindingAttr, null, types, null);

            var parameters = new object[]
            {
                value,
                id,
                true,
                null
            };

            var cacheable = (Cacheable<TEntity, TId>)cacheableCtor.Invoke(parameters);

            return cacheable;
        }
    }
}