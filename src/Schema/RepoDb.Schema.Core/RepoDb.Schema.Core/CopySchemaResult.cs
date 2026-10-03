#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;

namespace RepoDb.Schema
{
    /// <summary>
    /// A class that holds the details of a schema copy operation.
    /// </summary>
    public class CopySchemaResult
    {
        #region Properties

        /// <summary>
        /// Gets or sets a short, human readable word (i.e.: Skipped, Aligned, Thrown or Dropped) for what is done when the table already exists in the destination database.
        /// </summary>
        public string Action { get; set; }

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
        /// Gets or sets the time (in UTC) when the schema copy operation has ended.
        /// </summary>
        public DateTime EndTime { get; set; }

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
        /// Gets or sets the name of the table whose schema is being copied.
        /// </summary>
        public string TableName { get; set; }

        #endregion
    }
}
