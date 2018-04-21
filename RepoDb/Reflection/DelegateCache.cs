using RepoDb.Interfaces;
using RepoDb.Reflection.Delegates;
using System;
using System.Collections.Generic;

namespace RepoDb.Reflection
{
    /// <summary>
    /// An object that holds the cache for all Reflected delegates.
    /// </summary>
    public static class DelegateCache
    {
        private static readonly IDictionary<Type, Delegate> _dataReaderToDataEntityDelegateCache = new Dictionary<Type, Delegate>();
        private static readonly IDictionary<Type, Delegate> _dataEntityToDataRowDelegateCache = new Dictionary<Type, Delegate>();

        /// <summary>
        /// Creates a Delegate for mapping a System.Data.Common.DbDataReader to RepoDb.Interfaces.IDataEntity object.
        /// </summary>
        /// <typeparam name="TEntity">The RepoDb.Interfaces.IDataEntity type to convert.</typeparam>
        /// <returns>An IL emitted Delegate object used to convert the System.Data.Common.DbDataReader to RepoDb.Interfaces.IDataEntity.</returns>
        public static DataReaderToDataEntityDelegate<TEntity> GetDataReaderToDataEntityDelegate<TEntity>()
            where TEntity : IDataEntity
        {
            var key = typeof(TEntity);
            if (!_dataReaderToDataEntityDelegateCache.ContainsKey(key))
            {
                var value = DelegateFactory.GetDataReaderToDataEntityDelegate<TEntity>();
                _dataReaderToDataEntityDelegateCache.Add(key, value);
            }
            return (DataReaderToDataEntityDelegate<TEntity>)_dataReaderToDataEntityDelegateCache[key];
        }

        /// <summary>
        /// Creates a Delegate for mapping a System.Data.Common.DbDataReader to RepoDb.Interfaces.IDataEntity object.
        /// </summary>
        /// <typeparam name="TEntity">The RepoDb.Interfaces.IDataEntity type to convert.</typeparam>
        /// <returns>An IL emitted Delegate object used to convert the System.Data.Common.DbDataReader to RepoDb.Interfaces.IDataEntity.</returns>
        public static DataEntityToDataRowDelegate<TEntity> GetDataEntityToDataRowDelegate<TEntity>()
            where TEntity : IDataEntity
        {
            var key = typeof(TEntity);
            if (!_dataEntityToDataRowDelegateCache.ContainsKey(key))
            {
                var value = DelegateFactory.GetDataEntityToDataRowDelegate<TEntity>();
                _dataEntityToDataRowDelegateCache.Add(key, value);
            }
            return (DataEntityToDataRowDelegate<TEntity>)_dataEntityToDataRowDelegateCache[key];
        }
    }
}
