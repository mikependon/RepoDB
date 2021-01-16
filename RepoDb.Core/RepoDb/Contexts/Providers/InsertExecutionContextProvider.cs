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
    internal static class InsertExecutionContextProvider
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="tableName"></param>
        /// <param name="fields"></param>
        /// <param name="hints"></param>
        /// <returns></returns>
        private static string GetKey<TEntity>(string tableName,
            IEnumerable<Field> fields,
            string hints)
        {
            return string.Concat(typeof(TEntity).FullName,
                ";",
                tableName,
                ";",
                fields?.Select(f => f.Name).Join(","),
                ";",
                hints);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="fields"></param>
        /// <param name="hints"></param>
        /// <param name="transaction"></param>
        /// <param name="statementBuilder"></param>
        /// <returns></returns>
        public static InsertExecutionContext<TEntity> Create<TEntity>(IDbConnection connection,
            string tableName,
            IEnumerable<Field> fields,
            string hints = null,
            IDbTransaction transaction = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            var key = GetKey<TEntity>(tableName, fields, hints);

            // Get from cache
            var context = InsertExecutionContextCache.Get<TEntity>(key);
            if (context != null)
            {
                return context;
            }

            // Create
            var dbFields = DbFieldCache.Get(connection, tableName, transaction);
            var request = new InsertRequest(tableName,
                connection,
                transaction,
                fields,
                hints,
                statementBuilder);
            var commandText = CommandTextCache.GetInsertText(request);

            // Call
            context = CreateInternal<TEntity>(connection,
                dbFields,
                tableName,
                fields,
                commandText);

            // Add to cache
            InsertExecutionContextCache.Add<TEntity>(key, context);

            // Return
            return context;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="fields"></param>
        /// <param name="hints"></param>
        /// <param name="transaction"></param>
        /// <param name="statementBuilder"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<InsertExecutionContext<TEntity>> CreateAsync<TEntity>(IDbConnection connection,
            string tableName,
            IEnumerable<Field> fields,
            string hints = null,
            IDbTransaction transaction = null,
            IStatementBuilder statementBuilder = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            var key = GetKey<TEntity>(tableName, fields, hints);

            // Get from cache
            var context = InsertExecutionContextCache.Get<TEntity>(key);
            if (context != null)
            {
                return context;
            }

            // Create
            var dbFields = await DbFieldCache.GetAsync(connection, tableName, transaction, cancellationToken);
            var request = new InsertRequest(tableName,
                connection,
                transaction,
                fields,
                hints,
                statementBuilder);
            var commandText = await CommandTextCache.GetInsertTextAsync(request, cancellationToken);

            // Call
            context = CreateInternal<TEntity>(connection,
                dbFields,
                tableName,
                fields,
                commandText);

            // Add to cache
            InsertExecutionContextCache.Add<TEntity>(key, context);

            // Return
            return context;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="connection"></param>
        /// <param name="dbFields"></param>
        /// <param name="tableName"></param>
        /// <param name="fields"></param>
        /// <param name="commandText"></param>
        /// <returns></returns>
        private static InsertExecutionContext<TEntity> CreateInternal<TEntity>(IDbConnection connection,
            IEnumerable<DbField> dbFields,
            string tableName,
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
                    fields.FirstOrDefault(field =>
                        string.Equals(field.Name.AsUnquoted(true, dbSetting), dbField.Name.AsUnquoted(true, dbSetting), StringComparison.OrdinalIgnoreCase)) != null)
                .AsList();

            // Variables for the entity action
            var identityPropertySetter = (Action<TEntity, object>)null;

            // Get the identity setter
            if (identity != null)
            {
                identityPropertySetter = FunctionCache.GetDataEntityPropertySetterCompiledFunction<TEntity>(identity);
            }

            // Return the value
            return new InsertExecutionContext<TEntity>
            {
                CommandText = commandText,
                InputFields = inputFields,
                ParametersSetterFunc = FunctionCache.GetDataEntityDbParameterSetterCompiledFunction<TEntity>(
                    string.Concat(typeof(TEntity).FullName, StringConstant.Period, tableName, ".Insert"),
                    inputFields?.AsList(),
                    null,
                    dbSetting),
                IdentityPropertySetterFunc = identityPropertySetter
            };
        }
    }
}
