using RepoDb.Contexts.Cachers;
using RepoDb.Contexts.Execution;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.Requests;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
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
        /// <param name="tableName"></param>
        /// <param name="fields"></param>
        /// <param name="hints"></param>
        /// <param name="where"></param>
        /// <returns></returns>
        private static string GetKey<TEntity>(string tableName,
            IEnumerable<Field> fields,
            string hints,
            QueryGroup where)
        {
            return string.Concat(typeof(TEntity).FullName,
                ";",
                tableName,
                ";",
                fields?.Select(f => f.Name).Join(","),
                ";",
                hints,
                ";",
                where?.GetHashCode().ToString());
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
        public static UpdateExecutionContext<TEntity> Create<TEntity>(IDbConnection connection,
            string tableName,
            QueryGroup where,
            IEnumerable<Field> fields,
            string hints = null,
            IDbTransaction transaction = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            var key = GetKey<TEntity>(tableName, fields, hints, where);

            // Get from cache
            var context = UpdateExecutionContextCache.Get<TEntity>(key);
            if (context != null)
            {
                return context;
            }

            // Create
            var dbFields = DbFieldCache.Get(connection, tableName, transaction);
            var request = new UpdateRequest(tableName,
                connection,
                transaction,
                where,
                fields,
                hints,
                statementBuilder);
            var commandText = CommandTextCache.GetUpdateText(request);

            // Call
            context = CreateInternal<TEntity>(connection,
                tableName,
                dbFields,
                fields,
                commandText);

            // Add to cache
            UpdateExecutionContextCache.Add<TEntity>(key, context);

            // Return
            return context;
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
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<UpdateExecutionContext<TEntity>> CreateAsync<TEntity>(IDbConnection connection,
            string tableName,
            QueryGroup where,
            IEnumerable<Field> fields,
            string hints = null,
            IDbTransaction transaction = null,
            IStatementBuilder statementBuilder = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            var key = GetKey<TEntity>(tableName, fields, hints, where);

            // Get from cache
            var context = UpdateExecutionContextCache.Get<TEntity>(key);
            if (context != null)
            {
                return context;
            }

            // Create
            var dbFields = await DbFieldCache.GetAsync(connection, tableName, transaction, cancellationToken);
            var request = new UpdateRequest(tableName,
                connection,
                transaction,
                where,
                fields,
                hints,
                statementBuilder);
            var commandText = await CommandTextCache.GetUpdateTextAsync(request, cancellationToken);

            // Call
            context = CreateInternal<TEntity>(connection,
                tableName,
                dbFields,
                fields,
                commandText);

            // Add to cache
            UpdateExecutionContextCache.Add<TEntity>(key, context);

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
        /// <param name="fields"></param>
        /// <param name="commandText"></param>
        /// <returns></returns>
        private static UpdateExecutionContext<TEntity> CreateInternal<TEntity>(IDbConnection connection,
            string tableName,
            IEnumerable<DbField> dbFields,
            IEnumerable<Field> fields,
            string commandText)
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

            // Return the value
            return new UpdateExecutionContext<TEntity>
            {
                CommandText = commandText,
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
