using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FunPokedex.Application.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace FunPokedex.Application.Infrastructure
{
    public class ApplicationMemoryCache: IApplicationCache
    {

        private readonly IMemoryCache _memoryCache;
        private readonly MemoryCacheEntryOptions _cacheOptions;

        public ApplicationMemoryCache(IMemoryCache memoryCache, TimeSpan entryDuration)
        {
            _memoryCache = memoryCache;
            _cacheOptions = new MemoryCacheEntryOptions();

            if (entryDuration.TotalMicroseconds >= 1)
            {
                _cacheOptions.SetSlidingExpiration(entryDuration);
            }

        }


        public bool TryGetValue<T>(string key, out T? value)
        {
            value = default;

            if(_memoryCache.TryGetValue(key, out object? cacheValue) && cacheValue is T)
            {
                value = (T)cacheValue!;
                return true;
            }

            return false;
        }

        public void SetValue<T>(string key, T value)
        {
            _memoryCache.Set(key, value, _cacheOptions);
        }
    }
}
