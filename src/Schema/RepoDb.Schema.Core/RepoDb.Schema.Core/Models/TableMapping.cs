#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.Collections.Generic;

namespace RepoDb.Schema.Models
{
    /// <summary>
    /// A class that represents a table whose schema is copied with only some of its columns, used by the <c>CopyTableSchemaTo</c> operations
    /// that copy several tables: it holds the name of the table and the mappings of its columns.
    /// </summary>
    public class TableMapping
    {
        #region Constructors

        /// <summary>
        /// Creates a new instance of <see cref="TableMapping"/> class.
        /// </summary>
        /// <param name="source">The name of the table of the source database, with or without its schema (i.e.: <c>Person</c> or <c>dbo.Person</c>).</param>
        /// <param name="destination">The name of the table that is created in the destination database (<c>null</c> if the table keeps its name).</param>
        /// <param name="columns">The mappings of the columns of the table that are created in the destination database.</param>
        public TableMapping(string source,
            string destination,
            IList<ColumnMapping> columns)
        {
            Source = source;
            Destination = destination;
            Columns = columns;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the name of the table of the source database, with or without its schema (i.e.: <c>Person</c> or <c>dbo.Person</c>).
        /// </summary>
        public string Source { get; }

        /// <summary>
        /// Gets the name of the table that is created in the destination database (<c>null</c> if the table keeps its name). The foreign keys that reference the table follow the new name.
        /// </summary>
        public string Destination { get; }

        /// <summary>
        /// Gets the mappings of the columns of the table that are created in the destination database (see <see cref="ColumnMapping"/>).
        /// </summary>
        public IList<ColumnMapping> Columns { get; }

        #endregion
    }
}
