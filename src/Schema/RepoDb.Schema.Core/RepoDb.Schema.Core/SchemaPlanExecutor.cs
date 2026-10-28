#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using RepoDb.Interfaces;
using RepoDb.Schema.Enumerations;
using RepoDb.Schema.Models;
using static RepoDb.Schema.DbConnectionExtension;

namespace RepoDb.Schema
{
    /// <summary>
    /// Shares destination checks, plan execution, and completion callbacks between schema copies and file loads.
    /// A null source connection identifies a file definition; its source metadata comes from the table.
    /// </summary>
    internal static class SchemaPlanExecutor
    {
        #region Methods

        /// <summary>
        /// Executes a schema plan and reports each table at the same completion point for copies and loads.
        /// </summary>
        [UnconditionalSuppressMessage("Trimming", "IL2026:RequiresUnreferencedCode", Justification = "No parameter object is passed to the RepoDB execute methods.")]
        internal static CopyTableSchemaResult Execute(DateTime startTime,
            IList<PlanTable> tables,
            ISchemaComposer schemaComposer,
            IDbConnection connection,
            IDbConnection destinationConnection,
            TableExistsBehavior tableExistenceBehavior,
            Action<CopyTableSchemaResultItem> createdCallback,
            Action<ExecutionError> errorCallback,
            int? commandTimeout,
            string traceKey,
            ITrace trace,
            IDbTransaction transaction)
        {
            ProbeTables(tables, tableExistenceBehavior, schemaComposer, destinationConnection, commandTimeout, trace, transaction, connection == null ? traceKey : null);
            var plan = Plan.Create(tables, tableExistenceBehavior, schemaComposer);
            var errors = new List<ExecutionError>();
            var items = new List<CopyTableSchemaResultItem>();

            // Report the tables that have nothing to execute
            Report(createdCallback, items, plan.Immediate, errors, connection, destinationConnection, tableExistenceBehavior, schemaComposer);

            // Iterate
            for (var i = 0; i < plan.Steps.Count; i++)
            {
                try
                {
                    if (!string.IsNullOrWhiteSpace(plan.Steps[i].Statement))
                    {
                        destinationConnection.ExecuteNonQuery(plan.Steps[i].Statement,
                            traceKey: traceKey,
                            commandTimeout: commandTimeout,
                            transaction: transaction,
                            trace: trace);
                    }
                }
                catch (Exception e) when (errorCallback != null && !(e is OperationCanceledException))
                {
                    var error = CreateError(e, plan.Steps[i], i);
                    errors.Add(error);
                    errorCallback(error);
                }

                // Report
                Report(createdCallback, items, plan.Completions[i], errors, connection, destinationConnection, tableExistenceBehavior, schemaComposer);
            }

            // Report
            Report(createdCallback, items, plan.Completions[-1], errors, connection, destinationConnection, tableExistenceBehavior, schemaComposer);
            return CreateResult(startTime, items, errors);
        }

        /// <summary>
        /// Asynchronously executes a schema plan with cancellation and the same completion callbacks as copies.
        /// </summary>
        [UnconditionalSuppressMessage("Trimming", "IL2026:RequiresUnreferencedCode", Justification = "No parameter object is passed to the RepoDB execute methods.")]
        internal static async Task<CopyTableSchemaResult> ExecuteAsync(DateTime startTime,
            IList<PlanTable> tables,
            ISchemaComposer schemaComposer,
            IDbConnection connection,
            IDbConnection destinationConnection,
            TableExistsBehavior tableExistenceBehavior,
            Action<CopyTableSchemaResultItem> createdCallback,
            Action<ExecutionError> errorCallback,
            int? commandTimeout,
            string traceKey,
            ITrace trace,
            IDbTransaction transaction,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await ProbeTablesAsync(tables, tableExistenceBehavior, schemaComposer, destinationConnection, commandTimeout, trace, transaction, cancellationToken, connection == null ? traceKey : null).ConfigureAwait(false);
            var plan = Plan.Create(tables, tableExistenceBehavior, schemaComposer);
            var errors = new List<ExecutionError>();
            var items = new List<CopyTableSchemaResultItem>();

            // Report the tables that have nothing to execute
            Report(createdCallback, items, plan.Immediate, errors, connection, destinationConnection, tableExistenceBehavior, schemaComposer);

            // Iterate
            for (var i = 0; i < plan.Steps.Count; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();
                try
                {
                    if (!string.IsNullOrWhiteSpace(plan.Steps[i].Statement))
                    {
                        await destinationConnection.ExecuteNonQueryAsync(plan.Steps[i].Statement,
                            traceKey: traceKey,
                            commandTimeout: commandTimeout,
                            transaction: transaction,
                            trace: trace,
                            cancellationToken: cancellationToken).ConfigureAwait(false);
                    }
                }
                catch (Exception e) when (errorCallback != null && !(e is OperationCanceledException))
                {
                    var error = CreateError(e, plan.Steps[i], i);
                    errors.Add(error);
                    errorCallback(error);
                }

                // Report
                Report(createdCallback, items, plan.Completions[i], errors, connection, destinationConnection, tableExistenceBehavior, schemaComposer);
            }

            // Report
            Report(createdCallback, items, plan.Completions[-1], errors, connection, destinationConnection, tableExistenceBehavior, schemaComposer);
            return CreateResult(startTime, items, errors);
        }

        #endregion
    }
}
