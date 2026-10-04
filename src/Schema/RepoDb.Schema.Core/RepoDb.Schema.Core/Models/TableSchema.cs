#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.Collections.Generic;

namespace RepoDb.Schema.Models
{
    /// <summary>
    /// A class that holds the whole schema of a table. It is the hand-off between an <see cref="ISchemaReader"/> and an <see cref="ISchemaComposer"/>.
    /// </summary>
    public class TableSchema
    {
        #region Constructors

        /// <summary>
        /// Creates a new instance of <see cref="TableSchema"/> class.
        /// </summary>
        public TableSchema()
        { }

        /// <summary>
        /// Creates a new instance of <see cref="TableSchema"/> class.
        /// </summary>
        /// <param name="name">The name of the table.</param>
        /// <param name="schema">The name of the schema that owns the table.</param>
        public TableSchema(string name,
            string schema)
        {
            Table = new TableInfo(name, schema);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the check constraints of the table.
        /// </summary>
        public IList<CheckConstraintInfo> CheckConstraints { get; internal set; } = new List<CheckConstraintInfo>();

        /// <summary>
        /// Gets or sets the columns of the table, in their ordinal order.
        /// </summary>
        public IList<ColumnInfo> Columns { get; internal set; } = new List<ColumnInfo>();

        /// <summary>
        /// Gets or sets the foreign keys of the table.
        /// </summary>
        public IList<ForeignKeyInfo> ForeignKeys { get; internal set; } = new List<ForeignKeyInfo>();

        /// <summary>
        /// Gets or sets the indexes of the table.
        /// </summary>
        public IList<IndexInfo> Indexes { get; internal set; } = new List<IndexInfo>();

        /// <summary>
        /// Gets or sets the primary key of the table, or <c>null</c> if the table has no primary key.
        /// </summary>
        public PrimaryKeyInfo PrimaryKey { get; internal set; }

        /// <summary>
        /// Gets or sets the identity (name and schema) of the table.
        /// </summary>
        public TableInfo Table { get; internal set; } = new TableInfo();

        /// <summary>
        /// Gets or sets the unique constraints of the table.
        /// </summary>
        public IList<UniqueConstraintInfo> UniqueConstraints { get; internal set; } = new List<UniqueConstraintInfo>();

        #endregion
    }
}
