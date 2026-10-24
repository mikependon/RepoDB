#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;

namespace RepoDb.Data.Models
{
    /// <summary>
    /// A class that holds the details of a copy operation: when it ran, how much it has copied so far, and the tables that it copied
    /// (each one with its source and its target table, its batches and its logs). The same instance is passed to the progress callback after every batch,
    /// so read what is needed inside the callback.
    /// </summary>
    public class CopyProgress
    {
        #region Properties

        /// <summary>
        /// Gets the number of batches that were copied so far, for all the tables.
        /// </summary>
        public int BatchCount { get; internal set; }

        /// <summary>
        /// Gets the number of tables whose copy has ended.
        /// </summary>
        public int CompletedTableCount { get; internal set; }

        /// <summary>
        /// Gets the table that is being copied (or the last one that was copied, once the operation has ended).
        /// </summary>
        public CopyProgressItem Current { get; internal set; }

        /// <summary>
        /// Gets the last batch that was copied for the <see cref="Current"/> table, or <c>null</c> if there is none yet.
        /// </summary>
        public CopyBatch CurrentBatch => Current?.Batches.LastOrDefault();

        /// <summary>
        /// Gets the time that the operation took (or has taken so far, if it has not ended).
        /// </summary>
        public TimeSpan Elapsed => EndTime - StartTime;

        /// <summary>
        /// Gets the time (in UTC) when the operation has ended (or when its last batch has ended, if it has not ended).
        /// </summary>
        public DateTime EndTime { get; internal set; }

        /// <summary>
        /// Gets the tables that were copied (see <see cref="CopyProgressItem"/>), in the order that they were copied: each one has its source table and the target table
        /// that it was copied into. A table is added when its copy starts.
        /// </summary>
        public IList<CopyProgressItem> Items { get; internal set; } = new List<CopyProgressItem>();

        /// <summary>
        /// Gets the logs of the operation (see <see cref="Log"/>), for all the tables.
        /// </summary>
        public IList<Log> Logs { get; internal set; } = new List<Log>();

        /// <summary>
        /// Gets the number of rows that were copied so far, for all the tables.
        /// </summary>
        public int RowCount { get; internal set; }

        /// <summary>
        /// Gets the number of rows that were copied per second so far, or <c>0</c> if no time has passed.
        /// </summary>
        public double RowsPerSecond => Elapsed.TotalSeconds > 0 ? RowCount / Elapsed.TotalSeconds : 0;

        /// <summary>
        /// Gets the time (in UTC) when the operation has started.
        /// </summary>
        public DateTime StartTime { get; internal set; }

        /// <summary>
        /// Gets the number of tables that are to be copied (the requested table and, depending on the relationship behavior, its related tables).
        /// </summary>
        public int TableCount { get; internal set; }

        #endregion
    }
}
