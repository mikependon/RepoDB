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
using System.Threading.Tasks;

namespace RepoDb
{
    /// <summary>
    /// A class that is used to cache the compiled functions.
    /// </summary>
    internal static class FunctionCache
    {
        #region Helpers

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private static long GetReaderFieldsHashCode(DbDataReader reader)
        {
            if (reader == null)
            {
                return 0;
            }
            var hashCode = (long)0;
            for (var ordinal = 0; ordinal < reader.FieldCount; ordinal++)
            {
                // The spatial data type is null.
                hashCode += reader.GetName(ordinal).GetHashCode() + (reader.GetFieldType(ordinal)?.GetHashCode()).GetValueOrDefault();
            }
            return hashCode;
        }

        #endregion

        #region GetDataReaderToDataEntityCompiledFunction

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="reader"></param>
        /// <param name="connection"></param>
        /// <param name="connectionString"></param>
        /// <param name="transaction"></param>
        /// <param name="enableValidation"></param>
        /// <returns></returns>
        internal static Func<DbDataReader, TResult> GetDataReaderToTypeCompiledFunction<TResult>(DbDataReader reader,
            IDbConnection connection,
            string connectionString,
            IDbTransaction transaction,
            bool enableValidation) =>
            DataReaderToTypeCache<TResult>.Get(reader, connection, connectionString, transaction, enableValidation);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="reader"></param>
        /// <param name="connection"></param>
        /// <param name="connectionString"></param>
        /// <param name="transaction"></param>
        /// <param name="enableValidation"></param>
        /// <returns></returns>
        internal static Task<Func<DbDataReader, TResult>> GetDataReaderToTypeCompiledFunctionAsync<TResult>(DbDataReader reader,
            IDbConnection connection,
            string connectionString,
            IDbTransaction transaction,
            bool enableValidation) =>
            DataReaderToTypeCache<TResult>.GetAsync(reader, connection, connectionString, transaction, enableValidation);

        #region DataReaderToTypeCache

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        private static class DataReaderToTypeCache<TResult>
        {
            private static ConcurrentDictionary<long, Func<DbDataReader, TResult>> cache = new ConcurrentDictionary<long, Func<DbDataReader, TResult>>();

            /// <summary>
            /// 
            /// </summary>
            /// <param name="reader"></param>
            /// <param name="connection"></param>
            /// <param name="connectionString"></param>
            /// <param name="transaction"></param>
            /// <param name="enableValidation"></param>
            /// <returns></returns>
            internal static Func<DbDataReader, TResult> Get(DbDataReader reader,
                IDbConnection connection,
                string connectionString,
                IDbTransaction transaction,
                bool enableValidation)
            {
                var result = (Func<DbDataReader, TResult>)null;
                var key = GetKey(reader, connection);
                if (cache.TryGetValue(key, out result) == false)
                {
                    result = FunctionFactory.CompileDataReaderToType<TResult>(reader, connection, connectionString, transaction, enableValidation);
                    cache.TryAdd(key, result);
                }
                return result;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="reader"></param>
            /// <param name="connection"></param>
            /// <param name="connectionString"></param>
            /// <param name="transaction"></param>
            /// <param name="enableValidation"></param>
            /// <returns></returns>
            internal static async Task<Func<DbDataReader, TResult>> GetAsync(DbDataReader reader,
                IDbConnection connection,
                string connectionString,
                IDbTransaction transaction,
                bool enableValidation)
            {
                var result = (Func<DbDataReader, TResult>)null;
                var key = GetKey(reader, connection);
                if (cache.TryGetValue(key, out result) == false)
                {
                    result = await FunctionFactory.CompileDataReaderToTypeAsync<TResult>(reader, connection, connectionString, transaction, enableValidation);
                    cache.TryAdd(key, result);
                }
                return result;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="reader"></param>
            /// <param name="connection"></param>
            /// <returns></returns>
            private static long GetKey(DbDataReader reader,
                IDbConnection connection) =>
                GetReaderFieldsHashCode(reader) + typeof(TResult).GetHashCode() +
                   (connection?.ConnectionString?.GetHashCode()).GetValueOrDefault();
        }

        #endregion

        #endregion

        #region GetDataReaderToExpandoObjectCompileFunction

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="tableName"></param>
        /// <param name="connection"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        internal static Func<DbDataReader, dynamic> GetDataReaderToExpandoObjectCompileFunction(DbDataReader reader,
            string tableName,
            IDbConnection connection,
            IDbTransaction transaction) =>
            DataReaderToExpandoObjectCache.Get(reader, tableName, connection, transaction);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="tableName"></param>
        /// <param name="connection"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        internal static Task<Func<DbDataReader, dynamic>> GetDataReaderToExpandoObjectCompileFunctionAsync(DbDataReader reader,
            string tableName,
            IDbConnection connection,
            IDbTransaction transaction) =>
            DataReaderToExpandoObjectCache.GetAsync(reader, tableName, connection, transaction);

