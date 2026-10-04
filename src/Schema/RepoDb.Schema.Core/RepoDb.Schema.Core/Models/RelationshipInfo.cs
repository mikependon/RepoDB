#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.Collections.Generic;

namespace RepoDb.Schema.Models
{
    /// <summary>
    /// A class that holds the relationships of a table with the other tables, as defined by the foreign keys.
    /// The relationships of all the tables form a graph: the same <see cref="RelationshipInfo"/> instance is shared by the
    /// <see cref="Parents"/> and the <see cref="Children"/> of the other tables, and the graph can be circular
    /// (i.e.: 2 tables that reference each other), so do not serialize it as it is.
    /// </summary>
    public class RelationshipInfo
    {
        #region Properties

        /// <summary>
        /// Gets or sets the schema of the table.
        /// </summary>
        public TableSchema Table { get; set; }

        /// <summary>
        /// Gets or sets the tables that this table references through its foreign keys (the tables that this table depends on,
        /// so they must exist first). A table that references itself is not included.
        /// </summary>
        public IList<RelationshipInfo> Parents { get; set; } = new List<RelationshipInfo>();

        /// <summary>
        /// Gets or sets the tables that reference this table through their foreign keys (the tables that depend on this table).
        /// A table that references itself is not included.
        /// </summary>
        public IList<RelationshipInfo> Children { get; set; } = new List<RelationshipInfo>();

        #endregion
    }
}
