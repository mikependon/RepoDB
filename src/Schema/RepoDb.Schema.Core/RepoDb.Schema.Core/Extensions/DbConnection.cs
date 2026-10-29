#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RepoDb.Exceptions;
using RepoDb.Interfaces;
using RepoDb.Schema.Enumerations;
using RepoDb.Schema.Models;

namespace RepoDb.Schema
{
    /// <summary>
    /// Contains the helper methods for copying table schemas and extracting definitions, including destination checks, result reporting, and argument validation.
    /// </summary>
    internal static class DbConnectionExtension
    {
        #region Public Methods

        /// <summary>
        /// Extracts the selected table schemas and saves their definition in the requested format.
        /// </summary>
        /// <param name="connection">The source connection with a registered schema reader.</param>
        /// <param name="tableName">The source table, optionally schema-qualified.</param>
        /// <param name="filename">The output filename.</param>
        /// <param name="relationshipBehavior">The related tables to include.</param>
        /// <param name="commandTimeout">The catalog query timeout in seconds.</param>
        /// <param name="traceKey">The tracking key for catalog queries.</param>
        /// <param name="trace">The catalog query trace.</param>
        /// <param name="format">The output format: yaml, json, or xml.</param>
        internal static void SaveDefinition(IDbConnection connection,
            string tableName,
            string filename,
            RelatedTableBehavior relationshipBehavior,
            int? commandTimeout,
            string traceKey,
            ITrace trace,
            string format)
        {
            ValidateDefinitionArguments(connection,
                tableName,
                filename,
                relationshipBehavior,
                commandTimeout);
            var path = Path.GetFullPath(filename);
            var reader = SchemaReaderMapper.Get(connection) ?? throw new MissingMappingException($"There is no schema reader mapping for '{connection.GetType().FullName}'. Register one through SchemaReaderMapper.");
            string content;
            using (new SchemaReadContext(commandTimeout,
                traceKey,
                trace))
            {
                if (!reader.TableExists(tableName))
                    throw new InvalidOperationException($"The table '{tableName}' does not exist.");
                var root = reader.GetTableSchema(tableName);
                var names = relationshipBehavior == RelatedTableBehavior.TableOnly ? new[]
                {
                    tableName
                }

                : reader.GetRelatedTables(new[] { tableName },
                    relationshipBehavior);
                var tables = names.Select(name => string.Equals(name,
                    tableName,
                    StringComparison.Ordinal) ? root : reader.GetTableSchema(name)).Concat(new[] { root }).GroupBy(table => table.Table).Select(group => group.First()).ToList();
                var definition = SchemaDefinition.Create(root.Table,
                    tables,
                    relationshipBehavior);
                content = SchemaDefinitionSerializer.Serialize(definition,
                    format);
            }

            // Keep the temporary file on the same volume to allow atomic replacement.
            WriteDefinitionFile(path,
                content);
        }

