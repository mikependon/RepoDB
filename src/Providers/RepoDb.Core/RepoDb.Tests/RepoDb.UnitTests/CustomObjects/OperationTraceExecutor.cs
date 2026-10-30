#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RepoDb.Enumerations;
using RepoDb.Interfaces;

namespace RepoDb.UnitTests.CustomObjects
{
    /// <summary>
    /// Executes each command operation through its public API for tracing tests.
    /// </summary>
    internal static class OperationTraceExecutor
    {
        #region Private Variables

        private static readonly IDictionary<string, Func<CustomTracedDbConnection, ITrace, bool, Task<object>>> _operations =
            new Dictionary<string, Func<CustomTracedDbConnection, ITrace, bool, Task<object>>>(StringComparer.Ordinal)
            {
                ["Average"] = ExecuteAverageAsync,
                ["AverageAll"] = ExecuteAverageAllAsync,
                ["BatchQuery"] = ExecuteBatchQueryAsync,
                ["Count"] = ExecuteCountAsync,
                ["CountAll"] = ExecuteCountAllAsync,
                ["Delete"] = ExecuteDeleteAsync,
                ["DeleteAll"] = ExecuteDeleteAllAsync,
                ["Exists"] = ExecuteExistsAsync,
                ["Insert"] = ExecuteInsertAsync,
                ["InsertAll"] = ExecuteInsertAllAsync,
                ["InsertAllBatch"] = ExecuteInsertAllBatchAsync,
                ["Max"] = ExecuteMaxAsync,
                ["MaxAll"] = ExecuteMaxAllAsync,
                ["Merge"] = ExecuteMergeAsync,
                ["MergeAll"] = ExecuteMergeAllAsync,
                ["MergeAllBatch"] = ExecuteMergeAllBatchAsync,
                ["Min"] = ExecuteMinAsync,
                ["MinAll"] = ExecuteMinAllAsync,
                ["Query"] = ExecuteQueryAsync,
                ["QueryAll"] = ExecuteQueryAllAsync,
                ["QueryFirst"] = ExecuteQueryFirstAsync,
                ["QueryMultiple"] = ExecuteQueryMultipleAsync,
                ["QuerySingle"] = ExecuteQuerySingleAsync,
                ["SkipQuery"] = ExecuteSkipQueryAsync,
                ["Sum"] = ExecuteSumAsync,
                ["SumAll"] = ExecuteSumAllAsync,
                ["Truncate"] = ExecuteTruncateAsync,
                ["Update"] = ExecuteUpdateAsync,
                ["UpdateAll"] = ExecuteUpdateAllAsync,
                ["UpdateAllBatch"] = ExecuteUpdateAllBatchAsync,
            };

        #endregion

        #region Public Methods

        /// <summary>
        /// Executes the requested synchronous or asynchronous operation.
        /// </summary>
        /// <param name="connection">The configured test connection.</param>
        /// <param name="trace">The trace receiving notifications.</param>
        /// <param name="operation">The operation name.</param>
        /// <param name="asynchronous">Whether to use the asynchronous API.</param>
        /// <returns>The operation result.</returns>
        public static Task<object> ExecuteAsync(CustomTracedDbConnection connection,
            ITrace trace,
            string operation,
            bool asynchronous)
            => _operations.TryGetValue(operation,
                out var execute)
                ? execute(connection,
                    trace,
                    asynchronous)
                : throw new ArgumentOutOfRangeException(nameof(operation));

        #endregion

        #region Private Methods

        /// <summary>
        /// Executes Average through the selected API.
        /// </summary>
        /// <param name="connection">The configured test connection.</param>
        /// <param name="trace">The trace receiving notifications.</param>
        /// <param name="asynchronous">Whether to use the asynchronous API.</param>
        /// <returns>The operation result.</returns>
        private static async Task<object> ExecuteAverageAsync(CustomTracedDbConnection connection,
            ITrace trace,
            bool asynchronous)
            => asynchronous
                ? await connection.AverageAsync<OperationTraceEntity>(field: item => item.Id,
                    where: (QueryGroup)null,
                    trace: trace).ConfigureAwait(false)
                : connection.Average<OperationTraceEntity>(field: item => item.Id,
                    where: (QueryGroup)null,
                    trace: trace);

