using RepoDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Dynamic;
using System.Threading.Tasks;

namespace RepoDb.Reflection
{
    /// <summary>
    /// A static factory class used to create a custom compiled function.
    /// </summary>
    internal static class FunctionFactory
    {
        #region CompileDataReaderToDataEntityAsync

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
        public static Func<DbDataReader, TResult> CompileDataReaderToType<TResult>(DbDataReader reader,
            IDbConnection connection,
            string connectionString,
            IDbTransaction transaction,
            bool enableValidation) =>
            Compiler.CompileDataReaderToType<TResult>(reader,
                connection,
                connectionString,
                transaction,
                enableValidation);

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
        public static Task<Func<DbDataReader, TResult>> CompileDataReaderToTypeAsync<TResult>(DbDataReader reader,
            IDbConnection connection,
            string connectionString,
            IDbTransaction transaction,
            bool enableValidation) =>
            Compiler.CompileDataReaderToTypeAsync<TResult>(reader,
                connection,
                connectionString,
                transaction,
                enableValidation);

        #endregion

        #region CompileDataReaderToExpandoObject

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="tableName"></param>
        /// <param name="connection"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public static Func<DbDataReader, ExpandoObject> CompileDataReaderToExpandoObject(DbDataReader reader,
            string tableName,
            IDbConnection connection,
            IDbTransaction transaction) =>
            Compiler.CompileDataReaderToExpandoObject(reader,
                tableName,
                connection,
                transaction);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="tableName"></param>
        /// <param name="connection"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public static Task<Func<DbDataReader, ExpandoObject>> CompileDataReaderToExpandoObjectAsync(DbDataReader reader,
            string tableName,
            IDbConnection connection,
            IDbTransaction transaction) =>
            Compiler.CompileDataReaderToExpandoObjectAsync(reader,
                tableName,
                connection,
                transaction);

        #endregion

        #region CompileDataEntityDbParameterSetter

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="inputFields"></param>
        /// <param name="outputFields"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        public static Action<DbCommand, TEntity> CompileDataEntityDbParameterSetter<TEntity>(IEnumerable<DbField> inputFields,
            IEnumerable<DbField> outputFields,
            IDbSetting dbSetting)
            where TEntity : class =>
            Compiler.CompileDataEntityDbParameterSetter<TEntity>(inputFields, outputFields, dbSetting);

        #endregion

        #region CompileDataEntityListDbParameterSetter

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="inputFields"></param>
        /// <param name="outputFields"></param>
        /// <param name="batchSize"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        public static Action<DbCommand, IList<TEntity>> CompileDataEntityListDbParameterSetter<TEntity>(IEnumerable<DbField> inputFields,
            IEnumerable<DbField> outputFields,
            int batchSize,
            IDbSetting dbSetting)
            where TEntity : class =>
            Compiler.CompileDataEntityListDbParameterSetter<TEntity>(inputFields, outputFields, batchSize, dbSetting);

        #endregion

        #region CompileDbCommandToProperty

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="field"></param>
        /// <param name="parameterName"></param>
        /// <param name="index"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        public static Action<TEntity, DbCommand> CompileDbCommandToProperty<TEntity>(Field field,
            string parameterName,
            int index,
            IDbSetting dbSetting)
            where TEntity : class =>
            Compiler.CompileDbCommandToProperty<TEntity>(field, parameterName, index, dbSetting);

        #endregion

        #region CompileDataEntityPropertySetter

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="field"></param>
        /// <returns></returns>
        public static Action<TEntity, object> CompileDataEntityPropertySetter<TEntity>(Field field)
            where TEntity : class =>
            Compiler.CompileDataEntityPropertySetter<TEntity>(field);

        #endregion

        #region GetPlainTypeToDbParametersCompiledFunction

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Action<DbCommand, object> GetPlainTypeToDbParametersCompiledFunction(Type type) =>
            Compiler.GetPlainTypeToDbParametersCompiledFunction(type);

        #endregion
    }
}
