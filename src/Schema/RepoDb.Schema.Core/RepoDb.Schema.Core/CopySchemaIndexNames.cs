#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RepoDb.Schema.Enumerations;
using RepoDb.Schema.Models;

namespace RepoDb.Schema
{
    /// <summary>
    /// A class that resolves the names of the indexes that are going to be created in a destination database where the name of an index must be unique in the whole schema
    /// (or database), see <see cref="RepoDb.Interfaces.IDbSetting.AreIndexNamesUniquePerSchema"/>. The source database may allow the same name for the indexes of different tables
    /// (i.e.: <c>PostalCode</c> in <c>Customers</c> and <c>Employees</c> of SQL Server): the index whose name is already used is created with the name of its table as a prefix
    /// (i.e.: <c>Employees_PostalCode</c>), so it is not lost.
    /// </summary>
    internal static class CopySchemaIndexNames
    {
        #region Private Variables

        /// <summary>
        /// The number of names that are tried for an index (the name of the index, then the name of the table as a prefix and then a number as a suffix).
        /// </summary>
        private const int MaxAttempts = 100;

        #endregion

        #region Public Methods

        /// <summary>
        /// Resolves the names of the indexes that are going to be created. A name that is used by an index of the tables that are copied, or that already exists in the destination database, is replaced.
        /// </summary>
        /// <param name="tables">The state of the tables (see <see cref="CopySchemaTable"/>), after their existence was probed.</param>
        /// <param name="behavior">Defines what happens to the tables that already exist in the destination database.</param>
        /// <param name="composer">The composer of the destination database.</param>
        /// <param name="areIndexNamesUniquePerSchema">Whether the name of an index must be unique in the whole schema (or database) of the destination database (see <see cref="RepoDb.Interfaces.IDbSetting.AreIndexNamesUniquePerSchema"/>).</param>
        /// <param name="probe">Executes an existence statement (<c>null</c> if the statement is not composed, or its result is not known).</param>
        public static void Resolve(IEnumerable<CopySchemaTable> tables,
            CopySchemaExistsBehavior behavior,
            ISchemaComposer composer,
            bool areIndexNamesUniquePerSchema,
            Func<string, bool?> probe)
        {
            if (!areIndexNamesUniquePerSchema)
            {
                return;
            }

            var used = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var table in tables)
            {
                RemoveRenamedIndexes(table, behavior, composer, probe);
                foreach (var index in GetIndexesToCreate(table, behavior))
                {
                    var name = index.Name;
                    var attempt = 0;
                    while (attempt < MaxAttempts && (used.Contains(name) || (probe(composer.ComposeIndexNameExists(table.Name, name)) == true && !IsOwn(table, name, behavior, composer, probe))))
                    {
                        name = GetName(table, index, ++attempt);
                    }
                    Use(table, index, name, used);
                }
            }
        }