        /// <summary>
        /// Executes AverageAll through the selected API.
        /// </summary>
        /// <param name="connection">The configured test connection.</param>
        /// <param name="trace">The trace receiving notifications.</param>
        /// <param name="asynchronous">Whether to use the asynchronous API.</param>
        /// <returns>The operation result.</returns>
        private static async Task<object> ExecuteAverageAllAsync(CustomTracedDbConnection connection,
            ITrace trace,
            bool asynchronous)
            => asynchronous
                ? await connection.AverageAllAsync<OperationTraceEntity>(field: item => item.Id,
                    trace: trace).ConfigureAwait(false)
                : connection.AverageAll<OperationTraceEntity>(field: item => item.Id,
                    trace: trace);

        /// <summary>
        /// Executes BatchQuery through the selected API.
        /// </summary>
        /// <param name="connection">The configured test connection.</param>
        /// <param name="trace">The trace receiving notifications.</param>
        /// <param name="asynchronous">Whether to use the asynchronous API.</param>
        /// <returns>The operation result.</returns>
        private static async Task<object> ExecuteBatchQueryAsync(CustomTracedDbConnection connection,
            ITrace trace,
            bool asynchronous)
            => asynchronous
                ? await connection.BatchQueryAsync<OperationTraceEntity>(0,
                10,
                    orderBy: new[] { new OrderField("Id", Order.Ascending) },
                    trace: trace).ConfigureAwait(false)
                : connection.BatchQuery<OperationTraceEntity>(0,
                10,
                    orderBy: new[] { new OrderField("Id", Order.Ascending) },
                    trace: trace);

        /// <summary>
        /// Executes Count through the selected API.
        /// </summary>
        /// <param name="connection">The configured test connection.</param>
        /// <param name="trace">The trace receiving notifications.</param>
        /// <param name="asynchronous">Whether to use the asynchronous API.</param>
        /// <returns>The operation result.</returns>
        private static async Task<object> ExecuteCountAsync(CustomTracedDbConnection connection,
            ITrace trace,
            bool asynchronous)
            => asynchronous
                ? await connection.CountAsync<OperationTraceEntity>(where: (QueryGroup)null,
                    trace: trace).ConfigureAwait(false)
                : connection.Count<OperationTraceEntity>(where: (QueryGroup)null,
                    trace: trace);

        /// <summary>
        /// Executes CountAll through the selected API.
        /// </summary>
        /// <param name="connection">The configured test connection.</param>
        /// <param name="trace">The trace receiving notifications.</param>
        /// <param name="asynchronous">Whether to use the asynchronous API.</param>
        /// <returns>The operation result.</returns>
        private static async Task<object> ExecuteCountAllAsync(CustomTracedDbConnection connection,
            ITrace trace,
            bool asynchronous)
            => asynchronous
                ? await connection.CountAllAsync<OperationTraceEntity>(trace: trace).ConfigureAwait(false)
                : connection.CountAll<OperationTraceEntity>(trace: trace);

        /// <summary>
        /// Executes Delete through the selected API.
        /// </summary>
        /// <param name="connection">The configured test connection.</param>
        /// <param name="trace">The trace receiving notifications.</param>
        /// <param name="asynchronous">Whether to use the asynchronous API.</param>
        /// <returns>The operation result.</returns>
        private static async Task<object> ExecuteDeleteAsync(CustomTracedDbConnection connection,
            ITrace trace,
            bool asynchronous)
            => asynchronous
                ? await connection.DeleteAsync<OperationTraceEntity>(where: (QueryGroup)null,
                    trace: trace).ConfigureAwait(false)
                : connection.Delete<OperationTraceEntity>(where: (QueryGroup)null,
                    trace: trace);

