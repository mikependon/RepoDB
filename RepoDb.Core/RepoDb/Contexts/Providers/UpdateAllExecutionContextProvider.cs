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
    internal static class UpdateAllExecutionContextProvider
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="tableName"></param>
        /// <param name="qualifiers"></param>
        /// <param name="fields"></param>
        /// <param name="batchSize"></param>
        /// <param name="hints"></param>
        /// <returns></returns>
        private static string GetKey<TEntity>(string tableName,
            IEnumerable<Field> qualifiers,
            IEnumerable<Field> fields,
            int batchSize,
            string hints)
        {
            return string.Concat(typeof(TEntity).FullName,
                ";",
                tableName,
                ";",
                qualifiers?.Select(f => f.Name).Join(","),
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
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="entities"></param>
        /// <param name="qualifiers"></param>
        /// <param name="batchSize"></param>
        /// <param name="fields"></param>
        /// <param name="hints"></param>
        /// <param name="transaction"></param>
        /// <param name="statementBuilder"></param>
        /// <returns></returns>
        public static UpdateAllExecutionContext<TEntity> Create<TEntity>(IDbConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<Field> qualifiers,
            int batchSize,
            IEnumerable<Field> fields,
            string hints = null,
            IDbTransaction transaction = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            var key = GetKey<TEntity>(tableName, qualifiers, fields, batchSize, hints);

            // Get from cache
            var context = UpdateAllExecutionContextCache.Get<TEntity>(key);
            if (context != null)
            {
                return context;
            }

            // Create
            var dbFields = DbFieldCache.Get(connection, tableName, transaction);
            var request = new UpdateAllRequest(tableName,
                connection,
                transaction,
                fields,
                qualifiers,
                batchSize,
                hints,
                statementBuilder);
            var commandText = CommandTextCache.GetUpdateAllText(request);

            // Call
            context = CreateInternal<TEntity>(connection,
                tableName,
                entities,
                dbFields,
                qualifiers,
                batchSize,
                fields,
                commandText);

            // Add to cache
            UpdateAllExecutionContextCache.Add<TEntity>(key, context);

            // Return
            return context;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="entities"></param>
        /// <param name="qualifiers"></param>
        /// <param name="batchSize"></param>
        /// <param name="fields"></param>
        /// <param name="hints"></param>
        /// <param name="transaction"></param>
        /// <param name="statementBuilder"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<UpdateAllExecutionContext<TEntity>> CreateAsync<TEntity>(IDbConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<Field> qualifiers,
            int batchSize,
            IEnumerable<Field> fields,
            string hints = null,
            IDbTransaction transaction = null,
            IStatementBuilder statementBuilder = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            var key = GetKey<TEntity>(tableName, qualifiers, fields, batchSize, hints);

            // Get from cache
            var context = UpdateAllExecutionContextCache.Get<TEntity>(key);
            if (context != null)
            {
                return context;
            }

            // Create
            var dbFields = await DbFieldCache.GetAsync(connection, tableName, transaction, cancellationToken);
            var request = new UpdateAllRequest(tableName,
                connection,
                transaction,
                fields,
                qualifiers,
                batchSize,
                hints,
                statementBuilder);
            var commandText = await CommandTextCache.GetUpdateAllTextAsync(request, cancellationToken);

            // Call
            context = CreateInternal<TEntity>(connection,
                tableName,
                entities,
                dbFields,
                qualifiers,
                batchSize,
                fields,
                commandText);

            // Add to cache
            UpdateAllExecutionContextCache.Add<TEntity>(key, context);

            // Return
            return context;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="entities"></param>
        /// <param name="dbFields"></param>
        /// <param name="qualifiers"></param>
        /// <param name="batchSize"></param>
        /// <param name="fields"></param>
        /// <param name="commandText"></param>
        /// <returns></returns>
        private static UpdateAllExecutionContext<TEntity> CreateInternal<TEntity>(IDbConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<DbField> dbFields,
            IEnumerable<Field> qualifiers,
            int batchSize,
            IEnumerable<Field> fields,
            string commandText)
            where TEntity : class
        {
            // Variables needed
            var dbSetting = connection.GetDbSetting();
            var inputFields = new List<DbField>();

            // Check the qualifiers
            if (qualifiers?.Any() != true)
            {
                var key = dbFields?.FirstOrDefault(dbField => dbField.IsPrimary == true) ??
                    dbFields?.FirstOrDefault(dbField => dbField.IsIdentity == true);
                qualifiers = key?.AsField().AsEnumerable();
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

            // Identity which objects to set
            if (batchSize <= 1)
            {
                singleEntityFunc = FunctionCache.GetDataEntityDbParameterSetterCompiledFunction<TEntity>(
                    string.Concat(typeof(TEntity).FullName, StringConstant.Period, tableName, ".UpdateAll"),
                    inputFields?.AsList(),
                    null,
                    dbSetting);
            }
            else
            {
                multipleEntitiesFunc = FunctionCache.GetDataEntityListDbParameterSetterCompiledFunction<TEntity>(
                    string.Concat(typeof(TEntity).FullName, StringConstant.Period, tableName, ".UpdateAll"),
                    inputFields?.AsList(),
                    null,
                    batchSize,
                    dbSetting);
            }

            // Return the value
            return new UpdateAllExecutionContext<TEntity>
            {
                CommandText = commandText,
                InputFields = inputFields,
                SingleDataEntityParametersSetterFunc = singleEntityFunc,
                MultipleDataEntitiesParametersSetterFunc = multipleEntitiesFunc
            };
        }
    }
}
