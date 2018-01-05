using JetBrains.Annotations;

namespace Common
{
    /// <summary>
    /// <see cref="CachedDataDictionary{TKey,TValue}"/> cache update strategies
    /// </summary>
    [PublicAPI]
    public enum CachedDataDictionaryUpdateStrategy
    {
        /// <summary>
        /// If cache is expired or not had been initialized yet and <see cref="CachedDataDictionary{TKey,TValue}"/>
        /// recieves multiple concurrent calls which wants to obtain cached data,
        /// the getData delegate of the <see cref="CachedDataDictionary{TKey,TValue}"/> constructor can be called
        /// multiple times concurrently.
        /// </summary>
        AllowConcurrentUpdates,

        /// <summary>
        /// If cache is expired or not had benn initialized yet and <see cref="CachedDataDictionary{TKey,TValue}"/>
        /// recieves multiple concurrent calls which wants to obtain cached data,
        /// the getData delegate of the <see cref="CachedDataDictionary{TKey,TValue}"/> constructor will be called
        /// only once. Other calls will retrieve data from the cache.
        /// </summary>
        UseSynchronizedUpdates
    }
}
