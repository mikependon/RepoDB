using RepoDb.Enumerations;
using RepoDb.Exceptions;
using RepoDb.Interfaces;
using System.Collections.Generic;

namespace RepoDb
{
    /// <summary>
    /// A class that manage the mappings of a RepoDb.Interfaces.IDataEntity object into database object.
    /// </summary>
    public static class DataEntityMapper
    {
        private static readonly IDictionary<string, DataEntityMap> _cache;
        private static readonly object _syncLock;

        static DataEntityMapper()
        {
            _cache = new Dictionary<string, DataEntityMap>();
            _syncLock = new object();
        }

        /// <summary>
        /// Add an entity mapping to the mapping collection.
        /// </summary>
        /// <typeparam name="TEntity">The entity to be mapped.</typeparam>
        /// <param name="command">The type of command to be used for mapping.</param>
        /// <param name="map">The mapping for database object.</param>
        public static void For<TEntity>(Command command, DataEntityMap map)
            where TEntity : IDataEntity
        {
            var type = typeof(TEntity);
            var key = $"{type.FullName}.{command.ToString()}".ToLower();
            lock (_syncLock)
            {
                if (_cache.ContainsKey(key))
                {
                    throw new DuplicateDataEntityMapException<TEntity>(command);
                }
                _cache.Add(key, map);
            }
        }
    }
}
