#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using RepoDb.Exceptions;
using RepoDb.Interfaces;
using RepoDb.Schema.Enumerations;
using RepoDb.Schema.Models;

namespace RepoDb.Schema
{
    /// <summary>
    /// Applies an already-read file definition through the shared schema copy execution path.
    /// </summary>
    internal static class SchemaDefinitionLoader
    {
        #region Public Methods

        /// <summary>
        /// Validates arguments before reading the file or accessing the destination.
        /// </summary>
        /// <param name="connection">The connection of the destination database.</param>
        /// <param name="filename">The name of the definition file.</param>
        /// <param name="tableExistenceBehavior">Defines what happens when a table already exists in the destination database.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used on the destination connection.</param>
        internal static void Validate(IDbConnection connection,
            string filename,
            TableExistsBehavior tableExistenceBehavior,
            int? commandTimeout,
            IDbTransaction transaction,
            SchemaMappingMode schemaMappingMode = SchemaMappingMode.Preserve)
        {
            if (!Enum.IsDefined(typeof(SchemaMappingMode), schemaMappingMode))
                throw new ArgumentOutOfRangeException(nameof(schemaMappingMode));
            if (connection == null)
                throw new ArgumentNullException(nameof(connection));
            if (string.IsNullOrWhiteSpace(filename))
                throw new ArgumentException("A filename is required.",
                    nameof(filename));
            if (!Enum.IsDefined(typeof(TableExistsBehavior),
                tableExistenceBehavior))
                throw new ArgumentOutOfRangeException(nameof(tableExistenceBehavior));
            if (commandTimeout < 0)
                throw new ArgumentOutOfRangeException(nameof(commandTimeout));
            if (transaction != null && !ReferenceEquals(transaction.Connection,
                connection))
                throw new ArgumentException("The transaction must belong to the destination connection.",
                    nameof(transaction));
        }

        /// <summary>
        /// Applies all included tables, reporting each completion just as a schema copy does.
        /// </summary>
        /// <param name="connection">The connection of the destination database.</param>
        /// <param name="definition">The definition that is read from the file (nothing is applied if it is <c>null</c>).</param>
        /// <param name="tableExistenceBehavior">Defines what happens when a table already exists in the destination database.</param>
        /// <param name="createdCallback">The callback that receives the item of each table once its schema is created.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="traceKey">The tracking key to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="transaction">The transaction to be used on the destination connection.</param>
        /// <returns>The result of the load, or <c>null</c> if there is no definition.</returns>
        internal static LoadDefinitionResult Apply(IDbConnection connection,
            SchemaDefinitionInfo definition,
            TableExistsBehavior tableExistenceBehavior,
            Action<CopyTableSchemaResultItem> createdCallback,
            int? commandTimeout,
            string traceKey,
            ITrace trace,
            IDbTransaction transaction,
            SchemaMappingMode schemaMappingMode = SchemaMappingMode.Preserve)
        {
            if (definition == null)
                return null;
            var startTime = DateTime.UtcNow;
            var composer = Composer(connection);
            var tables = CreateTables(definition,
                composer, connection.GetDbSetting(), schemaMappingMode);
            var copy = SchemaPlanExecutor.Execute(startTime,
                tables,
                composer,
                null,
                connection,
                tableExistenceBehavior,
                createdCallback,
                null,
                commandTimeout,
                traceKey,
                trace,
                transaction);
            return new LoadDefinitionResult
            {
                Definition = definition,
                CopyResult = copy
            };
        }

        /// <summary>
        /// Asynchronously applies all included tables with cancellation and asynchronous database tracing.
        /// </summary>
        /// <param name="connection">The connection of the destination database.</param>
        /// <param name="definition">The definition that is read from the file (nothing is applied if it is <c>null</c>).</param>
        /// <param name="tableExistenceBehavior">Defines what happens when a table already exists in the destination database.</param>
        /// <param name="createdCallback">The callback that receives the item of each table once its schema is created.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="traceKey">The tracking key to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="transaction">The transaction to be used on the destination connection.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is the result of the load, or <c>null</c> if there is no definition.</returns>
        internal static async Task<LoadDefinitionResult> ApplyAsync(IDbConnection connection,
            SchemaDefinitionInfo definition,
            TableExistsBehavior tableExistenceBehavior,
            Action<CopyTableSchemaResultItem> createdCallback,
            int? commandTimeout,
            string traceKey,
            ITrace trace,
            IDbTransaction transaction,
            CancellationToken cancellationToken,
            SchemaMappingMode schemaMappingMode = SchemaMappingMode.Preserve)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (definition == null)
                return null;
            var startTime = DateTime.UtcNow;
            var composer = Composer(connection);
            var tables = CreateTables(definition,
                composer, connection.GetDbSetting(), schemaMappingMode);
            var copy = await SchemaPlanExecutor.ExecuteAsync(startTime,
                tables,
                composer,
                null,
                connection,
                tableExistenceBehavior,
                createdCallback,
                null,
                commandTimeout,
                traceKey,
                trace,
                transaction,
                cancellationToken).ConfigureAwait(false);
            return new LoadDefinitionResult
            {
                Definition = definition,
                CopyResult = copy
            };
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets the registered destination composer; a source schema reader is not required.
        /// </summary>
        /// <param name="connection">The connection of the destination database.</param>
        /// <returns>The schema composer that is mapped to the connection.</returns>
        private static ISchemaComposer Composer(IDbConnection connection) =>
            SchemaComposerMapper.Get(connection) ?? throw new MissingMappingException($"There is no schema composer mapping found for '{connection.GetType().FullName}'. Register one via SchemaComposerMapper.");

        /// <summary>
        /// Uses the shared cycle-aware dependency orderer and preserves the schema identities saved in the file.
        /// External dependencies are not expanded into additional tables.
        /// </summary>
        /// <param name="definition">The definition that is read from the file.</param>
        /// <param name="composer">The composer of the destination database.</param>
        /// <returns>The tables of the plan, in dependency order.</returns>
        private static System.Collections.Generic.IList<PlanTable> CreateTables(SchemaDefinitionInfo definition,
            ISchemaComposer composer,
            IDbSetting setting,
            SchemaMappingMode schemaMappingMode)
        {
            SchemaDefinitionMapping.Validate(definition, setting, schemaMappingMode);
            var keys = definition.Tables.Select((table,
                position) => new { table.Table, Key = position.ToString(CultureInfo.InvariantCulture) }).ToDictionary(item => item.Table,
                    item => item.Key);
            var ordered = TableOrderer.Order(definition.Tables,
                table => keys[table.Table],
                table => table != null && keys.TryGetValue(table,
                    out var key) ? key : "external");
            var tables = ordered.Select(item =>
            {
                var source = item.Schema;
                var target = SchemaDefinitionMapping.Map(source, schemaMappingMode);
                return new PlanTable(source, target, composer.ComposeName(target.Table));
            }).ToList();
            var duplicate = tables.GroupBy(table => table.Name, StringComparer.OrdinalIgnoreCase)
                .FirstOrDefault(group => group.Count() > 1);
            if (duplicate != null)
                throw new InvalidOperationException($"Multiple source tables map to destination name '{duplicate.Key}' using {schemaMappingMode}.");
            return tables;
        }

        #endregion
    }
}
