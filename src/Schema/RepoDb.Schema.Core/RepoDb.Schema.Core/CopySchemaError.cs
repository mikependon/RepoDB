#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;

namespace RepoDb.Schema
{
    /// <summary>
    /// A class that holds the details of a statement that has failed while copying the schemas.
    /// </summary>
    public class CopySchemaError
    {
        #region Properties

        /// <summary>
        /// Gets or sets the exception that was raised by the statement.
        /// </summary>
        public Exception Exception { get; set; }

        /// <summary>
        /// Gets or sets the name of the schema of the table that the statement belongs to, or <c>null</c> if it is not known.
        /// </summary>
        public string SchemaName { get; set; }

        /// <summary>
        /// Gets or sets the statement that has failed.
        /// </summary>
        public string Statement { get; set; }

        /// <summary>
        /// Gets or sets the position (starting at 0) of the statement in the script that is being executed.
        /// </summary>
        public int StatementIndex { get; set; }

        /// <summary>
        /// Gets or sets the name of the table that the statement belongs to (it creates the table, one of its indexes or one of its foreign keys),
        /// or <c>null</c> if it is not known.
        /// </summary>
        public string TableName { get; set; }

        #endregion
    }
}
