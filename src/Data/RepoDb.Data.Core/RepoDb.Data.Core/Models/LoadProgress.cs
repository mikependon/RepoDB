#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;

namespace RepoDb.Data.Models
{
    /// <summary>
    /// A class that holds the progress of a load of the rows of a file into a table (see <c>ImportDataFromJson</c>, <c>ImportDataFromXml</c> and <c>ImportDataFromCsv</c>). The same instance is passed to the progress callback
    /// every time, and it is returned when the load ends, so read what is needed inside the callback.
    /// </summary>
    public class LoadProgress
    {
        #region Properties

        /// <summary>
        /// Gets the number of the batch (or bulk) operation that has just succeeded, starting at 1. It is 0 until the first one succeeds.
        /// </summary>
        public int BatchNumber { get; internal set; }

        /// <summary>
        /// Gets the number of rows that were loaded into the table so far.
        /// </summary>
        public long LoadedRowCount { get; internal set; }

        /// <summary>
        /// Gets the time (in UTC) when the load has started.
        /// </summary>
        public DateTime Start { get; internal set; }

        /// <summary>
        /// Gets the number of rows that the file has, which is counted before any row is loaded.
        /// </summary>
        public long TotalRowCount { get; internal set; }

        #endregion
    }
}
