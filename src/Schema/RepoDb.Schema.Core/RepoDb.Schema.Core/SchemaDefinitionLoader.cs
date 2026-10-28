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
        #region Methods

        /// <summary>
        /// Validates arguments before reading the file or accessing the destination.
        /// </summary>
        internal static void Validate(IDbConnection connection, string filename,
            TableExistsBehavior tableExistenceBehavior, int? commandTimeout, IDbTransaction transaction)
        {
            if (connection == null) throw new ArgumentNullException(nameof(connection));
            if (string.IsNullOrWhiteSpace(filename)) throw new ArgumentException("A filename is required.", nameof(filename));
            if (!Enum.IsDefined(typeof(TableExistsBehavior), tableExistenceBehavior)) throw new ArgumentOutOfRangeException(nameof(tableExistenceBehavior));
            if (commandTimeout < 0) throw new ArgumentOutOfRangeException(nameof(commandTimeout));
            if (transaction != null && !ReferenceEquals(transaction.Connection, connection))
                throw new ArgumentException("The transaction must belong to the destination connection.", nameof(transaction));
        }

        /// <summary>
        /// Applies all included tables, reporting each completion just as a schema copy does.
        /// </summary>
        internal static LoadDefinitionResult Apply(IDbConnection connection, SchemaDefinitionResult definition,
            TableExistsBehavior tableExistenceBehavior, Action<CopyTableSchemaResultItem> createdCallback,
            int? commandTimeout, string traceKey, ITrace trace, IDbTransaction transaction)
        {
            if (definition == null) return null;
            var startTime = DateTime.UtcNow;
            var composer = Composer(connection);
            var tables = CreateTables(definition, composer);
            var copy = SchemaPlanExecutor.Execute(startTime, tables, composer, null, connection,
                tableExistenceBehavior, createdCallback, null, commandTimeout, traceKey, trace, transaction);
            return new LoadDefinitionResult { Definition = definition, CopyResult = copy };
        }

        /// <summary>
        /// Asynchronously applies all included tables with cancellation and asynchronous database tracing.
        /// </summary>
        internal static async Task<LoadDefinitionResult> ApplyAsync(IDbConnection connection, SchemaDefinitionResult definition,
            TableExistsBehavior tableExistenceBehavior, Action<CopyTableSchemaResultItem> createdCallback,
            int? commandTimeout, string traceKey, ITrace trace, IDbTransaction transaction, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (definition == null) return null;
            var startTime = DateTime.UtcNow;
            var composer = Composer(connection);
            var tables = CreateTables(definition, composer);
            var copy = await SchemaPlanExecutor.ExecuteAsync(startTime, tables, composer, null, connection,
                tableExistenceBehavior, createdCallback, null, commandTimeout, traceKey, trace, transaction, cancellationToken).ConfigureAwait(false);
            return new LoadDefinitionResult { Definition = definition, CopyResult = copy };
        }

        /// <summary>
        /// Gets the registered destination composer; a source schema reader is not required.
        /// </summary>
        private static ISchemaComposer Composer(IDbConnection connection) =>
            SchemaComposerMapper.Get(connection) ?? throw new MissingMappingException(
                $"There is no schema composer mapping found for '{connection.GetType().FullName}'. Register one via SchemaComposerMapper.");

        /// <summary>
        /// Uses the shared cycle-aware dependency orderer and preserves the schema identities saved in the file.
        /// External dependencies are not expanded into additional tables.
        /// </summary>
        private static System.Collections.Generic.IList<PlanTable> CreateTables(SchemaDefinitionResult definition, ISchemaComposer composer)
        {
            var keys = definition.Tables.Select((table, position) => new
            {
                table.Table, Key = position.ToString(CultureInfo.InvariantCulture)
            }).ToDictionary(item => item.Table, item => item.Key);
            var ordered = TableOrderer.Order(definition.Tables, table => keys[table.Table],
                table => table != null && keys.TryGetValue(table, out var key) ? key : "external");
            return Plan.CreateTables(ordered.Select(item => item.Schema), null, composer);
        }

        #endregion
    }
}
