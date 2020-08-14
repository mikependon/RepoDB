using RepoDb.Contexts.Execution;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.Requests;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.Contexts.Providers
{
    /// <summary>
    /// 
    /// </summary>
    internal static class UpdateExecutionContextProvider
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="where"></param>
        /// <param name="fields"></param>
        /// <param name="hints"></param>
        /// <param name="transaction"></param>
        /// <param name="statementBuilder"></param>
        /// <returns></returns>
        public static UpdateExecutionContext<TEntity> UpdateExecutionContext<TEntity>(IDbConnection connection,
            string tableName,
            QueryGroup where,
            IEnumerable<Field> fields,
            string hints = null,
            IDbTransaction transaction = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            // Get the DB fields
            var dbFields = DbFieldCache.Get(connection, tableName, transaction);

            // Returnt the context
            return UpdateExecutionContext<TEntity>(connection,
                tableName,
                where,
                dbFields,
                fields,
                hints,
                transaction,
                statementBuilder);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="where"></param>
        /// <param name="fields"></param>
        /// <param name="hints"></param>
        /// <param name="transaction"></param>
        /// <param name="statementBuilder"></param>
        /// <returns></returns>
        public static async Task<UpdateExecutionContext<TEntity>> UpdateExecutionContextAsync<TEntity>(IDbConnection connection,
            string tableName,
            QueryGroup where,
            IEnumerable<Field> fields,
            string hints = null,
            IDbTransaction transaction = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            // Get the DB fields
            var dbFields = await DbFieldCache.GetAsync(connection, tableName, transaction);

            // Return the context
            return UpdateExecutionContext<TEntity>(connection,
                tableName,
                where,
                dbFields,
                fields,
                hints,
                transaction,
                statementBuilder);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="where"></param>
        /// <param name="dbFields"></param>
        /// <param name="fields"></param>
        /// <param name="hints"></param>
        /// <param name="transaction"></param>
        /// <param name="statementBuilder"></param>
        /// <returns></returns>
        private static UpdateExecutionContext<TEntity> UpdateExecutionContext<TEntity>(IDbConnection connection,
            string tableName,
            QueryGroup where,
            IEnumerable<DbField> dbFields,
            IEnumerable<Field> fields,
            string hints = null,
            IDbTransaction transaction = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            var dbSetting = connection.GetDbSetting();
            var inputFields = new List<DbField>();

            // Filter the actual properties for input fields
            inputFields = dbFields?
                .Where(dbField => dbField.IsIdentity == false)
                .Where(dbField =>
                    fields.FirstOrDefault(field => string.Equals(field.Name.AsUnquoted(true, dbSetting), dbField.Name.AsUnquoted(true, dbSetting), StringComparison.OrdinalIgnoreCase)) != null)
                .AsList();

            // Identify the requests
            var updateRequest = (UpdateRequest)null;

            // Create a different kind of requests
            if (typeof(TEntity).IsClassType() == false)
            {
                updateRequest = new UpdateRequest(tableName,
                    connection,
                    transaction,
                    where,
                    fields,
                    hints,
                    statementBuilder);
            }
            else
            {
                updateRequest = new UpdateRequest(typeof(TEntity),
                    connection,
                    transaction,
                    where,
                    fields,
                    hints,
                    statementBuilder);
            }

            // Return the value
            return new UpdateExecutionContext<TEntity>
            {
                CommandText = CommandTextCache.GetUpdateText(updateRequest),
                InputFields = inputFields,
                ParametersSetterFunc = FunctionCache.GetDataEntityDbParameterSetterCompiledFunction<TEntity>(
                    string.Concat(typeof(TEntity).FullName, StringConstant.Period, tableName, ".Update"),
                    inputFields?.AsList(),
                    null,
                    dbSetting)
            };
        }
    }
}
