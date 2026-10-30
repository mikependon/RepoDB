#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using RepoDb.Interfaces;
using RepoDb.SqlServer.BulkOperations.IntegrationTests.Models;

namespace RepoDb.SqlServer.BulkOperations.IntegrationTests.Setup
{
    /// <summary>
    /// Exercises public bulk APIs with each supported source representation.
    /// </summary>
    internal static class BulkTraceExecutor
    {
        #region Public Methods

        /// <summary>
        /// Executes one bulk operation through the selected API.
        /// </summary>
        /// <param name="connection">The SQL Server connection.</param>
        /// <param name="tableName">The isolated target table.</param>
        /// <param name="operation">The operation name.</param>
        /// <param name="form">The input representation.</param>
        /// <param name="asynchronous">Whether to use the asynchronous API.</param>
        /// <param name="trace">The execution observer.</param>
        /// <returns>The affected row count.</returns>
        public static async Task<int> ExecuteAsync(SqlConnection connection,
            string tableName,
            string operation,
            string form,
            bool asynchronous,
            ITrace trace)
        {
            var rows = new[] { new BulkTraceRow { Id = operation is "Insert" or "Merge" ? 2 : 1, Value = 10 } };
            using var table = new DataTable();
            table.Columns.Add("Id",
                typeof(int));
            table.Columns.Add("Value",
                typeof(int));
            table.Rows.Add(rows[0].Id,
                rows[0].Value);
            using var reader = table.CreateDataReader();
            if (operation == "DeleteByKey")
            {
                return asynchronous
                    ? await connection.BulkDeleteByKeyAsync(tableName,
                        new[] { 1 },
                        trace: trace).ConfigureAwait(false)
                    : connection.BulkDeleteByKey(tableName,
                        new[] { 1 },
                        trace: trace);
            }
            return asynchronous
                ? await DispatchAsync(connection,
                    tableName,
                    rows,
                    table,
                    reader,
                    operation,
                    form,
                    trace).ConfigureAwait(false)
                : Dispatch(connection,
                    tableName,
                    rows,
                    table,
                    reader,
                    operation,
                    form,
                    trace);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Dispatches the bulk operation to its source-specific overload.
        /// </summary>
        /// <param name="connection">The SQL Server connection.</param>
        /// <param name="tableName">The isolated target table.</param>
        /// <param name="rows">The entity input.</param>
        /// <param name="table">The tabular input.</param>
        /// <param name="reader">The reader input.</param>
        /// <param name="operation">The operation name.</param>
        /// <param name="form">The source representation.</param>
        /// <param name="trace">The execution observer.</param>
        /// <returns>The affected row count.</returns>
        private static int Dispatch(SqlConnection connection,
            string tableName,
            BulkTraceRow[] rows,
            DataTable table,
            DbDataReader reader,
            string operation,
            string form,
            ITrace trace)
            => operation switch
            {
                "Insert" => Insert(connection,
                    tableName,
                    rows,
                    table,
                    reader,
                    form,
                    trace),
                "Update" => Update(connection,
                    tableName,
                    rows,
                    table,
                    reader,
                    form,
                    trace),
                "Merge" => Merge(connection,
                    tableName,
                    rows,
                    table,
                    reader,
                    form,
                    trace),
                "Delete" => Delete(connection,
                    tableName,
                    rows,
                    table,
                    reader,
                    form,
                    trace),
                _ => throw new ArgumentOutOfRangeException(nameof(operation))
            };

        /// <summary>
        /// Dispatches the bulk operation to its source-specific overload.
        /// </summary>
        /// <param name="connection">The SQL Server connection.</param>
        /// <param name="tableName">The isolated target table.</param>
        /// <param name="rows">The entity input.</param>
        /// <param name="table">The tabular input.</param>
        /// <param name="reader">The reader input.</param>
        /// <param name="operation">The operation name.</param>
        /// <param name="form">The source representation.</param>
        /// <param name="trace">The execution observer.</param>
        /// <returns>The affected row count.</returns>
        private static Task<int> DispatchAsync(SqlConnection connection,
            string tableName,
            BulkTraceRow[] rows,
            DataTable table,
            DbDataReader reader,
            string operation,
            string form,
            ITrace trace)
            => operation switch
            {
                "Insert" => InsertAsync(connection,
                    tableName,
                    rows,
                    table,
                    reader,
                    form,
                    trace),
                "Update" => UpdateAsync(connection,
                    tableName,
                    rows,
                    table,
                    reader,
                    form,
                    trace),
                "Merge" => MergeAsync(connection,
                    tableName,
                    rows,
                    table,
                    reader,
                    form,
                    trace),
                "Delete" => DeleteAsync(connection,
                    tableName,
                    rows,
                    table,
                    reader,
                    form,
                    trace),
                _ => throw new ArgumentOutOfRangeException(nameof(operation))
            };


        /// <summary>
        /// Executes bulk insert using the requested input representation.
        /// </summary>
        /// <param name="connection">The SQL Server connection.</param>
        /// <param name="tableName">The isolated target table.</param>
        /// <param name="rows">The entity input.</param>
        /// <param name="table">The tabular input.</param>
        /// <param name="reader">The reader input.</param>
        /// <param name="form">The selected input representation.</param>
        /// <param name="trace">The execution observer.</param>
        /// <returns>The affected row count.</returns>
        private static int Insert(SqlConnection connection,
            string tableName,
            BulkTraceRow[] rows,
            DataTable table,
            DbDataReader reader,
            string form,
            ITrace trace)
            => form switch
            {
                "Entities" => connection.BulkInsert(tableName,
                    rows,
                    options: SqlBulkCopyOptions.FireTriggers,
                    trace: trace),
                "Table" => connection.BulkInsert(tableName,
                    table,
                    options: SqlBulkCopyOptions.FireTriggers,
                    trace: trace),
                _ => connection.BulkInsert(tableName,
                    reader,
                    options: SqlBulkCopyOptions.FireTriggers,
                    trace: trace)
            };

        /// <summary>
        /// Executes asynchronous bulk insert using the requested input representation.
        /// </summary>
        /// <param name="connection">The SQL Server connection.</param>
        /// <param name="tableName">The isolated target table.</param>
        /// <param name="rows">The entity input.</param>
        /// <param name="table">The tabular input.</param>
        /// <param name="reader">The reader input.</param>
        /// <param name="form">The selected input representation.</param>
        /// <param name="trace">The execution observer.</param>
        /// <returns>The affected row count.</returns>
        private static Task<int> InsertAsync(SqlConnection connection,
            string tableName,
            BulkTraceRow[] rows,
            DataTable table,
            DbDataReader reader,
            string form,
            ITrace trace)
            => form switch
            {
                "Entities" => connection.BulkInsertAsync(tableName,
                    rows,
                    options: SqlBulkCopyOptions.FireTriggers,
                    trace: trace),
                "Table" => connection.BulkInsertAsync(tableName,
                    table,
                    options: SqlBulkCopyOptions.FireTriggers,
                    trace: trace),
                _ => connection.BulkInsertAsync(tableName,
                    reader,
                    options: SqlBulkCopyOptions.FireTriggers,
                    trace: trace)
            };

        /// <summary>
        /// Executes bulk update using the requested input representation.
        /// </summary>
        /// <param name="connection">The SQL Server connection.</param>
        /// <param name="tableName">The isolated target table.</param>
        /// <param name="rows">The entity input.</param>
        /// <param name="table">The tabular input.</param>
        /// <param name="reader">The reader input.</param>
        /// <param name="form">The selected input representation.</param>
        /// <param name="trace">The execution observer.</param>
        /// <returns>The affected row count.</returns>
        private static int Update(SqlConnection connection,
            string tableName,
            BulkTraceRow[] rows,
            DataTable table,
            DbDataReader reader,
            string form,
            ITrace trace)
            => form switch
            {
                "Entities" => connection.BulkUpdate(tableName,
                    rows,
                    options: SqlBulkCopyOptions.FireTriggers,
                    trace: trace),
                "Table" => connection.BulkUpdate(tableName,
                    table,
                    options: SqlBulkCopyOptions.FireTriggers,
                    trace: trace),
                _ => connection.BulkUpdate(tableName,
                    reader,
                    options: SqlBulkCopyOptions.FireTriggers,
                    trace: trace)
            };

        /// <summary>
        /// Executes asynchronous bulk update using the requested input representation.
        /// </summary>
        /// <param name="connection">The SQL Server connection.</param>
        /// <param name="tableName">The isolated target table.</param>
        /// <param name="rows">The entity input.</param>
        /// <param name="table">The tabular input.</param>
        /// <param name="reader">The reader input.</param>
        /// <param name="form">The selected input representation.</param>
        /// <param name="trace">The execution observer.</param>
        /// <returns>The affected row count.</returns>
        private static Task<int> UpdateAsync(SqlConnection connection,
            string tableName,
            BulkTraceRow[] rows,
            DataTable table,
            DbDataReader reader,
            string form,
            ITrace trace)
            => form switch
            {
                "Entities" => connection.BulkUpdateAsync(tableName,
                    rows,
                    options: SqlBulkCopyOptions.FireTriggers,
                    trace: trace),
                "Table" => connection.BulkUpdateAsync(tableName,
                    table,
                    options: SqlBulkCopyOptions.FireTriggers,
                    trace: trace),
                _ => connection.BulkUpdateAsync(tableName,
                    reader,
                    options: SqlBulkCopyOptions.FireTriggers,
                    trace: trace)
            };

        /// <summary>
        /// Executes bulk merge using the requested input representation.
        /// </summary>
        /// <param name="connection">The SQL Server connection.</param>
        /// <param name="tableName">The isolated target table.</param>
        /// <param name="rows">The entity input.</param>
        /// <param name="table">The tabular input.</param>
        /// <param name="reader">The reader input.</param>
        /// <param name="form">The selected input representation.</param>
        /// <param name="trace">The execution observer.</param>
        /// <returns>The affected row count.</returns>
        private static int Merge(SqlConnection connection,
            string tableName,
            BulkTraceRow[] rows,
            DataTable table,
            DbDataReader reader,
            string form,
            ITrace trace)
            => form switch
            {
                "Entities" => connection.BulkMerge(tableName,
                    rows,
                    options: SqlBulkCopyOptions.FireTriggers,
                    trace: trace),
                "Table" => connection.BulkMerge(tableName,
                    table,
                    options: SqlBulkCopyOptions.FireTriggers,
                    trace: trace),
                _ => connection.BulkMerge(tableName,
                    reader,
                    options: SqlBulkCopyOptions.FireTriggers,
                    trace: trace)
            };

        /// <summary>
        /// Executes asynchronous bulk merge using the requested input representation.
        /// </summary>
        /// <param name="connection">The SQL Server connection.</param>
        /// <param name="tableName">The isolated target table.</param>
        /// <param name="rows">The entity input.</param>
        /// <param name="table">The tabular input.</param>
        /// <param name="reader">The reader input.</param>
        /// <param name="form">The selected input representation.</param>
        /// <param name="trace">The execution observer.</param>
        /// <returns>The affected row count.</returns>
        private static Task<int> MergeAsync(SqlConnection connection,
            string tableName,
            BulkTraceRow[] rows,
            DataTable table,
            DbDataReader reader,
            string form,
            ITrace trace)
            => form switch
            {
                "Entities" => connection.BulkMergeAsync(tableName,
                    rows,
                    options: SqlBulkCopyOptions.FireTriggers,
                    trace: trace),
                "Table" => connection.BulkMergeAsync(tableName,
                    table,
                    options: SqlBulkCopyOptions.FireTriggers,
                    trace: trace),
                _ => connection.BulkMergeAsync(tableName,
                    reader,
                    options: SqlBulkCopyOptions.FireTriggers,
                    trace: trace)
            };

        /// <summary>
        /// Executes bulk delete using the requested input representation.
        /// </summary>
        /// <param name="connection">The SQL Server connection.</param>
        /// <param name="tableName">The isolated target table.</param>
        /// <param name="rows">The entity input.</param>
        /// <param name="table">The tabular input.</param>
        /// <param name="reader">The reader input.</param>
        /// <param name="form">The selected input representation.</param>
        /// <param name="trace">The execution observer.</param>
        /// <returns>The affected row count.</returns>
        private static int Delete(SqlConnection connection,
            string tableName,
            BulkTraceRow[] rows,
            DataTable table,
            DbDataReader reader,
            string form,
            ITrace trace)
            => form switch
            {
                "Entities" => connection.BulkDelete(tableName,
                    rows,
                    options: SqlBulkCopyOptions.FireTriggers,
                    trace: trace),
                "Table" => connection.BulkDelete(tableName,
                    table,
                    options: SqlBulkCopyOptions.FireTriggers,
                    trace: trace),
                _ => connection.BulkDelete(tableName,
                    reader,
                    options: SqlBulkCopyOptions.FireTriggers,
                    trace: trace)
            };

        /// <summary>
        /// Executes asynchronous bulk delete using the requested input representation.
        /// </summary>
        /// <param name="connection">The SQL Server connection.</param>
        /// <param name="tableName">The isolated target table.</param>
        /// <param name="rows">The entity input.</param>
        /// <param name="table">The tabular input.</param>
        /// <param name="reader">The reader input.</param>
        /// <param name="form">The selected input representation.</param>
        /// <param name="trace">The execution observer.</param>
        /// <returns>The affected row count.</returns>
        private static Task<int> DeleteAsync(SqlConnection connection,
            string tableName,
            BulkTraceRow[] rows,
            DataTable table,
            DbDataReader reader,
            string form,
            ITrace trace)
            => form switch
            {
                "Entities" => connection.BulkDeleteAsync(tableName,
                    rows,
                    options: SqlBulkCopyOptions.FireTriggers,
                    trace: trace),
                "Table" => connection.BulkDeleteAsync(tableName,
                    table,
                    options: SqlBulkCopyOptions.FireTriggers,
                    trace: trace),
                _ => connection.BulkDeleteAsync(tableName,
                    reader,
                    options: SqlBulkCopyOptions.FireTriggers,
                    trace: trace)
            };

        #endregion
    }
}

