using System;

namespace RepoDb
{
    /// <summary>
    /// An item used when caching an object in the repository object. This is the default class used by the <i>RepoDb.MemoryCache</i> object.
    /// </summary>
    public class CacheItem
    {
        /// <summary>
        /// Creates a new instance of <i>RepoDb.CacheItem</i> object.
        /// </summary>
        /// <param name="key">The key of the cache.</param>
        /// <param name="value">The value of the cache.</param>
        public CacheItem(string key, object value)
            : this(key, value, Constant.CacheItemExpirationInMinutes)
        {
        }

        /// <summary>
        /// Creates a new instance of <i>RepoDb.CacheItem</i> object.
        /// </summary>
        /// <param name="key">The key of the cache item.</param>
        /// <param name="value">The value of the cache item.</param>
        /// <param name="expirationInMinutes">The expiration in minutes of the cache item.</param>
        public CacheItem(string key, object value, int expirationInMinutes)
        {
            if (expirationInMinutes < 0)
            {
                throw new ArgumentOutOfRangeException("Expiration in minutes.");
            }
            Key = key;
            Value = value;
            CreatedDate = DateTime.UtcNow;
        }

        /// <summary>
        /// Updates the value of the current item based from the source item.
        /// </summary>
        /// <param name="item">The source item.</param>
        internal void UpdateFrom(CacheItem item)
        {
            if (!IsExpired())
            {
                throw new InvalidOperationException($"The current item is not yet expired.");
            }
            else
            {
                Value = item.Value;
                CreatedDate = DateTime.UtcNow;
            }
        }

        /// <summary>
        /// Gets the key of the cache.
        /// </summary>
        public string Key { get; }

        /// <summary>
        /// Gets or sets the value of the cache.
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// Gets the created timestamp of this cache item. By default, it is equals to the time
        /// of when this cache item object has been instantiated.
        /// </summary>
        public DateTime CreatedDate { get; private set; }

        /// <summary>
        /// Gets or sets the expiration date of this cache item.
        /// </summary>
        public DateTime Expiration { get; set; }

        /// <summary>
        /// Gets a value whether this cache item is expired.
        /// </summary>
        /// <returns>A boolean value that indicate whether this cache value is expired.</returns>
        public bool IsExpired()
        {
            return DateTime.UtcNow > Expiration;
        }
    }
}