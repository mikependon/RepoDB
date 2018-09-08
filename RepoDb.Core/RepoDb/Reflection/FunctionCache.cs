using RepoDb.Extensions;
using RepoDb.Reflection;
using System;
using System.Collections.Concurrent;
using System.Data.Common;
using System.Linq;

namespace RepoDb
{
    /// <summary>
    /// A class used to cache the compiled functions.
    /// </summary>
    public static class FunctionCache
    {
        /// <summary>
        /// Gets a delegate that is used to convert the <see cref="DbDataReader"/> object into data entity object.
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
        /// Gets a delegate that is used to convert the <see cref="DbDataReader"/> object into data entity object.
        /// </summary>
        /// <typeparam name="TEntity">The data entity object to convert to.</typeparam>
        /// <param name="reader">The <see cref="DbDataReader"/> to be converted.</param>
        /// <param name="basedOnFields">Check whether to create a delegate based on the data reader fields.</param>
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
                var key = $"{typeof(TEntity).FullName}.{fields}";
                if (m_cache.TryGetValue(key, out result) == false)
                {
                    result = FunctionFactory.GetDataReaderToDataEntityFunction<TEntity>(reader);
                    m_cache.TryAdd(key, result);
                }
                return result;
            }
        }

        #endregion
    }
}
