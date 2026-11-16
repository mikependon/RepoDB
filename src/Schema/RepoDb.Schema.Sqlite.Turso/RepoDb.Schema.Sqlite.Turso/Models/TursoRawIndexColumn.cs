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
    internal sealed class TursoRawIndexColumn
    {
        #region Properties

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the is descending.
        /// </summary>
        public bool IsDescending { get; set; }

        /// <summary>
        /// Gets or sets the is key.
        /// </summary>
        public bool IsKey { get; set; }
        #endregion
    }
}
