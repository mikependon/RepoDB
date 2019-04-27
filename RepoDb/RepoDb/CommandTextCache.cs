using RepoDb.Interfaces;
using RepoDb.Requests;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace RepoDb
{
    /// <summary>
    /// A class used to cache the already-built command texts.
    /// </summary>
    internal static class CommandTextCache
    {
        private static readonly ConcurrentDictionary<BaseRequest, string> m_cache = new ConcurrentDictionary<BaseRequest, string>();

        #region GetBatchQueryText

        /// <summary>
        /// Gets a command text from the cache for the batch query operation.
        /// </summary>
        /// <param name="request">The request object.</param>
        /// <returns>The cached command text.</returns>
        public static string GetBatchQueryText(BatchQueryRequest request)
        {
            var commandText = (string)null;
            if (m_cache.TryGetValue(request, out commandText) == false)
            {
                var statementBuilder = EnsureStatementBuilder(request.Connection, request.StatementBuilder);
                var fields = GetActualFields(request.Connection, request.Name, request.Fields);
                commandText = statementBuilder.CreateBatchQuery(new QueryBuilder(),
                    request.Name,
                    fields,
                    request.Page,
                    request.RowsPerBatch,
                    request.OrderBy,
                    request.Where);
                m_cache.TryAdd(request, commandText);
            }
            return commandText;
        }

        #endregion

        #region GetCountText

        /// <summary>
        /// Gets a command text from the cache for the count operation.
        /// </summary>
        /// <param name="request">The request object.</param>
        /// <returns>The cached command text.</returns>
        public static string GetCountText(CountRequest request)
        {
            var commandText = (string)null;
            if (m_cache.TryGetValue(request, out commandText) == false)
            {
                var statementBuilder = EnsureStatementBuilder(request.Connection, request.StatementBuilder);
                commandText = statementBuilder.CreateCount(new QueryBuilder(),
                    request.Name,
                    request.Where);
                m_cache.TryAdd(request, commandText);
            }
            return commandText;
        }

        #endregion

        #region GetCountAllText

        /// <summary>
        /// Gets a command text from the cache for the count-all operation.
        /// </summary>
        /// <param name="request">The request object.</param>
        /// <returns>The cached command text.</returns>
        public static string GetCountAllText(CountAllRequest request)
        {
            var commandText = (string)null;
            if (m_cache.TryGetValue(request, out commandText) == false)
            {
                var statementBuilder = EnsureStatementBuilder(request.Connection, request.StatementBuilder);
                commandText = statementBuilder.CreateCountAll(new QueryBuilder(),
                    request.Name);
                m_cache.TryAdd(request, commandText);
            }
            return commandText;
        }

        #endregion

        #region GetDeleteText

        /// <summary>
        /// Gets a command text from the cache for the delete operation.
        /// </summary>
        /// <param name="request">The request object.</param>
        /// <returns>The cached command text.</returns>
        public static string GetDeleteText(DeleteRequest request)
        {
            var commandText = (string)null;
            if (m_cache.TryGetValue(request, out commandText) == false)
            {
                var statementBuilder = EnsureStatementBuilder(request.Connection, request.StatementBuilder);
                commandText = statementBuilder.CreateDelete(new QueryBuilder(),
                    request.Name,
                    request.Where);
                m_cache.TryAdd(request, commandText);
            }
            return commandText;
        }

        #endregion

        #region GetDeleteAllText

        /// <summary>
        /// Gets a command text from the cache for the delete-all operation.
        /// </summary>
        /// <param name="request">The request object.</param>
        /// <returns>The cached command text.</returns>
        public static string GetDeleteAllText(DeleteAllRequest request)
        {
            var commandText = (string)null;
            if (m_cache.TryGetValue(request, out commandText) == false)
            {
                var statementBuilder = EnsureStatementBuilder(request.Connection, request.StatementBuilder);
                commandText = statementBuilder.CreateDeleteAll(new QueryBuilder(),
                    request.Name);
                m_cache.TryAdd(request, commandText);
            }
            return commandText;
        }

        #endregion

        #region GetInsertText

        /// <summary>
        /// Gets a command text from the cache for the insert operation.
        /// </summary>
        /// <param name="request">The request object.</param>
        /// <returns>The cached command text.</returns>
        public static string GetInsertText(InsertRequest request)
        {
            var commandText = (string)null;
            if (m_cache.TryGetValue(request, out commandText) == false)
            {
                var statementBuilder = EnsureStatementBuilder(request.Connection, request.StatementBuilder);
                var fields = GetActualFields(request.Connection, request.Name, request.Fields);
                commandText = statementBuilder.CreateInsert(new QueryBuilder(),
                    request.Name,
                    fields,
                    GetPrimaryField(request));
                m_cache.TryAdd(request, commandText);
            }
            return commandText;
        }

        #endregion

        #region GetMergeText

        /// <summary>
        /// Gets a command text from the cache for the merge operation.
        /// </summary>
        /// <param name="request">The request object.</param>
        /// <returns>The cached command text.</returns>
        public static string GetMergeText(MergeRequest request)
        {
            var commandText = (string)null;
            if (m_cache.TryGetValue(request, out commandText) == false)
            {
                var statementBuilder = EnsureStatementBuilder(request.Connection, request.StatementBuilder);
                var fields = GetActualFields(request.Connection, request.Name, request.Fields);
                commandText = statementBuilder.CreateMerge(new QueryBuilder(),
                    request.Name,
                    fields,
                    request.Qualifiers,
                    GetPrimaryField(request));
                m_cache.TryAdd(request, commandText);
            }
            return commandText;
        }

        #endregion

        #region GetQueryText

        /// <summary>
        /// Gets a command text from the cache for the query operation.
        /// </summary>
        /// <typeparam name="TEntity">The type of the target entity.</typeparam>
        /// <param name="request">The request object.</param>
        /// <returns>The cached command text.</returns>
        public static string GetQueryText<TEntity>(QueryRequest request)
            where TEntity : class
        {
            var commandText = (string)null;
            if (m_cache.TryGetValue(request, out commandText) == false)
            {
                var statementBuilder = EnsureStatementBuilder(request.Connection, request.StatementBuilder);
                var fields = GetActualFields(request.Connection, request.Name, request.Fields);
                commandText = statementBuilder.CreateQuery(new QueryBuilder(),
                    request.Name,
                    fields,
                    request.Where,
                    request.OrderBy,
                    request.Top,
                    request.Hints);
                m_cache.TryAdd(request, commandText);
            }
            return commandText;
        }

        #endregion

        #region GetQueryAllText

        /// <summary>
        /// Gets a command text from the cache for the query-all operation.
        /// </summary>
        /// <typeparam name="TEntity">The type of the target entity.</typeparam>
        /// <param name="request">The request object.</param>
        /// <returns>The cached command text.</returns>
        public static string GetQueryAllText<TEntity>(QueryAllRequest request)
            where TEntity : class
        {
            var commandText = (string)null;
            if (m_cache.TryGetValue(request, out commandText) == false)
            {
                var statementBuilder = EnsureStatementBuilder(request.Connection, request.StatementBuilder);
                var fields = GetActualFields(request.Connection, request.Name, request.Fields);
                commandText = statementBuilder.CreateQueryAll(new QueryBuilder(),
                    request.Name,
                    fields,
                    request.OrderBy,
                    request.Hints);
                m_cache.TryAdd(request, commandText);
            }
            return commandText;
        }

        #endregion

        #region GetQueryMultipleText

        /// <summary>
        /// Gets a command text from the cache for the query-multiple operation.
        /// </summary>
        /// <typeparam name="TEntity">The type of the target entity.</typeparam>
        /// <param name="request">The request object.</param>
        /// <returns>The cached command text.</returns>
        public static string GetQueryMultipleText<TEntity>(QueryMultipleRequest request)
            where TEntity : class
        {
            var commandText = (string)null;
            if (m_cache.TryGetValue(request, out commandText) == false)
            {
                var statementBuilder = EnsureStatementBuilder(request.Connection, request.StatementBuilder);
                var fields = GetActualFields(request.Connection, request.Name, request.Fields);
                commandText = statementBuilder.CreateQuery(new QueryBuilder(),
                    request.Name,
                    fields,
                    request.Where,
                    request.OrderBy,
                    request.Top,
                    request.Hints);
                m_cache.TryAdd(request, commandText);
            }
            return commandText;
        }

        #endregion

        #region GetTruncateText

        /// <summary>
        /// Gets a command text from the cache for the truncate operation.
        /// </summary>
        /// <param name="request">The request object.</param>
        /// <returns>The cached command text.</returns>
        public static string GetTruncateText(TruncateRequest request)
        {
            var commandText = (string)null;
            if (m_cache.TryGetValue(request, out commandText) == false)
            {
                var statementBuilder = EnsureStatementBuilder(request.Connection, request.StatementBuilder);
                commandText = statementBuilder.CreateTruncate(new QueryBuilder(),
                    request.Name);
                m_cache.TryAdd(request, commandText);
            }
            return commandText;
        }

        #endregion

        #region GetUpdateText

        /// <summary>
        /// Gets a command text from the cache for the update operation.
        /// </summary>
        /// <param name="request">The request object.</param>
        /// <returns>The cached command text.</returns>
        public static string GetUpdateText(UpdateRequest request)
        {
            var commandText = (string)null;
            if (m_cache.TryGetValue(request, out commandText) == false)
            {
                var statementBuilder = EnsureStatementBuilder(request.Connection, request.StatementBuilder);
                var fields = GetActualFields(request.Connection, request.Name, request.Fields);
                commandText = statementBuilder.CreateUpdate(new QueryBuilder(),
                    request.Name,
                    fields,
                    request.Where,
                    GetPrimaryField(request));
                m_cache.TryAdd(request, commandText);
            }
            return commandText;
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Gets the list of <see cref="Field"/> objects based on the actual list of <see cref="DbField"/> objects.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="fields">The target name of the table.</param>
        /// <param name="tableName">The list of fields from the data entity object.</param>
        /// <returns>The list of actual <see cref="Field"/> objects of the table.</returns>
        private static IEnumerable<Field> GetActualFields(IDbConnection connection, string tableName, IEnumerable<Field> fields)
        {
            if (fields?.Any() == false)
            {
                return null;
            }

            // Get all the fields from the database
            var dbFields = DbFieldCache.Get(connection, tableName);

            // Return the filtered one
            return dbFields?.Any() == true ?
                fields.Where(f => dbFields.FirstOrDefault(df => df.UnquotedName.ToLower() == f.UnquotedName.ToLower()) != null) :
                fields;
        }

        /// <summary>
        /// Gets the primary <see cref="DbField"/> object.
        /// </summary>
        /// <param name="request">The request object.</param>
        /// <returns>The primary <see cref="DbField"/> object.</returns>
        private static DbField GetPrimaryField(BaseRequest request)
        {
            if (request.Type != null)
            {
                var primaryPropery = PrimaryCache.Get(request.Type);
                var isIdentity = false;
                if (primaryPropery != null)
                {
                    isIdentity = IdentityCache.Get(request.Type) != null;
                    if (isIdentity == false)
                    {
                        isIdentity = DbFieldCache
                            .Get(request.Connection, request.Name)?
                            .FirstOrDefault(f => f.IsPrimary)?
                            .IsIdentity == true;
                    }
                    return new DbField(primaryPropery.GetUnquotedMappedName(), true, isIdentity, false);
                }
            }
            return DbFieldCache.Get(request.Connection, request.Name)?.FirstOrDefault(f => f.IsPrimary);
        }

        /// <summary>
        /// Throws an exception of the builder is not defined.
        /// </summary>
        /// <param name="connection">The connection object to identified.</param>
        /// <param name="builder">The builder to be checked.</param>
        /// <returns>The instance of available statement builder.</returns>
        private static IStatementBuilder EnsureStatementBuilder(IDbConnection connection, IStatementBuilder builder)
        {
            builder = builder ?? StatementBuilderMapper.Get(connection.GetType());
            if (builder == null)
            {
                throw new InvalidOperationException($"There is no '{nameof(IStatementBuilder)}' object defined. Please visit your mappings and make sure to map a type of '{nameof(DbConnection)}' to a correct '{nameof(IStatementBuilder)}' object.");
            }
            return builder;
        }

        #endregion
    }
}
