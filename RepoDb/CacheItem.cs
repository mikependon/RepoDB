using System;
using RepoDb.Interfaces;

namespace RepoDb
{
    /// <summary>
    /// An item used when caching an object in the repository object. This is the default class used by the
    /// <i>RepoDb.MemoryCache</i> object.
    /// </summary>
    public class CacheItem : ICacheItem
    {
        /// <summary>
        /// Creates a new instance of <i>RepoDb.CacheItem</i> object.
        /// </summary>
        /// <param name="key">The key of the cache.</param>
        /// <param name="value">The value of the cache.</param>
        public CacheItem(string key, object value)
        {
            Key = key;
            Value = value;
            Timestamp = DateTime.UtcNow;
        }

        /// <summary>
        /// Gets the key of the cache.
        /// </summary>
        public string Key { get; }

        /// <summary>
        /// Gets the value of the cache.
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// Gets or sets the timestamp of this cache item. By default, it is equals to the time
        /// of when this cache item object has been instantiated.
        /// </summary>
        public DateTime Timestamp { get; set; }
    }
}