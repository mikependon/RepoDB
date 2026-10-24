#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;

namespace RepoDb.Data.Models
{
    /// <summary>
    /// A class that holds the progress of an export of the rows of a table into a file (see <c>ExportDataAsJson</c>). The same instance is passed to the progress callback
    /// every time, and it is returned when the export ends, so read what is needed inside the callback.
    /// </summary>
    public class ExportProgress
    {
        #region Properties

        /// <summary>
        /// Gets the position of the data reader: the number of rows that it has read so far (0 before the first row, then the position of the row that was just written).
        /// </summary>
        public long Position { get; internal set; }

        /// <summary>
        /// Gets the number of rows that were written into the file so far.
        /// </summary>
        public long CurrentRowCount { get; internal set; }

        /// <summary>
        /// Gets the time (in UTC) when the export has started.
        /// </summary>
        public DateTime Start { get; internal set; }

        /// <summary>
        /// Gets the time that has passed since the export has started.
        /// </summary>
        public TimeSpan Elapsed { get; internal set; }

        /// <summary>
        /// Gets the time (in UTC) when the export has ended, or <c>null</c> while it is running.
        /// </summary>
        public DateTime? End { get; internal set; }

        /// <summary>
        /// Gets the number of rows that the table has (the ones that the filter selects), which is counted before any row is written.
        /// </summary>
        public long TotalRowCount { get; internal set; }

        #endregion
    }
}
