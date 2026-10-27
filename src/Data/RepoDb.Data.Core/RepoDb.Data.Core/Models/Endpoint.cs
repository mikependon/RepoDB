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
    public class Endpoint
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
        /// On the target it is <c>null</c>: the copy operation does not query destination row counts.
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
