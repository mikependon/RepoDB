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
    internal static class MergeAllExecutionContextProvider
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="connection"></param>
        /// <param name="entities"></param>
        /// <param name="tableName"></param>
        /// <param name="qualifiers"></param>
        /// <param name="batchSize"></param>
        /// <param name="fields"></param>
        /// <param name="hints"></param>
        /// <param name="transaction"></param>
        /// <param name="statementBuilder"></param>
        /// <param name="skipIdentityCheck"></param>
        /// <returns></returns>
        public static MergeAllExecutionContext<TEntity> MergeAllExecutionContext<TEntity>(IDbConnection connection,
            IEnumerable<TEntity> entities,
            string tableName,
            IEnumerable<Field> qualifiers,
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
            return MergeAllExecutionContext<TEntity>(connection,
                entities,
                dbFields,
                tableName,
                qualifiers,
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
        /// <param name="entities"></param>
        /// <param name="tableName"></param>
        /// <param name="qualifiers"></param>
        /// <param name="batchSize"></param>
        /// <param name="fields"></param>
        /// <param name="hints"></param>
        /// <param name="transaction"></param>
        /// <param name="statementBuilder"></param>
        /// <param name="skipIdentityCheck"></param>
        /// <returns></returns>
        public static async Task<MergeAllExecutionContext<TEntity>> MergeAllExecutionContextAsync<TEntity>(IDbConnection connection,
            IEnumerable<TEntity> entities,
            string tableName,
            IEnumerable<Field> qualifiers,
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
            return MergeAllExecutionContext<TEntity>(connection,
                entities,
                dbFields,
                tableName,
                qualifiers,
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
        /// <param name="entities"></param>
        /// <param name="dbFields"></param>
        /// <param name="tableName"></param>
        /// <param name="qualifiers"></param>
        /// <param name="batchSize"></param>
        /// <param name="fields"></param>
        /// <param name="hints"></param>
        /// <param name="transaction"></param>
        /// <param name="statementBuilder"></param>
        /// <param name="skipIdentityCheck"></param>
        /// <returns></returns>
        private static MergeAllExecutionContext<TEntity> MergeAllExecutionContext<TEntity>(IDbConnection connection,
            IEnumerable<TEntity> entities,
            IEnumerable<DbField> dbFields,
            string tableName,
            IEnumerable<Field> qualifiers,
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

            // Check the fields
            if (fields?.Any() != true)
            {
                fields = dbFields?.AsFields();
            }

            // Check the qualifiers
            if (qualifiers?.Any() != true)
            {
                var primary = dbFields?.FirstOrDefault(dbField => dbField.IsPrimary == true);
                qualifiers = primary?.AsField().AsEnumerable();
            }

            // Set the identity value
            if (skipIdentityCheck == false)
            {
                identity = IdentityCache.Get<TEntity>()?.AsField();
                if (identity == null && identityDbField != null)
                {
                    identity = FieldCache.Get<TEntity>()?.FirstOrDefault(field =>
                        string.Equals(field.Name.AsUnquoted(true, dbSetting), identityDbField.Name.AsUnquoted(true, dbSetting), StringComparison.OrdinalIgnoreCase));
                }
            }

            // Filter the actual properties for input fields
            inputFields = dbFields?
                .Where(dbField =>
                    fields.FirstOrDefault(field => string.Equals(field.Name.AsUnquoted(true, dbSetting), dbField.Name.AsUnquoted(true, dbSetting), StringComparison.OrdinalIgnoreCase)) != null)
                .AsList();

            // Exclude the fields not on the actual entity
            if (typeof(TEntity).IsClassType() == false)
            {
                var entityFields = Field.Parse(entities?.FirstOrDefault());
                inputFields = inputFields?
                    .Where(field =>
                        entityFields.FirstOrDefault(f => string.Equals(f.Name.AsUnquoted(true, dbSetting), field.Name.AsUnquoted(true, dbSetting), StringComparison.OrdinalIgnoreCase)) != null)
                    .AsList();
            }

            // Variables for the context
            var multipleEntitiesFunc = (Action<DbCommand, IList<TEntity>>)null;
            var singleEntityFunc = (Action<DbCommand, TEntity>)null;
            var identitySetterFunc = (Action<TEntity, object>)null;

            // Get if we have not skipped it
            if (skipIdentityCheck == false && identity != null && typeof(TEntity).IsClassType())
            {
                identitySetterFunc = FunctionCache.GetDataEntityPropertySetterCompiledFunction<TEntity>(identity);
            }

            // Identity which objects to set
            if (batchSize <= 1)
            {
                singleEntityFunc = FunctionCache.GetDataEntityDbParameterSetterCompiledFunction<TEntity>(
                    string.Concat(typeof(TEntity).FullName, StringConstant.Period, tableName, ".MergeAll"),
                    inputFields?.AsList(),
                    null,
                    dbSetting);
            }
            else
            {
                multipleEntitiesFunc = FunctionCache.GetDataEntityListDbParameterSetterCompiledFunction<TEntity>(
                    string.Concat(typeof(TEntity).FullName, StringConstant.Period, tableName, ".MergeAll"),
                    inputFields?.AsList(),
                    null,
                    batchSize,
                    dbSetting);
            }

            // Identify the requests
            var mergeAllRequest = (MergeAllRequest)null;
            var mergeRequest = (MergeRequest)null;

            // Create a different kind of requests
            if (typeof(TEntity).IsClassType() == false)
            {
                if (batchSize > 1)
                {
                    mergeAllRequest = new MergeAllRequest(tableName,
                        connection,
                        transaction,
                        fields,
                        qualifiers,
                        batchSize,
                        hints,
                        statementBuilder);
                }
                else
                {
                    mergeRequest = new MergeRequest(tableName,
                        connection,
                        transaction,
                        fields,
                        qualifiers,
                        hints,
                        statementBuilder);
                }
            }
            else
            {
                if (batchSize > 1)
                {
                    mergeAllRequest = new MergeAllRequest(typeof(TEntity),
                        connection,
                        transaction,
                        fields,
                        qualifiers,
                        batchSize,
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
            }

            // Return the value
            return new MergeAllExecutionContext<TEntity>
            {
                CommandText = batchSize > 1 ? CommandTextCache.GetMergeAllText(mergeAllRequest) : CommandTextCache.GetMergeText(mergeRequest),
                InputFields = inputFields,
                BatchSize = batchSize,
                SingleDataEntityParametersSetterFunc = singleEntityFunc,
                MultipleDataEntitiesParametersSetterFunc = multipleEntitiesFunc,
                IdentityPropertySetterFunc = identitySetterFunc
            };
        }
    }
}
