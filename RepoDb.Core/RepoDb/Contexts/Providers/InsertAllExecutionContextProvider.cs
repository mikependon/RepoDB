using RepoDb.Contexts.Execution;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.Requests;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
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
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="batchSize"></param>
        /// <param name="fields"></param>
        /// <param name="hints"></param>
        /// <param name="transaction"></param>
        /// <param name="statementBuilder"></param>
        /// <param name="skipIdentityCheck"></param>
        /// <returns></returns>
        public static InsertAllExecutionContext<TEntity> InsertAllExecutionContext<TEntity>(IDbConnection connection,
            string tableName,
            int batchSize,
            IEnumerable<Field> fields,
            string hints = null,
            IDbTransaction transaction = null,
            IStatementBuilder statementBuilder = null,
            bool skipIdentityCheck = false)
            where TEntity : class
        {
            // Get the DB fields
            var dbFields = DbFieldCache.Get(connection, tableName, transaction);

            // Returnt the context
            return InsertAllExecutionContext<TEntity>(connection,
                dbFields,
                tableName,
                batchSize,
                fields,
                hints,
                transaction,
                statementBuilder,
                skipIdentityCheck);
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
        /// <param name="skipIdentityCheck"></param>
        /// <returns></returns>
        public static async Task<InsertAllExecutionContext<TEntity>> InsertAllExecutionContextAsync<TEntity>(IDbConnection connection,
            string tableName,
            int batchSize,
            IEnumerable<Field> fields,
            string hints = null,
            IDbTransaction transaction = null,
            IStatementBuilder statementBuilder = null,
            bool skipIdentityCheck = false)
            where TEntity : class
        {
            // Get the DB fields
            var dbFields = await DbFieldCache.GetAsync(connection, tableName, transaction);

            // Returnt the context
            return InsertAllExecutionContext<TEntity>(connection,
                dbFields,
                tableName,
                batchSize,
                fields,
                hints,
                transaction,
                statementBuilder,
                skipIdentityCheck);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="connection"></param>
        /// <param name="dbFields"></param>
        /// <param name="tableName"></param>
        /// <param name="batchSize"></param>
        /// <param name="fields"></param>
        /// <param name="hints"></param>
        /// <param name="transaction"></param>
        /// <param name="statementBuilder"></param>
        /// <param name="skipIdentityCheck"></param>
        /// <returns></returns>
        private static InsertAllExecutionContext<TEntity> InsertAllExecutionContext<TEntity>(IDbConnection connection,
            IEnumerable<DbField> dbFields,
            string tableName,
            int batchSize,
            IEnumerable<Field> fields,
            string hints = null,
            IDbTransaction transaction = null,
            IStatementBuilder statementBuilder = null,
            bool skipIdentityCheck = false)
            where TEntity : class
        {
            var dbSetting = connection.GetDbSetting();
            var identity = (Field)null;
            var inputFields = (IEnumerable<DbField>)null;
            var identityDbField = dbFields?.FirstOrDefault(f => f.IsIdentity);

            // Set the identity value
            if (skipIdentityCheck == false)
            {
                identity = IdentityCache.Get<TEntity>()?.AsField();
                if (identity == null && identityDbField != null)
                {
                    identity = FieldCache.Get<TEntity>().FirstOrDefault(field =>
                        string.Equals(field.Name.AsUnquoted(true, dbSetting), identityDbField.Name.AsUnquoted(true, dbSetting), StringComparison.OrdinalIgnoreCase));
                }
            }

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
            if (skipIdentityCheck == false && identity != null)
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

            // Identify the requests
            var insertAllRequest = (InsertAllRequest)null;
            var insertRequest = (InsertRequest)null;

            // Create a different kind of requests
            if (typeof(TEntity).IsClassType() == false)
            {
                if (batchSize > 1)
                {
                    insertAllRequest = new InsertAllRequest(tableName,
                        connection,
                        transaction,
                        fields,
                        batchSize,
                        hints,
                        statementBuilder);
                }
                else
                {
                    insertRequest = new InsertRequest(tableName,
                        connection,
                        transaction,
                        fields,
                        hints,
                        statementBuilder);
                }
            }
            else
            {
                if (batchSize > 1)
                {
                    insertAllRequest = new InsertAllRequest(tableName,
                        connection,
                        transaction,
                        fields,
                        batchSize,
                        hints,
                        statementBuilder);
                }
                else
                {
                    insertRequest = new InsertRequest(tableName,
                        connection,
                        transaction,
                        fields,
                        hints,
                        statementBuilder);
                }
            }

            // Return the value
            return new InsertAllExecutionContext<TEntity>
            {
                CommandText = batchSize > 1 ? CommandTextCache.GetInsertAllText(insertAllRequest) : CommandTextCache.GetInsertText(insertRequest),
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
