#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using RepoDb.Schema.Enumerations;

namespace RepoDb.Schema
{
    /// <summary>
    /// A class that holds the details of a schema copy operation.
    /// </summary>
    public class CopySchemaResult
    {
        #region Properties

        /// <summary>
        /// Gets or sets the behavior that was requested for when the table already exists in the destination database. See <see cref="Outcome"/> for what was actually done.
        /// </summary>
        public CopySchemaExistsBehavior Action { get; set; }

        /// <summary>
        /// Gets or sets the names of the columns that were added to an already existing table (see <see cref="CopySchemaOutcome.Aligned"/>).
        /// </summary>
        public IList<string> AddedColumns { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets the names of the indexes that were added to an already existing table (see <see cref="CopySchemaOutcome.Aligned"/>).
        /// </summary>
        public IList<string> AddedIndexes { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets the number of check constraints that were created.
        /// </summary>
        public int CheckConstraintCount { get; set; }

        /// <summary>
        /// Gets or sets the number of columns that were created.
        /// </summary>
        public int ColumnCount { get; set; }

        /// <summary>
        /// Gets or sets the name of the destination database.
        /// </summary>
        public string DestinationDatabase { get; set; }

        /// <summary>
        /// Gets or sets the type of the destination database (i.e.: the database engine).
        /// </summary>
        public string DestinationDatabaseType { get; set; }

        /// <summary>
        /// Gets or sets the name of the schema of the table in the destination database.
        /// </summary>
        public string DestinationSchema { get; set; }

        /// <summary>
        /// Gets or sets the name of the destination server.
        /// </summary>
        public string DestinationServer { get; set; }

        /// <summary>
        /// Gets the duration of the schema copy operation.
        /// </summary>
        public TimeSpan Duration => EndTime - StartTime;

        /// <summary>
        /// Gets or sets the time (in UTC) when the schema copy operation has ended.
        /// </summary>
        public DateTime EndTime { get; set; }

        /// <summary>
        /// Gets or sets the errors that were raised while the schema was being created (see <see cref="CopySchemaError"/>).
        /// </summary>
        public IList<CopySchemaError> Errors { get; set; } = new List<CopySchemaError>();

        /// <summary>
        /// Gets or sets the number of foreign keys that were created.
        /// </summary>
        public int ForeignKeyCount { get; set; }

        /// <summary>
        /// Gets or sets the number of indexes that were created.
        /// </summary>
        public int IndexCount { get; set; }

        /// <summary>
        /// Gets or sets what the operation has actually done to the table in the destination database.
        /// </summary>
        public CopySchemaOutcome Outcome { get; set; }

        /// <summary>
        /// Gets or sets the SQL script that was executed on the destination database.
        /// </summary>
        public string Script { get; set; }

        /// <summary>
        /// Gets or sets the name of the source database.
        /// </summary>
        public string SourceDatabase { get; set; }

        /// <summary>
        /// Gets or sets the type of the source database (i.e.: the database engine).
        /// </summary>
        public string SourceDatabaseType { get; set; }

        /// <summary>
        /// Gets or sets the name of the schema of the table in the source database.
        /// </summary>
        public string SourceSchema { get; set; }

        /// <summary>
        /// Gets or sets the name of the source server.
        /// </summary>
        public string SourceServer { get; set; }

        /// <summary>
        /// Gets or sets the time (in UTC) when the schema copy operation has started.
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether the table already existed in the destination database, or <c>null</c> if it was not checked.
        /// </summary>
        public bool? TableExisted { get; set; }

        /// <summary>
        /// Gets or sets the name of the table whose schema is being copied.
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// Gets or sets the number of unique constraints that were created.
        /// </summary>
        public int UniqueConstraintCount { get; set; }

        /// <summary>
        /// Gets or sets the warnings raised during the copy (i.e.: an unmapped column type or an unsupported feature that was skipped).
        /// </summary>
        public IList<string> Warnings { get; set; } = new List<string>();

        #endregion
    }
}
