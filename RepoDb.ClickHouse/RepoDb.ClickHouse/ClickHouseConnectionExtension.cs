using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using ClickHouse.Driver.ADO;

namespace RepoDb
{
    /// <summary>
    /// Provides extension methods for the ClickHouse connection.
    /// </summary>
    public static class ClickHouseConnectionExtension
    {
        private static readonly TimeSpan MutationPollInterval = TimeSpan.FromMilliseconds(100);
        private static readonly TimeSpan MutationWaitTimeout = TimeSpan.FromSeconds(5);

        /// <summary>
        /// Ensures that all mutations on the specified table have completed before proceeding. This method will block until all mutations are done or until a timeout occurs.
        /// </summary>
        /// <param name="connection">The ClickHouse connection.</param>
        /// <param name="tableName">The name of the table for which to wait for mutations.</param>
        /// <param name="transaction">The database transaction.</param>
        /// <exception cref="TimeoutException"></exception>
        public static void WaitForMutations(this ClickHouseConnection connection,
            string tableName,
            DbTransaction transaction)
        {
            var deadline = DateTime.UtcNow.Add(MutationWaitTimeout);
            while (true)
            {
                var pending = connection.ExecuteScalar<long>(
                    "SELECT COUNT(1) FROM system.mutations WHERE database = @Database AND table = @Table AND is_done = 0;",
                    new { connection.Database, Table = tableName },
                    transaction: transaction);
                if (pending == 0)
                {
                    return;
                }
                if (DateTime.UtcNow >= deadline)
                {
                    throw new TimeoutException($"Timed out waiting for pending mutations on table '{tableName}' to complete.");
                }
                Thread.Sleep(MutationPollInterval);
            }
        }

        /// <summary>
        /// Ensures that all mutations on the specified table have completed before proceeding. This method will block until all mutations are done or until a timeout occurs.
        /// </summary>
        /// <param name="connection">The ClickHouse connection.</param>
        /// <param name="tableName">The name of the table for which to wait for mutations.</param>
        /// <param name="transaction">The database transaction.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <exception cref="TimeoutException"></exception>
        public static async Task WaitForMutationsAsync(this ClickHouseConnection connection,
            string tableName,
            DbTransaction transaction,
            CancellationToken cancellationToken = default)
        {
            var deadline = DateTime.UtcNow.Add(MutationWaitTimeout);
            while (true)
            {
                var pending = await connection.ExecuteScalarAsync<long>(
                    "SELECT COUNT(1) FROM system.mutations WHERE database = @Database AND table = @Table AND is_done = 0;",
                    new { connection.Database, Table = tableName },
                    transaction: transaction,
                    cancellationToken: cancellationToken);
                if (pending == 0)
                {
                    return;
                }
                if (DateTime.UtcNow >= deadline)
                {
                    throw new TimeoutException($"Timed out waiting for pending mutations on table '{tableName}' to complete.");
                }
                Thread.Sleep(MutationPollInterval);
            }
        }
    }
}
