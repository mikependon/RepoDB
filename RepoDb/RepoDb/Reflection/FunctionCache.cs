using RepoDb.Extensions;
using RepoDb.Reflection;
using System;
using System.Collections.Concurrent;
using System.Data.Common;
using System.Dynamic;
using System.Linq;

namespace RepoDb
{
    /// <summary>
    /// A class used to cache the compiled functions.
    /// </summary>
    public static class FunctionCache
    {
        /// <summary>
        /// Gets a compiled function that is used to convert the <see cref="DbDataReader"/> object into data entity object.
        /// </summary>
        /// <typeparam name="TEntity">The data entity object to convert to.</typeparam>
        /// <param name="reader">The <see cref="DbDataReader"/> to be converted.</param>
        /// <returns>An instance of data entity object.</returns>
        public static Func<DbDataReader, TEntity> GetDataReaderToDataEntityFunction<TEntity>(DbDataReader reader)
            where TEntity : class
        {
            return GetDataReaderToDataEntityFunction<TEntity>(reader);
        }

        /// <summary>
        /// Gets a compiled function that is used to convert the <see cref="DbDataReader"/> object into data entity object.
        /// </summary>
        /// <typeparam name="TEntity">The data entity object to convert to.</typeparam>
        /// <param name="reader">The <see cref="DbDataReader"/> to be converted.</param>
        /// <param name="basedOnFields">Check whether to create a compiled function based on the data reader fields.</param>
        /// <returns>An compiled function that is used to cover the <see cref="DbDataReader"/> object into data entity object.</returns>
        internal static Func<DbDataReader, TEntity> GetDataReaderToDataEntityFunction<TEntity>(DbDataReader reader, bool basedOnFields = false)
            where TEntity : class
        {
            if (basedOnFields == false)
            {
                return DataReaderToDataEntityFunctionCache<TEntity>.Get(reader);
            }
            else
            {
                return FieldBasedDataReaderToDataEntityFunctionCache<TEntity>.Get(reader);
            }
        }

        /// <summary>
        /// Gets a compiled function that is used to convert the <see cref="DbDataReader"/> object into data entity object.
        /// </summary>
        /// <param name="reader">The <see cref="DbDataReader"/> to be converted.</param>
        /// <returns>An instance of data entity object via dynamics.</returns>
        public static Func<DbDataReader, ExpandoObject> GetDataReaderToExpandoObjectFunction(DbDataReader reader)
        {
            return DataReaderToExpandoObjectFunctionCache.Get(reader);
        }

        /// <summary>
        /// Gets a compiled function that is used to convert the <see cref="DbDataReader"/> object into data entity object.
        /// </summary>
        /// <param name="reader">The <see cref="DbDataReader"/> to be converted.</param>
        /// <param name="basedOnFields">Check whether to create a compiled function based on the data reader fields.</param>
        /// <returns>An instance of data entity object via dynamics.</returns>
        internal static Func<DbDataReader, ExpandoObject> GetDataReaderToExpandoObjectFunction(DbDataReader reader, bool basedOnFields = false)
        {
            if (basedOnFields == false)
            {
                return DataReaderToExpandoObjectFunctionCache.Get(reader);
            }
            else
            {
                return FieldBasedDataReaderToExpandoObjectFunctionCache.Get(reader);
            }
        }

        #region DataReaderToDataEntityFunctionCache

        private static class DataReaderToDataEntityFunctionCache<TEntity>
            where TEntity : class
        {
            private static Func<DbDataReader, TEntity> m_func;

            public static Func<DbDataReader, TEntity> Get(DbDataReader reader)
            {
                if (m_func == null)
                {
                    m_func = FunctionFactory.GetDataReaderToDataEntityFunction<TEntity>(reader);
                }
                return m_func;
            }
        }

        #endregion

        #region DataReaderWithCacheKeyToDataEntityDelegateCache

        private static class FieldBasedDataReaderToDataEntityFunctionCache<TEntity>
            where TEntity : class
        {
            private static ConcurrentDictionary<string, Func<DbDataReader, TEntity>> m_cache = new ConcurrentDictionary<string, Func<DbDataReader, TEntity>>();

            public static Func<DbDataReader, TEntity> Get(DbDataReader reader)
            {
                var result = (Func<DbDataReader, TEntity>)null;
                var fields = Enumerable.Range(0, reader.FieldCount)
                    .Select(reader.GetName)
                    .Join(".");
                var key = string.Concat(typeof(TEntity).FullName, ".", fields);
                if (m_cache.TryGetValue(key, out result) == false)
                {
                    result = FunctionFactory.GetDataReaderToDataEntityFunction<TEntity>(reader);
                    m_cache.TryAdd(key, result);
                }
                return result;
            }
        }

        #endregion

        #region DataReaderToExpandoObjectDelegateCache

        private static class DataReaderToExpandoObjectFunctionCache
        {
            private static Func<DbDataReader, ExpandoObject> m_func;

            public static Func<DbDataReader, ExpandoObject> Get(DbDataReader reader)
            {
                if (m_func == null)
                {
                    m_func = FunctionFactory.GetDataReaderToExpandoObjectFunction(reader);
                }
                return m_func;
            }
        }

        #endregion

        #region FieldBasedDataReaderToExpandoObjectFunctionCache

        private static class FieldBasedDataReaderToExpandoObjectFunctionCache
        {
            private static ConcurrentDictionary<string, Func<DbDataReader, ExpandoObject>> m_cache = new ConcurrentDictionary<string, Func<DbDataReader, ExpandoObject>>();

            public static Func<DbDataReader, ExpandoObject> Get(DbDataReader reader)
            {
                var result = (Func<DbDataReader, ExpandoObject>)null;
                var key = Enumerable.Range(0, reader.FieldCount)
                    .Select(reader.GetName)
                    .Join(".");
                if (m_cache.TryGetValue(key, out result) == false)
                {
                    result = FunctionFactory.GetDataReaderToExpandoObjectFunction(reader);
                    m_cache.TryAdd(key, result);
                }
                return result;
            }
        }

        #endregion
    }
}
