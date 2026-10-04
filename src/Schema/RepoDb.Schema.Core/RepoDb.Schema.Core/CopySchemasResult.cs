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
    /// A class that holds the details of a copy of the schema of multiple tables.
    /// </summary>
    public class CopySchemasResult
    {
        #region Properties

        /// <summary>
        /// Gets or sets the behavior that was requested for when a table already exists in the destination database.
        /// </summary>
        public CopySchemaExistsBehavior Action { get; set; }

        /// <summary>
        /// Gets or sets the name of the destination database.
        /// </summary>
        public string DestinationDatabase { get; set; }

        /// <summary>
        /// Gets or sets the type of the destination database (i.e.: the database engine).
        /// </summary>
        public string DestinationDatabaseType { get; set; }

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
        /// Gets or sets the whole SQL script that was executed on the destination database, in the order that it was executed.
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
        /// Gets or sets the name of the source server.
        /// </summary>
        public string SourceServer { get; set; }

        /// <summary>
        /// Gets or sets the time (in UTC) when the schema copy operation has started.
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// Gets the number of tables whose schema was copied.
        /// </summary>
        public int TableCount => Tables.Count;

        /// <summary>
        /// Gets or sets the result of each table, in the order that the tables were created in the destination database
        /// (the tables that are referenced by the other tables come first).
        /// </summary>
        public IList<CopySchemaResult> Tables { get; set; } = new List<CopySchemaResult>();

        #endregion
    }
}
