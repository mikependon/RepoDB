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
    internal sealed class SqliteRawColumn
    {
        #region Properties

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the declared type.
        /// </summary>
        public string DeclaredType { get; set; }

        /// <summary>
        /// Gets or sets the not null.
        /// </summary>
        public bool NotNull { get; set; }

        /// <summary>
        /// Gets or sets the default.
        /// </summary>
        public string Default { get; set; }

        /// <summary>
        /// Gets or sets the primary key position.
        /// </summary>
        public int PrimaryKeyPosition { get; set; }

        /// <summary>
        /// Gets or sets the hidden.
        /// </summary>
        public int Hidden { get; set; }
        #endregion
    }
}
