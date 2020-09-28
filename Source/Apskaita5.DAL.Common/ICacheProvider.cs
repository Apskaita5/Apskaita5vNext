using System;
using System.Threading.Tasks;

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
        /// Retrieve cached item of type <typeparamref name="T"/> or fetch one by the factory method. 
        /// </summary>
        /// <typeparam name="T">a type of the cached item to retrieve</typeparam>
        /// <param name="factory">a method to fetch the item to cache</param>
        /// <returns></returns>
        Task<T> GetOrCreate<T>(Func<Task<T>> factory);

        /// <summary>
        /// Retrieve cached item of type <typeparamref name="T"/> or fetch one by the factory method. 
        /// </summary>
        /// <typeparam name="T">a type of the cached item to retrieve</typeparam>
        /// <param name="region">a cache region, e.g. a database that is currently in use</param>
        /// <param name="factory">a method to fetch the item to cache</param>
        /// <returns></returns>
        Task<T> GetOrCreate<T>(string region, Func<Task<T>> factory);

        /// <summary>
        /// Remove item of type <typeparamref name="T"/> from cache.
        /// </summary>
        /// <typeparam name="T">a type of the cached item to clear</typeparam>
        void Clear<T>();

        /// <summary>
        /// Remove item of type <typeparamref name="T"/> from cache
        /// </summary>
        /// <typeparam name="T">a type of the cached item to clear</typeparam>
        /// <param name="region">a cache region, e.g. a database that is currently in use</param>
        void Clear<T>(string region);

        /// <summary>
        /// Remove item of type specified from cache.
        /// </summary>
        /// <param name="cachedItemType">a type of the cached item to clear</param>
        void Clear(Type cachedItemType);

        /// <summary>
        /// Remove item of type specified from cache.
        /// </summary>
        /// <param name="cachedItemType">a type of the cached item to clear</param>
        /// <param name="region">a cache region, e.g. a database that is currently in use</param>
        void Clear(Type cachedItemType, string region);

    }
}