        /// <summary>
        /// Asynchronously extracts and saves a definition, preserving the destination until the complete file is ready.
        /// </summary>
        /// <param name="connection">The source connection with a registered schema reader.</param>
        /// <param name="tableName">The source table, optionally schema-qualified.</param>
        /// <param name="filename">The output filename.</param>
        /// <param name="relationshipBehavior">The related tables to include.</param>
        /// <param name="commandTimeout">The catalog query timeout in seconds.</param>
        /// <param name="traceKey">The tracking key for catalog queries.</param>
        /// <param name="trace">The asynchronous catalog query trace.</param>
        /// <param name="format">The output format: yaml, json, or xml.</param>
        /// <param name="cancellationToken">The token for catalog queries and file writes.</param>
        internal static async Task SaveDefinitionAsync(IDbConnection connection,
            string tableName,
            string filename,
            RelatedTableBehavior relationshipBehavior,
            int? commandTimeout,
            string traceKey,
            ITrace trace,
            string format,
            CancellationToken cancellationToken)
        {
            ValidateDefinitionArguments(connection,
                tableName,
                filename,
                relationshipBehavior,
                commandTimeout);
            cancellationToken.ThrowIfCancellationRequested();
            var path = Path.GetFullPath(filename);
            var reader = SchemaReaderMapper.Get(connection) ?? throw new MissingMappingException($"There is no schema reader mapping for '{connection.GetType().FullName}'. Register one through SchemaReaderMapper.");
            string content;
            using (new SchemaReadContext(commandTimeout,
                traceKey,
                trace))
            {
                if (!await reader.TableExistsAsync(tableName,
                    cancellationToken).ConfigureAwait(false))
                    throw new InvalidOperationException($"The table '{tableName}' does not exist.");
                cancellationToken.ThrowIfCancellationRequested();
                var root = await reader.GetTableSchemaAsync(tableName,
                    cancellationToken).ConfigureAwait(false);
                cancellationToken.ThrowIfCancellationRequested();
                var names = relationshipBehavior == RelatedTableBehavior.TableOnly ? new[]
                {
                    tableName
                }

                : await reader.GetRelatedTablesAsync(new[] { tableName },
                    relationshipBehavior,
                    cancellationToken).ConfigureAwait(false);
                var tables = new List<TableSchema>();
                foreach (var name in names)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    tables.Add(string.Equals(name,
                        tableName,
                        StringComparison.Ordinal) ? root : await reader.GetTableSchemaAsync(name,
                            cancellationToken).ConfigureAwait(false));
                }

                tables.Add(root);
                cancellationToken.ThrowIfCancellationRequested();
                var definition = SchemaDefinition.Create(root.Table,
                    tables.GroupBy(table => table.Table).Select(group => group.First()).ToList(),
                    relationshipBehavior);
                content = SchemaDefinitionSerializer.Serialize(definition,
                    format);
            }

