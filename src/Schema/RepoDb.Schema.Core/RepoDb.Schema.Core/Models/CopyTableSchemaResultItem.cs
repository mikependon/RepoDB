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
    /// A class that holds the details of one table whose schema is copied: the source table and the target table that was created from it.
    /// </summary>
    public class CopyTableSchemaResultItem
    {
        #region Properties

        /// <summary>
        /// Gets the behavior that was requested for when the table already exists in the target database. See <see cref="Outcome"/> for what was actually done.
        /// </summary>
        public CopyTableSchemaExistsBehavior Action { get; internal set; }

        /// <summary>
        /// Gets the names of the columns that were added to an already existing table (see <see cref="CopyTableSchemaOutcome.Aligned"/>).
        /// </summary>
        public IList<string> AddedColumns { get; internal set; } = new List<string>();

        /// <summary>
        /// Gets the names of the foreign keys that were added to an already existing table (see <see cref="CopyTableSchemaOutcome.Aligned"/>).
        /// </summary>
        public IList<string> AddedForeignKeys { get; internal set; } = new List<string>();

        /// <summary>
        /// Gets the names of the indexes that were added to an already existing table (see <see cref="CopyTableSchemaOutcome.Aligned"/>).
        /// </summary>
        public IList<string> AddedIndexes { get; internal set; } = new List<string>();

        /// <summary>
        /// Gets the number of check constraints that were created.
        /// </summary>
        public int CheckConstraintCount { get; internal set; }

        /// <summary>
        /// Gets the number of columns that were created.
        /// </summary>
        public int ColumnCount { get; internal set; }

        /// <summary>
        /// Gets the errors that were raised while the schema of this table was being created (see <see cref="ExecutionError"/>).
        /// </summary>
        public IList<ExecutionError> Errors { get; internal set; } = new List<ExecutionError>();

        /// <summary>
        /// Gets the number of foreign keys that were created.
        /// </summary>
        public int ForeignKeyCount { get; internal set; }

        /// <summary>
        /// Gets the number of indexes that were created.
        /// </summary>
        public int IndexCount { get; internal set; }

        /// <summary>
        /// Gets what the operation has actually done to the table in the target database.
        /// </summary>
        public CopyTableSchemaOutcome Outcome { get; internal set; }

        /// <summary>
        /// Gets the SQL script that was executed on the target database.
        /// </summary>
        public string Script { get; internal set; }

        /// <summary>
        /// Gets the details of the source table.
        /// </summary>
        public Endpoint Source { get; internal set; } = new Endpoint();

        /// <summary>
        /// Gets a value that indicates whether the table already existed in the target database, or <c>null</c> if it was not checked.
        /// </summary>
        public bool? TableExisted { get; internal set; }

        /// <summary>
        /// Gets the details of the target table, which is the table that was created from the <see cref="Source"/>.
        /// </summary>
        public Endpoint Target { get; internal set; } = new Endpoint();

        /// <summary>
        /// Gets the number of unique constraints that were created.
        /// </summary>
        public int UniqueConstraintCount { get; internal set; }

        /// <summary>
        /// Gets the warnings raised during the copy of this table (i.e.: an unmapped column type or an unsupported feature that was skipped).
        /// </summary>
        public IList<string> Warnings { get; internal set; } = new List<string>();

        #endregion
    }
}
