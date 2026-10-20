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
    public class CopyTableSchemaTable
    {
        #region Constructors

        /// <summary>
        /// Creates a new instance of <see cref="CopyTableSchemaTable"/> class.
        /// </summary>
        /// <param name="tableName">The name of the table of the source database, with or without its schema (i.e.: <c>Person</c> or <c>dbo.Person</c>).</param>
        /// <param name="columns">The mappings of the columns of the table that are created in the destination database.</param>
        public CopyTableSchemaTable(string tableName,
            IList<CopyTableSchemaColumnMapping> columns)
        {
            TableName = tableName;
            Columns = columns;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the name of the table of the source database, with or without its schema (i.e.: <c>Person</c> or <c>dbo.Person</c>).
        /// </summary>
        public string TableName { get; }

        /// <summary>
        /// Gets the mappings of the columns of the table that are created in the destination database (see <see cref="CopyTableSchemaColumnMapping"/>).
        /// </summary>
        public IList<CopyTableSchemaColumnMapping> Columns { get; }

        #endregion
    }
}
