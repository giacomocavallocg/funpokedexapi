using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

namespace FunPokedex.Application.Infrastructure
{
    public class ApplicationMemoryCache(IMemoryCache memoryCache, TimeSpan entryDuration)
    {

        private readonly IMemoryCache _memoryCache = memoryCache;
        private readonly TimeSpan _entryDuration = entryDuration;


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
            MemoryCacheEntryOptions cacheOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(_entryDuration);

            _memoryCache.Set(key, value);
        }
    }
}
