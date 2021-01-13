using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.Reflection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

namespace RepoDb
{
    /// <summary>
    /// A class that is being used to cache the compiled functions.
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
                hashCode += string.Concat(reader.GetName(ordinal), "-", ordinal).GetHashCode() +
                    (reader.GetFieldType(ordinal)?.GetHashCode()).GetValueOrDefault();
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
        /// <param name="dbFields">The list of the <see cref="DbField"/> objects to be used.</param>
        /// <param name="dbSetting">The instance of <see cref="IDbSetting"/> object to be used.</param>
        /// <returns></returns>
        internal static Func<DbDataReader, TResult> GetDataReaderToTypeCompiledFunction<TResult>(DbDataReader reader,
            IEnumerable<DbField> dbFields = null,
            IDbSetting dbSetting = null) =>
            DataReaderToTypeCache<TResult>.Get(reader, dbFields, dbSetting);

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
            /// <param name="dbFields">The list of the <see cref="DbField"/> objects to be used.</param>
            /// <param name="dbSetting">The instance of <see cref="IDbSetting"/> object to be used.</param>
            /// <returns></returns>
            internal static Func<DbDataReader, TResult> Get(DbDataReader reader,
                IEnumerable<DbField> dbFields = null,
                IDbSetting dbSetting = null)
            {
                var key = GetKey(reader);
                if (cache.TryGetValue(key, out var result) == false)
                {
                    result = FunctionFactory.CompileDataReaderToType<TResult>(reader, dbFields, dbSetting);
                    cache.TryAdd(key, result);
                }
                return result;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="reader"></param>
            /// <returns></returns>
            private static long GetKey(DbDataReader reader) =>
                GetReaderFieldsHashCode(reader) + typeof(TResult).GetHashCode();
        }

        #endregion

        #endregion

        #region GetDataReaderToExpandoObjectCompileFunction

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="dbFields"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        internal static Func<DbDataReader, dynamic> GetDataReaderToExpandoObjectCompileFunction(DbDataReader reader,
            IEnumerable<DbField> dbFields = null,
            IDbSetting dbSetting = null) =>
            DataReaderToExpandoObjectCache.Get(reader, dbFields, dbSetting);

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
            /// <param name="dbFields"></param>
            /// <param name="dbSetting"></param>
            /// <returns></returns>
            internal static Func<DbDataReader, dynamic> Get(DbDataReader reader,
                IEnumerable<DbField> dbFields = null,
                IDbSetting dbSetting = null)
            {
                var key = GetKey(reader);
                if (cache.TryGetValue(key, out var result) == false)
                {
                    result = FunctionFactory.CompileDataReaderToExpandoObject(reader, dbFields, dbSetting);
                    cache.TryAdd(key, result);
                }
                return result;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="reader"></param>
            /// <returns></returns>
            private static long GetKey(DbDataReader reader) =>
                GetReaderFieldsHashCode(reader);
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
            IDbSetting dbSetting = null)
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
                IDbSetting dbSetting = null)
            {
                var key = GetKey(cacheKey, inputFields, outputFields);
                if (cache.TryGetValue(key, out var func) == false)
                {
                    if (typeof(TEntity).IsDictionaryStringObject())
                    {
                        func = FunctionFactory.CompileDictionaryStringObjectDbParameterSetter<TEntity>(inputFields, dbSetting);
                    }
                    else
                    {
                        func = FunctionFactory.CompileDataEntityDbParameterSetter<TEntity>(inputFields, outputFields, dbSetting);
                    }
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
            IDbSetting dbSetting = null)
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
                IDbSetting dbSetting = null)
            {
                var key = GetKey(cacheKey, inputFields, outputFields, batchSize);
                if (cache.TryGetValue(key, out var func) == false)
                {
                    if (typeof(TEntity).IsDictionaryStringObject())
                    {
                        func = FunctionFactory.CompileDictionaryStringObjectListDbParameterSetter<TEntity>(inputFields, batchSize, dbSetting);
                    }
                    else
                    {
                        func = FunctionFactory.CompileDataEntityListDbParameterSetter<TEntity>(inputFields, outputFields, batchSize, dbSetting);
                    }
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
            IDbSetting dbSetting = null)
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
                IDbSetting dbSetting = null)
            {
                var key = (long)typeof(TEntity).GetHashCode() + field.GetHashCode() +
                    parameterName.GetHashCode() + index.GetHashCode();
                if (cache.TryGetValue(key, out var func) == false)
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
                var key = (long)typeof(TEntity).GetHashCode() + field.GetHashCode();
                if (cache.TryGetValue(key, out var func) == false)
                {
                    if (typeof(TEntity).IsDictionaryStringObject())
                    {
                        func = FunctionFactory.CompileDictionaryStringObjectItemSetter<TEntity>(field);
                    }
                    else
                    {
                        func = FunctionFactory.CompileDataEntityPropertySetter<TEntity>(field);
                    }
                    cache.TryAdd(key, func);
                }
                return func;
            }
        }

        #endregion

        #endregion

        #region GetPlainTypeToDbParametersCompiledFunction

        /// <summary>
        /// 
        /// </summary>
        /// <param name="paramType"></param>
        /// <param name="entityType"></param>
        /// <param name="dbFields"></param>
        /// <returns></returns>
        internal static Action<DbCommand, object> GetPlainTypeToDbParametersCompiledFunction(Type paramType,
            Type entityType,
            IEnumerable<DbField> dbFields = null) =>
            PlainTypeToDbParametersCompiledFunctionCache.Get(paramType, entityType, dbFields);

        #region PlainTypeToDbParametersCompiledFunctionCache

        /// <summary>
        /// 
        /// </summary>
        private static class PlainTypeToDbParametersCompiledFunctionCache
        {
            private static ConcurrentDictionary<long, Action<DbCommand, object>> cache = new ConcurrentDictionary<long, Action<DbCommand, object>>();

            /// <summary>
            /// 
            /// </summary>
            /// <param name="paramType"></param>
            /// <param name="entityType"></param>
            /// <param name="dbFields"></param>
            /// <returns></returns>
            internal static Action<DbCommand, object> Get(Type paramType,
                Type entityType,
                IEnumerable<DbField> dbFields = null)
            {
                if (paramType == null)
                {
                    return null;
                }
                var key = paramType.GetHashCode() + Convert.ToInt32(entityType?.GetHashCode());
                if (cache.TryGetValue(key, out var func) == false)
                {
                    if (paramType.IsPlainType())
                    {
                        func = FunctionFactory.GetPlainTypeToDbParametersCompiledFunction(paramType, entityType, dbFields);
                    }
                    cache.TryAdd(key, func);
                }
                return func;
            }
        }

        #endregion

        #endregion
    }
}
