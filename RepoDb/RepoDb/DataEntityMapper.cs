using System;
using System.Collections.Generic;

namespace RepoDb
{
    /// <summary>
    /// A class that manage the mappings of a <i>RepoDb.DataEntity</i> object into database object.
    /// </summary>
    public static class DataEntityMapper
    {
        private static readonly IDictionary<Type, DataEntityMapItem> _cache;
        private static readonly object _syncLock;

        static DataEntityMapper()
        {
            _cache = new Dictionary<Type, DataEntityMapItem>();
            _syncLock = new object();
        }

        /// <summary>
        /// Create a new entity and database mapping.
        /// </summary>
        /// <param name="type">The type of command to be used for mapping.</param>
        /// <returns>An instance of <i>RepoDb.DataEntityMapItem</i> that is used for mapping.</returns>
        internal static DataEntityMapItem For(Type type)
        {
            var value = (DataEntityMapItem)null;
            lock (_syncLock)
            {
                if (_cache.ContainsKey(type))
                {
                    value = _cache[type];
                }
                else
                {
                    value = new DataEntityMapItem();
                    _cache.Add(type, value);
                }
            }
            return value;
        }

        /// <summary>
        /// Creates a new entity and database mapping.
        /// </summary>
        /// <typeparam name="TEntity">The <i>RepoDb.DataEntity</i> type where to apply the mapping.</typeparam>
        /// <returns>An instance of <i>RepoDb.DataEntityMapItem</i> that is used for mapping.</returns>
        public static DataEntityMapItem For<TEntity>()
            where TEntity : DataEntity
        {
            return For(typeof(TEntity));
        }
    }
}
