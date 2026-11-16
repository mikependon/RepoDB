using System;
using System.Collections.Generic;
// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

namespace RepoDb.Schema.Models
{
    /// <summary>
    /// Stores raw catalog metadata.
    /// </summary>
    internal sealed class TursoRawTable
    {
        #region Properties

        /// <summary>
        /// Gets or sets the schema.
        /// </summary>
        public string Schema { get; set; }

        /// <summary>
        /// Gets or sets the table.
        /// </summary>
        public string Table { get; set; }

        /// <summary>
        /// Gets or sets the exists.
        /// </summary>
        public bool Exists { get; set; }

        /// <summary>
        /// Gets or sets the definition.
        /// </summary>
        public string Definition { get; set; }

        /// <summary>
        /// Gets or sets the columns.
        /// </summary>
        public IList<TursoRawColumn> Columns { get; set; } = new List<TursoRawColumn>();

        /// <summary>
        /// Gets or sets the indexes.
        /// </summary>
        public IList<TursoRawIndex> Indexes { get; set; } = new List<TursoRawIndex>();

        /// <summary>
        /// Gets or sets the foreign keys.
        /// </summary>
        public IList<TursoRawForeignKey> ForeignKeys { get; set; } = new List<TursoRawForeignKey>();

        /// <summary>
        /// Gets or sets the referenced primary keys.
        /// </summary>
        public IDictionary<string, List<string>> ReferencedPrimaryKeys { get; } = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Gets or sets the referenced tables.
        /// </summary>
        public IDictionary<string, (string Name, IList<string> Columns)> ReferencedTables { get; } = new Dictionary<string, (string Name, IList<string> Columns)>(StringComparer.OrdinalIgnoreCase);
        #endregion
    }
}