        /// <summary>
        /// Executes DeleteAll through the selected API.
        /// </summary>
        /// <param name="connection">The configured test connection.</param>
        /// <param name="trace">The trace receiving notifications.</param>
        /// <param name="asynchronous">Whether to use the asynchronous API.</param>
        /// <returns>The operation result.</returns>
        private static async Task<object> ExecuteDeleteAllAsync(CustomTracedDbConnection connection,
            ITrace trace,
            bool asynchronous)
            => asynchronous
                ? await connection.DeleteAllAsync<OperationTraceEntity>(trace: trace).ConfigureAwait(false)
                : connection.DeleteAll<OperationTraceEntity>(trace: trace);

        /// <summary>
        /// Executes Exists through the selected API.
        /// </summary>
        /// <param name="connection">The configured test connection.</param>
        /// <param name="trace">The trace receiving notifications.</param>
        /// <param name="asynchronous">Whether to use the asynchronous API.</param>
        /// <returns>The operation result.</returns>
        private static async Task<object> ExecuteExistsAsync(CustomTracedDbConnection connection,
            ITrace trace,
            bool asynchronous)
            => asynchronous
                ? await connection.ExistsAsync<OperationTraceEntity>(where: (QueryGroup)null,
                    trace: trace).ConfigureAwait(false)
                : connection.Exists<OperationTraceEntity>(where: (QueryGroup)null,
                    trace: trace);

        /// <summary>
        /// Executes Insert through the selected API.
        /// </summary>
        /// <param name="connection">The configured test connection.</param>
        /// <param name="trace">The trace receiving notifications.</param>
        /// <param name="asynchronous">Whether to use the asynchronous API.</param>
        /// <returns>The operation result.</returns>
        private static async Task<object> ExecuteInsertAsync(CustomTracedDbConnection connection,
            ITrace trace,
            bool asynchronous)
            => asynchronous
                ? await connection.InsertAsync(new OperationTraceEntity { Id = 1, Name = "Test" },
                    trace: trace).ConfigureAwait(false)
                : connection.Insert(new OperationTraceEntity { Id = 1, Name = "Test" },
                    trace: trace);

        /// <summary>
        /// Executes InsertAll through the selected API.
        /// </summary>
        /// <param name="connection">The configured test connection.</param>
        /// <param name="trace">The trace receiving notifications.</param>
        /// <param name="asynchronous">Whether to use the asynchronous API.</param>
        /// <returns>The operation result.</returns>
        private static async Task<object> ExecuteInsertAllAsync(CustomTracedDbConnection connection,
            ITrace trace,
            bool asynchronous)
            => asynchronous
                ? await connection.InsertAllAsync(new[] { new OperationTraceEntity { Id = 1, Name = "Test" }, new OperationTraceEntity { Id = 1, Name = "Test" } },
                    batchSize: 1,
                    trace: trace).ConfigureAwait(false)
                : connection.InsertAll(new[] { new OperationTraceEntity { Id = 1, Name = "Test" }, new OperationTraceEntity { Id = 1, Name = "Test" } },
                    batchSize: 1,
                    trace: trace);

        /// <summary>
        /// Executes InsertAllBatch through the selected API.
        /// </summary>
        /// <param name="connection">The configured test connection.</param>
        /// <param name="trace">The trace receiving notifications.</param>
        /// <param name="asynchronous">Whether to use the asynchronous API.</param>
        /// <returns>The operation result.</returns>
        private static async Task<object> ExecuteInsertAllBatchAsync(CustomTracedDbConnection connection,
            ITrace trace,
            bool asynchronous)
            => asynchronous
                ? await connection.InsertAllAsync(new[] { new OperationTraceEntity { Id = 1, Name = "Test" }, new OperationTraceEntity { Id = 1, Name = "Test" } },
                    batchSize: 2,
                    trace: trace).ConfigureAwait(false)
                : connection.InsertAll(new[] { new OperationTraceEntity { Id = 1, Name = "Test" }, new OperationTraceEntity { Id = 1, Name = "Test" } },
                    batchSize: 2,
                    trace: trace);

