#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;

namespace RepoDb.Data.Models
{
    /// <summary>
    /// A class that holds the details of a copy operation.
    /// </summary>
    public class CopyDataProgress
    {
        #region Properties

        /// <summary>
        /// Gets or sets the batch number of the copy operation.
        /// </summary>
        public int BatchNumber { get; set; }

        /// <summary>
        /// Gets or sets the time (in UTC) when the copy operation has ended.
        /// </summary>
        public DateTime EndTime { get; set; }

        /// <summary>
        /// Gets or sets the current number of rows that were copied.
        /// </summary>
        public int RowCount { get; set; }

        /// <summary>
        /// Gets or sets the time (in UTC) when the copy operation has started.
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// Gets or sets the total number of rows that were copied.
        /// </summary>
        public int TotalCopiedRowCount { get; set; }

        #endregion
    }
}
