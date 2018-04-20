using RepoDb.Interfaces;
using System;
using System.Collections.Generic;

namespace RepoDb.Reflection
{
    /// <summary>
    /// An object that holds the cache for all Reflected delegates.
    /// </summary>
    public static class DelegateCache
    {
        private static readonly IDictionary<Type, Delegate> _createEntityDelegates = new Dictionary<Type, Delegate>();

        /// <summary>
        /// Creates a Delegate for mapping a System.Data.Common.DbDataReader to RepoDb.Interfaces.IDataEntity object.
        /// </summary>
        /// <typeparam name="TEntity">The RepoDb.Interfaces.IDataEntity type to convert.</typeparam>
        /// <returns>An IL emitted Delegate object used to convert the System.Data.Common.DbDataReader to RepoDb.Interfaces.IDataEntity.</returns>
        public static DataReaderToEntityMapperDelegate<TEntity> GetDataReaderToEntityMapperDelegate<TEntity>()
            where TEntity : IDataEntity
        {//GetDataReaderToEntity
            var key = typeof(TEntity);
            if (!_createEntityDelegates.ContainsKey(key))
            {
                var value = DelegateFactory.GetDataReaderToEntity<TEntity>();
                _createEntityDelegates.Add(key, value);
            }
            return (DataReaderToEntityMapperDelegate<TEntity>)_createEntityDelegates[key];
        }
    }
}
