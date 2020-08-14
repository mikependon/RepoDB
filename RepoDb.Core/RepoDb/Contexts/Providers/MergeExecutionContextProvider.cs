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
    internal static class MergeExecutionContextProvider
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="qualifiers"></param>
        /// <param name="fields"></param>
        /// <param name="hints"></param>
        /// <param name="transaction"></param>
        /// <param name="statementBuilder"></param>
        /// <param name="skipIdentityCheck"></param>
        /// <returns></returns>
        public static MergeExecutionContext<TEntity> MergeExecutionContext<TEntity>(IDbConnection connection,
            string tableName,
            IEnumerable<Field> qualifiers,
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
            return MergeExecutionContext<TEntity>(connection,
                dbFields,
                tableName,
                qualifiers,
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
        /// <param name="qualifiers"></param>
        /// <param name="fields"></param>
        /// <param name="hints"></param>
        /// <param name="transaction"></param>
        /// <param name="statementBuilder"></param>
        /// <param name="skipIdentityCheck"></param>
        /// <returns></returns>
        public static async Task<MergeExecutionContext<TEntity>> MergeExecutionContextAsync<TEntity>(IDbConnection connection,
            string tableName,
            IEnumerable<Field> qualifiers,
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
            return MergeExecutionContext<TEntity>(connection,
                dbFields,
                tableName,
                qualifiers,
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
        /// <param name="qualifiers"></param>
        /// <param name="fields"></param>
        /// <param name="hints"></param>
        /// <param name="transaction"></param>
        /// <param name="statementBuilder"></param>
        /// <param name="skipIdentityCheck"></param>
        /// <returns></returns>
        private static MergeExecutionContext<TEntity> MergeExecutionContext<TEntity>(IDbConnection connection,
            IEnumerable<DbField> dbFields,
            string tableName,
            IEnumerable<Field> qualifiers,
            IEnumerable<Field> fields,
            string hints = null,
            IDbTransaction transaction = null,
            IStatementBuilder statementBuilder = null,
            bool skipIdentityCheck = false)
            where TEntity : class
        {
            var dbSetting = connection.GetDbSetting();
            var identity = (Field)null;
            var inputFields = new List<DbField>();
            var identityDbField = dbFields?.FirstOrDefault(f => f.IsIdentity);

            // Set the identity field
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
                .Where(dbField =>
                    fields.FirstOrDefault(field => string.Equals(field.Name.AsUnquoted(true, dbSetting), dbField.Name.AsUnquoted(true, dbSetting), StringComparison.OrdinalIgnoreCase)) != null)
                .AsList();

            // Variables for the entity action
            var identityPropertySetter = (Action<TEntity, object>)null;

            // Get the identity setter
            if (skipIdentityCheck == false && identity != null)
            {
                identityPropertySetter = FunctionCache.GetDataEntityPropertySetterCompiledFunction<TEntity>(identity);
            }

            // Identify the requests
            var mergeRequest = (MergeRequest)null;

            // Create a different kind of requests
            if (typeof(TEntity).IsClassType() == false)
            {
                mergeRequest = new MergeRequest(tableName,
                    connection,
                    transaction,
                    fields,
                    qualifiers,
                    hints,
                    statementBuilder);
            }
            else
            {
                mergeRequest = new MergeRequest(typeof(TEntity),
                    connection,
                    transaction,
                    fields,
                    qualifiers,
                    hints,
                    statementBuilder);
            }

            // Return the value
            return new MergeExecutionContext<TEntity>
            {
                CommandText = CommandTextCache.GetMergeText(mergeRequest),
                InputFields = inputFields,
                ParametersSetterFunc = FunctionCache.GetDataEntityDbParameterSetterCompiledFunction<TEntity>(
                    string.Concat(typeof(TEntity).FullName, StringConstant.Period, tableName, ".Merge"),
                    inputFields?.AsList(),
                    null,
                    dbSetting),
                IdentityPropertySetterFunc = identityPropertySetter
            };
        }
    }
}