            await WriteDefinitionFileAsync(path,
                content,
                cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the schema of the destination database that the tables are created in: the given one, or the default schema of the setting of the destination connection (see <see cref="IDbSetting.DefaultSchema"/>).
        /// </summary>
        /// <param name="targetSchema">The given schema, or <c>null</c>.</param>
        /// <param name="destinationConnection">The connection of the destination database.</param>
        /// <returns>The schema (<c>null</c> if it is not given, and the setting has no default schema).</returns>
        internal static string GetTargetSchema(string targetSchema,
            IDbConnection destinationConnection)
        {
            var schema = string.IsNullOrWhiteSpace(targetSchema) ? destinationConnection.GetDbSetting().DefaultSchema : targetSchema;
            return string.IsNullOrWhiteSpace(schema) ? null : schema;
        }

        /// <summary>
        /// Reads the state of the tables in the destination database: whether each one exists and, when the existing tables are aligned,
        /// the columns, the indexes and the foreign keys that they are missing. A statement that the composer does not compose leaves the state unknown.
        /// </summary>
        /// <param name="tables">The state of the tables.</param>
        /// <param name="behavior">The behavior that is requested for the tables that already exist.</param>
        /// <param name="composer">The composer of the destination database.</param>
        /// <param name="destinationConnection">The connection of the destination database.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="transaction">The transaction to be used on the destination connection.</param>
        /// <param name="traceKey">The tracking key for file-load probes; null keeps the execute method's default.</param>
        [UnconditionalSuppressMessage("Trimming",
            "IL2026:RequiresUnreferencedCode",
            Justification = "No parameter object is passed to the RepoDB execute methods.")]
        internal static void ProbeTables(IEnumerable<PlanTable> tables,
            TableExistsBehavior behavior,
            ISchemaComposer composer,
            IDbConnection destinationConnection,
            int? commandTimeout,
            ITrace trace,
            IDbTransaction transaction,
            string traceKey = null)
        {
            bool? Probe(string statement) => string.IsNullOrWhiteSpace(statement) ? (bool? )null : ToBoolean(destinationConnection.ExecuteScalar(statement,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                traceKey: traceKey ?? "ExecuteScalar"));
            // A foreign key is added to a table that exists only if the table that it references is copied, or already exists (a table is not created to be referenced)
            bool CanBeReferenced(ForeignKeyInfo foreignKey)
            {
                if (foreignKey.ReferencedTable == null)
                {
                    return true;
                }

                var name = composer.ComposeName(foreignKey.ReferencedTable);
                return tables.Any(item => string.Equals(item.Name,
                    name,
                    StringComparison.Ordinal)) || Probe(composer.ComposeTableExists(name)) == true;
            }

            foreach (var table in tables)
            {
                table.Exists = Probe(composer.ComposeTableExists(table.Name));
                if (table.Exists == true && behavior == TableExistsBehavior.Align)
                {
                    table.MissingColumns = table.Schema.Columns.Where(column => Probe(composer.ComposeColumnExists(table.Name,
                        column.Field.Name)) == false).ToList();
                    table.MissingIndexes = table.Schema.Indexes.Where(index => index.Name != null && Probe(composer.ComposeIndexExists(table.Name,
                        index.Name)) == false).ToList();
                    table.MissingForeignKeys = table.Schema.ForeignKeys.Where(foreignKey => foreignKey.Name != null && Probe(composer.ComposeForeignKeyExists(table.Name,
                        foreignKey.Name)) == false && CanBeReferenced(foreignKey)).ToList();
                }
            }

            // The tables that are created are created without what the destination refuses: the foreign keys that reference a table that is not copied and does not exist,
            // and the check constraints that the composer does not accept
            foreach (var table in tables.Where(table => table.Exists != true || behavior == TableExistsBehavior.Drop).ToList())
            {
                SkipUnsupported(table,
                    table.Schema.ForeignKeys.Where(foreignKey => !CanBeReferenced(foreignKey)).ToList(),
                    composer);
            }

            IndexNameResolver.Resolve(tables,
                behavior,
                composer,
                destinationConnection.GetDbSetting()?.AreIndexNamesUniquePerSchema == true,
                Probe);
        }

        /// <summary>
        /// Reads the state of the tables in the destination database (see <see cref="ProbeTables"/>).
        /// </summary>
        /// <param name="tables">The state of the tables.</param>
        /// <param name="behavior">The behavior that is requested for the tables that already exist.</param>
        /// <param name="composer">The composer of the destination database.</param>
        /// <param name="destinationConnection">The connection of the destination database.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="transaction">The transaction to be used on the destination connection.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <param name="traceKey">The tracking key for file-load probes; null keeps the execute method's default.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        [UnconditionalSuppressMessage("Trimming",
            "IL2026:RequiresUnreferencedCode",
            Justification = "No parameter object is passed to the RepoDB execute methods.")]
        internal static async Task ProbeTablesAsync(IEnumerable<PlanTable> tables,
            TableExistsBehavior behavior,
            ISchemaComposer composer,
            IDbConnection destinationConnection,
            int? commandTimeout,
            ITrace trace,
            IDbTransaction transaction,
            CancellationToken cancellationToken,
            string traceKey = null)
        {
            async Task<bool?> Probe(string statement) => string.IsNullOrWhiteSpace(statement) ? (bool? )null : ToBoolean(await destinationConnection.ExecuteScalarAsync(statement,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                cancellationToken: cancellationToken,
                traceKey: traceKey ?? "ExecuteScalar").ConfigureAwait(false));
            // A foreign key is added to a table that exists only if the table that it references is copied, or already exists (a table is not created to be referenced)
            async Task<bool> CanBeReferenced(ForeignKeyInfo foreignKey)
            {
                if (foreignKey.ReferencedTable == null)
                {
                    return true;
                }

                var name = composer.ComposeName(foreignKey.ReferencedTable);
                return tables.Any(item => string.Equals(item.Name,
                    name,
                    StringComparison.Ordinal)) || await Probe(composer.ComposeTableExists(name)).ConfigureAwait(false) == true;
            }

            foreach (var table in tables)
            {
                table.Exists = await Probe(composer.ComposeTableExists(table.Name)).ConfigureAwait(false);
                if (table.Exists == true && behavior == TableExistsBehavior.Align)
                {
                    await FindMissingObjectsAsync(table,
                        composer,
                        Probe,
                        CanBeReferenced).ConfigureAwait(false);
                }
            }

            foreach (var table in tables.Where(table => table.Exists != true || behavior == TableExistsBehavior.Drop).ToList())
            {
                var unreferenced = new List<ForeignKeyInfo>();
                foreach (var foreignKey in table.Schema.ForeignKeys)
                {
                    if (!await CanBeReferenced(foreignKey).ConfigureAwait(false))
                    {
                        unreferenced.Add(foreignKey);
                    }
                }

                SkipUnsupported(table,
                    unreferenced,
                    composer);
            }

            await IndexNameResolver.ResolveAsync(tables,
                behavior,
                composer,
                destinationConnection.GetDbSetting()?.AreIndexNamesUniquePerSchema == true,
                Probe).ConfigureAwait(false);
        }

