using System;
using System.Collections.Generic;
using RepoDb.Interfaces;
using System.Linq;
using System.Collections;
using System.Collections.Concurrent;

namespace RepoDb
{
    /// <summary>
    /// An object used for caching a result of the repository query operation. This is the default cache object used by the repositories.
    /// </summary>
    public class MemoryCache : ICache
    {
        private readonly ConcurrentDictionary<string, CacheItem> m_cache;

        /// <summary>
        /// Creates a new instance <see cref="MemoryCache"/> object.
        /// </summary>
        public MemoryCache()
        {
            m_cache = new ConcurrentDictionary<string, CacheItem>();
        }

        /// <summary>
        /// Adds a cache item value.
        /// </summary>
        /// <param name="key">The key to the cache.</param>
        /// <param name="value">The value of the cache.</param>
        /// <param name="throwException">Throws an exception if the operation has failed to add an item.</param>
        public void Add(string key, object value, bool throwException = true)
        {
            Add(key, value, Constant.DefaultCacheItemExpirationInMinutes, throwException);
        }

        /// <summary>
        /// Adds a cache item value.
        /// </summary>
        /// <param name="key">The key to the cache.</param>
        /// <param name="value">The value of the cache.</param>
        /// <param name="expirationInMinutes">The expiration in minutes of the cache item.</param>
        /// <param name="throwException">Throws an exception if the operation has failed to add an item.</param>
        public void Add(string key, object value, int expirationInMinutes, bool throwException = true)
        {
            Add(new CacheItem(key, value, expirationInMinutes), throwException);
        }

        /// <summary>
        /// Adds a cache item value.
        /// </summary>
        /// <param name="item">The cache item to be added in the collection.</param>
        /// <param name="throwException">Throws an exception if the operation has failed to add an item.</param>
        public void Add(CacheItem item, bool throwException = true)
        {
            var cacheItem = GetItem(item.Key, false);
            if (cacheItem == null)
            {
                if (m_cache.TryAdd(item.Key, item) == false && throwException == true)
                {
                    throw new InvalidOperationException($"Fail to add an item into the cache for the key {item.Key}.");
                }
            }
            else
            {
                if (!cacheItem.IsExpired())
                {
                    throw new InvalidOperationException($"An existing cache for key '{item.Key}' already exists.");
                }
                cacheItem.UpdateFrom(item);
            }
        }

        /// <summary>
        /// Clears the collection of the cache.
        /// </summary>
        public void Clear()
        {
            m_cache.Clear();
        }

        /// <summary>
        /// Checks whether the key is present in the collection.
        /// </summary>
        /// <param name="key">The name of the key to be checked.</param>
        /// <returns>A boolean value that signifies the presence of the key from the collection.</returns>
        public bool Contains(string key)
        {
            var cacheItem = GetItem(key);
            return cacheItem != null && !cacheItem.IsExpired();
        }

        /// <summary>
        /// Gets an object from the cache collection.
        /// </summary>
        /// <param name="key">The key of the cache object to be retrieved.</param>
        /// <returns>A cached item object from the cache collection based on the given key.</returns>
        /// <param name="throwException">Throws an exception if the item is not found.</param>
        public CacheItem Get(string key, bool throwException = true)
        {
            var item = GetItem(key, throwException);
            if (item != null && !item.IsExpired())
            {
                return item;
            }
            return null;
        }

        /// <summary>
        /// Gets the enumerator of the cache collection.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<CacheItem> GetEnumerator()
        {
            return m_cache
                .Where(item => !item.Value.IsExpired())
                .Select(item => item.Value)
                .GetEnumerator();
        }

        /// <summary>
        /// Gets the enumerator of the cache collection.
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Removes the item from the cache collection.
        /// </summary>
        /// <param name="key">The key of the item to be removed from the cache collection.</param>
        /// <param name="throwException">Throws an exception if the operation has failed to remove an item.</param>
        public void Remove(string key, bool throwException = true)
        {
            var item = (CacheItem)null;
            if (m_cache.TryRemove(key, out item) == false && throwException == true)
            {
                throw new InvalidOperationException($"Failed to remove an item with key '{key}'.");
            }
        }

        /// <summary>
        /// Gets the cached item by key. This includes the expired cached item.
        /// </summary>
        /// <param name="key">The key of the cached item.</param>
        /// <returns>The cached item based on the given key.</returns>
        /// <param name="throwException">Throws an exception if the item is not found.</param>
        protected CacheItem GetItem(string key, bool throwException = true)
        {
            var value = (CacheItem)null;
            if (m_cache.TryGetValue(key, out value) == false && throwException == true)
            {
                throw new InvalidOperationException($"No item found with key '{key}'.");
            }
            return value;
        }
    }
}
