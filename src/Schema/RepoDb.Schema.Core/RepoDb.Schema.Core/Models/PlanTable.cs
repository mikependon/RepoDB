#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.Collections.Generic;
using RepoDb.Schema.Enumerations;

namespace RepoDb.Schema.Models
{
    /// <summary>
    /// A class that holds the state of a table that is being copied: its schema, its name in the destination database,
    /// and what is known about the table that already exists in the destination database.
    /// </summary>
    internal sealed class PlanTable
    {
        #region Constructors

        /// <summary>
        /// Creates a new instance of <see cref="PlanTable"/> class.
        /// </summary>
        /// <param name="source">The schema of the table, as read from the source database.</param>
        /// <param name="schema">The schema of the table that is created in the destination database (the same as the source schema if the table is not moved to another schema).</param>
        /// <param name="name">The name of the table in the dialect of the destination database.</param>
        public PlanTable(TableSchema source,
            TableSchema schema,
            string name)
        {
            Source = source;
            Original = source;
            Schema = schema;
            Name = name;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the schema of the table, as read from the source database and then limited to the selected columns (see <see cref="ColumnSelector"/>).
        /// </summary>
        public TableSchema Source { get; }

        /// <summary>
        /// Gets or sets the schema of the table exactly as it was read from the source database: all of its columns, with their original names.
        /// </summary>
        public TableSchema Original { get; set; }

        /// <summary>
        /// Gets the schema of the table that is created in the destination database.
        /// </summary>
        public TableSchema Schema { get; set; }

        /// <summary>
        /// Gets the name of the table in the dialect of the destination database (see <see cref="ISchemaComposer.ComposeName"/>).
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets or sets whether the table already exists in the destination database (<c>null</c> if it is not known).
        /// </summary>
        public bool? Exists { get; set; }

        /// <summary>
        /// Gets or sets the columns of the table that are missing in the destination table.
        /// </summary>
        public IList<ColumnInfo> MissingColumns { get; set; } = new List<ColumnInfo>();

        /// <summary>
        /// Gets or sets the indexes of the table that are missing in the destination table.
        /// </summary>
        public IList<IndexInfo> MissingIndexes { get; set; } = new List<IndexInfo>();

        /// <summary>
        /// Gets or sets the foreign keys of the table that are missing in the destination table.
        /// </summary>
        public IList<ForeignKeyInfo> MissingForeignKeys { get; set; } = new List<ForeignKeyInfo>();

        /// <summary>
        /// Gets or sets what is going to be done to the table.
        /// </summary>
        public CopyTableSchemaOutcome Outcome { get; set; } = CopyTableSchemaOutcome.Created;

        /// <summary>
        /// Gets or sets the warnings of the table: what is left out when the table is created (i.e.: a foreign key whose referenced table is not in the destination).
        /// </summary>
        public IList<string> Warnings { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets the statement that drops the existing table (when the table is dropped before it is created).
        /// </summary>
        public string DropStatement { get; set; }

        /// <summary>
        /// Gets or sets the statements that add the missing columns, indexes and foreign keys to the existing table (when the table is aligned).
        /// </summary>
        public IList<string> AlignStatements { get; set; } = new List<string>();

        #endregion
    }
}
