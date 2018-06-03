using System;

namespace RepoDb.Interfaces
{
    /// <summary>
    /// An interface used to mark a class to be an item used when caching an object.
    /// </summary>
    public interface ICacheItem
    {
        /// <summary>
        /// Gets the key of the cache.
        /// </summary>
        string Key { get; }

        /// <summary>
        /// Gets the value of the cache.
        /// </summary>
        object Value { get; set; }

        /// <summary>
        /// Gets or sets the timestamp of this cache item.
        /// </summary>
        DateTime Timestamp { get; set; }
    }
}
