#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;

namespace RepoDb.Data.Models
{
    /// <summary>
    /// A class that holds the details of the copy of one table: the source table, the target table that its rows were copied into, the batches and the logs.
    /// </summary>
    public class CopyProgressItem
    {
        #region Properties

        /// <summary>
        /// Gets the batches of rows that were copied so far (see <see cref="CopyBatch"/>).
        /// </summary>
        public IList<CopyBatch> Batches { get; internal set; } = new List<CopyBatch>();

        /// <summary>
        /// Gets the time that the copy of the table took (or has taken so far, if the copy of the table has not ended).
        /// </summary>
        public TimeSpan Elapsed => EndTime - StartTime;

        /// <summary>
        /// Gets the time (in UTC) when the copy of the table has ended (or when its last batch has ended, if the copy of the table has not ended).
        /// </summary>
        public DateTime EndTime { get; internal set; }

        /// <summary>
        /// Gets the logs of the copy of the table (see <see cref="Log"/>).
        /// </summary>
        public IList<Log> Logs { get; internal set; } = new List<Log>();

        /// <summary>
        /// Gets or sets the time (in UTC) when the next batch starts: the end of the progress callback of the previous batch, so the time that the callback takes
        /// is not counted in the next batch.
        /// </summary>
        internal DateTime NextBatchStart { get; set; }

        /// <summary>
        /// Gets the number of rows that were sent to the target table.
        /// </summary>
        public int RowCount { get; internal set; }

        /// <summary>
        /// Gets the number of rows that were sent to the target table but were not loaded by the database provider (i.e.: a key, a unique or a foreign key
        /// constraint was violated), or <c>null</c> if it is not known (it is known when the rows of the target table are counted).
        /// </summary>
        public long? SkippedRowCount { get; internal set; }

        /// <summary>
        /// Gets the details of the source table.
        /// </summary>
        public Endpoint Source { get; internal set; } = new Endpoint();

        /// <summary>
        /// Gets the time (in UTC) when the copy of the table has started.
        /// </summary>
        public DateTime StartTime { get; internal set; }

        /// <summary>
        /// Gets the details of the target table, which is the table that the rows of the <see cref="Source"/> were copied into.
        /// </summary>
        public Endpoint Target { get; internal set; } = new Endpoint();

        #endregion
    }
}
