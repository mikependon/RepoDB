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
    /// destination tables (see <see cref="CopySchemaTable"/>) and the <see cref="CopySchemaExistsBehavior"/>.
    /// </summary>
    internal sealed class CopySchemaPlan
    {
        #region Constructors

        private CopySchemaPlan(IList<CopySchemaStep> steps,
            ILookup<int, CopySchemaTable> completions,
            IList<CopySchemaTable> immediate)
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
        public IList<CopySchemaStep> Steps { get; }

        /// <summary>
        /// Gets the tables that are completed by each step (by the index of the step). The key <c>-1</c> has the tables whose steps are not known,
        /// as the script that was composed does not have the expected number of statements.
        /// </summary>
        public ILookup<int, CopySchemaTable> Completions { get; }

        /// <summary>
        /// Gets the tables that have nothing to be executed, so they are complete from the start.
        /// </summary>
        public IList<CopySchemaTable> Immediate { get; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates the state of each table of the schemas.
        /// </summary>
        /// <param name="schemas">The schemas of the tables, in the order that they must be created.</param>
        /// <param name="composer">The composer of the destination database.</param>
        /// <returns>The state of each table.</returns>
        public static IList<CopySchemaTable> CreateTables(IEnumerable<TableSchema> schemas,
            ISchemaComposer composer) =>
            schemas.Select(schema => new CopySchemaTable(schema, composer.ComposeName(schema.Table))).ToList();

        /// <summary>
        /// Creates the plan for the tables, from their state in the destination database.
        /// </summary>
        /// <param name="tables">The state of the tables (see <see cref="CopySchemaTable.Exists"/>, <see cref="CopySchemaTable.MissingColumns"/> and <see cref="CopySchemaTable.MissingIndexes"/>).</param>
        /// <param name="behavior">Defines what happens to the tables that already exist in the destination database.</param>
        /// <param name="composer">The composer of the destination database.</param>
        /// <returns>The plan.</returns>
        /// <exception cref="InvalidOperationException">The behavior is <see cref="CopySchemaExistsBehavior.Throw"/> and a table already exists.</exception>
        public static CopySchemaPlan Create(IList<CopySchemaTable> tables,
            CopySchemaExistsBehavior behavior,
            ISchemaComposer composer)
        {
            SetOutcomes(tables, behavior);
            var created = tables.Where(table => table.Exists != true || behavior == CopySchemaExistsBehavior.Drop).ToList();
            var steps = GetDropSteps(tables, composer).Concat(GetAlignSteps(tables, composer)).ToList();
            var createSteps = GetCreateSteps(created, composer);
            var completions = GetAlignCompletions(tables)
                .Concat(GetCreateCompletions(created, createSteps.Count, steps.Count))
                .ToLookup(x => x.Key, x => x.Table);
            var owners = GetCreateOwners(created, createSteps.Count);
            steps.AddRange(createSteps.Select((statement, i) => new CopySchemaStep(statement, owners[i])));
            var immediate = tables.Where(table => table.Outcome == CopySchemaOutcome.Skipped ||
                (table.Outcome == CopySchemaOutcome.Aligned && !table.MissingColumns.Any() && !table.MissingIndexes.Any())).ToList();
            return new CopySchemaPlan(steps, completions, immediate);
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Sets what is going to happen to each table, and throws if the behavior does not allow that a table already exists.
        /// </summary>
        /// <param name="tables"></param>
        /// <param name="behavior"></param>
        private static void SetOutcomes(IEnumerable<CopySchemaTable> tables,
            CopySchemaExistsBehavior behavior)
        {
            foreach (var table in tables.Where(table => table.Exists == true))
            {
                switch (behavior)
                {
                    case CopySchemaExistsBehavior.Throw:
                        throw new InvalidOperationException($"The table '{table.Schema.Table.Name}' already exists in the destination database.");
                    case CopySchemaExistsBehavior.Align:
                        table.Outcome = CopySchemaOutcome.Aligned;
                        break;
                    case CopySchemaExistsBehavior.Drop:
                        table.Outcome = CopySchemaOutcome.Dropped;
                        break;
                    default:
                        table.Outcome = CopySchemaOutcome.Skipped;
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
        private static IEnumerable<CopySchemaStep> GetDropSteps(IEnumerable<CopySchemaTable> tables,
            ISchemaComposer composer) =>
            tables
                .Where(table => table.Outcome == CopySchemaOutcome.Dropped)
                .Reverse()
                .Select(table => new CopySchemaStep(table.DropStatement = composer.ComposeDropTable(table.Name), table));

        /// <summary>
        /// Gets the steps that add the missing columns and indexes to the tables.
        /// </summary>
        /// <param name="tables"></param>
        /// <param name="composer"></param>
        /// <returns></returns>
        private static IEnumerable<CopySchemaStep> GetAlignSteps(IEnumerable<CopySchemaTable> tables,
            ISchemaComposer composer)
        {
            foreach (var table in tables.Where(table => table.Outcome == CopySchemaOutcome.Aligned))
            {
                var statements = table.MissingColumns.Select(column => composer.ComposeAddColumn(table.Name, column))
                    .Concat(table.MissingIndexes.Select(index => composer.ComposeCreateIndex(table.Name, index)))
                    .ToList();
                table.AlignStatements = statements;
                foreach (var statement in statements)
                {
                    yield return new CopySchemaStep(statement, table);
                }
            }
        }

        /// <summary>
        /// Gets the statements that create the tables (see <see cref="ISchemaComposer.ComposeSchemas"/>).
        /// </summary>
        /// <param name="created"></param>
        /// <param name="composer"></param>
        /// <returns></returns>
        private static IList<string> GetCreateSteps(IList<CopySchemaTable> created,
            ISchemaComposer composer) =>
            created.Count == 0
                ? new List<string>()
                : composer.ComposeSchemas(created.Select(table => table.Schema)).ToList();

        /// <summary>
        /// Gets the table that completes at the last statement of an align step: the tables are aligned one after the other.
        /// </summary>
        /// <param name="tables"></param>
        /// <returns></returns>
        private static IEnumerable<(int Key, CopySchemaTable Table)> GetAlignCompletions(IList<CopySchemaTable> tables)
        {
            var cursor = tables.Count(table => table.Outcome == CopySchemaOutcome.Dropped);
            foreach (var table in tables.Where(table => table.Outcome == CopySchemaOutcome.Aligned))
            {
                var count = table.MissingColumns.Count + table.MissingIndexes.Count;
                cursor += count;
                if (count > 0)
                {
                    yield return (cursor - 1, table);
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
        private static IEnumerable<(int Key, CopySchemaTable Table)> GetCreateCompletions(IList<CopySchemaTable> created,
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
        private static CopySchemaTable[] GetCreateOwners(IList<CopySchemaTable> created,
            int statementCount)
        {
            var owners = new List<CopySchemaTable>(created);
            owners.AddRange(created.SelectMany(table => table.Schema.Indexes.Select(_ => table)));
            owners.AddRange(created.SelectMany(table => table.Schema.ForeignKeys.Select(_ => table)));
            return owners.Count == statementCount
                ? owners.ToArray()
                : new CopySchemaTable[statementCount];
        }

        #endregion
    }
}
