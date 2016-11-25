using System;
using Microsoft.Extensions.Caching.Memory;

namespace Common.Cache
{
    /// <summary>
    /// Represents a manager for caching between HTTP requests
    /// </summary>
    public class MemoryCacheManager : ICacheManager
    {
        static MemoryCacheManager()
        {
            Cache = new MemoryCache(new MemoryCacheOptions());
        }

        /// <summary>
        /// Cache object
        /// </summary>
        protected static IMemoryCache Cache { get; set; }

        /// <summary>
        /// Gets or sets the value associated with the specified key.
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="key">The key of the value to get.</param>
        /// <returns>The value associated with the specified key.</returns>
        public virtual T Get<T>(string key)
        {
            return Cache.Get<T>(key);
        }

        /// <summary>
        /// Adds the specified key and object to the cache.
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="data">Data</param>
        /// <param name="cacheTime">Cache time</param>
        public virtual void Set(string key, object data, int cacheTime)
        {
            if (data == null)
                return;
            
            Cache.Set(key, data, DateTime.Now + TimeSpan.FromMinutes(cacheTime));
        }

        /// <summary>
        /// Gets a value indicating whether the value associated with the specified key is cached
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>Result</returns>
        public virtual bool IsSet(string key)
        {
            object value;
            return Cache.TryGetValue(key, out value);
        }

        /// <summary>
        /// Removes the value with the specified key from the cache
        /// </summary>
        /// <param name="key">/key</param>
        public virtual void Remove(string key)
        {
            Cache.Remove(key);
        }

        public void RemoveByPattern(string pattern)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Clear all cache data
        /// </summary>
        public virtual void Clear()
        {
            Cache.Dispose();
            Cache = new MemoryCache(new MemoryCacheOptions());
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public virtual void Dispose()
        {
        }
    }
}