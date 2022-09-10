using System;
using RepoDb.Interfaces;
using System.Linq;
using System.Collections;
using System.Collections.Concurrent;
using RepoDb.Exceptions;
using System.Threading;
using System.Threading.Tasks;

namespace RepoDb
{
    /// <summary>
    /// A class object that is used for caching the resultsets of the query operations. This is the default cache object used by the <see cref="DbRepository{TDbConnection}"/> and <see cref="BaseRepository{T, TDbConnection}"/> repository objects.
    /// </summary>
    public class MemoryCache : ICache
    {
        private readonly ConcurrentDictionary<string, object> _cache = new();

        #region Sync

        /// <summary>
        /// Adds a cache item value.
        /// </summary>
        /// <typeparam name="T">The type of the cache item value.</typeparam>
        /// <param name="key">The key to the cache.</param>
        /// <param name="value">The value of the cache.</param>
        /// <param name="expiration">The expiration in minutes of the cache item.</param>
        /// <param name="throwException">Throws an exception if the operation has failed to add an item.</param>
        public void Add<T>(string key,
            T value,
            int expiration = Constant.DefaultCacheItemExpirationInMinutes,
            bool throwException = true) =>
            Add(new CacheItem<T>(key, value, expiration), throwException);

        /// <summary>
        /// Adds a cache item value.
        /// </summary>
        /// <typeparam name="T">The type of the cache item value.</typeparam>
        /// <param name="item">The cache item to be added in the collection.</param>
        /// <param name="throwException">Throws an exception if the operation has failed to add an item.</param>
        public void Add<T>(CacheItem<T> item,
            bool throwException = true)
        {
            var cacheItem = (CacheItem<T>)null;
            if (_cache.TryGetValue(item.Key, out var value))
            {
                cacheItem = value as CacheItem<T>;
            }
            if (cacheItem == null)
            {
                if (_cache.TryAdd(item.Key, item) == false && throwException == true)
                {
                    throw new Exception($"Fail to add an item into the cache for the key {item.Key}.");
                }
            }
            else
            {
                if (!cacheItem.IsExpired() && throwException == true)
                {
                    throw new MappingExistsException($"An existing cache for key '{item.Key}' already exists.");
                }
                cacheItem.Update(item, throwException);
            }
        }

        /// <summary>
        /// Clears the collection of the cache.
        /// </summary>
        public void Clear() =>
            _cache.Clear();

        /// <summary>
        /// Checks whether the key is present in the collection.
        /// </summary>
        /// <param name="key">The name of the key to be checked.</param>
        /// <returns>A boolean value that signifies the presence of the key from the collection.</returns>
        public bool Contains(string key)
        {
            if (_cache.TryGetValue(key, out var value) == true)
            {
                if (value is IExpirable expirable)
                {
                    return expirable.IsExpired() == false;
                }
            }
            return false;
        }

        /// <summary>
        /// Gets an object from the cache collection.
        /// </summary>
        /// <typeparam name="T">The type of the cache item value.</typeparam>
        /// <param name="key">The key of the cache object to be retrieved.</param>
        /// <returns>A cached item object from the cache collection based on the given key.</returns>
        /// <param name="throwException">Throws an exception if the item is not found.</param>
        public CacheItem<T> Get<T>(string key,
            bool throwException = true)
        {
            var item = (CacheItem<T>)null;
            if (_cache.TryGetValue(key, out var value))
            {
                item = value as CacheItem<T>;
            }
            if (item?.IsExpired() == false)
            {
                return item;
            }
            return null;
        }

        /// <summary>
        /// Removes the item from the cache collection.
        /// </summary>
        /// <param name="key">The key of the item to be removed from the cache collection.</param>
        /// <param name="throwException">Throws an exception if the operation has failed to remove an item.</param>
        public void Remove(string key,
            bool throwException = true)
        {
            if (_cache.TryRemove(key, out var _) == false && throwException == true)
            {
                throw new ItemNotFoundException($"Failed to remove an item with key '{key}'.");
            }
        }

        #endregion

        #region Async

        /// <summary>
        /// Adds a cache item value in an asynchronous way.
        /// </summary>
        /// <typeparam name="T">The type of the cache item value.</typeparam>
        /// <param name="key">The key to the cache.</param>
        /// <param name="value">The value of the cache.</param>
        /// <param name="expiration">The expiration in minutes of the cache item.</param>
        /// <param name="throwException">Throws an exception if the operation has failed to add an item.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        public Task AddAsync<T>(string key,
            T value,
            int expiration = Constant.DefaultCacheItemExpirationInMinutes,
            bool throwException = true,
            CancellationToken cancellationToken = default) =>
            AddAsync(new CacheItem<T>(key, value, expiration), throwException, cancellationToken);

        /// <summary>
        /// Adds a cache item value in an asynchronous way.
        /// </summary>
        /// <typeparam name="T">The type of the cache item value.</typeparam>
        /// <param name="item">The cache item to be added in the collection.</param>
        /// <param name="throwException">Throws an exception if the operation has failed to add an item.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        public Task AddAsync<T>(CacheItem<T> item,
            bool throwException = true,
            CancellationToken cancellationToken = default)
        {
            Add(item, throwException);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Clears the collection of the cache in an asynchronous way.
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// </summary>
        public Task ClearAsync(CancellationToken cancellationToken = default)
        {
            _cache.Clear();
            return Task.CompletedTask;
        }

        /// <summary>
        /// Checks whether the key is present in the collection in an asynchronous way.
        /// </summary>
        /// <param name="key">The name of the key to be checked.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>A boolean value that signifies the presence of the key from the collection.</returns>
        public Task<bool> ContainsAsync(string key,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(Contains(key));

        /// <summary>
        /// Gets an object from the cache collection in an asynchronous way.
        /// </summary>
        /// <typeparam name="T">The type of the cache item value.</typeparam>
        /// <param name="throwException">Throws an exception if the item is not found.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <param name="key">The key of the cache object to be retrieved.</param>
        /// <returns>A cached item object from the cache collection based on the given key.</returns>
        public Task<CacheItem<T>> GetAsync<T>(string key,
            bool throwException = true,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(Get<T>(key, throwException));

        /// <summary>
        /// Removes the item from the cache collection in an asynchronous way.
        /// </summary>
        /// <param name="key">The key of the item to be removed from the cache collection.</param>
        /// <param name="throwException">Throws an exception if the operation has failed to remove an item.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        public Task RemoveAsync(string key,
            bool throwException = true,
            CancellationToken cancellationToken = default)
        {
            Remove(key, throwException);
            return Task.CompletedTask;
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Gets the enumerator of the cache collection.
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator() =>
            GetEnumerator();

        /// <summary>
        /// Gets the enumerator of the cache collection.
        /// </summary>
        /// <returns></returns>
        public IEnumerator GetEnumerator() =>
            _cache
                .Where(kvp => (kvp.Value as IExpirable)?.IsExpired() == false)
                .Select(kvp => kvp.Value)
                .GetEnumerator();

        #endregion
    }
}
