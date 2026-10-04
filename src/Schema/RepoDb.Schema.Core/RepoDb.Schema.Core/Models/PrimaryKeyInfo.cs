#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.Collections.Generic;

namespace RepoDb.Schema.Models
{
    /// <summary>
    /// A class that holds the definition of the primary key of a table.
    /// </summary>
    public class PrimaryKeyInfo
    {
        #region Properties

        /// <summary>
        /// Gets or sets the name of the primary key constraint.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the ordered names of the columns of the primary key.
        /// </summary>
        public IList<string> Columns { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets a value that indicates whether the primary key is clustered. The default is <c>true</c>, which is the default of the databases that cluster their primary keys.
        /// </summary>
        public bool IsClustered { get; set; } = true;

        #endregion
    }
}
