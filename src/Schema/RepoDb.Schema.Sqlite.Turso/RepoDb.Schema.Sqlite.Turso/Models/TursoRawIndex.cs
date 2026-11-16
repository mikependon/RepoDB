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
    internal sealed class TursoRawIndex
    {
        #region Properties

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the is unique.
        /// </summary>
        public bool IsUnique { get; set; }

        /// <summary>
        /// Gets or sets the origin.
        /// </summary>
        public string Origin { get; set; }

        /// <summary>
        /// Gets or sets the is partial.
        /// </summary>
        public bool IsPartial { get; set; }

        /// <summary>
        /// Gets or sets the definition.
        /// </summary>
        public string Definition { get; set; }

        /// <summary>
        /// Gets or sets the columns.
        /// </summary>
        public IList<TursoRawIndexColumn> Columns { get; set; } = new List<TursoRawIndexColumn>();
        #endregion
    }
}