        /// <summary>
        /// Leaves out of the schema of a table that is created the objects that the destination database refuses, and reports each one as a warning of the table:
        /// the foreign keys that reference a table that is not copied and does not exist in the destination, and the check constraints that the composer does not accept
        /// (see <see cref="ICheckConstraintSupport"/>). The default values that the composer does not create (see <see cref="IColumnDefaultSupport"/>) are reported too.
        /// </summary>
        /// <param name="table">The state of the table.</param>
        /// <param name="foreignKeys">The foreign keys of the table that reference a table that is not copied and does not exist in the destination.</param>
        /// <param name="composer">The composer of the destination database.</param>
        internal static void SkipUnsupported(PlanTable table,
            IList<ForeignKeyInfo> foreignKeys,
            ISchemaComposer composer)
        {
            // The default values that the destination can not create are not left out here (the composer does not write them): they are reported
            if (composer is IColumnDefaultSupport defaultSupport)
            {
                foreach (var column in table.Schema.Columns)
                {
                    var defaultReason = defaultSupport.GetUnsupportedDefaultReason(column);
                    if (defaultReason != null)
                    {
                        table.Warnings.Add($"Skipped the default value of the column '{column.Field?.Name}': {defaultReason}.");
                    }
                }
            }

            var support = composer as ICheckConstraintSupport;
            var checkConstraints = support == null ? new List<KeyValuePair<CheckConstraintInfo, string>>() : table.Schema.CheckConstraints.Select(checkConstraint => new KeyValuePair<CheckConstraintInfo, string>(checkConstraint,
                support.GetUnsupportedReason(checkConstraint))).Where(pair => pair.Value != null).ToList();
            if (foreignKeys.Count == 0 && checkConstraints.Count == 0)
            {
                return;
            }

            var schema = table.Schema;
            table.Schema = new TableSchema(schema.Table)
            {
                Columns = schema.Columns,
                PrimaryKey = schema.PrimaryKey,
                Indexes = schema.Indexes,
                UniqueConstraints = schema.UniqueConstraints,
                CheckConstraints = schema.CheckConstraints.Where(checkConstraint => checkConstraints.All(pair => !ReferenceEquals(pair.Key,
                    checkConstraint))).ToList(),
                ForeignKeys = schema.ForeignKeys.Where(foreignKey => !foreignKeys.Contains(foreignKey)).ToList()
            };
            foreach (var foreignKey in foreignKeys)
            {
                table.Warnings.Add($"Skipped the foreign key '{foreignKey.Name}': the table '{foreignKey.ReferencedTable.Name}' is not part of the copy and does not exist in the destination.");
            }

            foreach (var pair in checkConstraints)
            {
                table.Warnings.Add($"Skipped the check constraint '{pair.Key.Name}': {pair.Value}.");
            }
        }

        /// <summary>
        /// Converts the result of an existence statement (<c>1</c> if it exists).
        /// </summary>
        /// <param name="result">The result of the statement.</param>
        /// <returns><c>true</c> if it exists; otherwise, <c>false</c>.</returns>
        internal static bool ToBoolean(object result) =>
            result != null && result != DBNull.Value && Convert.ToInt32(result) == 1;

        /// <summary>
        /// Creates the error of a statement that has failed.
        /// </summary>
        /// <param name="exception">The exception that was thrown.</param>
        /// <param name="step">The step that has failed.</param>
        /// <param name="statementIndex">The position of the statement in the plan.</param>
        /// <returns>The error.</returns>
        internal static ExecutionError CreateError(Exception exception,
            PlanStep step,
            int statementIndex) =>
            new ExecutionError
        {
            Exception = exception,
            Statement = step.Statement,
            StatementIndex = statementIndex,
            TableName = step.Owner?.Schema.Table?.Name,
            SchemaName = step.Owner?.Schema.Table?.Schema
        };

