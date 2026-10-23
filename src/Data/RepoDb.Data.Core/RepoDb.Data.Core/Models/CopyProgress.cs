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
    /// A class that holds the details of a copy operation.
    /// </summary>
    public class CopyProgress
    {
        #region Properties

        /// <summary>
        /// Gets the number of rows that were copied in the current batch.
        /// </summary>
        public int BatchCopiedRowCount { get; internal set; }

        /// <summary>
        /// Gets the time (in UTC) when the current batch has ended.
        /// </summary>
        public DateTime BatchEndTime { get; internal set; }

        /// <summary>
        /// Gets the batch number of the copy operation.
        /// </summary>
        public int BatchNumber { get; internal set; }

        /// <summary>
        /// Gets the time (in UTC) when the current batch has started.
        /// </summary>
        public DateTime BatchStartTime { get; internal set; }

        /// <summary>
        /// Gets the time (in UTC) when the copy operation has ended (the end of the last batch that is copied so far).
        /// </summary>
        public DateTime EndTime { get; internal set; }

        /// <summary>
        /// Gets the logs (information and warnings) of the copy operation, i.e.: the rows that were skipped by the database provider.
        /// </summary>
        public List<CopyLog> Logs { get; internal set; } = new List<CopyLog>();

        /// <summary>
        /// Gets the current number of rows that were copied.
        /// </summary>
        public int RowCount { get; internal set; }

        /// <summary>
        /// Gets the name of the source database.
        /// </summary>
        public string SourceDatabase { get; internal set; }

        /// <summary>
        /// Gets the name of the source server (the data source of the source connection).
        /// </summary>
        public string SourceServer { get; internal set; }

        /// <summary>
        /// Gets the name of the source table that is being copied.
        /// </summary>
        public string SourceTable { get; internal set; }

        /// <summary>
        /// Gets the time (in UTC) when the copy operation has started.
        /// </summary>
        public DateTime StartTime { get; internal set; }

        /// <summary>
        /// Gets the name of the target database.
        /// </summary>
        public string TargetDatabase { get; internal set; }

        /// <summary>
        /// Gets the name of the target server (the data source of the destination connection).
        /// </summary>
        public string TargetServer { get; internal set; }

        /// <summary>
        /// Gets the name of the target table that is being copied into.
        /// </summary>
        public string TargetTable { get; internal set; }

        /// <summary>
        /// Gets the total number of rows that were copied.
        /// </summary>
        public int TotalCopiedRowCount { get; internal set; }

        #endregion
    }
}
