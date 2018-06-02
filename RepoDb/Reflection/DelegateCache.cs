using RepoDb.Interfaces;
using RepoDb.Reflection.Delegates;
using System;
using System.Collections.Generic;
using System.Data.Common;

namespace RepoDb.Reflection
{
    /// <summary>
    /// An object that holds the cache for all Reflected delegates.
    /// </summary>
    public static class DelegateCache
    {
        private static readonly IDictionary<string, Delegate> _cache = new Dictionary<string, Delegate>();

        /// <summary>
        /// Creates a Delegate for mapping a <i>System.Data.Common.DbDataReader</i> to <i>RepoDb.Interfaces.IDataEntity</i> object.
        /// </summary>
        /// <typeparam name="TEntity">The <i>RepoDb.Interfaces.IDataEntity</i> type to convert.</typeparam>
        /// <param name="reader">The <i>System.Data.Common.DbDataReader</i> to be converted.</param>
        /// <returns>An IL emitted Delegate object used to convert the <i>System.Data.Common.DbDataReader</i> to <i>RepoDb.Interfaces.IDataEntity</i> object.</returns>
        public static DataReaderToDataEntityDelegate<TEntity> GetDataReaderToDataEntityDelegate<TEntity>(DbDataReader reader)
            where TEntity : IDataEntity
        {
            var key = $"{typeof(TEntity).FullName}.DataReaderToDataEntity".ToLower();
            if (!_cache.ContainsKey(key))
            {
                var value = DelegateFactory.GetDataReaderToDataEntityDelegate<TEntity>(reader);
                _cache.Add(key, value);
            }
            return (DataReaderToDataEntityDelegate<TEntity>)_cache[key];
        }

        /// <summary>
        /// Creates a Delegate for mapping a <i>System.Data.Common.DbDataReader</i> to <i>RepoDb.Interfaces.IDataEntity</i> object.
        /// </summary>
        /// <typeparam name="TEntity">The <i>RepoDb.Interfaces.IDataEntity type to convert.</typeparam>
        /// <returns>An IL emitted Delegate object used to convert the <i>System.Data.Common.DbDataReader</i> to <i>RepoDb.Interfaces.IDataEntity</i> object.</returns>
        public static DataEntityToDataRowDelegate<TEntity> GetDataEntityToDataRowDelegate<TEntity>()
            where TEntity : IDataEntity
        {
            var key = $"{typeof(TEntity).FullName}.DataEntityToDataRow".ToLower();
            if (!_cache.ContainsKey(key))
            {
                var value = DelegateFactory.GetDataEntityToDataRowDelegate<TEntity>();
                _cache.Add(key, value);
            }
            return (DataEntityToDataRowDelegate<TEntity>)_cache[key];
        }
    }
}