        /// <summary>
        /// Gets the script that was (or is going to be) executed for the table.
        /// </summary>
        /// <param name="table">The state of the table.</param>
        /// <param name="schemaComposer">The composer of the destination database.</param>
        /// <returns>The statements of the script.</returns>
        internal static IEnumerable<string> GetScript(PlanTable table,
            ISchemaComposer schemaComposer)
        {
            switch (table.Outcome)
            {
                case TableOutcome.Skipped:
                    return Enumerable.Empty<string>();
                case TableOutcome.Aligned:
                    return table.AlignStatements;
                default:
                    var create = schemaComposer.ComposeSchema(table.Schema) ?? Enumerable.Empty<string>();
                    return table.DropStatement == null ? create : new[]
                    {
                        table.DropStatement
                    }.Concat(create);
            }
        }

        /// <summary>
        /// Adds the item of each table that is completed to the items, and calls the callback with it.
        /// </summary>
        /// <param name="createdCallback">The callback that receives the item of each completed table, or <c>null</c>.</param>
        /// <param name="items">The items of the tables that were completed.</param>
        /// <param name="tables">The state of the tables that are completed.</param>
        /// <param name="errors">The errors that were raised so far.</param>
        /// <param name="connection">The connection of the source database.</param>
        /// <param name="destinationConnection">The connection of the destination database.</param>
        /// <param name="tableExistenceBehavior">The behavior that was requested for the tables that already exist.</param>
        /// <param name="schemaComposer">The composer of the destination database.</param>
        internal static void Report(Action<CopyTableSchemaResultItem> createdCallback,
            IList<CopyTableSchemaResultItem> items,
            IEnumerable<PlanTable> tables,
            IList<ExecutionError> errors,
            IDbConnection connection,
            IDbConnection destinationConnection,
            TableExistsBehavior tableExistenceBehavior,
            ISchemaComposer schemaComposer)
        {
            foreach (var table in tables)
            {
                var schema = table.Schema;
                var aligned = table.Outcome == TableOutcome.Aligned;
                var schemaErrors = errors.Where(error => error.TableName == null || (string.Equals(error.TableName,
                    schema.Table.Name,
                    StringComparison.Ordinal) && string.Equals(error.SchemaName,
                        schema.Table.Schema,
                        StringComparison.Ordinal))).ToList();
                var item = new CopyTableSchemaResultItem
                {
                    Action = tableExistenceBehavior,
                    Outcome = schemaErrors.Count > 0 ? TableOutcome.Failed : table.Outcome,
                    TableExisted = table.Exists,
                    AddedColumns = aligned ? table.MissingColumns.Select(column => column.Field.Name).ToList() : new List<string>(),
                    AddedIndexes = aligned ? table.MissingIndexes.Select(index => index.Name).ToList() : new List<string>(),
                    AddedForeignKeys = aligned ? table.MissingForeignKeys.Select(foreignKey => foreignKey.Name).ToList() : new List<string>(),
                    Errors = schemaErrors,
                    Warnings = table.Warnings.ToList(),
                    Source = new Endpoint
                    {
                        Table = table.Original,
                        Database = connection == null ? table.Original.Table.Database : connection.Database,
                        Server = (connection as System.Data.Common.DbConnection)?.DataSource,
                        DatabaseType = connection == null ? table.Original.Table.Provider : connection.GetType().Name
                    },
                    Target = new Endpoint
                    {
                        Table = schema,
                        Database = destinationConnection.Database,
                        Server = (destinationConnection as System.Data.Common.DbConnection)?.DataSource,
                        DatabaseType = destinationConnection.GetType().Name
                    },
                    Script = string.Join(Environment.NewLine,
                        GetScript(table,
                            schemaComposer)),
                    ColumnCount = schema.Columns.Count,
                    IndexCount = schema.Indexes.Count,
                    ForeignKeyCount = schema.ForeignKeys.Count,
                    UniqueConstraintCount = schema.UniqueConstraints.Count,
                    CheckConstraintCount = schema.CheckConstraints.Count
                };
                items.Add(item);
                createdCallback?.Invoke(item);
            }
        }

