#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using RepoDb.Data.Enumerations;
using RepoDb.Data.Models;
using RepoDb.Extensions;

namespace RepoDb.Data.Helpers
{
    /// <summary>
    /// Writes the logs of a copy operation. Some providers do not fail when the rows violate a key (they skip the rows): the logs tell it.
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

        /// <summary>
        /// Counts the rows of the table (<c>null</c> if the count is not possible).
        /// </summary>
        /// <param name="destinationConnection">The destination connection.</param>
        /// <param name="table">The name of the table.</param>
        /// <param name="transaction">The transaction of the destination connection.</param>
        /// <returns>The number of rows of the table, or <c>null</c> if the count is not possible.</returns>
        public static long? Count(IDbConnection destinationConnection,
            string table,
            IDbTransaction transaction)
        {
            try
            {
                return destinationConnection.Count(table, where: (QueryGroup)null, transaction: transaction);
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Counts the rows of the table in an asynchronous way (<c>null</c> if the count is not possible).
        /// </summary>
        /// <param name="destinationConnection">The destination connection.</param>
        /// <param name="table">The name of the table.</param>
        /// <param name="transaction">The transaction of the destination connection.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains: the number of rows of the table, or <c>null</c> if the count is not possible.</returns>
        public static async Task<long?> CountAsync(IDbConnection destinationConnection,
            string table,
            IDbTransaction transaction,
            CancellationToken cancellationToken)
        {
            try
            {
                return await destinationConnection.CountAsync(table, where: (QueryGroup)null, transaction: transaction, cancellationToken: cancellationToken).ConfigureAwait(false);
            }
            catch (Exception) when (!cancellationToken.IsCancellationRequested)
            {
                return null;
            }
        }

        /// <summary>
        /// Logs the result of the bulk insert of a table: the information of the copy, and the warning when the provider skipped the rows.
        /// </summary>
        /// <param name="progress">The progress that holds the logs.</param>
        /// <param name="destinationConnection">The destination connection.</param>
        /// <param name="table">The target table.</param>
        /// <param name="inserted">The number of rows that the bulk insert has reported.</param>
        /// <param name="before">The number of rows of the target table before the bulk insert.</param>
        /// <param name="after">The number of rows of the target table after the bulk insert.</param>
        /// <returns>The number of rows that the provider did not load, or <c>null</c> if the rows of the target table could not be counted.</returns>
        public static long? LogBulkInsert(CopyProgress progress,
            IDbConnection destinationConnection,
            string table,
            int inserted,
            long? before,
            long? after)
        {
            Add(progress, destinationConnection, LogLevel.Info, $"{inserted} row(s) were sent to the table '{table}' by the bulk insert.");
            if (!before.HasValue || !after.HasValue)
            {
                return null;
            }
            var loaded = after.Value - before.Value;
            if (loaded >= inserted)
            {
                return 0;
            }
            Add(progress, destinationConnection, LogLevel.Warning,
                $"{inserted - loaded} of {inserted} row(s) were skipped by the database provider in the table '{table}' (a key, a unique or a foreign key constraint was violated). The rows that reference the skipped rows are skipped too.");
            return inserted - loaded;
        }

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
