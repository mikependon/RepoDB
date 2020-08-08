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
    /// A class that is used to cache the compiled functions.
    /// </summary>
    internal static class FunctionCache
    {
        #region GetDataReaderToDataEntityCompiledFunction

        /// <summary>
        /// Gets a compiled function that is used to convert the <see cref="DbDataReader"/> object into a list of data entity objects.
        /// </summary>
        /// <typeparam name="TEntity">The data entity object to convert to.</typeparam>
        /// <param name="reader">The <see cref="DbDataReader"/> to be converted.</param>
        /// <param name="connection">The used <see cref="IDbConnection"/> object.</param>
        /// <param name="connectionString">The raw connection string.</param>
        /// <param name="transaction">The transaction object that is currently in used.</param>
        /// <param name="enableValidation">Enables the validation after retrieving the database fields.</param>
        /// <returns>A compiled function that is used to cover the <see cref="DbDataReader"/> object into a list of data entity objects.</returns>
        internal static Func<DbDataReader, TEntity> GetDataReaderToDataEntityCompiledFunction<TEntity>(DbDataReader reader,
            IDbConnection connection,
            string connectionString,
            IDbTransaction transaction,
            bool enableValidation)
            where TEntity : class
        {
            return DataReaderToDataEntityCache<TEntity>.Get(reader, connection, connectionString, transaction, enableValidation);
        }

        #region DataReaderToDataEntityCache

        private static class DataReaderToDataEntityCache<TEntity>
            where TEntity : class
        {
            private static ConcurrentDictionary<long, Func<DbDataReader, TEntity>> cache = new ConcurrentDictionary<long, Func<DbDataReader, TEntity>>();

            internal static Func<DbDataReader, TEntity> Get(DbDataReader reader,
                IDbConnection connection,
                string connectionString,
                IDbTransaction transaction,
                bool enableValidation)
            {
                var result = (Func<DbDataReader, TEntity>)null;
                var fields = Enumerable.Range(0, reader.FieldCount)
                    .Select(reader.GetName)
                    .Join(StringConstant.Period)
                    .GetHashCode();
                var key = typeof(TEntity).FullName.GetHashCode() + fields.GetHashCode();
                if (string.IsNullOrEmpty(connection?.ConnectionString) == false)
                {
                    key += connection.ConnectionString.GetHashCode();
                }
                if (cache.TryGetValue(key, out result) == false)
                {
                    result = FunctionFactory.CompileDataReaderToDataEntity<TEntity>(reader, connection, connectionString, transaction, enableValidation);
                    cache.TryAdd(key, result);
                }
                return result;
            }
        }

        #endregion

        #endregion

        #region GetDataReaderToExpandoObjectCompileFunction

        /// <summary>
        /// Gets a compiled function that is used to convert the <see cref="DbDataReader"/> object into a list of dynamic objects.
        /// </summary>
        /// <param name="reader">The <see cref="DbDataReader"/> to be converted.</param>
        /// <param name="transaction">The transaction object that is currently in used.</param>
        /// <returns>A compiled function that is used to convert the <see cref="DbDataReader"/> object into a list of dynamic objects.</returns>
        internal static Func<DbDataReader, ExpandoObject> GetDataReaderToExpandoObjectCompileFunction(DbDataReader reader,
            IDbTransaction transaction)
        {
            return GetDataReaderToExpandoObjectCompileFunction(reader, null, null, transaction);
        }

        /// <summary>
        /// Gets a compiled function that is used to convert the <see cref="DbDataReader"/> object into a list of dynamic objects.
        /// </summary>
        /// <param name="reader">The <see cref="DbDataReader"/> to be converted.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="connection">The used <see cref="IDbConnection"/> object.</param>
        /// <param name="transaction">The transaction object that is currently in used.</param>
        /// <returns>A compiled function that is used to convert the <see cref="DbDataReader"/> object into a list of dynamic objects.</returns>
        internal static Func<DbDataReader, ExpandoObject> GetDataReaderToExpandoObjectCompileFunction(DbDataReader reader,
            string tableName,
            IDbConnection connection,
            IDbTransaction transaction)
        {
            return DataReaderToExpandoObjectCache.Get(reader, tableName, connection, transaction);
        }

        #region DataReaderToExpandoObjectCache

        private static class DataReaderToExpandoObjectCache
        {
            private static ConcurrentDictionary<long, Func<DbDataReader, ExpandoObject>> cache = new ConcurrentDictionary<long, Func<DbDataReader, ExpandoObject>>();

            internal static Func<DbDataReader, ExpandoObject> Get(DbDataReader reader,
                string tableName,
                IDbConnection connection,
                IDbTransaction transaction)
            {
                var result = (Func<DbDataReader, ExpandoObject>)null;
                var key = (long)Enumerable.Range(0, reader.FieldCount)
                    .Select(reader.GetName)
                    .Join(StringConstant.Period)
                    .GetHashCode();
                if (tableName != null)
                {
                    key += tableName.GetHashCode();
                }
                if (string.IsNullOrEmpty(connection?.ConnectionString) == false)
                {
                    key += connection.ConnectionString.GetHashCode();
                }
                if (cache.TryGetValue(key, out result) == false)
                {
                    result = FunctionFactory.CompileDataReaderToExpandoObject(reader, tableName, connection, transaction);
                    cache.TryAdd(key, result);
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
            private static ConcurrentDictionary<long, Action<DbCommand, TEntity>> cache = new ConcurrentDictionary<long, Action<DbCommand, TEntity>>();

            internal static Action<DbCommand, TEntity> Get(string cacheKey,
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
                if (cache.TryGetValue(key, out func) == false)
                {
                    func = FunctionFactory.GetDataEntityDbCommandParameterSetterFunction<TEntity>(inputFields, outputFields, dbSetting);
                    cache.TryAdd(key, func);
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
            private static ConcurrentDictionary<long, Action<DbCommand, IList<TEntity>>> cache = new ConcurrentDictionary<long, Action<DbCommand, IList<TEntity>>>();

            internal static Action<DbCommand, IList<TEntity>> Get(string cacheKey,
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
                if (cache.TryGetValue(key, out func) == false)
                {
                    func = FunctionFactory.GetDataEntitiesDbCommandParameterSetterFunction<TEntity>(inputFields, outputFields, batchSize, dbSetting);
                    cache.TryAdd(key, func);
                }
                return func;
            }
        }

        #endregion

        #endregion

        #region GetDbCommandToPropertyCompiledFunction

        /// <summary>
        /// Gets a compiled function that is used to set the data entity object property value based from the value of <see cref="DbCommand"/> parameter object.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="field">The target <see cref="Field"/>.</param>
        /// <param name="parameterName">The name of the parameter.</param>
        /// <param name="index">The index of the batches.</param>
        /// <param name="dbSetting">The currently in used <see cref="IDbSetting"/> object.</param>
        /// <returns>A compiled function that is used to set the data entity object property value based from the value of <see cref="DbCommand"/> parameter object.</returns>
        internal static Action<TEntity, DbCommand> GetDbCommandToPropertyCompiledFunction<TEntity>(Field field,
            string parameterName,
            int index,
            IDbSetting dbSetting)
            where TEntity : class
        {
            return DbCommandToPropertyCache<TEntity>.Get(field, parameterName, index, dbSetting);
        }

        #region DbCommandToPropertyCache

        private static class DbCommandToPropertyCache<TEntity>
            where TEntity : class
        {
            private static ConcurrentDictionary<long, Action<TEntity, DbCommand>> cache = new ConcurrentDictionary<long, Action<TEntity, DbCommand>>();

            internal static Action<TEntity, DbCommand> Get(Field field,
                string parameterName,
                int index,
                IDbSetting dbSetting)
            {
                var key = (long)typeof(TEntity).FullName.GetHashCode() + field.GetHashCode() +
                    parameterName.GetHashCode() + index.GetHashCode();
                var func = (Action<TEntity, DbCommand>)null;
                if (cache.TryGetValue(key, out func) == false)
                {
                    func = FunctionFactory.CompileDbCommandToProperty<TEntity>(field, parameterName, index, dbSetting);
                    cache.TryAdd(key, func);
                }
                return func;
            }
        }

        #endregion

        #endregion

        #region GetDataEntityPropertySetterCompiledFunction

        /// <summary>
        /// Gets a compiled function that is used to set the data entity object property value.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="field">The target <see cref="Field"/>.</param>
        /// <returns>A compiled function that is used to set the data entity object property value.</returns>
        internal static Action<TEntity, object> GetDataEntityPropertySetterCompiledFunction<TEntity>(Field field)
            where TEntity : class
        {
            return DataEntityPropertySetterCache<TEntity>.Get(field);
        }

        #region DataEntityPropertySetterCache

        private static class DataEntityPropertySetterCache<TEntity>
            where TEntity : class
        {
            private static ConcurrentDictionary<long, Action<TEntity, object>> cache = new ConcurrentDictionary<long, Action<TEntity, object>>();

            internal static Action<TEntity, object> Get(Field field)
            {
                var key = (long)typeof(TEntity).FullName.GetHashCode() + field.Name.GetHashCode();
                var func = (Action<TEntity, object>)null;
                if (cache.TryGetValue(key, out func) == false)
                {
                    func = FunctionFactory.CompileDataEntityPropertySetter<TEntity>(field);
                    cache.TryAdd(key, func);
                }
                return func;
            }
        }

        #endregion

        #endregion
    }
}
