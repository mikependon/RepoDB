#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

namespace RepoDb.Schema.Models
{
    /// <summary>
    /// A class that holds the details of one side (the source or the target) of a table whose schema is copied.
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
        /// Gets the name of the server.
        /// </summary>
        public string Server { get; internal set; }

        /// <summary>
        /// Gets the schema of the table (its name and the name of the schema that it belongs to are in <see cref="TableSchema.Table"/>).
        /// </summary>
        public TableSchema Table { get; internal set; }

        #endregion
    }
}