        #region DataReaderToExpandoObjectCache

        /// <summary>
        /// 
        /// </summary>
        private static class DataReaderToExpandoObjectCache
        {
            private static ConcurrentDictionary<long, Func<DbDataReader, dynamic>> cache = new ConcurrentDictionary<long, Func<DbDataReader, dynamic>>();

            /// <summary>
            /// 
            /// </summary>
            /// <param name="reader"></param>
            /// <param name="tableName"></param>
            /// <param name="connection"></param>
            /// <param name="transaction"></param>
            /// <returns></returns>
            internal static Func<DbDataReader, dynamic> Get(DbDataReader reader,
                string tableName,
                IDbConnection connection,
                IDbTransaction transaction)
            {
                var result = (Func<DbDataReader, dynamic>)null;
                var key = GetKey(reader, tableName, connection);
                if (cache.TryGetValue(key, out result) == false)
                {
                    result = FunctionFactory.CompileDataReaderToExpandoObject(reader, tableName, connection, transaction);
                    cache.TryAdd(key, result);
                }
                return result;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="reader"></param>
            /// <param name="tableName"></param>
            /// <param name="connection"></param>
            /// <param name="transaction"></param>
            /// <returns></returns>
            internal static async Task<Func<DbDataReader, dynamic>> GetAsync(DbDataReader reader,
                string tableName,
                IDbConnection connection,
                IDbTransaction transaction)
            {
                var result = (Func<DbDataReader, dynamic>)null;
                var key = GetKey(reader, tableName, connection);
                if (cache.TryGetValue(key, out result) == false)
                {
                    result = await FunctionFactory.CompileDataReaderToExpandoObjectAsync(reader, tableName, connection, transaction);
                    cache.TryAdd(key, result);
                }
                return result;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="reader"></param>
            /// <param name="tableName"></param>
            /// <param name="connection"></param>
            /// <returns></returns>
            private static long GetKey(DbDataReader reader,
                string tableName,
                IDbConnection connection) =>
                GetReaderFieldsHashCode(reader) + (tableName?.GetHashCode()).GetValueOrDefault() +
                    (connection?.ConnectionString?.GetHashCode()).GetValueOrDefault();
        }

        #endregion

        #endregion

        #region GetDataEntityDbParameterSetterCompiledFunction

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="cacheKey"></param>
        /// <param name="inputFields"></param>
        /// <param name="outputFields"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        internal static Action<DbCommand, TEntity> GetDataEntityDbParameterSetterCompiledFunction<TEntity>(string cacheKey,
            IEnumerable<DbField> inputFields,
            IEnumerable<DbField> outputFields,
            IDbSetting dbSetting)
            where TEntity : class =>
            DataEntityDbParameterSetterCache<TEntity>.Get(cacheKey, inputFields, outputFields, dbSetting);

        #region DataEntityDbParameterSetterCache

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        private static class DataEntityDbParameterSetterCache<TEntity>
            where TEntity : class
        {
            private static ConcurrentDictionary<long, Action<DbCommand, TEntity>> cache = new ConcurrentDictionary<long, Action<DbCommand, TEntity>>();

            /// <summary>
            /// 
            /// </summary>
            /// <param name="cacheKey"></param>
            /// <param name="inputFields"></param>
            /// <param name="outputFields"></param>
            /// <param name="dbSetting"></param>
            /// <returns></returns>
            internal static Action<DbCommand, TEntity> Get(string cacheKey,
                IEnumerable<DbField> inputFields,
                IEnumerable<DbField> outputFields,
                IDbSetting dbSetting)
            {
                var func = (Action<DbCommand, TEntity>)null;
                var key = GetKey(cacheKey, inputFields, outputFields);
                if (cache.TryGetValue(key, out func) == false)
                {
                    func = FunctionFactory.CompileDataEntityDbParameterSetter<TEntity>(inputFields, outputFields, dbSetting);
                    cache.TryAdd(key, func);
                }
                return func;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="cacheKey"></param>
            /// <param name="inputFields"></param>
            /// <param name="outputFields"></param>
            /// <returns></returns>
            private static long GetKey(string cacheKey,
                IEnumerable<DbField> inputFields,
                IEnumerable<DbField> outputFields)
            {
                var key = (long)typeof(TEntity).GetHashCode() + cacheKey.GetHashCode();
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
                return key;
            }
        }

        #endregion

        #endregion

        #region GetDataEntityListDbParameterSetterCompiledFunction

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="cacheKey"></param>
        /// <param name="inputFields"></param>
        /// <param name="outputFields"></param>
        /// <param name="batchSize"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        internal static Action<DbCommand, IList<TEntity>> GetDataEntityListDbParameterSetterCompiledFunction<TEntity>(string cacheKey,
            IEnumerable<DbField> inputFields,
            IEnumerable<DbField> outputFields,
            int batchSize,
            IDbSetting dbSetting)
            where TEntity : class =>
            DataEntityListDbParameterSetterCache<TEntity>.Get(cacheKey, inputFields, outputFields, batchSize, dbSetting);

