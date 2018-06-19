using System.Collections.Generic;

namespace RepoDb.Interfaces
{
    /// <summary>
    /// An interface used to mark the class to be a cacher for an object.
    /// </summary>
    public interface ICache : IEnumerable<CacheItem>
    {
        /// <summary>
        /// Adds a cache item value.
        /// </summary>
        /// <param name="key">The key to the cache.</param>
        /// <param name="value">The value of the cache.</param>
        void Add(string key, object value);

        /// <summary>
        /// Adds a cache item value.
        /// </summary>
        /// <param name="item">
        /// The cache item to be added in the collection.
        /// </param>
        void Add(CacheItem item);

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
        /// <param name="key">The key of the cache object to be retrieved.</param>
        /// <returns>An object from the cache collection based on the given key.</returns>
        CacheItem Get(string key);

        /// <summary>
        /// Removes the item from the cache collection.
        /// </summary>
        /// <param name="key">
        /// The key of the item to be removed from the cache collection. If the given key is not present, this method will ignore it.
        /// </param>
        void Remove(string key);
    }
}
