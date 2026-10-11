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
        /// <summary>
        /// Adds a log to the progress.
        /// </summary>
        public static void Add(CopyDataProgress progress,
            IDbConnection destinationConnection,
            CopyDataLogLevel level,
            string text) =>
            progress?.Logs.Add(new CopyDataLog
            {
                DatabaseType = destinationConnection.GetType(),
                Level = level,
                Text = text,
                Timestamp = DateTime.UtcNow
            });

        /// <summary>
        /// Counts the rows of the table (<c>null</c> if the count is not possible).
        /// </summary>
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
        public static void LogBulkInsert(CopyDataProgress progress,
            IDbConnection destinationConnection,
            string table,
            int inserted,
            long? before,
            long? after)
        {
            Add(progress, destinationConnection, CopyDataLogLevel.Info, $"{inserted} row(s) were sent to the table '{table}' by the bulk insert.");
            if (before.HasValue && after.HasValue)
            {
                var loaded = after.Value - before.Value;
                if (loaded < inserted)
                {
                    Add(progress, destinationConnection, CopyDataLogLevel.Warning,
                        $"{inserted - loaded} of {inserted} row(s) were skipped by the database provider in the table '{table}' (a key, a unique or a foreign key constraint was violated). The rows that reference the skipped rows are skipped too.");
                }
            }
        }
    }
}
