using RepoDb.Contexts.Cachers;
using RepoDb.Contexts.Execution;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.Requests;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RepoDb.Contexts.Providers
{
    /// <summary>
    /// 
    /// </summary>
    internal static class InsertAllExecutionContextProvider
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="tableName"></param>
        /// <param name="fields"></param>
        /// <param name="batchSize"></param>
        /// <param name="hints"></param>
        /// <returns></returns>
        private static string GetKey<TEntity>(string tableName,
            IEnumerable<Field> fields,
            int batchSize,
            string hints)
        {
            return string.Concat(typeof(TEntity).FullName,
                ";",
                tableName,
                ";",
                fields?.Select(f => f.Name).Join(","),
                ";",
                batchSize,
                ";",
                hints);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="batchSize"></param>
        /// <param name="fields"></param>
        /// <param name="hints"></param>
        /// <param name="transaction"></param>
        /// <param name="statementBuilder"></param>
        /// <returns></returns>
        public static InsertAllExecutionContext<TEntity> Create<TEntity>(IDbConnection connection,
            string tableName,
            int batchSize,
            IEnumerable<Field> fields,
            string hints = null,
            IDbTransaction transaction = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            var key = GetKey<TEntity>(tableName, fields, batchSize, hints);

            // Get from cache
            var context = InsertAllExecutionContextCache.Get<TEntity>(key);
            if (context != null)
            {
                return context;
            }

            // Create
            var dbFields = DbFieldCache.Get(connection, tableName, transaction);
            var commandText = (string)null;

            // Create a different kind of requests
            if (batchSize > 1)
            {
                var request = new InsertAllRequest(tableName,
                    connection,
                    transaction,
                    fields,
                    batchSize,
                    hints,
                    statementBuilder);
                commandText = CommandTextCache.GetInsertAllText(request);
            }
            else
            {
                var request = new InsertRequest(tableName,
                    connection,
                    transaction,
                    fields,
                    hints,
                    statementBuilder);
                commandText = CommandTextCache.GetInsertText(request);
            }

            // Call
            context = CreateInternal<TEntity>(connection,
                tableName,
                dbFields,
                batchSize,
                fields,
                commandText);

            // Add to cache
            InsertAllExecutionContextCache.Add<TEntity>(key, context);

            // Return
            return context;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="batchSize"></param>
        /// <param name="fields"></param>
        /// <param name="hints"></param>
        /// <param name="transaction"></param>
        /// <param name="statementBuilder"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<InsertAllExecutionContext<TEntity>> CreateAsync<TEntity>(IDbConnection connection,
            string tableName,
            int batchSize,
            IEnumerable<Field> fields,
            string hints = null,
            IDbTransaction transaction = null,
            IStatementBuilder statementBuilder = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            var key = GetKey<TEntity>(tableName, fields, batchSize, hints);

            // Get from cache
            var context = InsertAllExecutionContextCache.Get<TEntity>(key);
            if (context != null)
            {
                return context;
            }

            // Create
            var dbFields = await DbFieldCache.GetAsync(connection, tableName, transaction, cancellationToken);
            var commandText = (string)null;

            // Create a different kind of requests
            if (batchSize > 1)
            {
                var request = new InsertAllRequest(tableName,
                    connection,
                    transaction,
                    fields,
                    batchSize,
                    hints,
                    statementBuilder);
                commandText = await CommandTextCache.GetInsertAllTextAsync(request, cancellationToken);
            }
            else
            {
                var request = new InsertRequest(tableName,
                    connection,
                    transaction,
                    fields,
                    hints,
                    statementBuilder);
                commandText = await CommandTextCache.GetInsertTextAsync(request, cancellationToken);
            }

            // Call
            context = CreateInternal<TEntity>(connection,
                tableName,
                dbFields,
                batchSize,
                fields,
                commandText);

            // Add to cache
            InsertAllExecutionContextCache.Add<TEntity>(key, context);

            // Return
            return context;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="dbFields"></param>
        /// <param name="batchSize"></param>
        /// <param name="fields"></param>
        /// <param name="commandText"></param>
        /// <returns></returns>
        private static InsertAllExecutionContext<TEntity> CreateInternal<TEntity>(IDbConnection connection,
            string tableName,
            IEnumerable<DbField> dbFields,
            int batchSize,
            IEnumerable<Field> fields,
            string commandText)
            where TEntity : class
        {
            var dbSetting = connection.GetDbSetting();
            var identity = (Field)null;
            var inputFields = (IEnumerable<DbField>)null;
            var identityDbField = dbFields?.FirstOrDefault(f => f.IsIdentity);

            // Set the identity field
            identity = IdentityCache.Get<TEntity>()?.AsField() ??
                FieldCache
                    .Get<TEntity>()?
                    .FirstOrDefault(field =>
                        string.Equals(field.Name.AsUnquoted(true, dbSetting), identityDbField?.Name.AsUnquoted(true, dbSetting), StringComparison.OrdinalIgnoreCase)) ??
                identityDbField?.AsField();

            // Filter the actual properties for input fields
            inputFields = dbFields?
                .Where(dbField => dbField.IsIdentity == false)
                .Where(dbField =>
                    fields.FirstOrDefault(field => string.Equals(field.Name.AsUnquoted(true, dbSetting), dbField.Name.AsUnquoted(true, dbSetting), StringComparison.OrdinalIgnoreCase)) != null)
                .AsList();

            // Variables for the context
            var multipleEntitiesFunc = (Action<DbCommand, IList<TEntity>>)null;
            var identitySettersFunc = (List<Action<TEntity, DbCommand>>)null;
            var singleEntityFunc = (Action<DbCommand, TEntity>)null;
            var identitySetterFunc = (Action<TEntity, object>)null;

            // Get if we have not skipped it
            if (identity != null)
            {
                identitySetterFunc = FunctionCache.GetDataEntityPropertySetterCompiledFunction<TEntity>(identity);
            }

            // Identity which objects to set
            if (batchSize <= 1)
            {
                singleEntityFunc = FunctionCache.GetDataEntityDbParameterSetterCompiledFunction<TEntity>(
                    string.Concat(typeof(TEntity).FullName, StringConstant.Period, tableName, ".InsertAll"),
                    inputFields?.AsList(),
                    null,
                    dbSetting);
            }
            else
            {
                multipleEntitiesFunc = FunctionCache.GetDataEntityListDbParameterSetterCompiledFunction<TEntity>(
                    string.Concat(typeof(TEntity).FullName, StringConstant.Period, tableName, ".InsertAll"),
                    inputFields?.AsList(),
                    null,
                    batchSize,
                    dbSetting);
            }

            // Return the value
            return new InsertAllExecutionContext<TEntity>
            {
                CommandText = commandText,
                InputFields = inputFields,
                BatchSize = batchSize,
                SingleDataEntityParametersSetterFunc = singleEntityFunc,
                MultipleDataEntitiesParametersSetterFunc = multipleEntitiesFunc,
                IdentityPropertySetterFunc = identitySetterFunc,
                IdentityPropertySettersFunc = identitySettersFunc
            };
        }
    }
}
