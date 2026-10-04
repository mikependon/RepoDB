#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.Collections.Generic;
using RepoDb.Schema.Enumerations;

namespace RepoDb.Schema.Models
{
    /// <summary>
    /// A class that holds the definition of a foreign key of a table.
    /// </summary>
    public class ForeignKeyInfo
    {
        #region Properties

        /// <summary>
        /// Gets or sets the name of the foreign key constraint.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the ordered names of the columns of the foreign key.
        /// </summary>
        public IList<string> Columns { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets the name of the table that is referenced by the foreign key.
        /// </summary>
        public string ReferencedTable { get; set; }

        /// <summary>
        /// Gets or sets the ordered names of the referenced columns.
        /// </summary>
        public IList<string> ReferencedColumns { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets the rule that is applied when the referenced row is updated.
        /// </summary>
        public ForeignKeyRule UpdateRule { get; set; } = ForeignKeyRule.NoAction;

        /// <summary>
        /// Gets or sets the rule that is applied when the referenced row is deleted.
        /// </summary>
        public ForeignKeyRule DeleteRule { get; set; } = ForeignKeyRule.NoAction;

        #endregion
    }
}
