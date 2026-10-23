#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using RepoDb.Schema.Models;

namespace RepoDb.Data.Models
{
    /// <summary>
    /// A class that holds the details of one side (the source or the target) of a table whose rows are copied.
    /// </summary>
    public class CopyEndpoint
    {
        #region Properties

        /// <summary>
        /// Gets the name of the database.
        /// </summary>
        public string Database { get; internal set; }

        /// <summary>
        /// Gets the type of the database (i.e.: the database engine).
        /// </summary>
        public string DatabaseType { get; internal set; }

        /// <summary>
        /// Gets the number of rows of the table. On the source it is the number of rows that were read by the copy (without the rows that the filter leaves out).
        /// On the target it is the number of rows that the table has after the copy, or <c>null</c> if the rows of the table are not counted (the target is counted
        /// when its rows are copied with the bulk insert of the provider).
        /// </summary>
        public long? RowCount { get; internal set; }

        /// <summary>
        /// Gets the name of the server.
        /// </summary>
        public string Server { get; internal set; }

        /// <summary>
        /// Gets the table (its name and the name of the schema that it belongs to).
        /// </summary>
        public TableInfo Table { get; internal set; }

        #endregion
    }
}
