using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Apskaita5.DAL.Common
{
    public sealed class ConcurrentDictionaryCacheProvider : ICacheProvider
    {

        private readonly ConcurrentDictionary<string, Task<object>> _dict 
            = new ConcurrentDictionary<string, Task<object>>();


        public ConcurrentDictionaryCacheProvider() { }


        public void Clear<T>() 
            => _dict.TryRemove(GetItemKey<T>(null), out Task<object> item);

        public void Clear<T>(string region)
        {
            if (region.IsNullOrWhiteSpace()) throw new ArgumentNullException(region);
            _dict.TryRemove(GetItemKey<T>(region), out Task<object> item);
        }

        public void Clear(Type cachedItemType) 
            => _dict.TryRemove(GetItemKey(cachedItemType, null), out Task<object> item);

        public void Clear(Type cachedItemType, string region)
            => _dict.TryRemove(GetItemKey(cachedItemType, region), out Task<object> item);


        public async Task<T> GetOrCreate<T>(Func<Task<T>> factory) 
            => await GetOrCreateInt<T>(string.Empty, factory);

        public async Task<T> GetOrCreate<T>(string region, Func<Task<T>> factory)
        {       
            if (region.IsNullOrWhiteSpace()) throw new ArgumentNullException(nameof(region));            
            return await GetOrCreateInt<T>(region, factory);
        }

        private async Task<T> GetOrCreateInt<T>(string region, Func<Task<T>> factory)
        {
            if (null == factory) throw new ArgumentNullException(nameof(factory));
            var result = _dict.GetOrAdd(GetItemKey<T>(region), k => factory().ContinueWith<object>(
                t => t.Result, TaskContinuationOptions.OnlyOnRanToCompletion));
            return (T)(await result);
        }


        private string GetItemKey<T>(string region)
        {
            return GetItemKey(typeof(T), region);
        }

        private string GetItemKey(Type cachedItemType, string region)
        {
            return region.IsNullOrWhiteSpace() ? cachedItemType.FullName :
                string.Format("{0}:{1}", region.Trim(), cachedItemType.FullName);
        }

    }
}
