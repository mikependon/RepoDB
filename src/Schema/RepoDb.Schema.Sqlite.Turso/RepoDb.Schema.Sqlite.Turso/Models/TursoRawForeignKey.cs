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
    internal sealed class TursoRawForeignKey
    {
        #region Properties

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the referenced table.
        /// </summary>
        public string ReferencedTable { get; set; }

        /// <summary>
        /// Gets or sets the column.
        /// </summary>
        public string Column { get; set; }

        /// <summary>
        /// Gets or sets the referenced column.
        /// </summary>
        public string ReferencedColumn { get; set; }

        /// <summary>
        /// Gets or sets the update.
        /// </summary>
        public string Update { get; set; }

        /// <summary>
        /// Gets or sets the delete.
        /// </summary>
        public string Delete { get; set; }
        #endregion
    }
}