        /// <summary>
        /// Executes Max through the selected API.
        /// </summary>
        /// <param name="connection">The configured test connection.</param>
        /// <param name="trace">The trace receiving notifications.</param>
        /// <param name="asynchronous">Whether to use the asynchronous API.</param>
        /// <returns>The operation result.</returns>
        private static async Task<object> ExecuteMaxAsync(CustomTracedDbConnection connection,
            ITrace trace,
            bool asynchronous)
            => asynchronous
                ? await connection.MaxAsync<OperationTraceEntity>(field: item => item.Id,
                    where: (QueryGroup)null,
                    trace: trace).ConfigureAwait(false)
                : connection.Max<OperationTraceEntity>(field: item => item.Id,
                    where: (QueryGroup)null,
                    trace: trace);

        /// <summary>
        /// Executes MaxAll through the selected API.
        /// </summary>
        /// <param name="connection">The configured test connection.</param>
        /// <param name="trace">The trace receiving notifications.</param>
        /// <param name="asynchronous">Whether to use the asynchronous API.</param>
        /// <returns>The operation result.</returns>
        private static async Task<object> ExecuteMaxAllAsync(CustomTracedDbConnection connection,
            ITrace trace,
            bool asynchronous)
            => asynchronous
                ? await connection.MaxAllAsync<OperationTraceEntity>(field: item => item.Id,
                    trace: trace).ConfigureAwait(false)
                : connection.MaxAll<OperationTraceEntity>(field: item => item.Id,
                    trace: trace);

        /// <summary>
        /// Executes Merge through the selected API.
        /// </summary>
        /// <param name="connection">The configured test connection.</param>
        /// <param name="trace">The trace receiving notifications.</param>
        /// <param name="asynchronous">Whether to use the asynchronous API.</param>
        /// <returns>The operation result.</returns>
        private static async Task<object> ExecuteMergeAsync(CustomTracedDbConnection connection,
            ITrace trace,
            bool asynchronous)
            => asynchronous
                ? await connection.MergeAsync(new OperationTraceEntity { Id = 1, Name = "Test" },
                    trace: trace).ConfigureAwait(false)
                : connection.Merge(new OperationTraceEntity { Id = 1, Name = "Test" },
                    trace: trace);

        /// <summary>
        /// Executes MergeAll through the selected API.
        /// </summary>
        /// <param name="connection">The configured test connection.</param>
        /// <param name="trace">The trace receiving notifications.</param>
        /// <param name="asynchronous">Whether to use the asynchronous API.</param>
        /// <returns>The operation result.</returns>
        private static async Task<object> ExecuteMergeAllAsync(CustomTracedDbConnection connection,
            ITrace trace,
            bool asynchronous)
            => asynchronous
                ? await connection.MergeAllAsync(new[] { new OperationTraceEntity { Id = 1, Name = "Test" }, new OperationTraceEntity { Id = 1, Name = "Test" } },
                    batchSize: 1,
                    trace: trace).ConfigureAwait(false)
                : connection.MergeAll(new[] { new OperationTraceEntity { Id = 1, Name = "Test" }, new OperationTraceEntity { Id = 1, Name = "Test" } },
                    batchSize: 1,
                    trace: trace);

