#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;

namespace RepoDb.Data.Models
{
    /// <summary>
    /// A class that holds the details of a batch of rows that was copied into a target table. A provider that has a bulk insert
    /// receives all the rows of a table in one go, so such a table is reported as one batch.
    /// </summary>
    public class CopyBatch
    {
        #region Properties

        /// <summary>
        /// Gets the time that the batch took.
        /// </summary>
        public TimeSpan Elapsed => EndTime - StartTime;

        /// <summary>
        /// Gets the time (in UTC) when the batch has ended.
        /// </summary>
        public DateTime EndTime { get; internal set; }

        /// <summary>
        /// Gets the number of the batch within its table (starting at 1).
        /// </summary>
        public int Number { get; internal set; }

        /// <summary>
        /// Gets the number of rows that were copied by the batch.
        /// </summary>
        public int RowCount { get; internal set; }

        /// <summary>
        /// Gets the time (in UTC) when the batch has started.
        /// </summary>
        public DateTime StartTime { get; internal set; }

        #endregion
    }
}
