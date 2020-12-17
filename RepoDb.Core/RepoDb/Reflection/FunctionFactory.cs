using RepoDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Dynamic;

namespace RepoDb.Reflection
{
    /// <summary>
    /// A static factory class used to create a custom compiled function.
    /// </summary>
    internal static class FunctionFactory
    {
        #region CompileDataReaderToType

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="reader"></param>
        /// <param name="dbFields">The list of the <see cref="DbField"/> objects to be used.</param>
        /// <param name="dbSetting">The instance of <see cref="IDbSetting"/> object to be used.</param>
        /// <returns></returns>
        public static Func<DbDataReader, TResult> CompileDataReaderToType<TResult>(DbDataReader reader,
            IEnumerable<DbField> dbFields = null,
            IDbSetting dbSetting = null) =>
            Compiler.CompileDataReaderToType<TResult>(reader, dbFields, dbSetting);

        #endregion

        #region CompileDataReaderToExpandoObject

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="dbFields"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        public static Func<DbDataReader, ExpandoObject> CompileDataReaderToExpandoObject(DbDataReader reader,
            IEnumerable<DbField> dbFields = null,
            IDbSetting dbSetting = null) =>
            Compiler.CompileDataReaderToExpandoObject(reader, dbFields, dbSetting);

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
            IDbSetting dbSetting = null)
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
            IDbSetting dbSetting = null)
            where TEntity : class =>
            Compiler.CompileDataEntityListDbParameterSetter<TEntity>(inputFields, outputFields, batchSize, dbSetting);

        #endregion

        #region CompileDictionaryStringObjectDbParameterSetter

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="inputFields"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        public static Action<DbCommand, TEntity> CompileDictionaryStringObjectDbParameterSetter<TEntity>(IEnumerable<DbField> inputFields,
            IDbSetting dbSetting = null)
            where TEntity : class =>
            Compiler.CompileDictionaryStringObjectDbParameterSetter<TEntity>(inputFields, dbSetting);

        #endregion

        #region CompileDictionaryStringObjectListDbParameterSetter

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="inputFields"></param>
        /// <param name="batchSize"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        public static Action<DbCommand, IList<TEntity>> CompileDictionaryStringObjectListDbParameterSetter<TEntity>(IEnumerable<DbField> inputFields,
            int batchSize,
            IDbSetting dbSetting = null)
            where TEntity : class =>
            Compiler.CompileDictionaryStringObjectListDbParameterSetter<TEntity>(inputFields, batchSize, dbSetting);

        #endregion

        #region CompileDictionaryStringObjectItemSetter

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="field"></param>
        /// <returns></returns>
        public static Action<TEntity, object> CompileDictionaryStringObjectItemSetter<TEntity>(Field field)
            where TEntity : class =>
            Compiler.CompileDictionaryStringObjectItemSetter<TEntity>(field);

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
            IDbSetting dbSetting = null)
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
        /// <param name="paramType"></param>
        /// <param name="entityType"></param>
        /// <param name="dbFields"></param>
        /// <returns></returns>
        public static Action<DbCommand, object> GetPlainTypeToDbParametersCompiledFunction(Type paramType,
            Type entityType,
            IEnumerable<DbField> dbFields = null) =>
            Compiler.GetPlainTypeToDbParametersCompiledFunction(paramType, entityType, dbFields);

        #endregion
    }
}
