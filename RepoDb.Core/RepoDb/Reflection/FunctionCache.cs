using RepoDb.Extensions;
using RepoDb.Reflection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Dynamic;
using System.Linq;

namespace RepoDb
{
    /// <summary>
    /// A class used to cache the compiled functions.
    /// </summary>
    internal static class FunctionCache
    {
        private static ConcurrentDictionary<string, Action<DbCommand, object>> m_cache = new ConcurrentDictionary<string, Action<DbCommand, object>>();

        #region GetDataReaderToDataEntityFunction<TEntity>

        /// <summary>
        /// Gets a compiled function that is used to convert the <see cref="DbDataReader"/> object into a list of data entity objects.
        /// </summary>
        /// <typeparam name="TEntity">The data entity object to convert to.</typeparam>
        /// <param name="reader">The <see cref="DbDataReader"/> to be converted.</param>
        /// <param name="connection">The used <see cref="IDbConnection"/> object.</param>
        /// <returns>A compiled function that is used to cover the <see cref="DbDataReader"/> object into a list of data entity objects.</returns>
        public static Func<DbDataReader, TEntity> GetDataReaderToDataEntityFunction<TEntity>(DbDataReader reader,
            IDbConnection connection)
            where TEntity : class
        {
            return GetDataReaderToDataEntityFunction<TEntity>(reader, connection);
        }

        /// <summary>
        /// Gets a compiled function that is used to convert the <see cref="DbDataReader"/> object into a list of data entity objects.
        /// </summary>
        /// <typeparam name="TEntity">The data entity object to convert to.</typeparam>
        /// <param name="reader">The <see cref="DbDataReader"/> to be converted.</param>
        /// <param name="connection">The used <see cref="IDbConnection"/> object.</param>
        /// <param name="basedOnFields">Check whether to create a compiled function based on the data reader fields.</param>
        /// <returns>A compiled function that is used to cover the <see cref="DbDataReader"/> object into a list of data entity objects.</returns>
        internal static Func<DbDataReader, TEntity> GetDataReaderToDataEntityFunction<TEntity>(DbDataReader reader,
            IDbConnection connection,
            bool basedOnFields = false)
            where TEntity : class
        {
            if (basedOnFields == false)
            {
                return DataReaderToDataEntityFunctionCache<TEntity>.Get(reader, connection);
            }
            else
            {
                return FieldBasedDataReaderToDataEntityFunctionCache<TEntity>.Get(reader, connection);
            }
        }

        #region DataReaderToDataEntityFunctionCache

        private static class DataReaderToDataEntityFunctionCache<TEntity>
            where TEntity : class
        {
            private static Func<DbDataReader, TEntity> m_func;

            public static Func<DbDataReader, TEntity> Get(DbDataReader reader, IDbConnection connection)
            {
                if (m_func == null)
                {
                    m_func = FunctionFactory.GetDataReaderToDataEntityFunction<TEntity>(reader, connection);
                }
                return m_func;
            }
        }

        #endregion

        #region FieldBasedDataReaderToDataEntityFunctionCache

        private static class FieldBasedDataReaderToDataEntityFunctionCache<TEntity>
            where TEntity : class
        {
            private static ConcurrentDictionary<string, Func<DbDataReader, TEntity>> m_cache = new ConcurrentDictionary<string, Func<DbDataReader, TEntity>>();

            public static Func<DbDataReader, TEntity> Get(DbDataReader reader, IDbConnection connection)
            {
                var result = (Func<DbDataReader, TEntity>)null;
                var fields = Enumerable.Range(0, reader.FieldCount)
                    .Select(reader.GetName)
                    .Join(".");
                var key = string.Concat(connection?.ConnectionString, ".",
                    typeof(TEntity).FullName, ".",
                    fields);
                if (m_cache.TryGetValue(key, out result) == false)
                {
                    result = FunctionFactory.GetDataReaderToDataEntityFunction<TEntity>(reader, connection);
                    m_cache.TryAdd(key, result);
                }
                return result;
            }
        }

        #endregion

        #endregion

        #region GetDataReaderToExpandoObjectFunction

        /// <summary>
        /// Gets a compiled function that is used to convert the <see cref="DbDataReader"/> object into a list of dynamic objects.
        /// </summary>
        /// <param name="reader">The <see cref="DbDataReader"/> to be converted.</param>
        /// <returns>A compiled function that is used to convert the <see cref="DbDataReader"/> object into a list of dynamic objects.</returns>
        public static Func<DbDataReader, ExpandoObject> GetDataReaderToExpandoObjectFunction(DbDataReader reader)
        {
            return DataReaderToExpandoObjectFunctionCache.Get(reader);
        }

        /// <summary>
        /// Gets a compiled function that is used to convert the <see cref="DbDataReader"/> object into a list of dynamic objects.
        /// </summary>
        /// <param name="reader">The <see cref="DbDataReader"/> to be converted.</param>
        /// <param name="basedOnFields">Check whether to create a compiled function based on the data reader fields.</param>
        /// <returns>A compiled function that is used to convert the <see cref="DbDataReader"/> object into a list of dynamic objects.</returns>
        internal static Func<DbDataReader, ExpandoObject> GetDataReaderToExpandoObjectFunction(DbDataReader reader,
            bool basedOnFields = false)
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

        #endregion

        #region GetDataCommandParameterSetterFunction

        /// <summary>
        /// Gets a compiled function that is used to set the <see cref="DbParameter"/> objects of the <see cref="DbCommand"/> object.
        /// </summary>
        /// <param name="key">The key to the cache.</param>
        /// <param name="inputFields">The list of the input <see cref="Field"/> objects to be used.</param>
        /// <param name="outputFields">The list of the output <see cref="Field"/> objects to be used.</param>
        /// <param name="batchSize">The batch size of the entity to be passed.</param>
        /// <returns>A compiled function that is used to set the <see cref="DbParameter"/> objects of the <see cref="DbCommand"/> object.</returns>
        public static Action<DbCommand, IList<TEntity>> GetDataCommandParameterSetterFunction<TEntity>(string key,
            IEnumerable<Field> inputFields,
            IEnumerable<Field> outputFields,
            int batchSize)
            where TEntity : class
        {
            return DataCommandParameterSetterCache<TEntity>.Get(key, inputFields, outputFields, batchSize);
        }

        #region DataCommandParameterSetterCache

        private static class DataCommandParameterSetterCache<TEntity>
            where TEntity : class
        {
            private static ConcurrentDictionary<string, Action<DbCommand, IList<TEntity>>> m_cache =
                new ConcurrentDictionary<string, Action<DbCommand, IList<TEntity>>>();

            public static Action<DbCommand, IList<TEntity>> Get(string key,
                IEnumerable<Field> inputFields,
                IEnumerable<Field> outputFields,
                int batchSize)
            {
                key = string.Concat(key, ".", inputFields.Select(field => field.UnquotedName).Join("."), ".",
                   outputFields?.Select(field => field.UnquotedName).Join("."), ".", batchSize);
                var func = (Action<DbCommand, IList<TEntity>>)null;
                if (m_cache.TryGetValue(key, out func) == false)
                {
                    func = FunctionFactory.GetDataCommandParameterSetterFunction<TEntity>(inputFields, outputFields, batchSize);
                    m_cache.TryAdd(key, func);
                }
                return func;
            }
        }

        #endregion

        #endregion
    }
}