        /// <summary>
        /// Resolves the names of the indexes that are going to be created, in an asynchronous way (see <see cref="Resolve"/>).
        /// </summary>
        /// <param name="tables">The state of the tables (see <see cref="CopySchemaTable"/>), after their existence was probed.</param>
        /// <param name="behavior">Defines what happens to the tables that already exist in the destination database.</param>
        /// <param name="composer">The composer of the destination database.</param>
        /// <param name="areIndexNamesUniquePerSchema">Whether the name of an index must be unique in the whole schema (or database) of the destination database (see <see cref="RepoDb.Interfaces.IDbSetting.AreIndexNamesUniquePerSchema"/>).</param>
        /// <param name="probe">Executes an existence statement (<c>null</c> if the statement is not composed, or its result is not known).</param>
        public static async Task ResolveAsync(IEnumerable<CopySchemaTable> tables,
            CopySchemaExistsBehavior behavior,
            ISchemaComposer composer,
            bool areIndexNamesUniquePerSchema,
            Func<string, Task<bool?>> probe)
        {
            if (!areIndexNamesUniquePerSchema)
            {
                return;
            }

            var used = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var table in tables)
            {
                await RemoveRenamedIndexesAsync(table, behavior, composer, probe).ConfigureAwait(false);
                foreach (var index in GetIndexesToCreate(table, behavior))
                {
                    var name = index.Name;
                    var attempt = 0;
                    while (attempt < MaxAttempts && (used.Contains(name) || (await probe(composer.ComposeIndexNameExists(table.Name, name)).ConfigureAwait(false) == true &&
                        !await IsOwnAsync(table, name, behavior, composer, probe).ConfigureAwait(false))))
                    {
                        name = GetName(table, index, ++attempt);
                    }
                    Use(table, index, name, used);
                }
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Removes from the missing indexes of the table that is aligned the indexes that the table already has under the name that an earlier copy gave them
        /// (i.e.: the table has <c>Employees_PostalCode</c>, so <c>PostalCode</c> is not missing), so aligning the table again does not add them again.
        /// </summary>
        /// <param name="table">The state of the table (see <see cref="CopySchemaTable"/>).</param>
        /// <param name="behavior">Defines what happens to the tables that already exist in the destination database.</param>
        /// <param name="composer">The composer of the destination database.</param>
        /// <param name="probe">Executes an existence statement (<c>null</c> if the statement is not composed, or its result is not known).</param>
        private static void RemoveRenamedIndexes(CopySchemaTable table,
            CopySchemaExistsBehavior behavior,
            ISchemaComposer composer,
            Func<string, bool?> probe)
        {
            if (table.Exists != true || behavior != CopySchemaExistsBehavior.Align)
            {
                return;
            }

            var present = new List<IndexInfo>();
            foreach (var index in table.MissingIndexes.Where(index => index.Name != null))
            {
                for (var attempt = 1; attempt <= MaxAttempts; attempt++)
                {
                    var name = GetName(table, index, attempt);
                    if (probe(composer.ComposeIndexExists(table.Name, name)) == true)
                    {
                        present.Add(index);
                        break;
                    }
                    if (probe(composer.ComposeIndexNameExists(table.Name, name)) != true)
                    {
                        break;
                    }
                }
            }
            table.MissingIndexes = table.MissingIndexes.Where(index => !present.Contains(index)).ToList();
        }

        /// <summary>
        /// Removes from the missing indexes of the table that is aligned the indexes that the table already has under the name that an earlier copy gave them (in an asynchronous way).
        /// </summary>
        /// <param name="table">The state of the table (see <see cref="CopySchemaTable"/>).</param>
        /// <param name="behavior">Defines what happens to the tables that already exist in the destination database.</param>
        /// <param name="composer">The composer of the destination database.</param>
        /// <param name="probe">Executes an existence statement (<c>null</c> if the statement is not composed, or its result is not known).</param>
        private static async Task RemoveRenamedIndexesAsync(CopySchemaTable table,
            CopySchemaExistsBehavior behavior,
            ISchemaComposer composer,
            Func<string, Task<bool?>> probe)
        {
            if (table.Exists != true || behavior != CopySchemaExistsBehavior.Align)
            {
                return;
            }

            var present = new List<IndexInfo>();
            foreach (var index in table.MissingIndexes.Where(index => index.Name != null))
            {
                for (var attempt = 1; attempt <= MaxAttempts; attempt++)
                {
                    var name = GetName(table, index, attempt);
                    if (await probe(composer.ComposeIndexExists(table.Name, name)).ConfigureAwait(false) == true)
                    {
                        present.Add(index);
                        break;
                    }
                    if (await probe(composer.ComposeIndexNameExists(table.Name, name)).ConfigureAwait(false) != true)
                    {
                        break;
                    }
                }
            }
            table.MissingIndexes = table.MissingIndexes.Where(index => !present.Contains(index)).ToList();
        }

        /// <summary>
        /// Gets the (named) indexes that are going to be created in the destination table.
        /// </summary>
        /// <param name="table">The state of the table (see <see cref="CopySchemaTable"/>).</param>
        /// <param name="behavior">Defines what happens to the tables that already exist in the destination database.</param>
        /// <returns>The named indexes that are going to be created.</returns>
        private static IList<IndexInfo> GetIndexesToCreate(CopySchemaTable table,
            CopySchemaExistsBehavior behavior)
        {
            IEnumerable<IndexInfo> indexes;
            if (table.Exists != true || behavior == CopySchemaExistsBehavior.Drop)
            {
                indexes = table.Schema.Indexes;
            }
            else if (behavior == CopySchemaExistsBehavior.Align)
            {
                indexes = table.MissingIndexes;
            }
            else
            {
                indexes = Enumerable.Empty<IndexInfo>();
            }
            return indexes.Where(index => index.Name != null).ToList();
        }

        /// <summary>
        /// Checks whether the name is used by an index of the table that is dropped before it is created, so the name is free again.
        /// </summary>
        /// <param name="table">The state of the table (see <see cref="CopySchemaTable"/>).</param>
        /// <param name="name">The name of the index.</param>
        /// <param name="behavior">Defines what happens to the tables that already exist in the destination database.</param>
        /// <param name="composer">The composer of the destination database.</param>
        /// <param name="probe">Executes an existence statement (<c>null</c> if the statement is not composed, or its result is not known).</param>
        /// <returns><c>true</c> if the name is used by an index of the table that is dropped; otherwise, <c>false</c>.</returns>
        private static bool IsOwn(CopySchemaTable table,
            string name,
            CopySchemaExistsBehavior behavior,
            ISchemaComposer composer,
            Func<string, bool?> probe) =>
            behavior == CopySchemaExistsBehavior.Drop && table.Exists == true && probe(composer.ComposeIndexExists(table.Name, name)) == true;

        /// <summary>
        /// Checks whether the name is used by an index of the table that is dropped before it is created, so the name is free again (in an asynchronous way).
        /// </summary>
        /// <param name="table">The state of the table (see <see cref="CopySchemaTable"/>).</param>
        /// <param name="name">The name of the index.</param>
        /// <param name="behavior">Defines what happens to the tables that already exist in the destination database.</param>
        /// <param name="composer">The composer of the destination database.</param>
        /// <param name="probe">Executes an existence statement (<c>null</c> if the statement is not composed, or its result is not known).</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains: <c>true</c> if the name is used by an index of the table that is dropped; otherwise, <c>false</c>.</returns>
        private static async Task<bool> IsOwnAsync(CopySchemaTable table,
            string name,
            CopySchemaExistsBehavior behavior,
            ISchemaComposer composer,
            Func<string, Task<bool?>> probe) =>
            behavior == CopySchemaExistsBehavior.Drop && table.Exists == true && await probe(composer.ComposeIndexExists(table.Name, name)).ConfigureAwait(false) == true;

        /// <summary>
        /// Gets the name of the index for the attempt: the name of the table is the prefix, and the number of the attempt is the suffix when the prefixed name is used too.
        /// </summary>
        /// <param name="table">The state of the table (see <see cref="CopySchemaTable"/>).</param>
        /// <param name="index">The index of the table.</param>
        /// <param name="attempt">The number of the attempt (1 is the name of the table as a prefix).</param>
        /// <returns>The name of the index for the attempt.</returns>
        private static string GetName(CopySchemaTable table,
            IndexInfo index,
            int attempt) =>
            attempt == 1
                ? $"{table.Schema.Table.Name}_{index.Name}"
                : $"{table.Schema.Table.Name}_{index.Name}_{attempt}";

        /// <summary>
        /// Uses the name for the index: the index of the table is replaced by a copy of it that has the name (the indexes of the source schema are not changed).
        /// </summary>
        /// <param name="table">The state of the table (see <see cref="CopySchemaTable"/>).</param>
        /// <param name="index">The index of the table.</param>
        /// <param name="name">The name of the index.</param>
        /// <param name="used">The names that are used by the indexes that were resolved.</param>
        private static void Use(CopySchemaTable table,
            IndexInfo index,
            string name,
            ISet<string> used)
        {
            used.Add(name);
            if (string.Equals(name, index.Name, StringComparison.Ordinal))
            {
                return;
            }

            var renamed = new IndexInfo(name)
            {
                IsUnique = index.IsUnique,
                Columns = index.Columns,
                IncludedColumns = index.IncludedColumns,
                DescendingColumns = index.DescendingColumns,
                IsClustered = index.IsClustered,
                Filter = index.Filter
            };
            if (ReferenceEquals(table.Schema, table.Source))
            {
                // The schema of the source is not moved to another schema, so it is copied before the indexes are replaced
                table.Schema = new TableSchema(table.Source.Table.Name, table.Source.Table.Schema)
                {
                    CheckConstraints = table.Source.CheckConstraints,
                    Columns = table.Source.Columns,
                    ForeignKeys = table.Source.ForeignKeys,
                    Indexes = table.Source.Indexes,
                    PrimaryKey = table.Source.PrimaryKey,
                    UniqueConstraints = table.Source.UniqueConstraints
                };
            }
            table.Schema.Indexes = table.Schema.Indexes.Select(item => ReferenceEquals(item, index) ? renamed : item).ToList();
            table.MissingIndexes = table.MissingIndexes.Select(item => ReferenceEquals(item, index) ? renamed : item).ToList();
        }

        #endregion
    }
}
