#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;

namespace RepoDb.Data.Models
{
    /// <summary>
    /// A class that holds the details of a batch of rows that was copied into a target table. Rows are buffered and reported one batch at a time.
    /// </summary>
    public class CopyBatch
    {
        #region Properties

        /// <summary>
        /// Gets the estimated cumulative bytes of copied values, excluding object overhead and wire encoding.
        /// Strings use two bytes per character; nulls and unsupported provider-specific values contribute zero.
        /// </summary>
        public long CopiedBytes { get; internal set; }

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