        /// <summary>
        /// Executes MergeAllBatch through the selected API.
        /// </summary>
        /// <param name="connection">The configured test connection.</param>
        /// <param name="trace">The trace receiving notifications.</param>
        /// <param name="asynchronous">Whether to use the asynchronous API.</param>
        /// <returns>The operation result.</returns>
        private static async Task<object> ExecuteMergeAllBatchAsync(CustomTracedDbConnection connection,
            ITrace trace,
            bool asynchronous)
            => asynchronous
                ? await connection.MergeAllAsync(new[] { new OperationTraceEntity { Id = 1, Name = "Test" }, new OperationTraceEntity { Id = 1, Name = "Test" } },
                    batchSize: 2,
                    trace: trace).ConfigureAwait(false)
                : connection.MergeAll(new[] { new OperationTraceEntity { Id = 1, Name = "Test" }, new OperationTraceEntity { Id = 1, Name = "Test" } },
                    batchSize: 2,
                    trace: trace);

        /// <summary>
        /// Executes Min through the selected API.
        /// </summary>
        /// <param name="connection">The configured test connection.</param>
        /// <param name="trace">The trace receiving notifications.</param>
        /// <param name="asynchronous">Whether to use the asynchronous API.</param>
        /// <returns>The operation result.</returns>
        private static async Task<object> ExecuteMinAsync(CustomTracedDbConnection connection,
            ITrace trace,
            bool asynchronous)
            => asynchronous
                ? await connection.MinAsync<OperationTraceEntity>(field: item => item.Id,
                    where: (QueryGroup)null,
                    trace: trace).ConfigureAwait(false)
                : connection.Min<OperationTraceEntity>(field: item => item.Id,
                    where: (QueryGroup)null,
                    trace: trace);

        /// <summary>
        /// Executes MinAll through the selected API.
        /// </summary>
        /// <param name="connection">The configured test connection.</param>
        /// <param name="trace">The trace receiving notifications.</param>
        /// <param name="asynchronous">Whether to use the asynchronous API.</param>
        /// <returns>The operation result.</returns>
        private static async Task<object> ExecuteMinAllAsync(CustomTracedDbConnection connection,
            ITrace trace,
            bool asynchronous)
            => asynchronous
                ? await connection.MinAllAsync<OperationTraceEntity>(field: item => item.Id,
                    trace: trace).ConfigureAwait(false)
                : connection.MinAll<OperationTraceEntity>(field: item => item.Id,
                    trace: trace);

        /// <summary>
        /// Executes Query through the selected API.
        /// </summary>
        /// <param name="connection">The configured test connection.</param>
        /// <param name="trace">The trace receiving notifications.</param>
        /// <param name="asynchronous">Whether to use the asynchronous API.</param>
        /// <returns>The operation result.</returns>
        private static async Task<object> ExecuteQueryAsync(CustomTracedDbConnection connection,
            ITrace trace,
            bool asynchronous)
            => asynchronous
                ? await connection.QueryAsync<OperationTraceEntity>(where: (QueryGroup)null,
                    trace: trace).ConfigureAwait(false)
                : connection.Query<OperationTraceEntity>(where: (QueryGroup)null,
                    trace: trace);

        /// <summary>
        /// Executes QueryAll through the selected API.
        /// </summary>
        /// <param name="connection">The configured test connection.</param>
        /// <param name="trace">The trace receiving notifications.</param>
        /// <param name="asynchronous">Whether to use the asynchronous API.</param>
        /// <returns>The operation result.</returns>
        private static async Task<object> ExecuteQueryAllAsync(CustomTracedDbConnection connection,
            ITrace trace,
            bool asynchronous)
            => asynchronous
                ? await connection.QueryAllAsync<OperationTraceEntity>(trace: trace).ConfigureAwait(false)
                : connection.QueryAll<OperationTraceEntity>(trace: trace);

