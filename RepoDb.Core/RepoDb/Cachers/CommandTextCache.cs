using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.Requests;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace RepoDb
{
    /// <summary>
    /// A class that is used to cache the already-built command texts.
    /// </summary>
    public static class CommandTextCache
    {
        private static readonly ConcurrentDictionary<BaseRequest, string> cache = new ConcurrentDictionary<BaseRequest, string>();

        #region GetAverageText

        /// <summary>
        /// Gets the cached command text for the 'Average' operation.
        /// </summary>
        /// <param name="request">The request object.</param>
        /// <returns>The cached command text.</returns>
        internal static string GetAverageText(AverageRequest request)
        {
            var commandText = (string)null;
            if (cache.TryGetValue(request, out commandText) == false)
            {
                var statementBuilder = EnsureStatementBuilder(request.Connection, request.StatementBuilder);
                commandText = statementBuilder.CreateAverage(new QueryBuilder(),
                    request.Name,
                    request.Field,
                    request.Where,
                    request.Hints);
                cache.TryAdd(request, commandText);
            }
            return commandText;
        }

        #endregion

        #region GetAverageAllText

        /// <summary>
        /// Gets the cached command text for the 'AverageAll' operation.
        /// </summary>
        /// <param name="request">The request object.</param>
        /// <returns>The cached command text.</returns>
        internal static string GetAverageAllText(AverageAllRequest request)
        {
            var commandText = (string)null;
            if (cache.TryGetValue(request, out commandText) == false)
            {
                var statementBuilder = EnsureStatementBuilder(request.Connection, request.StatementBuilder);
                commandText = statementBuilder.CreateAverageAll(new QueryBuilder(),
                    request.Name,
                    request.Field,
                    request.Hints);
                cache.TryAdd(request, commandText);
            }
            return commandText;
        }

        #endregion

        #region GetBatchQueryText

        /// <summary>
        /// Gets the cached command text for the 'BatchQuery' operation.
        /// </summary>
        /// <param name="request">The request object.</param>
        /// <returns>The cached command text.</returns>
        internal static string GetBatchQueryText(BatchQueryRequest request)
        {
            var commandText = (string)null;
            if (cache.TryGetValue(request, out commandText) == false)
            {
                var statementBuilder = EnsureStatementBuilder(request.Connection, request.StatementBuilder);
                var fields = GetActualFields(request.Connection, request.Name, request.Fields, request.Transaction);
                commandText = statementBuilder.CreateBatchQuery(new QueryBuilder(),
                    request.Name,
                    fields,
                    request.Page,
                    request.RowsPerBatch,
                    request.OrderBy,
                    request.Where,
                    request.Hints);
                cache.TryAdd(request, commandText);
            }
            return commandText;
        }

        #endregion

        #region GetCountText

        /// <summary>
        /// Gets the cached command text for the 'Count' operation.
        /// </summary>
        /// <param name="request">The request object.</param>
        /// <returns>The cached command text.</returns>
        internal static string GetCountText(CountRequest request)
        {
            var commandText = (string)null;
            if (cache.TryGetValue(request, out commandText) == false)
            {
                var statementBuilder = EnsureStatementBuilder(request.Connection, request.StatementBuilder);
                commandText = statementBuilder.CreateCount(new QueryBuilder(),
                    request.Name,
                    request.Where,
                    request.Hints);
                cache.TryAdd(request, commandText);
            }
            return commandText;
        }

        #endregion

        #region GetCountAllText

        /// <summary>
        /// Gets the cached command text for the 'CountAll' operation.
        /// </summary>
        /// <param name="request">The request object.</param>
        /// <returns>The cached command text.</returns>
        internal static string GetCountAllText(CountAllRequest request)
        {
            var commandText = (string)null;
            if (cache.TryGetValue(request, out commandText) == false)
            {
                var statementBuilder = EnsureStatementBuilder(request.Connection, request.StatementBuilder);
                commandText = statementBuilder.CreateCountAll(new QueryBuilder(),
                    request.Name,
                    request.Hints);
                cache.TryAdd(request, commandText);
            }
            return commandText;
        }

        #endregion

        #region GetDeleteText

        /// <summary>
        /// Gets the cached command text for the 'Delete' operation.
        /// </summary>
        /// <param name="request">The request object.</param>
        /// <returns>The cached command text.</returns>
        internal static string GetDeleteText(DeleteRequest request)
        {
            var commandText = (string)null;
            if (cache.TryGetValue(request, out commandText) == false)
            {
                var statementBuilder = EnsureStatementBuilder(request.Connection, request.StatementBuilder);
                commandText = statementBuilder.CreateDelete(new QueryBuilder(),
                    request.Name,
                    request.Where);
                cache.TryAdd(request, commandText);
            }
            return commandText;
        }

        #endregion

        #region GetDeleteAllText

        /// <summary>
        /// Gets the cached command text for the 'DeleteAll' operation.
        /// </summary>
        /// <param name="request">The request object.</param>
        /// <returns>The cached command text.</returns>
        internal static string GetDeleteAllText(DeleteAllRequest request)
        {
            var commandText = (string)null;
            if (cache.TryGetValue(request, out commandText) == false)
            {
                var statementBuilder = EnsureStatementBuilder(request.Connection, request.StatementBuilder);
                commandText = statementBuilder.CreateDeleteAll(new QueryBuilder(),
                    request.Name);
                cache.TryAdd(request, commandText);
            }
            return commandText;
        }

        #endregion

        #region GetExistsText

        /// <summary>
        /// Gets the cached command text for the 'Exists' operation.
        /// </summary>
        /// <param name="request">The request object.</param>
        /// <returns>The cached command text.</returns>
        internal static string GetExistsText(ExistsRequest request)
        {
            var commandText = (string)null;
            if (cache.TryGetValue(request, out commandText) == false)
            {
                var statementBuilder = EnsureStatementBuilder(request.Connection, request.StatementBuilder);
                commandText = statementBuilder.CreateExists(new QueryBuilder(),
                    request.Name,
                    request.Where,
                    request.Hints);
                cache.TryAdd(request, commandText);
            }
            return commandText;
        }

        #endregion

        #region GetInsertText

        /// <summary>
        /// Gets the cached command text for the 'Insert' operation.
        /// </summary>
        /// <param name="request">The request object.</param>
        /// <returns>The cached command text.</returns>
        internal static string GetInsertText(InsertRequest request)
        {
            var commandText = (string)null;
            if (cache.TryGetValue(request, out commandText) == false)
            {
                var statementBuilder = EnsureStatementBuilder(request.Connection, request.StatementBuilder);
                var fields = GetActualFields(request.Connection, request.Name, request.Fields, request.Transaction);
                var primaryField = GetPrimaryField(request);
                var identityField = GetIdentityField(request);
                commandText = statementBuilder.CreateInsert(new QueryBuilder(),
                    request.Name,
                    fields,
                    primaryField,
                    identityField);
                cache.TryAdd(request, commandText);
            }
            return commandText;
        }

        #endregion

        #region GetInsertAllText

        /// <summary>
        /// Gets the cached command text for the 'InsertAll' operation.
        /// </summary>
        /// <param name="request">The request object.</param>
        /// <returns>The cached command text.</returns>
        internal static string GetInsertAllText(InsertAllRequest request)
        {
            var commandText = (string)null;
            if (cache.TryGetValue(request, out commandText) == false)
            {
                var statementBuilder = EnsureStatementBuilder(request.Connection, request.StatementBuilder);
                var fields = GetActualFields(request.Connection, request.Name, request.Fields, request.Transaction);
                var primaryField = GetPrimaryField(request);
                var identityField = GetIdentityField(request);
                commandText = statementBuilder.CreateInsertAll(new QueryBuilder(),
                    request.Name,
                    fields,
                    request.BatchSize,
                    primaryField,
                    identityField);
                cache.TryAdd(request, commandText);
            }
            return commandText;
        }

        #endregion

        #region GetMaxText

        /// <summary>
        /// Gets the cached command text for the 'Max' operation.
        /// </summary>
        /// <param name="request">The request object.</param>
        /// <returns>The cached command text.</returns>
        internal static string GetMaxText(MaxRequest request)
        {
            var commandText = (string)null;
            if (cache.TryGetValue(request, out commandText) == false)
            {
                var statementBuilder = EnsureStatementBuilder(request.Connection, request.StatementBuilder);
                commandText = statementBuilder.CreateMax(new QueryBuilder(),
                    request.Name,
                    request.Field,
                    request.Where,
                    request.Hints);
                cache.TryAdd(request, commandText);
            }
            return commandText;
        }

        #endregion

        #region GetMaxAllText

        /// <summary>
        /// Gets the cached command text for the 'MaxAll' operation.
        /// </summary>
        /// <param name="request">The request object.</param>
        /// <returns>The cached command text.</returns>
        internal static string GetMaxAllText(MaxAllRequest request)
        {
            var commandText = (string)null;
            if (cache.TryGetValue(request, out commandText) == false)
            {
                var statementBuilder = EnsureStatementBuilder(request.Connection, request.StatementBuilder);
                commandText = statementBuilder.CreateMaxAll(new QueryBuilder(),
                    request.Name,
                    request.Field,
                    request.Hints);
                cache.TryAdd(request, commandText);
            }
            return commandText;
        }

        #endregion

        #region GetMergeText

        /// <summary>
        /// Gets the cached command text for the 'Merge' operation.
        /// </summary>
        /// <param name="request">The request object.</param>
        /// <returns>The cached command text.</returns>
        internal static string GetMergeText(MergeRequest request)
        {
            var commandText = (string)null;
            if (cache.TryGetValue(request, out commandText) == false)
            {
                var statementBuilder = EnsureStatementBuilder(request.Connection, request.StatementBuilder);
                var fields = GetActualFields(request.Connection, request.Name, request.Fields, request.Transaction);
                var primaryField = GetPrimaryField(request);
                var identityField = GetIdentityField(request);
                commandText = statementBuilder.CreateMerge(new QueryBuilder(),
                    request.Name,
                    fields,
                    request.Qualifiers,
                    primaryField,
                    identityField);
                cache.TryAdd(request, commandText);
            }
            return commandText;
        }

        #endregion

        #region GetMergeAllText

        /// <summary>
        /// Gets the cached command text for the 'MergeAll' operation.
        /// </summary>
        /// <param name="request">The request object.</param>
        /// <returns>The cached command text.</returns>
        internal static string GetMergeAllText(MergeAllRequest request)
        {
            var commandText = (string)null;
            if (cache.TryGetValue(request, out commandText) == false)
            {
                var statementBuilder = EnsureStatementBuilder(request.Connection, request.StatementBuilder);
                var fields = GetActualFields(request.Connection, request.Name, request.Fields, request.Transaction);
                var primaryField = GetPrimaryField(request);
                var identityField = GetIdentityField(request);
                commandText = statementBuilder.CreateMergeAll(new QueryBuilder(),
                    request.Name,
                    fields,
                    request.Qualifiers,
                    request.BatchSize,
                    primaryField,
                    identityField);
                cache.TryAdd(request, commandText);
            }
            return commandText;
        }

        #endregion

        #region GetMinText

        /// <summary>
        /// Gets the cached command text for the 'Min' operation.
        /// </summary>
        /// <param name="request">The request object.</param>
        /// <returns>The cached command text.</returns>
        internal static string GetMinText(MinRequest request)
        {
            var commandText = (string)null;
            if (cache.TryGetValue(request, out commandText) == false)
            {
                var statementBuilder = EnsureStatementBuilder(request.Connection, request.StatementBuilder);
                commandText = statementBuilder.CreateMin(new QueryBuilder(),
                    request.Name,
                    request.Field,
                    request.Where,
                    request.Hints);
                cache.TryAdd(request, commandText);
            }
            return commandText;
        }

        #endregion

        #region GetMinAllText

        /// <summary>
        /// Gets the cached command text for the 'MinAll' operation.
        /// </summary>
        /// <param name="request">The request object.</param>
        /// <returns>The cached command text.</returns>
        internal static string GetMinAllText(MinAllRequest request)
        {
            var commandText = (string)null;
            if (cache.TryGetValue(request, out commandText) == false)
            {
                var statementBuilder = EnsureStatementBuilder(request.Connection, request.StatementBuilder);
                commandText = statementBuilder.CreateMinAll(new QueryBuilder(),
                    request.Name,
                    request.Field,
                    request.Hints);
                cache.TryAdd(request, commandText);
            }
            return commandText;
        }

        #endregion

        #region GetQueryText

        /// <summary>
        /// Gets the cached command text for the 'Query' operation.
        /// </summary>
        /// <param name="request">The request object.</param>
        /// <returns>The cached command text.</returns>
        internal static string GetQueryText(QueryRequest request)
        {
            var commandText = (string)null;
            if (cache.TryGetValue(request, out commandText) == false)
            {
                var statementBuilder = EnsureStatementBuilder(request.Connection, request.StatementBuilder);
                var fields = GetActualFields(request.Connection, request.Name, request.Fields, request.Transaction);
                commandText = statementBuilder.CreateQuery(new QueryBuilder(),
                    request.Name,
                    fields,
                    request.Where,
                    request.OrderBy,
                    request.Top,
                    request.Hints);
                cache.TryAdd(request, commandText);
            }
            return commandText;
        }

        #endregion

        #region GetQueryAllText

        /// <summary>
        /// Gets the cached command text for the 'QueryAll' operation.
        /// </summary>
        /// <param name="request">The request object.</param>
        /// <returns>The cached command text.</returns>
        internal static string GetQueryAllText(QueryAllRequest request)
        {
            var commandText = (string)null;
            if (cache.TryGetValue(request, out commandText) == false)
            {
                var statementBuilder = EnsureStatementBuilder(request.Connection, request.StatementBuilder);
                var fields = GetActualFields(request.Connection, request.Name, request.Fields, request.Transaction);
                commandText = statementBuilder.CreateQueryAll(new QueryBuilder(),
                    request.Name,
                    fields,
                    request.OrderBy,
                    request.Hints);
                cache.TryAdd(request, commandText);
            }
            return commandText;
        }

        #endregion

        #region GetQueryMultipleText

        /// <summary>
        /// Gets the cached command text for the 'QueryMultiple' operation.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="request">The request object.</param>
        /// <returns>The cached command text.</returns>
        internal static string GetQueryMultipleText<TEntity>(QueryMultipleRequest request)
            where TEntity : class
        {
            var commandText = (string)null;
            if (cache.TryGetValue(request, out commandText) == false)
            {
                var statementBuilder = EnsureStatementBuilder(request.Connection, request.StatementBuilder);
                var fields = GetActualFields(request.Connection, request.Name, request.Fields, request.Transaction);
                commandText = statementBuilder.CreateQuery(new QueryBuilder(),
                    request.Name,
                    fields,
                    request.Where,
                    request.OrderBy,
                    request.Top,
                    request.Hints);
                cache.TryAdd(request, commandText);
            }
            return commandText;
        }

        #endregion

        #region GetSumText

        /// <summary>
        /// Gets the cached command text for the 'Sum' operation.
        /// </summary>
        /// <param name="request">The request object.</param>
        /// <returns>The cached command text.</returns>
        internal static string GetSumText(SumRequest request)
        {
            var commandText = (string)null;
            if (cache.TryGetValue(request, out commandText) == false)
            {
                var statementBuilder = EnsureStatementBuilder(request.Connection, request.StatementBuilder);
                commandText = statementBuilder.CreateSum(new QueryBuilder(),
                    request.Name,
                    request.Field,
                    request.Where,
                    request.Hints);
                cache.TryAdd(request, commandText);
            }
            return commandText;
        }

        #endregion

        #region GetSumAllText

        /// <summary>
        /// Gets the cached command text for the 'SumAll' operation.
        /// </summary>
        /// <param name="request">The request object.</param>
        /// <returns>The cached command text.</returns>
        internal static string GetSumAllText(SumAllRequest request)
        {
            var commandText = (string)null;
            if (cache.TryGetValue(request, out commandText) == false)
            {
                var statementBuilder = EnsureStatementBuilder(request.Connection, request.StatementBuilder);
                commandText = statementBuilder.CreateSumAll(new QueryBuilder(),
                    request.Name,
                    request.Field,
                    request.Hints);
                cache.TryAdd(request, commandText);
            }
            return commandText;
        }

        #endregion

        #region GetTruncateText

        /// <summary>
        /// Gets the cached command text for the 'Truncate' operation.
        /// </summary>
        /// <param name="request">The request object.</param>
        /// <returns>The cached command text.</returns>
        internal static string GetTruncateText(TruncateRequest request)
        {
            var commandText = (string)null;
            if (cache.TryGetValue(request, out commandText) == false)
            {
                var statementBuilder = EnsureStatementBuilder(request.Connection, request.StatementBuilder);
                commandText = statementBuilder.CreateTruncate(new QueryBuilder(),
                    request.Name);
                cache.TryAdd(request, commandText);
            }
            return commandText;
        }

        #endregion

        #region GetUpdateText

        /// <summary>
        /// Gets the cached command text for the 'Update' operation.
        /// </summary>
        /// <param name="request">The request object.</param>
        /// <returns>The cached command text.</returns>
        internal static string GetUpdateText(UpdateRequest request)
        {
            var commandText = (string)null;
            if (cache.TryGetValue(request, out commandText) == false)
            {
                var statementBuilder = EnsureStatementBuilder(request.Connection, request.StatementBuilder);
                var fields = GetActualFields(request.Connection, request.Name, request.Fields, request.Transaction);
                var primaryField = GetPrimaryField(request);
                var identityField = GetIdentityField(request);
                commandText = statementBuilder.CreateUpdate(new QueryBuilder(),
                    request.Name,
                    fields,
                    request.Where,
                    primaryField,
                    identityField);
                cache.TryAdd(request, commandText);
            }
            return commandText;
        }

        #endregion

        #region GetUpdateAllText

        /// <summary>
        /// Gets the cached command text for the 'UpdateAll' operation.
        /// </summary>
        /// <param name="request">The request object.</param>
        /// <returns>The cached command text.</returns>
        internal static string GetUpdateAllText(UpdateAllRequest request)
        {
            var commandText = (string)null;
            if (cache.TryGetValue(request, out commandText) == false)
            {
                var statementBuilder = EnsureStatementBuilder(request.Connection, request.StatementBuilder);
                var fields = GetActualFields(request.Connection, request.Name, request.Fields, request.Transaction);
                var qualifiers = request.Qualifiers;
                var primaryField = GetPrimaryField(request);
                var identityField = GetIdentityField(request);
                commandText = statementBuilder.CreateUpdateAll(new QueryBuilder(),
                    request.Name,
                    fields,
                    request.Qualifiers,
                    request.BatchSize,
                    primaryField,
                    identityField);
                cache.TryAdd(request, commandText);
            }
            return commandText;
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Flushes all the existing cached command texts.
        /// </summary>
        public static void Flush()
        {
            cache.Clear();
        }

        /// <summary>
        /// Get the actual list of <see cref="Field"/> objects of the table based on the actual list of <see cref="DbField"/> objects.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="fields">The target name of the table.</param>
        /// <param name="tableName">The list of fields from the data entity object.</param>
        /// <param name="transaction">The transaction object that is currently in used.</param>
        /// <returns>The actual list of <see cref="Field"/> objects of the table.</returns>
        private static IEnumerable<Field> GetActualFields(IDbConnection connection,
            string tableName,
            IEnumerable<Field> fields,
            IDbTransaction transaction)
        {
            if (fields?.Any() != true)
            {
                return null;
            }

            // Get all the fields from the database, and the setting
            var dbFields = DbFieldCache.Get(connection, tableName, transaction);
            var dbSetting = connection.GetDbSetting();

            // Return the filtered one
            return dbFields?.Any() == true ?
                fields.Where(f => dbFields.FirstOrDefault(df =>
                    string.Equals(df.Name.AsUnquoted(true, dbSetting), f.Name.AsUnquoted(true, dbSetting), StringComparison.OrdinalIgnoreCase)) != null) :
                fields;
        }

        /// <summary>
        /// Gets the primary <see cref="DbField"/> object.
        /// </summary>
        /// <param name="request">The request object.</param>
        /// <returns>The primary <see cref="DbField"/> object.</returns>
        private static DbField GetPrimaryField(BaseRequest request)
        {
            if (request.Type != null && request.Type != typeof(object))
            {
                var primaryProperty = PrimaryCache.Get(request.Type);
                if (primaryProperty != null)
                {
                    var identityProperty = IdentityCache.Get(request.Type);
                    var isIdentity = false;
                    if (identityProperty != null)
                    {
                        isIdentity = string.Equals(identityProperty.GetMappedName(), primaryProperty.GetMappedName(), StringComparison.OrdinalIgnoreCase);
                    }
                    return new DbField(primaryProperty.GetMappedName(),
                        true,
                        isIdentity,
                        false,
                        primaryProperty.PropertyInfo.PropertyType,
                        null,
                        null,
                        null,
                        null);
                }
            }
            return DbFieldCache.Get(request.Connection, request.Name, request.Transaction)?.FirstOrDefault(f => f.IsPrimary);
        }

        /// <summary>
        /// Gets the identity <see cref="DbField"/> object.
        /// </summary>
        /// <param name="request">The request object.</param>
        /// <returns>The identity <see cref="DbField"/> object.</returns>
        private static DbField GetIdentityField(BaseRequest request)
        {
            if (request.Type != null && request.Type != typeof(object))
            {
                var identityProperty = IdentityCache.Get(request.Type);
                if (identityProperty != null)
                {
                    var primaryProperty = PrimaryCache.Get(request.Type);
                    var isPrimary = false;
                    if (primaryProperty != null)
                    {
                        isPrimary = string.Equals(primaryProperty.GetMappedName(), identityProperty.GetMappedName(), StringComparison.OrdinalIgnoreCase);
                    }
                    return new DbField(identityProperty.GetMappedName(),
                        isPrimary,
                        true,
                        false,
                        identityProperty.PropertyInfo.PropertyType,
                        null,
                        null,
                        null,
                        null);
                }
            }
            return DbFieldCache.Get(request.Connection, request.Name, request.Transaction)?.FirstOrDefault(f => f.IsIdentity);
        }

        /// <summary>
        /// Throws an exception of the builder is not defined.
        /// </summary>
        /// <param name="connection">The connection object to identified.</param>
        /// <param name="builder">The builder to be checked.</param>
        /// <returns>The instance of available statement builder.</returns>
        private static IStatementBuilder EnsureStatementBuilder(IDbConnection connection,
            IStatementBuilder builder)
        {
            return builder ?? connection.GetStatementBuilder();
        }

        #endregion
    }
}
