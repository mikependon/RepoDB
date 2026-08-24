using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using ClickHouse.Driver.ADO;

namespace RepoDb.ClickHouse.BulkOperations
{
    internal static class ClickHouseConnectionExtension
    {
        private static readonly TimeSpan MutationPollInterval = TimeSpan.FromMilliseconds(100);
        private static readonly TimeSpan MutationWaitTimeout = TimeSpan.FromSeconds(5);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="transaction"></param>
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
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="transaction"></param>
        /// <param name="token"></param>
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