        #region DataEntityListDbParameterSetterCache

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        private static class DataEntityListDbParameterSetterCache<TEntity>
            where TEntity : class
        {
            private static ConcurrentDictionary<long, Action<DbCommand, IList<TEntity>>> cache = new ConcurrentDictionary<long, Action<DbCommand, IList<TEntity>>>();

            /// <summary>
            /// 
            /// </summary>
            /// <param name="cacheKey"></param>
            /// <param name="inputFields"></param>
            /// <param name="outputFields"></param>
            /// <param name="batchSize"></param>
            /// <param name="dbSetting"></param>
            /// <returns></returns>
            internal static Action<DbCommand, IList<TEntity>> Get(string cacheKey,
                IEnumerable<DbField> inputFields,
                IEnumerable<DbField> outputFields,
                int batchSize,
                IDbSetting dbSetting)
            {
                var func = (Action<DbCommand, IList<TEntity>>)null;
                var key = GetKey(cacheKey, inputFields, outputFields, batchSize);
                if (cache.TryGetValue(key, out func) == false)
                {
                    func = FunctionFactory.CompileDataEntityListDbParameterSetter<TEntity>(inputFields, outputFields, batchSize, dbSetting);
                    cache.TryAdd(key, func);
                }
                return func;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="cacheKey"></param>
            /// <param name="inputFields"></param>
            /// <param name="outputFields"></param>
            /// <param name="batchSize"></param>
            /// <returns></returns>
            private static long GetKey(string cacheKey,
                IEnumerable<DbField> inputFields,
                IEnumerable<DbField> outputFields,
                int batchSize)
            {
                var key = (long)typeof(TEntity).GetHashCode() + batchSize.GetHashCode() +
                    (cacheKey?.GetHashCode()).GetValueOrDefault();
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
                return key;
            }
        }

        #endregion

        #endregion

        #region GetDbCommandToPropertyCompiledFunction

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="field"></param>
        /// <param name="parameterName"></param>
        /// <param name="index"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        internal static Action<TEntity, DbCommand> GetDbCommandToPropertyCompiledFunction<TEntity>(Field field,
            string parameterName,
            int index,
            IDbSetting dbSetting)
            where TEntity : class =>
            DbCommandToPropertyCache<TEntity>.Get(field, parameterName, index, dbSetting);

        #region DbCommandToPropertyCache

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        private static class DbCommandToPropertyCache<TEntity>
            where TEntity : class
        {
            private static ConcurrentDictionary<long, Action<TEntity, DbCommand>> cache = new ConcurrentDictionary<long, Action<TEntity, DbCommand>>();

            /// <summary>
            /// 
            /// </summary>
            /// <param name="field"></param>
            /// <param name="parameterName"></param>
            /// <param name="index"></param>
            /// <param name="dbSetting"></param>
            /// <returns></returns>
            internal static Action<TEntity, DbCommand> Get(Field field,
                string parameterName,
                int index,
                IDbSetting dbSetting)
            {
                var key = (long)typeof(TEntity).GetHashCode() + field.GetHashCode() +
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
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="field"></param>
        /// <returns></returns>
        internal static Action<TEntity, object> GetDataEntityPropertySetterCompiledFunction<TEntity>(Field field)
            where TEntity : class =>
            DataEntityPropertySetterCache<TEntity>.Get(field);

        #region DataEntityPropertySetterCache

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        private static class DataEntityPropertySetterCache<TEntity>
            where TEntity : class
        {
            private static ConcurrentDictionary<long, Action<TEntity, object>> cache = new ConcurrentDictionary<long, Action<TEntity, object>>();

            /// <summary>
            /// 
            /// </summary>
            /// <param name="field"></param>
            /// <returns></returns>
            internal static Action<TEntity, object> Get(Field field)
            {
                var key = (long)typeof(TEntity).GetHashCode() + field.Name.GetHashCode();
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
