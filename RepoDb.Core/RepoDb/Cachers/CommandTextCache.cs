using RepoDb.Exceptions;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.Requests;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RepoDb
{
    /// <summary>
    /// A class that is being used to cache the already-built command texts.
    /// </summary>
    public static class CommandTextCache
    {
        private static readonly ConcurrentDictionary<BaseRequest, string> cache = new();

        #region GetAverageText

        /// <summary>
        ///
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        internal static string GetAverageText(AverageRequest request)
        {
            if (cache.TryGetValue(request, out var commandText) == false)
            {
                var statementBuilder = EnsureStatementBuilder(request.Connection, request.StatementBuilder);
                commandText = statementBuilder.CreateAverage(request.Name,
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
        ///
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        internal static string GetAverageAllText(AverageAllRequest request)
        {
            if (cache.TryGetValue(request, out var commandText) == false)
            {
                var statementBuilder = EnsureStatementBuilder(request.Connection, request.StatementBuilder);
                commandText = statementBuilder.CreateAverageAll(request.Name,
                    request.Field,
                    request.Hints);
                cache.TryAdd(request, commandText);
            }
            return commandText;
        }

        #endregion

        #region GetBatchQueryText

        /// <summary>
        ///
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        internal static string GetBatchQueryText(BatchQueryRequest request)
        {
            if (cache.TryGetValue(request, out var commandText) == false)
            {
                var fields = GetTargetFields(request.Connection,
                    request.Name,
                    request.Fields,
                    request.Transaction);
                ValidateOrderFields(request.Connection,
                    request.Name,
                    request.OrderBy,
                    request.Transaction);
                commandText = GetBatchQueryTextInternal(request, fields);
                cache.TryAdd(request, commandText);
            }
            return commandText;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        internal static async Task<string> GetBatchQueryTextAsync(BatchQueryRequest request,
            CancellationToken cancellationToken = default)
        {
            if (cache.TryGetValue(request, out var commandText) == false)
            {
                var fields = await GetTargetFieldsAsync(request.Connection,
                    request.Name,
                    request.Fields,
                    request.Transaction,
                    cancellationToken);
                await ValidateOrderFieldsAsync(request.Connection,
                    request.Name,
                    request.OrderBy,
                    request.Transaction,
                    cancellationToken);
                commandText = GetBatchQueryTextInternal(request, fields);
                cache.TryAdd(request, commandText);
            }
            return commandText;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="request"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        internal static string GetBatchQueryTextInternal(BatchQueryRequest request,
            IEnumerable<Field> fields)
        {
            var statementBuilder = EnsureStatementBuilder(request.Connection, request.StatementBuilder);
            return statementBuilder.CreateBatchQuery(request.Name,
                fields,
                request.Page,
                request.RowsPerBatch,
                request.OrderBy,
                request.Where,
                request.Hints);
        }

        #endregion

        #region GetCountText

        /// <summary>
        ///
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        internal static string GetCountText(CountRequest request)
        {
            if (cache.TryGetValue(request, out var commandText) == false)
            {
                var statementBuilder = EnsureStatementBuilder(request.Connection, request.StatementBuilder);
                commandText = statementBuilder.CreateCount(request.Name,
                    request.Where,
                    request.Hints);
                cache.TryAdd(request, commandText);
            }
            return commandText;
        }

        #endregion

        #region GetCountAllText

        /// <summary>
        ///
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        internal static string GetCountAllText(CountAllRequest request)
        {
            if (cache.TryGetValue(request, out var commandText) == false)
            {
                var statementBuilder = EnsureStatementBuilder(request.Connection, request.StatementBuilder);
                commandText = statementBuilder.CreateCountAll(request.Name,
                    request.Hints);
                cache.TryAdd(request, commandText);
            }
            return commandText;
        }

        #endregion

        #region GetDeleteText

        /// <summary>
        ///
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        internal static string GetDeleteText(DeleteRequest request)
        {
            if (cache.TryGetValue(request, out var commandText) == false)
            {
                var statementBuilder = EnsureStatementBuilder(request.Connection, request.StatementBuilder);
                commandText = statementBuilder.CreateDelete(request.Name,
                    request.Where,
                    request.Hints);
                cache.TryAdd(request, commandText);
            }
            return commandText;
        }

        #endregion

        #region GetDeleteAllText

        /// <summary>
        ///
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        internal static string GetDeleteAllText(DeleteAllRequest request)
        {
            if (cache.TryGetValue(request, out var commandText) == false)
            {
                var statementBuilder = EnsureStatementBuilder(request.Connection, request.StatementBuilder);
                commandText = statementBuilder.CreateDeleteAll(request.Name,
                    request.Hints);
                cache.TryAdd(request, commandText);
            }
            return commandText;
        }

        #endregion

        #region GetExistsText

        /// <summary>
        ///
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        internal static string GetExistsText(ExistsRequest request)
        {
            if (cache.TryGetValue(request, out var commandText) == false)
            {
                var statementBuilder = EnsureStatementBuilder(request.Connection, request.StatementBuilder);
                commandText = statementBuilder.CreateExists(request.Name,
                    request.Where,
                    request.Hints);
                cache.TryAdd(request, commandText);
            }
            return commandText;
        }

        #endregion

        #region GetInsertText

        /// <summary>
        ///
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        internal static string GetInsertText(InsertRequest request)
        {
            if (cache.TryGetValue(request, out var commandText) == false)
            {
                var fields = GetTargetFields(request.Connection,
                    request.Name,
                    request.Fields,
                    request.Transaction);
                var primaryField = GetPrimaryField(request);
                var identityField = GetIdentityField(request);
                commandText = GetInsertTextInternal(request, fields, primaryField, identityField);
                cache.TryAdd(request, commandText);
            }
            return commandText;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        internal static async Task<string> GetInsertTextAsync(InsertRequest request,
            CancellationToken cancellationToken = default)
        {
            if (cache.TryGetValue(request, out var commandText) == false)
            {
                var fields = await GetTargetFieldsAsync(request.Connection,
                    request.Name,
                    request.Fields,
                    request.Transaction,
                    cancellationToken);
                var primaryField = await GetPrimaryFieldAsync(request, cancellationToken);
                var identityField = await GetIdentityFieldAsync(request, cancellationToken);
                commandText = GetInsertTextInternal(request, fields, primaryField, identityField);
                cache.TryAdd(request, commandText);
            }
            return commandText;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="fields"></param>
        /// <param name="primaryField"></param>
        /// <param name="identityField"></param>
        /// <returns></returns>
        private static string GetInsertTextInternal(InsertRequest request,
            IEnumerable<Field> fields,
            DbField primaryField,
            DbField identityField)
        {
            var statementBuilder = EnsureStatementBuilder(request.Connection, request.StatementBuilder);
            return statementBuilder.CreateInsert(request.Name,
                fields,
                primaryField,
                identityField,
                request.Hints);
        }

        #endregion

        #region GetInsertAllText

        /// <summary>
        ///
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        internal static string GetInsertAllText(InsertAllRequest request)
        {
            if (cache.TryGetValue(request, out var commandText) == false)
            {
                var fields = GetTargetFields(request.Connection,
                    request.Name,
                    request.Fields,
                    request.Transaction);
                var primaryField = GetPrimaryField(request);
                var identityField = GetIdentityField(request);
                commandText = GetInsertAllTextInternal(request, fields, primaryField, identityField);
                cache.TryAdd(request, commandText);
            }
            return commandText;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        internal static async Task<string> GetInsertAllTextAsync(InsertAllRequest request,
            CancellationToken cancellationToken = default)
        {
            if (cache.TryGetValue(request, out var commandText) == false)
            {
                var fields = await GetTargetFieldsAsync(request.Connection,
                    request.Name,
                    request.Fields,
                    request.Transaction,
                    cancellationToken);
                var primaryField = await GetPrimaryFieldAsync(request, cancellationToken);
                var identityField = await GetIdentityFieldAsync(request, cancellationToken);
                commandText = GetInsertAllTextInternal(request, fields, primaryField, identityField);
                cache.TryAdd(request, commandText);
            }
            return commandText;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="fields"></param>
        /// <param name="primaryField"></param>
        /// <param name="identityField"></param>
        /// <returns></returns>
        private static string GetInsertAllTextInternal(InsertAllRequest request,
            IEnumerable<Field> fields,
            DbField primaryField,
            DbField identityField)
        {
            var statementBuilder = EnsureStatementBuilder(request.Connection, request.StatementBuilder);
            return statementBuilder.CreateInsertAll(request.Name,
                fields,
                request.BatchSize,
                primaryField,
                identityField,
                request.Hints);
        }

        #endregion

        #region GetMaxText

        /// <summary>
        ///
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        internal static string GetMaxText(MaxRequest request)
        {
            if (cache.TryGetValue(request, out var commandText) == false)
            {
                var statementBuilder = EnsureStatementBuilder(request.Connection, request.StatementBuilder);
                commandText = statementBuilder.CreateMax(request.Name,
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
        ///
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        internal static string GetMaxAllText(MaxAllRequest request)
        {
            if (cache.TryGetValue(request, out var commandText) == false)
            {
                var statementBuilder = EnsureStatementBuilder(request.Connection, request.StatementBuilder);
                commandText = statementBuilder.CreateMaxAll(request.Name,
                    request.Field,
                    request.Hints);
                cache.TryAdd(request, commandText);
            }
            return commandText;
        }

        #endregion

        #region GetMergeText

        /// <summary>
        ///
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        internal static string GetMergeText(MergeRequest request)
        {
            if (cache.TryGetValue(request, out var commandText) == false)
            {
                var fields = GetTargetFields(request.Connection,
                    request.Name,
                    request.Fields,
                    request.Transaction);
                var primaryField = GetPrimaryField(request);
                var identityField = GetIdentityField(request);
                commandText = GetMergeTextInternal(request, fields, primaryField, identityField);
                cache.TryAdd(request, commandText);
            }
            return commandText;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        internal static async Task<string> GetMergeTextAsync(MergeRequest request,
            CancellationToken cancellationToken = default)
        {
            if (cache.TryGetValue(request, out var commandText) == false)
            {
                var fields = await GetTargetFieldsAsync(request.Connection,
                    request.Name,
                    request.Fields,
                    request.Transaction,
                    cancellationToken);
                var primaryField = await GetPrimaryFieldAsync(request, cancellationToken);
                var identityField = await GetIdentityFieldAsync(request, cancellationToken);
                commandText = GetMergeTextInternal(request, fields, primaryField, identityField);
                cache.TryAdd(request, commandText);
            }
            return commandText;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="fields"></param>
        /// <param name="primaryField"></param>
        /// <param name="identityField"></param>
        /// <returns></returns>
        private static string GetMergeTextInternal(MergeRequest request,
            IEnumerable<Field> fields,
            DbField primaryField,
            DbField identityField)
        {
            var statementBuilder = EnsureStatementBuilder(request.Connection, request.StatementBuilder);
            return statementBuilder.CreateMerge(request.Name,
                fields,
                request.Qualifiers,
                primaryField,
                identityField,
                request.Hints);
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
            if (cache.TryGetValue(request, out var commandText) == false)
            {
                var fields = GetTargetFields(request.Connection,
                    request.Name,
                    request.Fields,
                    request.Transaction);
                var primaryField = GetPrimaryField(request);
                var identityField = GetIdentityField(request);
                commandText = GetMergeAllTextInternal(request, fields, primaryField, identityField);
                cache.TryAdd(request, commandText);
            }
            return commandText;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        internal static async Task<string> GetMergeAllTextAsync(MergeAllRequest request,
            CancellationToken cancellationToken = default)
        {
            if (cache.TryGetValue(request, out var commandText) == false)
            {
                var fields = await GetTargetFieldsAsync(request.Connection,
                    request.Name,
                    request.Fields,
                    request.Transaction,
                    cancellationToken);
                var primaryField = await GetPrimaryFieldAsync(request, cancellationToken);
                var identityField = await GetIdentityFieldAsync(request, cancellationToken);
                commandText = GetMergeAllTextInternal(request, fields, primaryField, identityField);
                cache.TryAdd(request, commandText);
            }
            return commandText;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="fields"></param>
        /// <param name="primaryField"></param>
        /// <param name="identityField"></param>
        /// <returns></returns>
        private static string GetMergeAllTextInternal(MergeAllRequest request,
            IEnumerable<Field> fields,
            DbField primaryField,
            DbField identityField)
        {
            var statementBuilder = EnsureStatementBuilder(request.Connection, request.StatementBuilder);
            return statementBuilder.CreateMergeAll(request.Name,
                fields,
                request.Qualifiers,
                request.BatchSize,
                primaryField,
                identityField,
                request.Hints);
        }

        #endregion

        #region GetMinText

        /// <summary>
        ///
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        internal static string GetMinText(MinRequest request)
        {
            if (cache.TryGetValue(request, out var commandText) == false)
            {
                var statementBuilder = EnsureStatementBuilder(request.Connection, request.StatementBuilder);
                commandText = statementBuilder.CreateMin(request.Name,
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
        ///
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        internal static string GetMinAllText(MinAllRequest request)
        {
            if (cache.TryGetValue(request, out var commandText) == false)
            {
                var statementBuilder = EnsureStatementBuilder(request.Connection, request.StatementBuilder);
                commandText = statementBuilder.CreateMinAll(request.Name,
                    request.Field,
                    request.Hints);
                cache.TryAdd(request, commandText);
            }
            return commandText;
        }

        #endregion

        #region GetQueryText

        /// <summary>
        ///
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        internal static string GetQueryText(QueryRequest request)
        {
            if (cache.TryGetValue(request, out var commandText) == false)
            {
                var fields = GetTargetFields(request.Connection,
                    request.Name,
                    request.Fields,
                    request.Transaction);
                ValidateOrderFields(request.Connection,
                    request.Name,
                    request.OrderBy,
                    request.Transaction);
                commandText = GetQueryTextInternal(request, fields);
                cache.TryAdd(request, commandText);
            }
            return commandText;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        internal static async Task<string> GetQueryTextAsync(QueryRequest request,
            CancellationToken cancellationToken = default)
        {
            if (cache.TryGetValue(request, out var commandText) == false)
            {
                var fields = await GetTargetFieldsAsync(request.Connection,
                    request.Name,
                    request.Fields,
                    request.Transaction,
                    cancellationToken);
                await ValidateOrderFieldsAsync(request.Connection,
                    request.Name,
                    request.OrderBy,
                    request.Transaction,
                    cancellationToken);
                commandText = GetQueryTextInternal(request, fields);
                cache.TryAdd(request, commandText);
            }
            return commandText;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="request"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        private static string GetQueryTextInternal(QueryRequest request,
            IEnumerable<Field> fields)
        {
            var statementBuilder = EnsureStatementBuilder(request.Connection, request.StatementBuilder);
            return statementBuilder.CreateQuery(request.Name,
                fields,
                request.Where,
                request.OrderBy,
                request.Top,
                request.Hints);
        }

        #endregion

        #region GetQueryAllText

        /// <summary>
        ///
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        internal static string GetQueryAllText(QueryAllRequest request)
        {
            if (cache.TryGetValue(request, out var commandText) == false)
            {
                var fields = GetTargetFields(request.Connection,
                    request.Name,
                    request.Fields,
                    request.Transaction);
                ValidateOrderFields(request.Connection,
                    request.Name,
                    request.OrderBy,
                    request.Transaction);
                commandText = GetQueryAllTextInternal(request, fields);
                cache.TryAdd(request, commandText);
            }
            return commandText;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        internal static async Task<string> GetQueryAllTextAsync(QueryAllRequest request,
            CancellationToken cancellationToken = default)
        {
            if (cache.TryGetValue(request, out var commandText) == false)
            {
                var fields = await GetTargetFieldsAsync(request.Connection,
                    request.Name,
                    request.Fields,
                    request.Transaction,
                    cancellationToken);
                await ValidateOrderFieldsAsync(request.Connection,
                    request.Name,
                    request.OrderBy,
                    request.Transaction,
                    cancellationToken);
                commandText = GetQueryAllTextInternal(request, fields);
                cache.TryAdd(request, commandText);
            }
            return commandText;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="request"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        private static string GetQueryAllTextInternal(QueryAllRequest request,
            IEnumerable<Field> fields)
        {
            var statementBuilder = EnsureStatementBuilder(request.Connection, request.StatementBuilder);
            return statementBuilder.CreateQueryAll(request.Name,
                fields,
                request.OrderBy,
                request.Hints);
        }

        #endregion

        #region GetQueryMultipleText

        /// <summary>
        ///
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        internal static string GetQueryMultipleText(QueryMultipleRequest request)
        {
            if (cache.TryGetValue(request, out var commandText) == false)
            {
                var fields = GetTargetFields(request.Connection,
                    request.Name,
                    request.Fields,
                    request.Transaction);
                ValidateOrderFields(request.Connection,
                    request.Name,
                    request.OrderBy,
                    request.Transaction);
                commandText = GetQueryMultipleTextInternal(request, fields);
                cache.TryAdd(request, commandText);
            }
            return commandText;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        internal static async Task<string> GetQueryMultipleTextAsync(QueryMultipleRequest request,
            CancellationToken cancellationToken = default)
        {
            if (cache.TryGetValue(request, out var commandText) == false)
            {
                var fields = await GetTargetFieldsAsync(request.Connection,
                    request.Name,
                    request.Fields,
                    request.Transaction,
                    cancellationToken);
                await ValidateOrderFieldsAsync(request.Connection,
                    request.Name,
                    request.OrderBy,
                    request.Transaction,
                    cancellationToken);
                commandText = GetQueryMultipleTextInternal(request, fields);
                cache.TryAdd(request, commandText);
            }
            return commandText;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="request"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        private static string GetQueryMultipleTextInternal(QueryMultipleRequest request,
            IEnumerable<Field> fields)
        {
            var statementBuilder = EnsureStatementBuilder(request.Connection, request.StatementBuilder);
            return statementBuilder.CreateQuery(request.Name,
                fields,
                request.Where,
                request.OrderBy,
                request.Top,
                request.Hints);
        }

        #endregion

        #region GetSkipQueryText

        /// <summary>
        ///
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        internal static string GetSkipQueryText(SkipQueryRequest request)
        {
            if (cache.TryGetValue(request, out var commandText) == false)
            {
                var fields = GetTargetFields(request.Connection,
                    request.Name,
                    request.Fields,
                    request.Transaction);
                ValidateOrderFields(request.Connection,
                    request.Name,
                    request.OrderBy,
                    request.Transaction);
                commandText = GetSkipQueryTextInternal(request, fields);
                cache.TryAdd(request, commandText);
            }
            return commandText;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        internal static async Task<string> GetSkipQueryTextAsync(SkipQueryRequest request,
            CancellationToken cancellationToken = default)
        {
            if (cache.TryGetValue(request, out var commandText) == false)
            {
                var fields = await GetTargetFieldsAsync(request.Connection,
                    request.Name,
                    request.Fields,
                    request.Transaction,
                    cancellationToken);
                await ValidateOrderFieldsAsync(request.Connection,
                    request.Name,
                    request.OrderBy,
                    request.Transaction,
                    cancellationToken);
                commandText = GetSkipQueryTextInternal(request, fields);
                cache.TryAdd(request, commandText);
            }
            return commandText;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="request"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        internal static string GetSkipQueryTextInternal(SkipQueryRequest request,
            IEnumerable<Field> fields)
        {
            var statementBuilder = EnsureStatementBuilder(request.Connection, request.StatementBuilder);
            return statementBuilder.CreateSkipQuery(request.Name,
                fields,
                request.Skip,
                request.RowsPerBatch,
                request.OrderBy,
                request.Where,
                request.Hints);
        }

        #endregion


        #region GetSumText

        /// <summary>
        ///
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        internal static string GetSumText(SumRequest request)
        {
            if (cache.TryGetValue(request, out var commandText) == false)
            {
                var statementBuilder = EnsureStatementBuilder(request.Connection, request.StatementBuilder);
                commandText = statementBuilder.CreateSum(request.Name,
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
        ///
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        internal static string GetSumAllText(SumAllRequest request)
        {
            if (cache.TryGetValue(request, out var commandText) == false)
            {
                var statementBuilder = EnsureStatementBuilder(request.Connection, request.StatementBuilder);
                commandText = statementBuilder.CreateSumAll(request.Name,
                    request.Field,
                    request.Hints);
                cache.TryAdd(request, commandText);
            }
            return commandText;
        }

        #endregion

        #region GetTruncateText

        /// <summary>
        ///
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        internal static string GetTruncateText(TruncateRequest request)
        {
            if (cache.TryGetValue(request, out var commandText) == false)
            {
                var statementBuilder = EnsureStatementBuilder(request.Connection, request.StatementBuilder);
                commandText = statementBuilder.CreateTruncate(request.Name);
                cache.TryAdd(request, commandText);
            }
            return commandText;
        }

        #endregion

        #region GetUpdateText

        /// <summary>
        ///
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        internal static string GetUpdateText(UpdateRequest request)
        {
            if (cache.TryGetValue(request, out var commandText) == false)
            {
                var fields = GetTargetFields(request.Connection,
                    request.Name,
                    request.Fields,
                    request.Transaction);
                var primaryField = GetPrimaryField(request);
                var identityField = GetIdentityField(request);
                commandText = GetUpdateTextInternal(request, fields, primaryField, identityField);
                cache.TryAdd(request, commandText);
            }
            return commandText;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        internal static async Task<string> GetUpdateTextAsync(UpdateRequest request,
            CancellationToken cancellationToken = default)
        {
            if (cache.TryGetValue(request, out var commandText) == false)
            {
                var fields = await GetTargetFieldsAsync(request.Connection,
                    request.Name,
                    request.Fields,
                    request.Transaction,
                    cancellationToken);
                var primaryField = await GetPrimaryFieldAsync(request, cancellationToken);
                var identityField = await GetIdentityFieldAsync(request, cancellationToken);
                commandText = GetUpdateTextInternal(request, fields, primaryField, identityField);
                cache.TryAdd(request, commandText);
            }
            return commandText;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="fields"></param>
        /// <param name="primaryField"></param>
        /// <param name="identityField"></param>
        /// <returns></returns>
        private static string GetUpdateTextInternal(UpdateRequest request,
            IEnumerable<Field> fields,
            DbField primaryField,
            DbField identityField)
        {
            var statementBuilder = EnsureStatementBuilder(request.Connection, request.StatementBuilder);
            return statementBuilder.CreateUpdate(request.Name,
                fields,
                request.Where,
                primaryField,
                identityField,
                request.Hints);
        }

        #endregion

        #region GetUpdateAllText

        /// <summary>
        ///
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        internal static string GetUpdateAllText(UpdateAllRequest request)
        {
            if (cache.TryGetValue(request, out var commandText) == false)
            {
                var fields = GetTargetFields(request.Connection,
                    request.Name,
                    request.Fields,
                    request.Transaction);
                var primaryField = GetPrimaryField(request);
                var identityField = GetIdentityField(request);
                commandText = GetUpdateAllTextInternal(request, fields, primaryField, identityField);
                cache.TryAdd(request, commandText);
            }
            return commandText;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        internal static async Task<string> GetUpdateAllTextAsync(UpdateAllRequest request,
            CancellationToken cancellationToken = default)
        {
            if (cache.TryGetValue(request, out var commandText) == false)
            {
                var fields = await GetTargetFieldsAsync(request.Connection,
                    request.Name,
                    request.Fields,
                    request.Transaction,
                    cancellationToken);
                var primaryField = await GetPrimaryFieldAsync(request, cancellationToken);
                var identityField = await GetIdentityFieldAsync(request, cancellationToken);
                commandText = GetUpdateAllTextInternal(request, fields, primaryField, identityField);
                cache.TryAdd(request, commandText);
            }
            return commandText;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="fields"></param>
        /// <param name="primaryField"></param>
        /// <param name="identityField"></param>
        /// <returns></returns>
        private static string GetUpdateAllTextInternal(UpdateAllRequest request,
            IEnumerable<Field> fields,
            DbField primaryField,
            DbField identityField)
        {
            var statementBuilder = EnsureStatementBuilder(request.Connection, request.StatementBuilder);
            return statementBuilder.CreateUpdateAll(request.Name,
                fields,
                request.Qualifiers,
                request.BatchSize,
                primaryField,
                identityField,
                request.Hints);
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Flushes all the existing cached command texts.
        /// </summary>
        public static void Flush() =>
            cache.Clear();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="orderFields"></param>
        /// <param name="transaction"></param>
        private static void ValidateOrderFields(IDbConnection connection,
            string tableName,
            IEnumerable<OrderField> orderFields,
            IDbTransaction transaction)
        {
            if (orderFields?.Any() == true)
            {
                var dbFields = DbFieldCache.Get(connection, tableName, transaction);
                ValidateOrderFieldsInternal(orderFields, dbFields, connection.GetDbSetting());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="orderFields"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private static async Task ValidateOrderFieldsAsync(IDbConnection connection,
            string tableName,
            IEnumerable<OrderField> orderFields,
            IDbTransaction transaction,
            CancellationToken cancellationToken = default)
        {
            if (orderFields?.Any() == true)
            {
                var dbFields = await DbFieldCache.GetAsync(connection, tableName, transaction, cancellationToken);
                ValidateOrderFieldsInternal(orderFields, dbFields, connection.GetDbSetting());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="orderFields"></param>
        /// <param name="dbFields"></param>
        /// <param name="dbSetting"></param>
        private static void ValidateOrderFieldsInternal(IEnumerable<OrderField> orderFields,
            DbFieldCollection dbFields,
            IDbSetting dbSetting)
        {
            var unmatchesOrderFields = dbFields?.IsEmpty() == false ?
                orderFields
                    .Where(of =>
                        dbFields.GetByUnquotedName(of.Name.AsUnquoted(true, dbSetting)) == null) : null;
            if (unmatchesOrderFields?.Any() == true)
            {
                throw new MissingFieldsException($"The order fields '{unmatchesOrderFields.Select(of => of.Name).Join(", ")}' are not present from the actual table.");
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="fields"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private static IEnumerable<Field> GetTargetFields(IDbConnection connection,
            string tableName,
            IEnumerable<Field> fields,
            IDbTransaction transaction)
        {
            if (fields?.Any() != true)
            {
                return null;
            }
            var dbFields = DbFieldCache.Get(connection, tableName, transaction);
            return GetTargetFieldsInternal(fields, dbFields, connection.GetDbSetting());
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="fields"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private static async Task<IEnumerable<Field>> GetTargetFieldsAsync(IDbConnection connection,
            string tableName,
            IEnumerable<Field> fields,
            IDbTransaction transaction,
            CancellationToken cancellationToken = default)
        {
            if (fields?.Any() != true)
            {
                return null;
            }
            var dbFields = await DbFieldCache.GetAsync(connection, tableName, transaction, cancellationToken);
            return GetTargetFieldsInternal(fields, dbFields, connection.GetDbSetting());
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="fields"></param>
        /// <param name="dbFields"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        private static IEnumerable<Field> GetTargetFieldsInternal(IEnumerable<Field> fields,
            DbFieldCollection dbFields,
            IDbSetting dbSetting)
        {
            return dbFields?.IsEmpty() == false ?
                fields
                    .Where(f =>
                        dbFields.GetByUnquotedName(f.Name.AsUnquoted(true, dbSetting)) != null) :
                fields;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private static DbField GetPrimaryField(BaseRequest request)
        {
            var dbFields = DbFieldCache.Get(request.Connection, request.Name, request.Transaction);
            return GetPrimaryField(request, dbFields);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private static async Task<DbField> GetPrimaryFieldAsync(BaseRequest request,
            CancellationToken cancellationToken = default)
        {
            var dbFields = await DbFieldCache.GetAsync(request.Connection, request.Name, request.Transaction, cancellationToken);
            return GetPrimaryField(request, dbFields);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="dbFields"></param>
        /// <returns></returns>
        private static DbField GetPrimaryField(BaseRequest request,
            DbFieldCollection dbFields)
        {
            var primaryField = GetPrimaryField(request.Type, dbFields);

            if (primaryField != null)
            {
                var identityField = GetIdentityField(request.Type, dbFields);
                var isIdentity = identityField == primaryField ||
                    string.Equals(identityField?.Name, primaryField.Name, StringComparison.OrdinalIgnoreCase);

                return new DbField(primaryField.Name,
                    true,
                    isIdentity,
                    false,
                    primaryField.Type,
                    null,
                    null,
                    null,
                    null,
                    false);
            }

            return null;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private static DbField GetIdentityField(BaseRequest request)
        {
            var dbFields = DbFieldCache.Get(request.Connection, request.Name, request.Transaction);
            return GetIdentityField(request, dbFields);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private static async Task<DbField> GetIdentityFieldAsync(BaseRequest request,
            CancellationToken cancellationToken = default)
        {
            var dbFields = await DbFieldCache.GetAsync(request.Connection, request.Name, request.Transaction, cancellationToken);
            return GetIdentityField(request, dbFields);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="dbFields"></param>
        /// <returns></returns>
        private static DbField GetIdentityField(BaseRequest request,
            DbFieldCollection dbFields)
        {
            var identityField = GetIdentityField(request.Type, dbFields);

            if (identityField != null)
            {
                var primaryField = GetPrimaryField(request.Type, dbFields);
                var isPrimary = identityField == primaryField ||
                    string.Equals(primaryField?.Name, identityField.Name, StringComparison.OrdinalIgnoreCase);

                return new DbField(identityField.Name,
                    isPrimary,
                    true,
                    false,
                    identityField.Type,
                    null,
                    null,
                    null,
                    null,
                    false);
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="dbFields"></param>
        /// <returns></returns>
        private static Field GetPrimaryField(Type type,
            DbFieldCollection dbFields) =>
            (type != null && type.IsObjectType() == false ? PrimaryCache.Get(type) : null)?.AsField() ??
                dbFields?.GetPrimary()?.AsField();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="dbFields"></param>
        /// <returns></returns>
        private static Field GetIdentityField(Type type,
            DbFieldCollection dbFields) =>
            (type != null && type.IsObjectType() == false ? IdentityCache.Get(type) : null)?.AsField() ??
                dbFields?.GetIdentity()?.AsField();

        /// <summary>
        ///
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="builder"></param>
        /// <returns></returns>
        private static IStatementBuilder EnsureStatementBuilder(IDbConnection connection,
            IStatementBuilder builder) =>
            builder ?? connection.GetStatementBuilder();

        #endregion
    }
}
