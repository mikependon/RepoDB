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

        #region GetDataReaderToDataEntityConverterFunction<TEntity>

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
        /// <param name="basedOnFields">Check whether to create a compiled function based on the data reader fields.</param>
        /// <returns>A compiled function that is used to cover the <see cref="DbDataReader"/> object into a list of data entity objects.</returns>
        internal static Func<DbDataReader, TEntity> GetDataReaderToDataEntityFunction<TEntity>(DbDataReader reader,
            IDbConnection connection,
            bool basedOnFields = false)
            where TEntity : class
        {
            if (basedOnFields == false)
            {
                return DataReaderToDataEntityConverterFunctionCache<TEntity>.Get(reader, connection);
            }
            else
            {
                return FieldBasedDataReaderToDataEntityConverterFunctionCache<TEntity>.Get(reader, connection);
            }
        }

        #region DataReaderToDataEntityConverterFunctionCache

        private static class DataReaderToDataEntityConverterFunctionCache<TEntity>
            where TEntity : class
        {
            private static Func<DbDataReader, TEntity> m_func;

            public static Func<DbDataReader, TEntity> Get(DbDataReader reader, IDbConnection connection)
            {
                if (m_func == null)
                {
                    m_func = FunctionFactory.GetDataReaderToDataEntityConverterFunction<TEntity>(reader, connection);
                }
                return m_func;
            }
        }

        #endregion

        #region FieldBasedDataReaderToDataEntityConverterFunctionCache

        private static class FieldBasedDataReaderToDataEntityConverterFunctionCache<TEntity>
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
                    result = FunctionFactory.GetDataReaderToDataEntityConverterFunction<TEntity>(reader, connection);
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
        /// <returns>A compiled function that is used to convert the <see cref="DbDataReader"/> object into a list of dynamic objects.</returns>
        public static Func<DbDataReader, ExpandoObject> GetDataReaderToExpandoObjectConverterFunction(DbDataReader reader)
        {
            return DataReaderToExpandoObjectConverterFunctionCache.Get(reader);
        }

        /// <summary>
        /// Gets a compiled function that is used to convert the <see cref="DbDataReader"/> object into a list of dynamic objects.
        /// </summary>
        /// <param name="reader">The <see cref="DbDataReader"/> to be converted.</param>
        /// <param name="basedOnFields">Check whether to create a compiled function based on the data reader fields.</param>
        /// <returns>A compiled function that is used to convert the <see cref="DbDataReader"/> object into a list of dynamic objects.</returns>
        internal static Func<DbDataReader, ExpandoObject> GetDataReaderToExpandoObjectConverterFunction(DbDataReader reader,
            bool basedOnFields = false)
        {
            if (basedOnFields == false)
            {
                return DataReaderToExpandoObjectConverterFunctionCache.Get(reader);
            }
            else
            {
                return FieldBasedDataReaderToExpandoObjectConverterFunctionCache.Get(reader);
            }
        }

        #region DataReaderToExpandoObjectConverterFunctionCache

        private static class DataReaderToExpandoObjectConverterFunctionCache
        {
            private static Func<DbDataReader, ExpandoObject> m_func;

            public static Func<DbDataReader, ExpandoObject> Get(DbDataReader reader)
            {
                if (m_func == null)
                {
                    m_func = FunctionFactory.GetDataReaderToExpandoObjectConverterFunction(reader);
                }
                return m_func;
            }
        }

        #endregion

        #region FieldBasedDataReaderToExpandoObjectConverterFunctionCache

        private static class FieldBasedDataReaderToExpandoObjectConverterFunctionCache
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
                    result = FunctionFactory.GetDataReaderToExpandoObjectConverterFunction(reader);
                    m_cache.TryAdd(key, result);
                }
                return result;
            }
        }

        #endregion

        #endregion

        #region GetDbCommandParameterSetterFunction<TEntity>

        /// <summary>
        /// Gets a compiled function that is used to set the <see cref="DbParameter"/> objects of the <see cref="DbCommand"/> object based from the values of the data entity/dynamic object.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity/dynamic object.</typeparam>
        /// <param name="key">The key to the cache.</param>
        /// <param name="inputFields">The list of the input <see cref="DbField"/> objects to be used.</param>
        /// <param name="outputFields">The list of the output <see cref="DbField"/> objects to be used.</param>
        /// <param name="batchSize">The batch size of the entities to be passed.</param>
        /// <returns>A compiled function that is used to set the <see cref="DbParameter"/> objects of the <see cref="DbCommand"/> object.</returns>
        public static Action<DbCommand, IList<TEntity>> GetDbCommandParameterSetterFunction<TEntity>(string key,
            IEnumerable<DbField> inputFields,
            IEnumerable<DbField> outputFields,
            int batchSize)
            where TEntity : class
        {
            return DataCommandParameterSetterCache<TEntity>.Get(key, inputFields, outputFields, batchSize);
        }

        #region DataCommandParameterSetterCache

        private static class DataCommandParameterSetterCache<TEntity>
            where TEntity : class
        {
            private static ConcurrentDictionary<string, Action<DbCommand, IList<TEntity>>> m_cache = new ConcurrentDictionary<string, Action<DbCommand, IList<TEntity>>>();

            public static Action<DbCommand, IList<TEntity>> Get(string key,
                IEnumerable<DbField> inputFields,
                IEnumerable<DbField> outputFields,
                int batchSize)
            {
                key = string.Concat(key, ".", inputFields.Select(field => field.UnquotedName).Join("."), ".",
                   outputFields?.Select(field => field.UnquotedName).Join("."), ".", batchSize);
                var func = (Action<DbCommand, IList<TEntity>>)null;
                if (m_cache.TryGetValue(key, out func) == false)
                {
                    func = FunctionFactory.GetDbCommandParameterSetterFunction<TEntity>(inputFields, outputFields, batchSize);
                    m_cache.TryAdd(key, func);
                }
                return func;
            }
        }

        #endregion

        #endregion

        #region GetDataEntityPropertySetterFromDbCommandParameterFunction<TEntity>

        /// <summary>
        /// Gets a compiled function that is used to set the data entity object property value based from the value of <see cref="DbCommand"/> parameter object.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="field">The target <see cref="Field"/>.</param>
        /// <param name="index">The index of the batches.</param>
        /// <returns>A compiled function that is used to set the data entity object property value based from the value of <see cref="DbCommand"/> parameter object.</returns>
        public static Action<TEntity, DbCommand> GetDataEntityPropertySetterFromDbCommandParameterFunction<TEntity>(Field field,
            int index = 0)
            where TEntity : class
        {
            return GetDataEntityPropertySetterFromDbCommandParameterFunctionCache<TEntity>.Get(field, index);
        }

        #region GetDataEntityPropertySetterFromDbCommandParameterFunctionCache

        private static class GetDataEntityPropertySetterFromDbCommandParameterFunctionCache<TEntity>
            where TEntity : class
        {
            private static ConcurrentDictionary<int, Action<TEntity, DbCommand>> m_cache = new ConcurrentDictionary<int, Action<TEntity, DbCommand>>();

            public static Action<TEntity, DbCommand> Get(Field field,
                int index)
            {
                var key = typeof(TEntity).GetHashCode() + field.GetHashCode() + index.GetHashCode();
                var func = (Action<TEntity, DbCommand>)null;
                if (m_cache.TryGetValue(key, out func) == false)
                {
                    func = FunctionFactory.GetDataEntityPropertySetterFromDbCommandParameterFunction<TEntity>(field, index);
                    m_cache.TryAdd(key, func);
                }
                return func;
            }
        }

        #endregion

        #endregion
    }
}
