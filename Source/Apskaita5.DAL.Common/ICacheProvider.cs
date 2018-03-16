namespace Apskaita5.DAL.Common
{

    /// <summary>
    /// Represents an interface that should be implemented by a particular caching
    /// implementation, e.g. System.Runtime.Caching.MemoryCache for full .NET
    /// or Microsoft.Extensions.Caching.Memory.IMemoryCache for .NET core.
    /// </summary>
    public interface ICacheProvider
    {

        /// <summary>
        /// Retrieve cached item
        /// </summary>
        /// <typeparam name="T">Type of cached item</typeparam>
        /// <param name="region">Cache region to use (the reserved region is "Accounting.DAL.Common")</param>
        /// <param name="key">Name of cached item</param>
        /// <param name="value">Cached value. Default(T) if item doesn't exist.</param>
        /// <returns>Cached item as type</returns>
        bool TryGet<T>(string region, string key, out T value);

        /// <summary>
        /// Insert value into the cache using appropriate name/value pairs
        /// </summary>
        /// <typeparam name="T">Type of cached item</typeparam>
        /// <param name="value">Item to be cached</param>
        /// <param name="key">Name of item</param>
        /// <param name="region">Cache region to use (the reserved region is "Accounting.DAL.Common")</param>
        void Set<T>(string region, string key, T value);

        /// <summary>
        /// Remove item from cache
        /// </summary>
        /// <param name="region">Cache region to use (the reserved region is "Accounting.DAL.Common")</param>
        /// <param name="key">Name of cached item</param>        
        void Clear(string region, string key);

    }
}