        /// <summary>
        /// Executes QueryFirst through the selected API.
        /// </summary>
        /// <param name="connection">The configured test connection.</param>
        /// <param name="trace">The trace receiving notifications.</param>
        /// <param name="asynchronous">Whether to use the asynchronous API.</param>
        /// <returns>The operation result.</returns>
        private static async Task<object> ExecuteQueryFirstAsync(CustomTracedDbConnection connection,
            ITrace trace,
            bool asynchronous)
            => asynchronous
                ? await connection.QueryFirstAsync<OperationTraceEntity>(where: (QueryGroup)null,
                    trace: trace).ConfigureAwait(false)
                : connection.QueryFirst<OperationTraceEntity>(where: (QueryGroup)null,
                    trace: trace);

        /// <summary>
        /// Executes QueryMultiple through the selected API.
        /// </summary>
        /// <param name="connection">The configured test connection.</param>
        /// <param name="trace">The trace receiving notifications.</param>
        /// <param name="asynchronous">Whether to use the asynchronous API.</param>
        /// <returns>The operation result.</returns>
        private static async Task<object> ExecuteQueryMultipleAsync(CustomTracedDbConnection connection,
            ITrace trace,
            bool asynchronous)
            => asynchronous
                ? await connection.QueryMultipleAsync<OperationTraceEntity, OperationTraceEntity>(what1: (object)null,
                    what2: (object)null,
                    trace: trace).ConfigureAwait(false)
                : connection.QueryMultiple<OperationTraceEntity, OperationTraceEntity>(what1: (object)null,
                    what2: (object)null,
                    trace: trace);

        /// <summary>
        /// Executes QuerySingle through the selected API.
        /// </summary>
        /// <param name="connection">The configured test connection.</param>
        /// <param name="trace">The trace receiving notifications.</param>
        /// <param name="asynchronous">Whether to use the asynchronous API.</param>
        /// <returns>The operation result.</returns>
        private static async Task<object> ExecuteQuerySingleAsync(CustomTracedDbConnection connection,
            ITrace trace,
            bool asynchronous)
            => asynchronous
                ? await connection.QuerySingleAsync<OperationTraceEntity>(where: (QueryGroup)null,
                    trace: trace).ConfigureAwait(false)
                : connection.QuerySingle<OperationTraceEntity>(where: (QueryGroup)null,
                    trace: trace);

        /// <summary>
        /// Executes SkipQuery through the selected API.
        /// </summary>
        /// <param name="connection">The configured test connection.</param>
        /// <param name="trace">The trace receiving notifications.</param>
        /// <param name="asynchronous">Whether to use the asynchronous API.</param>
        /// <returns>The operation result.</returns>
        private static async Task<object> ExecuteSkipQueryAsync(CustomTracedDbConnection connection,
            ITrace trace,
            bool asynchronous)
            => asynchronous
                ? await connection.SkipQueryAsync<OperationTraceEntity>(0,
                10,
                    orderBy: new[] { new OrderField("Id", Order.Ascending) },
                    trace: trace).ConfigureAwait(false)
                : connection.SkipQuery<OperationTraceEntity>(0,
                10,
                    orderBy: new[] { new OrderField("Id", Order.Ascending) },
                    trace: trace);

        /// <summary>
        /// Executes Sum through the selected API.
        /// </summary>
        /// <param name="connection">The configured test connection.</param>
        /// <param name="trace">The trace receiving notifications.</param>
        /// <param name="asynchronous">Whether to use the asynchronous API.</param>
        /// <returns>The operation result.</returns>
        private static async Task<object> ExecuteSumAsync(CustomTracedDbConnection connection,
            ITrace trace,
            bool asynchronous)
            => asynchronous
                ? await connection.SumAsync<OperationTraceEntity>(field: item => item.Id,
                    where: (QueryGroup)null,
                    trace: trace).ConfigureAwait(false)
                : connection.Sum<OperationTraceEntity>(field: item => item.Id,
                    where: (QueryGroup)null,
                    trace: trace);

