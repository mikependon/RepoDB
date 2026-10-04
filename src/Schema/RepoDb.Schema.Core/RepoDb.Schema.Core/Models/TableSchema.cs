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
        #region Properties

        /// <summary>
        /// Gets or sets the check constraints of the table.
        /// </summary>
        public IList<CheckConstraintInfo> CheckConstraints { get; set; } = new List<CheckConstraintInfo>();

        /// <summary>
        /// Gets or sets the columns of the table, in their ordinal order.
        /// </summary>
        public IList<ColumnInfo> Columns { get; set; } = new List<ColumnInfo>();

        /// <summary>
        /// Gets or sets the foreign keys of the table.
        /// </summary>
        public IList<ForeignKeyInfo> ForeignKeys { get; set; } = new List<ForeignKeyInfo>();

        /// <summary>
        /// Gets or sets the indexes of the table.
        /// </summary>
        public IList<IndexInfo> Indexes { get; set; } = new List<IndexInfo>();

        /// <summary>
        /// Gets or sets the primary key of the table, or <c>null</c> if the table has no primary key.
        /// </summary>
        public PrimaryKeyInfo PrimaryKey { get; set; }

        /// <summary>
        /// Gets or sets the name of the schema that owns the table.
        /// </summary>
        public string SchemaName { get; set; }

        /// <summary>
        /// Gets or sets the name of the table.
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// Gets or sets the unique constraints of the table.
        /// </summary>
        public IList<UniqueConstraintInfo> UniqueConstraints { get; set; } = new List<UniqueConstraintInfo>();

        #endregion
    }
}
