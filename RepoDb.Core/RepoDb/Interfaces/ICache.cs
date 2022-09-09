using System.Collections;
using System.Threading;
using System.Threading.Tasks;

namespace RepoDb.Interfaces
{
    /// <summary>
    /// An interface that is used to mark a class to be a caching class object.
    /// </summary>
    public interface ICache : IEnumerable
    {
        #region Sync

        /// <summary>
        /// Adds a cache item value.
        /// </summary>
        /// <typeparam name="T">The type of the cache item value.</typeparam>
        /// <param name="key">The key to the cache.</param>
        /// <param name="value">The value of the cache.</param>
        /// <param name="expiration">The expiration in minutes of the cache item.</param>
        /// <param name="throwException">Throws an exception if the operation has failed to add an item.</param>
        void Add<T>(string key,
            T value,
            int expiration = Constant.DefaultCacheItemExpirationInMinutes,
            bool throwException = true);

        /// <summary>
        /// Adds a cache item value.
        /// </summary>
        /// <typeparam name="T">The type of the cache item value.</typeparam>
        /// <param name="item">The cache item to be added in the collection.</param>
        /// <param name="throwException">Throws an exception if the operation has failed to add an item.</param>
        void Add<T>(CacheItem<T> item,
            bool throwException = true);

        /// <summary>
        /// Clears the collection of the cache.
        /// </summary>
        void Clear();

        /// <summary>
        /// Checks whether the key is present in the collection.
        /// </summary>
        /// <param name="key">The name of the key to be checked.</param>
        /// <returns>A boolean value that signifies the presence of the key from the collection.</returns>
        bool Contains(string key);

        /// <summary>
        /// Gets an object from the cache collection.
        /// </summary>
        /// <typeparam name="T">The type of the cache item value.</typeparam>
        /// <param name="key">The key of the cache object to be retrieved.</param>
        /// <returns>A cached item object from the cache collection based on the given key.</returns>
        /// <param name="throwException">Throws an exception if the item is not found.</param>
        CacheItem<T> Get<T>(string key,
            bool throwException = true);

        /// <summary>
        /// Removes the item from the cache collection.
        /// </summary>
        /// <param name="key">The key of the item to be removed from the cache collection.</param>
        /// <param name="throwException">Throws an exception if the operation has failed to remove an item.</param>
        void Remove(string key,
            bool throwException = true);

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
        Task AddAsync<T>(string key,
            T value,
            int expiration = Constant.DefaultCacheItemExpirationInMinutes,
            bool throwException = true,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Adds a cache item value in an asynchronous way.
        /// </summary>
        /// <typeparam name="T">The type of the cache item value.</typeparam>
        /// <param name="item">The cache item to be added in the collection.</param>
        /// <param name="throwException">Throws an exception if the operation has failed to add an item.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        Task AddAsync<T>(CacheItem<T> item,
            bool throwException = true,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Clears the collection of the cache in an asynchronous way.
        /// </summary>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        Task ClearAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Checks whether the key is present in the collection in an asynchronous way.
        /// </summary>
        /// <param name="key">The name of the key to be checked.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>A boolean value that signifies the presence of the key from the collection.</returns>
        Task<bool> ContainsAsync(string key,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets an object from the cache collection in an asynchronous way.
        /// </summary>
        /// <typeparam name="T">The type of the cache item value.</typeparam>
        /// <param name="key">The key of the cache object to be retrieved.</param>
        /// <param name="throwException">Throws an exception if the item is not found.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>A cached item object from the cache collection based on the given key.</returns>
        Task<CacheItem<T>> GetAsync<T>(string key,
            bool throwException = true,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Removes the item from the cache collection in an asynchronous way.
        /// </summary>
        /// <param name="key">The key of the item to be removed from the cache collection.</param>
        /// <param name="throwException">Throws an exception if the operation has failed to remove an item.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        Task RemoveAsync(string key,
            bool throwException = true,
            CancellationToken cancellationToken = default);

        #endregion
    }
}