        /// <summary>
        /// Executes SumAll through the selected API.
        /// </summary>
        /// <param name="connection">The configured test connection.</param>
        /// <param name="trace">The trace receiving notifications.</param>
        /// <param name="asynchronous">Whether to use the asynchronous API.</param>
        /// <returns>The operation result.</returns>
        private static async Task<object> ExecuteSumAllAsync(CustomTracedDbConnection connection,
            ITrace trace,
            bool asynchronous)
            => asynchronous
                ? await connection.SumAllAsync<OperationTraceEntity>(field: item => item.Id,
                    trace: trace).ConfigureAwait(false)
                : connection.SumAll<OperationTraceEntity>(field: item => item.Id,
                    trace: trace);

        /// <summary>
        /// Executes Truncate through the selected API.
        /// </summary>
        /// <param name="connection">The configured test connection.</param>
        /// <param name="trace">The trace receiving notifications.</param>
        /// <param name="asynchronous">Whether to use the asynchronous API.</param>
        /// <returns>The operation result.</returns>
        private static async Task<object> ExecuteTruncateAsync(CustomTracedDbConnection connection,
            ITrace trace,
            bool asynchronous)
            => asynchronous
                ? await connection.TruncateAsync<OperationTraceEntity>(trace: trace).ConfigureAwait(false)
                : connection.Truncate<OperationTraceEntity>(trace: trace);

        /// <summary>
        /// Executes Update through the selected API.
        /// </summary>
        /// <param name="connection">The configured test connection.</param>
        /// <param name="trace">The trace receiving notifications.</param>
        /// <param name="asynchronous">Whether to use the asynchronous API.</param>
        /// <returns>The operation result.</returns>
        private static async Task<object> ExecuteUpdateAsync(CustomTracedDbConnection connection,
            ITrace trace,
            bool asynchronous)
            => asynchronous
                ? await connection.UpdateAsync(new OperationTraceEntity { Id = 1, Name = "Test" },
                    trace: trace).ConfigureAwait(false)
                : connection.Update(new OperationTraceEntity { Id = 1, Name = "Test" },
                    trace: trace);

        /// <summary>
        /// Executes UpdateAll through the selected API.
        /// </summary>
        /// <param name="connection">The configured test connection.</param>
        /// <param name="trace">The trace receiving notifications.</param>
        /// <param name="asynchronous">Whether to use the asynchronous API.</param>
        /// <returns>The operation result.</returns>
        private static async Task<object> ExecuteUpdateAllAsync(CustomTracedDbConnection connection,
            ITrace trace,
            bool asynchronous)
            => asynchronous
                ? await connection.UpdateAllAsync(new[] { new OperationTraceEntity { Id = 1, Name = "Test" }, new OperationTraceEntity { Id = 1, Name = "Test" } },
                    batchSize: 1,
                    trace: trace).ConfigureAwait(false)
                : connection.UpdateAll(new[] { new OperationTraceEntity { Id = 1, Name = "Test" }, new OperationTraceEntity { Id = 1, Name = "Test" } },
                    batchSize: 1,
                    trace: trace);

        /// <summary>
        /// Executes UpdateAllBatch through the selected API.
        /// </summary>
        /// <param name="connection">The configured test connection.</param>
        /// <param name="trace">The trace receiving notifications.</param>
        /// <param name="asynchronous">Whether to use the asynchronous API.</param>
        /// <returns>The operation result.</returns>
        private static async Task<object> ExecuteUpdateAllBatchAsync(CustomTracedDbConnection connection,
            ITrace trace,
            bool asynchronous)
            => asynchronous
                ? await connection.UpdateAllAsync(new[] { new OperationTraceEntity { Id = 1, Name = "Test" }, new OperationTraceEntity { Id = 1, Name = "Test" } },
                    batchSize: 2,
                    trace: trace).ConfigureAwait(false)
                : connection.UpdateAll(new[] { new OperationTraceEntity { Id = 1, Name = "Test" }, new OperationTraceEntity { Id = 1, Name = "Test" } },
                    batchSize: 2,
                    trace: trace);
        #endregion
    }
}

