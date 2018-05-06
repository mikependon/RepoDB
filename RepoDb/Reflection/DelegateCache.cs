using RepoDb.Interfaces;
using RepoDb.Reflection.Delegates;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Dynamic;

namespace RepoDb.Reflection
{
    /// <summary>
    /// An object that holds the cache for all Reflected delegates.
    /// </summary>
    public static class DelegateCache
    {
        private static readonly IDictionary<string, Delegate> _cache = new Dictionary<string, Delegate>();

        /// <summary>
        /// Creates a Delegate for mapping a System.Data.Common.DbDataReader to RepoDb.Interfaces.IDataEntity object.
        /// </summary>
        /// <typeparam name="TEntity">The RepoDb.Interfaces.IDataEntity type to convert.</typeparam>
        /// <param name="reader">The System.Data.Common.DbDataReader to be converted.</param>
        /// <returns>An IL emitted Delegate object used to convert the System.Data.Common.DbDataReader to RepoDb.Interfaces.IDataEntity.</returns>
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

        ///// <summary>
        ///// Creates a Delegate for mapping a System.Data.Common.DbDataReader to System.Dynamic.ExpandoObject object.
        ///// </summary>
        ///// <param name="reader">The System.Data.Common.DbDataReader to be converted.</param>
        ///// <returns>An IL emitted Delegate object used to convert the System.Data.Common.DbDataReader to System.Dynamic.ExpandoObject.</returns>
        //public static DataReaderToExpandoObjectDelegate GetDataReaderToExpandoObjectDelegate(DbDataReader reader)
        //{
        //    var key = $"{typeof(ExpandoObject).FullName}.DataReaderToExpandoObject".ToLower();
        //    if (!_cache.ContainsKey(key))
        //    {
        //        var value = DelegateFactory.GetDataReaderToExpandoObjectDelegate(reader);
        //        _cache.Add(key, value);
        //    }
        //    return (DataReaderToExpandoObjectDelegate)_cache[key];
        //}

        /// <summary>
        /// Creates a Delegate for mapping a System.Data.Common.DbDataReader to RepoDb.Interfaces.IDataEntity object.
        /// </summary>
        /// <typeparam name="TEntity">The RepoDb.Interfaces.IDataEntity type to convert.</typeparam>
        /// <returns>An IL emitted Delegate object used to convert the System.Data.Common.DbDataReader to RepoDb.Interfaces.IDataEntity.</returns>
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
