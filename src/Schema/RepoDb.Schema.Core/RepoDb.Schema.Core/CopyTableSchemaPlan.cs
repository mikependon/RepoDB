#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using RepoDb.Schema.Enumerations;
using RepoDb.Schema.Models;

namespace RepoDb.Schema
{
    /// <summary>
    /// A class that holds what a schema copy is going to do to the tables of the destination database: the statements to be executed,
    /// the table that each statement belongs to, and the statement that completes each table. It is created from the state of the
    /// destination tables (see <see cref="CopyTableSchemaTable"/>) and the <see cref="CopyTableSchemaExistsBehavior"/>.
    /// </summary>
    internal sealed class CopyTableSchemaPlan
    {
        #region Constructors

        private CopyTableSchemaPlan(IList<CopyTableSchemaStep> steps,
            ILookup<int, CopyTableSchemaTable> completions,
            IList<CopyTableSchemaTable> immediate)
        {
            Steps = steps;
            Completions = completions;
            Immediate = immediate;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the statements to be executed, in order.
        /// </summary>
        public IList<CopyTableSchemaStep> Steps { get; }

        /// <summary>
        /// Gets the tables that are completed by each step (by the index of the step). The key <c>-1</c> has the tables whose steps are not known,
        /// as the script that was composed does not have the expected number of statements.
        /// </summary>
        public ILookup<int, CopyTableSchemaTable> Completions { get; }

        /// <summary>
        /// Gets the tables that have nothing to be executed, so they are complete from the start.
        /// </summary>
        public IList<CopyTableSchemaTable> Immediate { get; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates the state of each table of the schemas.
        /// </summary>
        /// <param name="schemas">The schemas of the tables, in the order that they must be created.</param>
        /// <param name="targetSchema">The schema of the destination database that the tables are created in. <c>null</c> keeps the schema of each source table.</param>
        /// <param name="composer">The composer of the destination database.</param>
        /// <returns>The state of each table.</returns>
        /// <exception cref="InvalidOperationException">Two tables would have the same name in the destination database.</exception>
        public static IList<CopyTableSchemaTable> CreateTables(IEnumerable<TableSchema> schemas,
            string targetSchema,
            ISchemaComposer composer)
        {
            var tables = schemas
                .Select(source => CreateTable(source, MoveTo(source, targetSchema), composer))
                .ToList();
            EnsureUniqueNames(tables);
            return tables;
        }

        /// <summary>
        /// Creates the plan for the tables, from their state in the destination database.
        /// </summary>
        /// <param name="tables">The state of the tables (see <see cref="CopyTableSchemaTable.Exists"/>, <see cref="CopyTableSchemaTable.MissingColumns"/>, <see cref="CopyTableSchemaTable.MissingIndexes"/> and <see cref="CopyTableSchemaTable.MissingForeignKeys"/>).</param>
        /// <param name="behavior">Defines what happens to the tables that already exist in the destination database.</param>
        /// <param name="composer">The composer of the destination database.</param>
        /// <returns>The plan.</returns>
        /// <exception cref="InvalidOperationException">The behavior is <see cref="CopyTableSchemaExistsBehavior.Throw"/> and a table already exists.</exception>
        public static CopyTableSchemaPlan Create(IList<CopyTableSchemaTable> tables,
            CopyTableSchemaExistsBehavior behavior,
            ISchemaComposer composer)
        {
            SetOutcomes(tables, behavior);
            var created = tables.Where(table => table.Exists != true || behavior == CopyTableSchemaExistsBehavior.Drop).ToList();
            var steps = GetDropSteps(tables, composer).Concat(GetAlignSteps(tables, composer)).ToList();
            var createSteps = GetCreateSteps(created, composer);
            var foreignKeyOffset = steps.Count + createSteps.Count;
            var completions = GetAlignCompletions(tables, foreignKeyOffset)
                .Concat(GetCreateCompletions(created, createSteps.Count, steps.Count))
                .ToLookup(x => x.Key, x => x.Table);
            var owners = GetCreateOwners(created, createSteps.Count);
            steps.AddRange(createSteps.Select((statement, i) => new CopyTableSchemaStep(statement, owners[i])));
            steps.AddRange(GetAlignForeignKeySteps(tables, composer).ToList());
            var immediate = tables.Where(table => table.Outcome == CopyTableSchemaOutcome.Skipped ||
                (table.Outcome == CopyTableSchemaOutcome.Aligned && !table.MissingColumns.Any() && !table.MissingIndexes.Any() && !table.MissingForeignKeys.Any())).ToList();
            return new CopyTableSchemaPlan(steps, completions, immediate);
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Creates the state of a table.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="schema"></param>
        /// <param name="composer"></param>
        /// <returns></returns>
        private static CopyTableSchemaTable CreateTable(TableSchema source,
            TableSchema schema,
            ISchemaComposer composer) =>
            new CopyTableSchemaTable(source, schema, composer.ComposeName(schema.Table));

        /// <summary>
        /// Gets the schema of the table as it is created in the target schema: its name, and the foreign keys that reference the other tables, are in that schema.
        /// The schema is the one of the source if there is no target schema, or if nothing has to be moved (the table, and the tables that it references, are already in it).
        /// </summary>
        /// <param name="source"></param>
        /// <param name="targetSchema"></param>
        /// <returns></returns>
        private static TableSchema MoveTo(TableSchema source,
            string targetSchema)
        {
            if (string.IsNullOrWhiteSpace(targetSchema) || (IsIn(source.Table, targetSchema) && source.ForeignKeys.All(foreignKey => IsIn(foreignKey.ReferencedTable, targetSchema))))
            {
                return source;
            }

            return new TableSchema(source.Table.Name, targetSchema)
            {
                CheckConstraints = source.CheckConstraints,
                Columns = source.Columns,
                Indexes = source.Indexes,
                PrimaryKey = source.PrimaryKey,
                UniqueConstraints = source.UniqueConstraints,
                ForeignKeys = source.ForeignKeys.Select(foreignKey => MoveTo(foreignKey, targetSchema)).ToList()
            };
        }

        /// <summary>
        /// Checks whether the table is in the schema.
        /// </summary>
        /// <param name="table"></param>
        /// <param name="schema"></param>
        /// <returns></returns>
        private static bool IsIn(TableInfo table,
            string schema) =>
            string.Equals(table.Schema, schema, StringComparison.Ordinal);

        /// <summary>
        /// Gets the foreign key that references the table in the target schema.
        /// </summary>
        /// <param name="foreignKey"></param>
        /// <param name="targetSchema"></param>
        /// <returns></returns>
        private static ForeignKeyInfo MoveTo(ForeignKeyInfo foreignKey,
            string targetSchema) =>
            new ForeignKeyInfo(foreignKey.Name)
            {
                Columns = foreignKey.Columns,
                ReferencedTable = new TableInfo(foreignKey.ReferencedTable.Name, targetSchema),
                ReferencedColumns = foreignKey.ReferencedColumns,
                UpdateRule = foreignKey.UpdateRule,
                DeleteRule = foreignKey.DeleteRule
            };

        /// <summary>
        /// Throws if two of the tables would have the same name in the destination database (i.e.: the tables of different schemas that are copied into one schema).
        /// </summary>
        /// <param name="tables"></param>
        private static void EnsureUniqueNames(IEnumerable<CopyTableSchemaTable> tables)
        {
            var duplicate = tables
                .Where(table => table.Name != null)
                .GroupBy(table => table.Name, StringComparer.Ordinal)
                .FirstOrDefault(group => group.Count() > 1);
            if (duplicate != null)
            {
                throw new InvalidOperationException($"The tables '{string.Join("', '", duplicate.Select(table => $"{table.Source.Table.Schema}.{table.Source.Table.Name}"))}' would have the same name '{duplicate.Key}' in the destination database.");
            }
        }

        /// <summary>
        /// Sets what is going to happen to each table, and throws if the behavior does not allow that a table already exists.
        /// </summary>
        /// <param name="tables"></param>
        /// <param name="behavior"></param>
        private static void SetOutcomes(IEnumerable<CopyTableSchemaTable> tables,
            CopyTableSchemaExistsBehavior behavior)
        {
            foreach (var table in tables.Where(table => table.Exists == true))
            {
                switch (behavior)
                {
                    case CopyTableSchemaExistsBehavior.Throw:
                        throw new InvalidOperationException($"The table '{table.Schema.Table.Name}' already exists in the destination database.");
                    case CopyTableSchemaExistsBehavior.Align:
                        table.Outcome = CopyTableSchemaOutcome.Aligned;
                        break;
                    case CopyTableSchemaExistsBehavior.Drop:
                        table.Outcome = CopyTableSchemaOutcome.Dropped;
                        break;
                    default:
                        table.Outcome = CopyTableSchemaOutcome.Skipped;
                        break;
                }
            }
        }

        /// <summary>
        /// Gets the steps that drop the tables, the referencing tables first.
        /// </summary>
        /// <param name="tables"></param>
        /// <param name="composer"></param>
        /// <returns></returns>
        private static IEnumerable<CopyTableSchemaStep> GetDropSteps(IEnumerable<CopyTableSchemaTable> tables,
            ISchemaComposer composer) =>
            tables
                .Where(table => table.Outcome == CopyTableSchemaOutcome.Dropped)
                .Reverse()
                .Select(table => new CopyTableSchemaStep(table.DropStatement = composer.ComposeDropTable(table.Name), table));

        /// <summary>
        /// Gets the steps that add the missing columns and indexes to the tables.
        /// </summary>
        /// <param name="tables"></param>
        /// <param name="composer"></param>
        /// <returns></returns>
        private static IEnumerable<CopyTableSchemaStep> GetAlignSteps(IEnumerable<CopyTableSchemaTable> tables,
            ISchemaComposer composer)
        {
            foreach (var table in tables.Where(table => table.Outcome == CopyTableSchemaOutcome.Aligned))
            {
                var statements = table.MissingColumns.Select(column => composer.ComposeAddColumn(table.Name, column))
                    .Concat(table.MissingIndexes.Select(index => composer.ComposeCreateIndex(table.Name, index)))
                    .ToList();
                table.AlignStatements = statements;
                foreach (var statement in statements)
                {
                    yield return new CopyTableSchemaStep(statement, table);
                }
            }
        }

        /// <summary>
        /// Gets the steps that add the missing foreign keys to the tables that are aligned. They are the last steps of the plan, so the tables that they reference
        /// (the tables that are created, and the columns that are added) exist when the foreign keys are added.
        /// </summary>
        /// <param name="tables"></param>
        /// <param name="composer"></param>
        /// <returns></returns>
        private static IEnumerable<CopyTableSchemaStep> GetAlignForeignKeySteps(IEnumerable<CopyTableSchemaTable> tables,
            ISchemaComposer composer)
        {
            foreach (var table in tables.Where(table => table.Outcome == CopyTableSchemaOutcome.Aligned))
            {
                var statements = table.MissingForeignKeys.Select(foreignKey => composer.ComposeAddForeignKey(table.Name, foreignKey)).ToList();
                table.AlignStatements = table.AlignStatements.Concat(statements).ToList();
                foreach (var statement in statements)
                {
                    yield return new CopyTableSchemaStep(statement, table);
                }
            }
        }

        /// <summary>
        /// Gets the statements that create the tables (see <see cref="ISchemaComposer.ComposeSchemas"/>).
        /// </summary>
        /// <param name="created"></param>
        /// <param name="composer"></param>
        /// <returns></returns>
        private static IList<string> GetCreateSteps(IList<CopyTableSchemaTable> created,
            ISchemaComposer composer) =>
            created.Count == 0
                ? new List<string>()
                : composer.ComposeSchemas(created.Select(table => table.Schema)).ToList();

        /// <summary>
        /// Gets the table that completes at the last statement of an align step: the tables are aligned one after the other (the columns and the indexes), and then
        /// their foreign keys are added (they are the last steps of the plan). A table is completed by its last statement.
        /// </summary>
        /// <param name="tables"></param>
        /// <param name="foreignKeyOffset">The index of the first step that adds a foreign key.</param>
        /// <returns></returns>
        private static IEnumerable<(int Key, CopyTableSchemaTable Table)> GetAlignCompletions(IList<CopyTableSchemaTable> tables,
            int foreignKeyOffset)
        {
            var cursor = tables.Count(table => table.Outcome == CopyTableSchemaOutcome.Dropped);
            var foreignKeyCursor = foreignKeyOffset;
            foreach (var table in tables.Where(table => table.Outcome == CopyTableSchemaOutcome.Aligned))
            {
                var count = table.MissingColumns.Count + table.MissingIndexes.Count;
                cursor += count;
                var last = count > 0 ? cursor - 1 : -1;
                foreignKeyCursor += table.MissingForeignKeys.Count;
                if (table.MissingForeignKeys.Count > 0)
                {
                    last = foreignKeyCursor - 1;
                }
                if (last >= 0)
                {
                    yield return (last, table);
                }
            }
        }

        /// <summary>
        /// Gets the table that is completed by each statement of the script that creates the tables. The script has one statement for each table, then one for each index
        /// and then one for each foreign key, so a table is completed by its last statement. If the script does not have the expected number
        /// of statements, the statements that complete the tables are not known, so all the tables are completed at the key <c>-1</c>.
        /// </summary>
        /// <param name="created"></param>
        /// <param name="statementCount"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        private static IEnumerable<(int Key, CopyTableSchemaTable Table)> GetCreateCompletions(IList<CopyTableSchemaTable> created,
            int statementCount,
            int offset)
        {
            var last = new int[created.Count];
            var cursor = 0;
            for (var i = 0; i < created.Count; i++) last[i] = cursor++;
            foreach (var group in new Func<TableSchema, int>[] { s => s.Indexes.Count, s => s.ForeignKeys.Count })
            {
                for (var i = 0; i < created.Count; i++)
                {
                    for (var j = 0; j < group(created[i].Schema); j++) last[i] = cursor++;
                }
            }
            var known = cursor == statementCount;
            return created.Select((table, i) => (known ? offset + last[i] : -1, table));
        }

        /// <summary>
        /// Gets the table that each statement of the script that creates the tables belongs to (<c>null</c> if the script does not have the expected number of statements).
        /// </summary>
        /// <param name="created"></param>
        /// <param name="statementCount"></param>
        /// <returns></returns>
        private static CopyTableSchemaTable[] GetCreateOwners(IList<CopyTableSchemaTable> created,
            int statementCount)
        {
            var owners = new List<CopyTableSchemaTable>(created);
            owners.AddRange(created.SelectMany(table => table.Schema.Indexes.Select(_ => table)));
            owners.AddRange(created.SelectMany(table => table.Schema.ForeignKeys.Select(_ => table)));
            return owners.Count == statementCount
                ? owners.ToArray()
                : new CopyTableSchemaTable[statementCount];
        }

        #endregion
    }
}
