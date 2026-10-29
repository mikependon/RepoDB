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
        #region Public Methods

        /// <summary>
        /// Executes a schema plan and reports each table at the same completion point for copies and loads.
        /// </summary>
        /// <param name="startTime">The time (in UTC) when the operation has started.</param>
        /// <param name="tables">The tables of the plan.</param>
        /// <param name="schemaComposer">The composer of the destination database.</param>
        /// <param name="connection">The source connection, or <c>null</c> for a file definition.</param>
        /// <param name="destinationConnection">The connection of the destination database.</param>
        /// <param name="tableExistenceBehavior">Defines what happens when a table already exists in the destination database.</param>
        /// <param name="createdCallback">The callback that receives the item of each table once its schema is created.</param>
        /// <param name="errorCallback">The callback that receives an error when a statement fails.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="traceKey">The tracking key to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="transaction">The transaction to be used on the destination connection.</param>
        /// <returns>The result of the schema operation.</returns>
        [UnconditionalSuppressMessage("Trimming",
            "IL2026:RequiresUnreferencedCode",
            Justification = "No parameter object is passed to the RepoDB execute methods.")]
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
            ProbeTables(tables,
                tableExistenceBehavior,
                schemaComposer,
                destinationConnection,
                commandTimeout,
                trace,
                transaction,
                connection == null ? traceKey : null);
            var plan = Plan.Create(tables,
                tableExistenceBehavior,
                schemaComposer);
            var errors = new List<ExecutionError>();
            var items = new List<CopyTableSchemaResultItem>();
            Action<IEnumerable<PlanTable>> reportCompleted = completed => Report(createdCallback,
                items,
                completed,
                errors,
                connection,
                destinationConnection,
                tableExistenceBehavior,
                schemaComposer);
            // Report the tables that have nothing to execute
            reportCompleted(plan.Immediate);
            // Iterate
            for (var i = 0; i < plan.Steps.Count; i++)
            {
                ExecuteStep(plan.Steps[i],
                    i,
                    destinationConnection,
                    errorCallback,
                    errors,
                    commandTimeout,
                    traceKey,
                    trace,
                    transaction);
                // Report
                reportCompleted(plan.Completions[i]);
            }

            // Report
            reportCompleted(plan.Completions[-1]);
            return CreateResult(startTime,
                items,
                errors);
        }

        /// <summary>
        /// Asynchronously executes a schema plan with cancellation and the same completion callbacks as copies.
        /// </summary>
        /// <param name="startTime">The time (in UTC) when the operation has started.</param>
        /// <param name="tables">The tables of the plan.</param>
        /// <param name="schemaComposer">The composer of the destination database.</param>
        /// <param name="connection">The source connection, or <c>null</c> for a file definition.</param>
        /// <param name="destinationConnection">The connection of the destination database.</param>
        /// <param name="tableExistenceBehavior">Defines what happens when a table already exists in the destination database.</param>
        /// <param name="createdCallback">The callback that receives the item of each table once its schema is created.</param>
        /// <param name="errorCallback">The callback that receives an error when a statement fails.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="traceKey">The tracking key to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="transaction">The transaction to be used on the destination connection.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is the result of the schema operation.</returns>
        [UnconditionalSuppressMessage("Trimming",
            "IL2026:RequiresUnreferencedCode",
            Justification = "No parameter object is passed to the RepoDB execute methods.")]
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
            await ProbeTablesAsync(tables,
                tableExistenceBehavior,
                schemaComposer,
                destinationConnection,
                commandTimeout,
                trace,
                transaction,
                cancellationToken,
                connection == null ? traceKey : null).ConfigureAwait(false);
            var plan = Plan.Create(tables,
                tableExistenceBehavior,
                schemaComposer);
            var errors = new List<ExecutionError>();
            var items = new List<CopyTableSchemaResultItem>();
            Action<IEnumerable<PlanTable>> reportCompleted = completed => Report(createdCallback,
                items,
                completed,
                errors,
                connection,
                destinationConnection,
                tableExistenceBehavior,
                schemaComposer);
            // Report the tables that have nothing to execute
            reportCompleted(plan.Immediate);
            // Iterate
            for (var i = 0; i < plan.Steps.Count; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await ExecuteStepAsync(plan.Steps[i],
                    i,
                    destinationConnection,
                    errorCallback,
                    errors,
                    commandTimeout,
                    traceKey,
                    trace,
                    transaction,
                    cancellationToken).ConfigureAwait(false);
                // Report
                reportCompleted(plan.Completions[i]);
            }

            // Report
            reportCompleted(plan.Completions[-1]);
            return CreateResult(startTime,
                items,
                errors);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Executes one plan statement and records errors accepted by the caller's error callback.
        /// </summary>
        /// <param name="step">The statement and owning table to execute.</param>
        /// <param name="position">The step's zero-based position in the plan.</param>
        /// <param name="destinationConnection">The connection of the destination database.</param>
        /// <param name="errorCallback">The callback that receives an error when a statement fails.</param>
        /// <param name="errors">The errors collected during plan execution.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="traceKey">The tracking key to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="transaction">The transaction to be used on the destination connection.</param>
        [UnconditionalSuppressMessage("Trimming",
            "IL2026:RequiresUnreferencedCode",
            Justification = "No parameter object is passed to the RepoDB execute methods.")]
        private static void ExecuteStep(PlanStep step,
            int position,
            IDbConnection destinationConnection,
            Action<ExecutionError> errorCallback,
            List<ExecutionError> errors,
            int? commandTimeout,
            string traceKey,
            ITrace trace,
            IDbTransaction transaction)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(step.Statement))
                {
                    destinationConnection.ExecuteNonQuery(step.Statement,
                        traceKey: traceKey,
                        commandTimeout: commandTimeout,
                        transaction: transaction,
                        trace: trace);
                }
            }
            catch (Exception e) when (errorCallback != null && !(e is OperationCanceledException))
            {
                var error = CreateError(e,
                    step,
                    position);
                errors.Add(error);
                errorCallback(error);
            }
        }

        /// <summary>
        /// Executes one plan statement and records errors accepted by the caller's error callback.
        /// </summary>
        /// <param name="step">The statement and owning table to execute.</param>
        /// <param name="position">The step's zero-based position in the plan.</param>
        /// <param name="destinationConnection">The connection of the destination database.</param>
        /// <param name="errorCallback">The callback that receives an error when a statement fails.</param>
        /// <param name="errors">The errors collected during plan execution.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="traceKey">The tracking key to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="transaction">The transaction to be used on the destination connection.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task representing statement execution and error reporting.</returns>
        [UnconditionalSuppressMessage("Trimming",
            "IL2026:RequiresUnreferencedCode",
            Justification = "No parameter object is passed to the RepoDB execute methods.")]
        private static async Task ExecuteStepAsync(PlanStep step,
            int position,
            IDbConnection destinationConnection,
            Action<ExecutionError> errorCallback,
            List<ExecutionError> errors,
            int? commandTimeout,
            string traceKey,
            ITrace trace,
            IDbTransaction transaction,
            CancellationToken cancellationToken)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(step.Statement))
                {
                    await destinationConnection.ExecuteNonQueryAsync(step.Statement,
                        traceKey: traceKey,
                        commandTimeout: commandTimeout,
                        transaction: transaction,
                        trace: trace,
                        cancellationToken: cancellationToken).ConfigureAwait(false);
                }
            }
            catch (Exception e) when (errorCallback != null && !(e is OperationCanceledException))
            {
                var error = CreateError(e,
                    step,
                    position);
                errors.Add(error);
                errorCallback(error);
            }
        }

        #endregion
    }
}
