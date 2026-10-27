#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Data;
using RepoDb.Data.Enumerations;
using RepoDb.Data.Models;

namespace RepoDb.Data.Helpers
{
    /// <summary>
    /// Writes the logs of a copy operation using the counts of rows sent from memory.
    /// </summary>
    internal static class CopyDataLogger
    {
        #region Public Methods

        /// <summary>
        /// Adds a log to the progress, and to the table that is being copied.
        /// </summary>
        /// <param name="progress">The progress that holds the logs (nothing is added if it is <c>null</c>).</param>
        /// <param name="destinationConnection">The destination connection, where the activity has happened.</param>
        /// <param name="level">The level (the kind) of the log.</param>
        /// <param name="text">The text of the log.</param>
        public static void Add(CopyProgress progress,
            IDbConnection destinationConnection,
            LogLevel level,
            string text)
        {
            if (progress == null)
            {
                return;
            }
            var log = new Log
            {
                DatabaseType = destinationConnection.GetType(),
                Level = level,
                Text = text,
                Timestamp = DateTime.UtcNow
            };
            progress.Logs.Add(log);
            progress.Current?.Logs.Add(log);
        }

        /// <summary>Logs the number of rows sent from the in-memory batches.</summary>
        public static void LogBulkInsert(CopyProgress progress,
            IDbConnection destinationConnection,
            string table,
            int inserted) =>
            Add(progress, destinationConnection, LogLevel.Info, $"{inserted} row(s) were sent to the table '{table}' by the bulk insert.");

        /// <summary>
        /// Logs the result of the bulk merge of a table. The rows of the target table are not compared, as the merge updates the rows that already exist instead of adding them.
        /// </summary>
        /// <param name="progress">The progress that holds the logs.</param>
        /// <param name="destinationConnection">The destination connection.</param>
        /// <param name="table">The target table.</param>
        /// <param name="merged">The number of rows that the bulk merge has reported.</param>
        public static void LogBulkMerge(CopyProgress progress,
            IDbConnection destinationConnection,
            string table,
            int merged) =>
            Add(progress, destinationConnection, LogLevel.Info, $"{merged} row(s) were merged into the table '{table}' by the bulk merge.");

        #endregion
    }
}
