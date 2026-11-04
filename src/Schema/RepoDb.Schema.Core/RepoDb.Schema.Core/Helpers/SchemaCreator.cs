#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using RepoDb.Interfaces;
using RepoDb.Schema.Models;

namespace RepoDb.Schema
{
    /// <summary>
    /// Creates the schemas of the tables of a plan that do not exist yet in the destination database,
    /// if the composer of the destination database supports it (see <see cref="ISchemaCreationSupport"/>).
    /// </summary>
    internal static class SchemaCreator
    {
        #region Private Methods

        /// <summary>
        /// Gets the distinct names of the schemas that own the tables.
        /// </summary>
        /// <param name="tables">The tables of the plan.</param>
        /// <returns>The names of the schemas, without the empty ones.</returns>
        private static IList<string> GetSchemaNames(IEnumerable<PlanTable> tables) =>
            tables.Select(table => table.Schema.Table.Schema)
                .Where(name => !string.IsNullOrWhiteSpace(name))
                .Distinct(StringComparer.Ordinal)
                .ToList();

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates the schemas of the tables that do not exist yet.
        /// </summary>
        /// <param name="connection">The connection of the destination database.</param>
        /// <param name="composer">The composer of the destination database.</param>
        /// <param name="tables">The tables of the plan.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="traceKey">The tracking key to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="transaction">The transaction to be used on the destination connection.</param>
        internal static void Create(IDbConnection connection,
            ISchemaComposer composer,
            IEnumerable<PlanTable> tables,
            int? commandTimeout,
            string traceKey,
            ITrace trace,
            IDbTransaction transaction)
        {
            if (!(composer is ISchemaCreationSupport support))
            {
                return;
            }

            foreach (var name in GetSchemaNames(tables))
            {
                CreateSchemaExtension.CreateSchema(connection,
                    name,
                    support.SchemaParameterPrefix,
                    support.SchemaExistsSql,
                    support.CreateSchemaSql,
                    support.QuoteSchemaName,
                    commandTimeout,
                    traceKey,
                    trace,
                    transaction);
            }
        }

        /// <summary>
        /// Creates the schemas of the tables that do not exist yet in an asynchronous way.
        /// </summary>
        /// <param name="connection">The connection of the destination database.</param>
        /// <param name="composer">The composer of the destination database.</param>
        /// <param name="tables">The tables of the plan.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="traceKey">The tracking key to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="transaction">The transaction to be used on the destination connection.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        internal static async Task CreateAsync(IDbConnection connection,
            ISchemaComposer composer,
            IEnumerable<PlanTable> tables,
            int? commandTimeout,
            string traceKey,
            ITrace trace,
            IDbTransaction transaction,
            CancellationToken cancellationToken)
        {
            if (!(composer is ISchemaCreationSupport support))
            {
                return;
            }

            foreach (var name in GetSchemaNames(tables))
            {
                cancellationToken.ThrowIfCancellationRequested();
                await CreateSchemaExtension.CreateSchemaAsync(connection,
                    name,
                    support.SchemaParameterPrefix,
                    support.SchemaExistsSql,
                    support.CreateSchemaSql,
                    support.QuoteSchemaName,
                    commandTimeout,
                    traceKey,
                    trace,
                    transaction,
                    cancellationToken).ConfigureAwait(false);
            }
        }

        #endregion
    }
}
