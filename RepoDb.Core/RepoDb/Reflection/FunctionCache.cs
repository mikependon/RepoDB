using RepoDb.Extensions;
using RepoDb.Interfaces;
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

        #region GetDataReaderToDataEntityConverterFunction

        /// <summary>
        /// Gets a compiled function that is used to convert the <see cref="DbDataReader"/> object into a list of data entity objects.
        /// </summary>
        /// <typeparam name="TEntity">The data entity object to convert to.</typeparam>
        /// <param name="reader">The <see cref="DbDataReader"/> to be converted.</param>
        /// <param name="connection">The used <see cref="IDbConnection"/> object.</param>
        /// <returns>A compiled function that is used to cover the <see cref="DbDataReader"/> object into a list of data entity objects.</returns>
        public static Func<DbDataReader, TEntity> GetDataReaderToDataEntityConverterFunction<TEntity>(DbDataReader reader,
            IDbConnection connection)
            where TEntity : class
        {
            return GetDataReaderToDataEntityConverterFunction<TEntity>(reader, connection);
        }

        /// <summary>
        /// Gets a compiled function that is used to convert the <see cref="DbDataReader"/> object into a list of data entity objects.
        /// </summary>
        /// <typeparam name="TEntity">The data entity object to convert to.</typeparam>
        /// <param name="reader">The <see cref="DbDataReader"/> to be converted.</param>
        /// <param name="connection">The used <see cref="IDbConnection"/> object.</param>
        /// <param name="transaction">The transaction object that is currently in used.</param>
        /// <returns>A compiled function that is used to cover the <see cref="DbDataReader"/> object into a list of data entity objects.</returns>
        internal static Func<DbDataReader, TEntity> GetDataReaderToDataEntityFunction<TEntity>(DbDataReader reader,
            IDbConnection connection,
            IDbTransaction transaction)
            where TEntity : class
        {
            return GetFieldBasedDataReaderToDataEntityFunctionCache<TEntity>.Get(reader, connection, transaction);
        }

        #region GetDataReaderToDataEntityConverterFunctionCache

        private static class GetDataReaderToDataEntityConverterFunctionCache<TEntity>
            where TEntity : class
        {
            private static Func<DbDataReader, TEntity> m_func;

            public static Func<DbDataReader, TEntity> Get(DbDataReader reader, IDbConnection connection, IDbTransaction transaction)
            {
                if (m_func == null)
                {
                    m_func = FunctionFactory.GetDataReaderToDataEntityConverterFunction<TEntity>(reader, connection, transaction);
                }
                return m_func;
            }
        }

        #endregion

        #region GetFieldBasedDataReaderToDataEntityFunctionCache

        private static class GetFieldBasedDataReaderToDataEntityFunctionCache<TEntity>
            where TEntity : class
        {
            private static ConcurrentDictionary<long, Func<DbDataReader, TEntity>> m_cache = new ConcurrentDictionary<long, Func<DbDataReader, TEntity>>();

            public static Func<DbDataReader, TEntity> Get(DbDataReader reader,
                IDbConnection connection,
                IDbTransaction transaction)
            {
                var result = (Func<DbDataReader, TEntity>)null;
                var fields = Enumerable.Range(0, reader.FieldCount)
                    .Select(reader.GetName)
                    .Join(".")
                    .GetHashCode();
                var key = typeof(TEntity).FullName.GetHashCode() + fields.GetHashCode();
                if (string.IsNullOrEmpty(connection?.ConnectionString) == false)
                {
                    key += connection.ConnectionString.GetHashCode();
                }
                if (m_cache.TryGetValue(key, out result) == false)
                {
                    result = FunctionFactory.GetDataReaderToDataEntityConverterFunction<TEntity>(reader, connection, transaction);
                    m_cache.TryAdd(key, result);
                }
                return result;
            }
        }

        #endregion

        #endregion

        #region GetDataReaderToExpandoObjectConverterFunction

        /// <summary>
        /// Gets a compiled function that is used to convert the <see cref="DbDataReader"/> object into a list of dynamic objects.
        /// </summary>
        /// <param name="reader">The <see cref="DbDataReader"/> to be converted.</param>
        /// <param name="transaction">The transaction object that is currently in used.</param>
        /// <returns>A compiled function that is used to convert the <see cref="DbDataReader"/> object into a list of dynamic objects.</returns>
        public static Func<DbDataReader, ExpandoObject> GetDataReaderToExpandoObjectConverterFunction(DbDataReader reader,
            IDbTransaction transaction)
        {
            return GetDataReaderToExpandoObjectConverterFunction(reader, null, null, transaction);
        }

        /// <summary>
        /// Gets a compiled function that is used to convert the <see cref="DbDataReader"/> object into a list of dynamic objects.
        /// </summary>
        /// <param name="reader">The <see cref="DbDataReader"/> to be converted.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="connection">The used <see cref="IDbConnection"/> object.</param>
        /// <param name="transaction">The transaction object that is currently in used.</param>
        /// <returns>A compiled function that is used to convert the <see cref="DbDataReader"/> object into a list of dynamic objects.</returns>
        internal static Func<DbDataReader, ExpandoObject> GetDataReaderToExpandoObjectConverterFunction(DbDataReader reader,
            string tableName,
            IDbConnection connection,
            IDbTransaction transaction)
        {
            return GetDataReaderToExpandoObjectConverterFunctionCache.Get(reader, tableName, connection, transaction);
        }

        #region GetDataReaderToExpandoObjectConverterFunctionCache

        private static class GetDataReaderToExpandoObjectConverterFunctionCache
        {
            private static ConcurrentDictionary<long, Func<DbDataReader, ExpandoObject>> m_cache = new ConcurrentDictionary<long, Func<DbDataReader, ExpandoObject>>();

            public static Func<DbDataReader, ExpandoObject> Get(DbDataReader reader,
                string tableName,
                IDbConnection connection,
                IDbTransaction transaction)
            {
                var result = (Func<DbDataReader, ExpandoObject>)null;
                var key = (long)Enumerable.Range(0, reader.FieldCount)
                    .Select(reader.GetName)
                    .Join(".")
                    .GetHashCode();
                if (tableName != null)
                {
                    key += tableName.GetHashCode();
                }
                if (string.IsNullOrEmpty(connection?.ConnectionString) == false)
                {
                    key += connection.ConnectionString.GetHashCode();
                }
                if (m_cache.TryGetValue(key, out result) == false)
                {
                    result = FunctionFactory.GetDataReaderToExpandoObjectConverterFunction(reader, tableName, connection, transaction);
                    m_cache.TryAdd(key, result);
                }
                return result;
            }
        }

        #endregion

        #endregion

        #region GetDataEntityDbCommandParameterSetterFunction

        /// <summary>
        /// Gets a compiled function that is used to set the <see cref="DbParameter"/> objects of the <see cref="DbCommand"/> object based from the values of the data entity/dynamic object.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity/dynamic objects.</typeparam>
        /// <param name="cacheKey">The key to the cache.</param>
        /// <param name="inputFields">The list of the input <see cref="DbField"/> objects to be used.</param>
        /// <param name="outputFields">The list of the ouput <see cref="DbField"/> objects to be used.</param>
        /// <param name="dbSetting">The currently in used <see cref="IDbSetting"/> object.</param>
        /// <returns>The compiled function.</returns>
        internal static Action<DbCommand, TEntity> GetDataEntityDbCommandParameterSetterFunction<TEntity>(string cacheKey,
            IEnumerable<DbField> inputFields,
            IEnumerable<DbField> outputFields,
            IDbSetting dbSetting)
            where TEntity : class
        {
            return GetDataEntityDbCommandParameterSetterFunctionCache<TEntity>.Get(cacheKey, inputFields, outputFields, dbSetting);
        }

        #region GetDataEntityDbCommandParameterSetterFunctionCache

        private static class GetDataEntityDbCommandParameterSetterFunctionCache<TEntity>
            where TEntity : class
        {
            private static ConcurrentDictionary<long, Action<DbCommand, TEntity>> m_cache = new ConcurrentDictionary<long, Action<DbCommand, TEntity>>();

            public static Action<DbCommand, TEntity> Get(string cacheKey,
                IEnumerable<DbField> inputFields,
                IEnumerable<DbField> outputFields,
                IDbSetting dbSetting)
            {
                var key = (long)cacheKey.GetHashCode();
                var func = (Action<DbCommand, TEntity>)null;
                if (inputFields != null)
                {
                    foreach (var field in inputFields)
                    {
                        key += field.GetHashCode();
                    }
                }
                if (outputFields != null)
                {
                    foreach (var field in outputFields)
                    {
                        key += field.GetHashCode();
                    }
                }
                if (m_cache.TryGetValue(key, out func) == false)
                {
                    func = FunctionFactory.GetDataEntityDbCommandParameterSetterFunction<TEntity>(inputFields, outputFields, dbSetting);
                    m_cache.TryAdd(key, func);
                }
                return func;
            }
        }

        #endregion

        #endregion

        #region GetDataEntitiesDbCommandParameterSetterFunction

        /// <summary>
        /// Gets a compiled function that is used to set the <see cref="DbParameter"/> objects of the <see cref="DbCommand"/> object based from the values of the data entity/dynamic objects.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity/dynamic objects.</typeparam>
        /// <param name="cacheKey">The key to the cache.</param>
        /// <param name="inputFields">The list of the input <see cref="DbField"/> objects to be used.</param>
        /// <param name="outputFields">The list of the output <see cref="DbField"/> objects to be used.</param>
        /// <param name="batchSize">The batch size of the entities to be passed.</param>
        /// <param name="dbSetting">The currently in used <see cref="IDbSetting"/> object.</param>
        /// <returns>The compiled function.</returns>
        internal static Action<DbCommand, IList<TEntity>> GetDataEntitiesDbCommandParameterSetterFunction<TEntity>(string cacheKey,
            IEnumerable<DbField> inputFields,
            IEnumerable<DbField> outputFields,
            int batchSize,
            IDbSetting dbSetting)
            where TEntity : class
        {
            return DataEntitiesDbCommandParameterSetterFunctionCache<TEntity>.Get(cacheKey, inputFields, outputFields, batchSize, dbSetting);
        }

        #region DataEntitiesDbCommandParameterSetterFunctionCache

        private static class DataEntitiesDbCommandParameterSetterFunctionCache<TEntity>
            where TEntity : class
        {
            private static ConcurrentDictionary<long, Action<DbCommand, IList<TEntity>>> m_cache = new ConcurrentDictionary<long, Action<DbCommand, IList<TEntity>>>();

            public static Action<DbCommand, IList<TEntity>> Get(string cacheKey,
                IEnumerable<DbField> inputFields,
                IEnumerable<DbField> outputFields,
                int batchSize,
                IDbSetting dbSetting)
            {
                var key = (long)cacheKey.GetHashCode() + batchSize.GetHashCode();
                if (inputFields?.Any() == true)
                {
                    foreach (var field in inputFields)
                    {
                        key += field.GetHashCode();
                    }
                }
                if (outputFields?.Any() == true)
                {
                    foreach (var field in outputFields)
                    {
                        key += field.GetHashCode();
                    }
                }
                var func = (Action<DbCommand, IList<TEntity>>)null;
                if (m_cache.TryGetValue(key, out func) == false)
                {
                    func = FunctionFactory.GetDataEntitiesDbCommandParameterSetterFunction<TEntity>(inputFields, outputFields, batchSize, dbSetting);
                    m_cache.TryAdd(key, func);
                }
                return func;
            }
        }

        #endregion

        #endregion

        #region GetDataEntityPropertySetterFromDbCommandParameterFunction

        /// <summary>
        /// Gets a compiled function that is used to set the data entity object property value based from the value of <see cref="DbCommand"/> parameter object.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="field">The target <see cref="Field"/>.</param>
        /// <param name="parameterName">The name of the parameter.</param>
        /// <param name="index">The index of the batches.</param>
        /// <param name="dbSetting">The currently in used <see cref="IDbSetting"/> object.</param>
        /// <returns>A compiled function that is used to set the data entity object property value based from the value of <see cref="DbCommand"/> parameter object.</returns>
        internal static Action<TEntity, DbCommand> GetDataEntityPropertySetterFromDbCommandParameterFunction<TEntity>(Field field,
            string parameterName,
            int index,
            IDbSetting dbSetting)
            where TEntity : class
        {
            return GetDataEntityPropertySetterFromDbCommandParameterFunctionCache<TEntity>.Get(field, parameterName, index, dbSetting);
        }

        #region GetDataEntityPropertySetterFromDbCommandParameterFunctionCache

        private static class GetDataEntityPropertySetterFromDbCommandParameterFunctionCache<TEntity>
            where TEntity : class
        {
            private static ConcurrentDictionary<long, Action<TEntity, DbCommand>> m_cache = new ConcurrentDictionary<long, Action<TEntity, DbCommand>>();

            public static Action<TEntity, DbCommand> Get(Field field,
                string parameterName,
                int index,
                IDbSetting dbSetting)
            {
                var key = (long)typeof(TEntity).FullName.GetHashCode() + field.GetHashCode() +
                    parameterName.GetHashCode() + index.GetHashCode();
                var func = (Action<TEntity, DbCommand>)null;
                if (m_cache.TryGetValue(key, out func) == false)
                {
                    func = FunctionFactory.GetDataEntityPropertySetterFromDbCommandParameterFunction<TEntity>(field, parameterName, index, dbSetting);
                    m_cache.TryAdd(key, func);
                }
                return func;
            }
        }

        #endregion

        #endregion

        #region GetDataEntityPropertyValueSetterFunction

        /// <summary>
        /// Gets a compiled function that is used to set the data entity object property value.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="field">The target <see cref="Field"/>.</param>
        /// <returns>A compiled function that is used to set the data entity object property value.</returns>
        public static Action<TEntity, object> GetDataEntityPropertyValueSetterFunction<TEntity>(Field field)
            where TEntity : class
        {
            return GetDataEntityPropertyValueSetterFunctionCache<TEntity>.Get(field);
        }

        #region GetDataEntityPropertyValueSetterFunctionCache

        private static class GetDataEntityPropertyValueSetterFunctionCache<TEntity>
            where TEntity : class
        {
            private static ConcurrentDictionary<long, Action<TEntity, object>> m_cache = new ConcurrentDictionary<long, Action<TEntity, object>>();

            public static Action<TEntity, object> Get(Field field)
            {
                var key = (long)typeof(TEntity).FullName.GetHashCode() + field.Name.GetHashCode();
                var func = (Action<TEntity, object>)null;
                if (m_cache.TryGetValue(key, out func) == false)
                {
                    func = FunctionFactory.GetDataEntityPropertyValueSetterFunction<TEntity>(field);
                    m_cache.TryAdd(key, func);
                }
                return func;
            }
        }

        #endregion

        #endregion
    }
}