        /// <summary>
        /// Creates the result of the schema copy operation from the items of the tables that were copied.
        /// </summary>
        /// <param name="startTime">The time (in UTC) when the operation has started.</param>
        /// <param name="items">The items of the tables that were copied.</param>
        /// <param name="errors">The errors that were raised.</param>
        /// <returns>The result.</returns>
        internal static CopyTableSchemaResult CreateResult(DateTime startTime,
            IList<CopyTableSchemaResultItem> items,
            IList<ExecutionError> errors) =>
            new CopyTableSchemaResult
        {
            StartTime = startTime,
            EndTime = DateTime.UtcNow,
            Items = items,
            Errors = errors,
            Warnings = items.SelectMany(item => item.Warnings).ToList()
        };

        /// <summary>
        /// Validates the arguments of the copy of a single table.
        /// </summary>
        /// <param name="connection">The source connection.</param>
        /// <param name="tableName">The name of the table.</param>
        /// <param name="destinationConnection">The connection of the destination database.</param>
        /// <exception cref="ArgumentNullException"></exception>
        internal static void Validate(IDbConnection connection,
            string tableName,
            IDbConnection destinationConnection)
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }

            if (string.IsNullOrWhiteSpace(tableName))
            {
                throw new ArgumentNullException(nameof(tableName));
            }

            if (destinationConnection == null)
            {
                throw new ArgumentNullException(nameof(destinationConnection));
            }
        }

        /// <summary>
        /// Validates the arguments of the copy of several tables.
        /// </summary>
        /// <param name="connection">The source connection.</param>
        /// <param name="tableNames">The names of the tables.</param>
        /// <param name="destinationConnection">The connection of the destination database.</param>
        /// <returns>The names of the tables, as a list.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        internal static IList<string> Validate(IDbConnection connection,
            IEnumerable<string> tableNames,
            IDbConnection destinationConnection)
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }

            if (tableNames == null)
            {
                throw new ArgumentNullException(nameof(tableNames));
            }

            if (destinationConnection == null)
            {
                throw new ArgumentNullException(nameof(destinationConnection));
            }

            var names = tableNames.ToList();
            if (names.Any(string.IsNullOrWhiteSpace))
            {
                throw new ArgumentException("The table names cannot contain a null or a white-space name.",
                    nameof(tableNames));
            }

            return names;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Finds columns, indexes, and foreign keys missing from an existing destination table.
        /// </summary>
        /// <param name="table">The table whose missing objects are populated.</param>
        /// <param name="composer">The destination SQL composer.</param>
        /// <param name="probe">Executes an existence query with the caller's transaction and cancellation.</param>
        /// <param name="canBeReferenced">Checks whether a foreign-key target is available.</param>
        /// <returns>A task representing completion of the alignment probes.</returns>
        private static async Task FindMissingObjectsAsync(PlanTable table,
            ISchemaComposer composer,
            Func<string, Task<bool?>> probe,
            Func<ForeignKeyInfo, Task<bool>> canBeReferenced)
        {
            table.MissingColumns = new List<ColumnInfo>();
            foreach (var column in table.Schema.Columns)
            {
                if (await probe(composer.ComposeColumnExists(table.Name,
                    column.Field.Name)).ConfigureAwait(false) == false)
                {
                    table.MissingColumns.Add(column);
                }
            }

            table.MissingIndexes = new List<IndexInfo>();
            foreach (var index in table.Schema.Indexes.Where(index => index.Name != null))
            {
                if (await probe(composer.ComposeIndexExists(table.Name,
                    index.Name)).ConfigureAwait(false) == false)
                {
                    table.MissingIndexes.Add(index);
                }
            }

            table.MissingForeignKeys = new List<ForeignKeyInfo>();
            foreach (var foreignKey in table.Schema.ForeignKeys.Where(foreignKey => foreignKey.Name != null))
            {
                if (await probe(composer.ComposeForeignKeyExists(table.Name,
                    foreignKey.Name)).ConfigureAwait(false) == false && await canBeReferenced(foreignKey).ConfigureAwait(false))
                {
                    table.MissingForeignKeys.Add(foreignKey);
                }
            }
        }

        /// <summary>
        /// Validates the shared arguments before reading a catalog or writing a definition.
        /// </summary>
        /// <param name="connection">The source connection with a registered schema reader.</param>
        /// <param name="tableName">The source table, optionally schema-qualified.</param>
        /// <param name="filename">The output filename.</param>
        /// <param name="relationshipBehavior">The related tables to include.</param>
        /// <param name="commandTimeout">The catalog query timeout in seconds.</param>
        private static void ValidateDefinitionArguments(IDbConnection connection,
            string tableName,
            string filename,
            RelatedTableBehavior relationshipBehavior,
            int? commandTimeout)
        {
            if (connection == null)
                throw new ArgumentNullException(nameof(connection));
            if (string.IsNullOrWhiteSpace(tableName))
                throw new ArgumentException("A table name is required.",
                    nameof(tableName));
            if (string.IsNullOrWhiteSpace(filename))
                throw new ArgumentException("A filename is required.",
                    nameof(filename));
            if (!Enum.IsDefined(typeof(RelatedTableBehavior),
                relationshipBehavior))
                throw new ArgumentOutOfRangeException(nameof(relationshipBehavior));
            if (commandTimeout < 0)
                throw new ArgumentOutOfRangeException(nameof(commandTimeout));
        }

        /// <summary>
        /// Writes a complete definition to a temporary file and commits it atomically.
        /// </summary>
        /// <param name="path">The absolute destination filename.</param>
        /// <param name="content">The complete serialized definition.</param>
        private static void WriteDefinitionFile(string path,
            string content)
        {
            // Keep the temporary file on the same volume to allow atomic replacement.
            var temporary = Path.Combine(Path.GetDirectoryName(path),
                ".repodb-" + Guid.NewGuid().ToString("N") + ".tmp");
            try
            {
                using (var stream = new FileStream(temporary,
                    FileMode.CreateNew,
                    FileAccess.Write,
                    FileShare.None))
                using (var writer = new StreamWriter(stream,
                    new UTF8Encoding(false,
                        true)))
                    writer.Write(content);
                if (File.Exists(path))
                    File.Replace(temporary,
                        path,
                        null);
                else
                    File.Move(temporary,
                        path);
            }
            finally
            {
                if (File.Exists(temporary))
                    File.Delete(temporary);
            }
        }

        /// <summary>
        /// Writes a complete definition to a temporary file and commits it atomically with cancellation before the commit.
        /// </summary>
        /// <param name="path">The absolute destination filename.</param>
        /// <param name="content">The complete serialized definition.</param>
        /// <param name="cancellationToken">The token for catalog queries and file writes.</param>
        /// <returns>A task representing the temporary write and commit.</returns>
        private static async Task WriteDefinitionFileAsync(string path,
            string content,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var bytes = new UTF8Encoding(false,
                true).GetBytes(content);
            var temporary = Path.Combine(Path.GetDirectoryName(path),
                ".repodb-" + Guid.NewGuid().ToString("N") + ".tmp");
            try
            {
                using (var stream = new FileStream(temporary,
                    FileMode.CreateNew,
                    FileAccess.Write,
                    FileShare.None,
                    4096,
                    FileOptions.Asynchronous | FileOptions.SequentialScan))
                {
                    await stream.WriteAsync(bytes,
                        0,
                        bytes.Length,
                        cancellationToken).ConfigureAwait(false);
                    await stream.FlushAsync(cancellationToken).ConfigureAwait(false);
                }

                // Cancellation is honored before committing the completed file, never after it is committed.
                cancellationToken.ThrowIfCancellationRequested();
                if (File.Exists(path))
                    File.Replace(temporary,
                        path,
                        null);
                else
                    File.Move(temporary,
                        path);
            }
            finally
            {
                if (File.Exists(temporary))
                    File.Delete(temporary);
            }
        }

        #endregion
    }
}
