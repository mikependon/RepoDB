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
        /// <param name="entityType"></param>
        /// <param name="tableName"></param>
        /// <param name="fields"></param>
        /// <param name="batchSize"></param>
        /// <param name="hints"></param>
        /// <returns></returns>
        private static string GetKey(Type entityType,
            string tableName,
            IEnumerable<Field> fields,
            int batchSize,
            string hints)
        {
            return string.Concat(entityType.FullName,
                ";",
                tableName,
                ";",
                fields?.Select(f => f.Name).Join(","),
                ";",
                batchSize.ToString(),
                ";",
                hints);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entityType"></param>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="batchSize"></param>
        /// <param name="fields"></param>
        /// <param name="hints"></param>
        /// <param name="transaction"></param>
        /// <param name="statementBuilder"></param>
        /// <returns></returns>
        public static InsertAllExecutionContext Create(Type entityType,
            IDbConnection connection,
            string tableName,
            int batchSize,
            IEnumerable<Field> fields,
            string hints = null,
            IDbTransaction transaction = null,
            IStatementBuilder statementBuilder = null)
        {
            var key = GetKey(entityType, tableName, fields, batchSize, hints);

            // Get from cache
            var context = InsertAllExecutionContextCache.Get(key);
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
            context = CreateInternal(entityType,
                connection,
                tableName,
                dbFields,
                batchSize,
                fields,
                commandText);

            // Add to cache
            InsertAllExecutionContextCache.Add(key, context);

            // Return
            return context;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entityType"></param>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="batchSize"></param>
        /// <param name="fields"></param>
        /// <param name="hints"></param>
        /// <param name="transaction"></param>
        /// <param name="statementBuilder"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<InsertAllExecutionContext> CreateAsync(Type entityType,
            IDbConnection connection,
            string tableName,
            int batchSize,
            IEnumerable<Field> fields,
            string hints = null,
            IDbTransaction transaction = null,
            IStatementBuilder statementBuilder = null,
            CancellationToken cancellationToken = default)
        {
            var key = GetKey(entityType, tableName, fields, batchSize, hints);

            // Get from cache
            var context = InsertAllExecutionContextCache.Get(key);
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
            context = CreateInternal(entityType,
                connection,
                tableName,
                dbFields,
                batchSize,
                fields,
                commandText);

            // Add to cache
            InsertAllExecutionContextCache.Add(key, context);

            // Return
            return context;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entityType"></param>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="dbFields"></param>
        /// <param name="batchSize"></param>
        /// <param name="fields"></param>
        /// <param name="commandText"></param>
        /// <returns></returns>
        private static InsertAllExecutionContext CreateInternal(Type entityType,
            IDbConnection connection,
            string tableName,
            IEnumerable<DbField> dbFields,
            int batchSize,
            IEnumerable<Field> fields,
            string commandText)
        {
            var dbSetting = connection.GetDbSetting();
            var primary = (Field)null;
            var inputFields = (IEnumerable<DbField>)null;
            var primaryDbField = dbFields?.FirstOrDefault(f => f.IsPrimary);

            // Set the primary field
            primary = PrimaryCache.Get(entityType)?.AsField() ??
                FieldCache
                    .Get(entityType)?
                    .FirstOrDefault(field =>
                        string.Equals(field.Name.AsUnquoted(true, dbSetting), primaryDbField?.Name.AsUnquoted(true, dbSetting), StringComparison.OrdinalIgnoreCase)) ??
                primaryDbField?.AsField();

            // Filter the actual properties for input fields
            inputFields = dbFields?
                .Where(dbField =>
                    dbField.IsIdentity == false)
                .Where(dbField =>
                    fields.FirstOrDefault(field =>
                        string.Equals(field.Name.AsUnquoted(true, dbSetting), dbField.Name.AsUnquoted(true, dbSetting), StringComparison.OrdinalIgnoreCase)) != null)
                .AsList();

            // Variables for the context
            var multipleEntitiesFunc = (Action<DbCommand, IList<object>>)null;
            var singleEntityFunc = (Action<DbCommand, object>)null;
            var primarySetterFunc = (Action<object, object>)null;

            // Get if we have not skipped it
            if (primary != null)
            {
                primarySetterFunc = FunctionCache
                    .GetDataEntityPropertySetterCompiledFunction(entityType, primary);
            }

            // Identity which objects to set
            if (batchSize <= 1)
            {
                singleEntityFunc = FunctionCache
                    .GetDataEntityDbParameterSetterCompiledFunction(entityType,
                        string.Concat(entityType.FullName, StringConstant.Period, tableName, ".InsertAll"),
                        inputFields,
                        null,
                        dbSetting);
            }
            else
            {
                multipleEntitiesFunc = FunctionCache
                    .GetDataEntityListDbParameterSetterCompiledFunction(entityType,
                        string.Concat(entityType.FullName, StringConstant.Period, tableName, ".InsertAll"),
                        inputFields,
                        null,
                        batchSize,
                        dbSetting);
            }

            // Return the value
            return new InsertAllExecutionContext
            {
                CommandText = commandText,
                InputFields = inputFields,
                BatchSize = batchSize,
                SingleDataEntityParametersSetterFunc = singleEntityFunc,
                MultipleDataEntitiesParametersSetterFunc = multipleEntitiesFunc,
                PrimaryPropertySetterFunc = primarySetterFunc
            };
        }
    }
}
