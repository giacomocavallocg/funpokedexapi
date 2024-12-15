using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FunPokedex.Application.Infrastructure;
using FunPokedex.Application.Models;
using Microsoft.Extensions.Caching.Memory;

namespace FunPokedex.Tests.FunPokedex.Application.Infrastructure
{
    public class ApplicationMemoryCacheTest
    {

        [Fact]
        public void Test_Exists()
        {
            MemoryCacheOptions options = new();
            var cache = new ApplicationMemoryCache(new MemoryCache(options), TimeSpan.FromHours(1));

            cache.SetValue("key1", "value1");
            Assert.True(cache.TryGetValue("key1", out string? value));
            Assert.Equal("value1", value);

            var p = new Pokemon() { Name = "test", Description = "", Habitat = "", IsLegendary = true };
            cache.SetValue("key1", p);
            Assert.True(cache.TryGetValue("key1", out Pokemon? pOut));
            Assert.Equal(p.Name, pOut!.Name);
        }

        [Fact]
        public void Test_NotExists()
        {
            MemoryCacheOptions options = new();
            var cache = new ApplicationMemoryCache(new MemoryCache(options), TimeSpan.FromHours(1));

            Assert.False(cache.TryGetValue("key1", out string? _));
            Assert.False(cache.TryGetValue("key1", out Pokemon? _));

            cache.SetValue("key1", "string");
            Assert.False(cache.TryGetValue("key1", out Pokemon? _));

        }

        [Fact]
        public void Test_Expiration()
        {
            MemoryCacheOptions options = new();
            var cache = new ApplicationMemoryCache(new MemoryCache(options), TimeSpan.FromMicroseconds(1));
            Thread.Sleep(TimeSpan.FromMicroseconds(1));

            cache.SetValue("key1", "string");
            Assert.False(cache.TryGetValue("key1", out string? _));

        }

        [Fact]
        public void Test_Expiration_InvalidDuration()
        {
            MemoryCacheOptions options = new();
            var cache = new ApplicationMemoryCache(new MemoryCache(options), TimeSpan.FromMicroseconds(0.5));
            Thread.Sleep(TimeSpan.FromMicroseconds(1));

            cache.SetValue("key1", "string");
            Assert.True(cache.TryGetValue("key1", out string? _)); // expiration is not set because is less than 1 microseconds

        }
    }
}
