using RepoDb.Extensions;
using RepoDb.Reflection;
using RepoDb.Reflection.Delegates;
using System.Collections.Concurrent;
using System.Data.Common;
using System.Linq;

namespace RepoDb
{
    /// <summary>
    /// A class used to cache the dynamic delegate.
    /// </summary>
    public static class DelegateCache
    {
        /// <summary>
        /// Gets a delegate that is used to convert the <see cref="DbDataReader"/> object into data entity object.
        /// </summary>
        /// <typeparam name="TEntity">The data entity object to convert to.</typeparam>
        /// <param name="reader">The <see cref="DbDataReader"/> to be converted.</param>
        /// <returns>An instance of data entity object.</returns>
        public static DataReaderToDataEntityDelegate<TEntity> GetDataReaderToDataEntityDelegate<TEntity>(DbDataReader reader)
            where TEntity : class
        {
            return GetDataReaderToDataEntityDelegate<TEntity>(reader);
        }

        /// <summary>
        /// Gets a delegate that is used to convert the <see cref="DbDataReader"/> object into data entity object.
        /// </summary>
        /// <typeparam name="TEntity">The data entity object to convert to.</typeparam>
        /// <param name="reader">The <see cref="DbDataReader"/> to be converted.</param>
        /// <param name="basedOnFields">Check whether to create a delegate based on the data reader fields.</param>
        /// <returns>An instance of data entity object.</returns>
        internal static DataReaderToDataEntityDelegate<TEntity> GetDataReaderToDataEntityDelegate<TEntity>(DbDataReader reader, bool basedOnFields = false)
            where TEntity : class
        {
            if (basedOnFields == false)
            {
                return DataReaderToDataEntityDelegateCache<TEntity>.Get(reader);
            }
            else
            {
                return FieldBasedDataReaderToDataEntityDelegateCache<TEntity>.Get(reader);
            }
        }

        /// <summary>
        /// Gets a delegate that is used to convert the <see cref="DbDataReader"/> object into data entity object.
        /// </summary>
        /// <param name="reader">The <see cref="DbDataReader"/> to be converted.</param>
        /// <returns>An instance of data entity object.</returns>
        public static DataReaderToExpandoObjectDelegate GetDataReaderToExpandoObjectDelegate(DbDataReader reader)
        {
            return DataReaderToExpandoObjectDelegateCache.Get(reader);
        }

        /// <summary>
        /// Gets a delegate that is used to convert the <see cref="DbDataReader"/> object into data entity object.
        /// </summary>
        /// <param name="reader">The <see cref="DbDataReader"/> to be converted.</param>
        /// <param name="basedOnFields">Check whether to create a delegate based on the data reader fields.</param>
        /// <returns>An instance of data entity object.</returns>
        internal static DataReaderToExpandoObjectDelegate GetDataReaderToExpandoObjectDelegate(DbDataReader reader, bool basedOnFields = false)
        {
            if (basedOnFields == false)
            {
                return DataReaderToExpandoObjectDelegateCache.Get(reader);
            }
            else
            {
                return FieldBasedDataReaderToExpandoObjectDelegateCache.Get(reader);
            }
        }

        #region DataReaderToDataEntityDelegateCache

        private static class DataReaderToDataEntityDelegateCache<TEntity>
            where TEntity : class
        {
            private static DataReaderToDataEntityDelegate<TEntity> m_delegate;

            public static DataReaderToDataEntityDelegate<TEntity> Get(DbDataReader reader)
            {
                if (m_delegate == null)
                {
                    m_delegate = DelegateFactory.GetDataReaderToDataEntityDelegate<TEntity>(reader);
                }
                return m_delegate;
            }
        }

        #endregion

        #region DataReaderWithCacheKeyToDataEntityDelegateCache

        private static class FieldBasedDataReaderToDataEntityDelegateCache<TEntity>
            where TEntity : class
        {
            private static ConcurrentDictionary<string, DataReaderToDataEntityDelegate<TEntity>> m_cache = new ConcurrentDictionary<string, DataReaderToDataEntityDelegate<TEntity>>();

            public static DataReaderToDataEntityDelegate<TEntity> Get(DbDataReader reader)
            {
                var result = (DataReaderToDataEntityDelegate<TEntity>)null;
                var fields = Enumerable.Range(0, reader.FieldCount)
                    .Select(reader.GetName)
                    .Join(".");
                var key = $"{typeof(TEntity).FullName}.{fields}";
                if (m_cache.TryGetValue(key, out result) == false)
                {
                    result = DelegateFactory.GetDataReaderToDataEntityDelegate<TEntity>(reader);
                    m_cache.TryAdd(key, result);
                }
                return result;
            }
        }

        #endregion

        #region DataReaderToExpandoObjectDelegateCache

        private static class DataReaderToExpandoObjectDelegateCache
        {
            private static DataReaderToExpandoObjectDelegate m_delegate;

            public static DataReaderToExpandoObjectDelegate Get(DbDataReader reader)
            {
                if (m_delegate == null)
                {
                    m_delegate = DelegateFactory.GetDataReaderToExpandoObjectDelegate(reader);
                }
                return m_delegate;
            }
        }

        #endregion

        #region FieldBasedDataReaderToExpandoObjectDelegateCache

        private static class FieldBasedDataReaderToExpandoObjectDelegateCache
        {
            private static ConcurrentDictionary<string, DataReaderToExpandoObjectDelegate> m_cache = new ConcurrentDictionary<string, DataReaderToExpandoObjectDelegate>();

            public static DataReaderToExpandoObjectDelegate Get(DbDataReader reader)
            {
                var result = (DataReaderToExpandoObjectDelegate)null;
                var key = Enumerable.Range(0, reader.FieldCount)
                    .Select(reader.GetName)
                    .Join(".");
                if (m_cache.TryGetValue(key, out result) == false)
                {
                    result = DelegateFactory.GetDataReaderToExpandoObjectDelegate(reader);
                    m_cache.TryAdd(key, result);
                }
                return result;
            }
        }

        #endregion
    }
}
