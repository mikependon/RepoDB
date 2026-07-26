using Oracle.ManagedDataAccess.Client;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

namespace RepoDb
{
    /// <summary>
    /// Generic transaction-enlistment wrapper shared by every Oracle bulk operation. The equivalent of the
    /// PostgreSQL bulk package's <c>TransactionalExecute</c>/<c>TransactionalExecuteAsync</c> - unlike that
    /// package, there is no single all-ops <c>PseudoBasedBinaryImport</c> orchestrator here, since Oracle's
    /// four operations are shaped too differently to share one (BulkInsert never touches a staging table at
    /// all; BulkMerge needs an extra identity-lookup step; BulkUpdate/BulkDelete are a single ExecuteNonQuery).
    /// Each operation's own orchestration lives directly in its <c>Base/*.cs</c> file instead.
    /// </summary>
    internal static class OracleWrapper
    {
        /// <summary>
        /// Runs <paramref name="execute"/> inside a transaction, opening the connection and starting/committing/
        /// rolling back a local transaction when the caller did not already supply one (or is not already
        /// inside an ambient <see cref="System.Transactions.Transaction"/>).
        /// </summary>
        public static TResult TransactionalExecute<TResult>(this OracleConnection connection,
            Func<OracleTransaction, TResult> execute,
            OracleTransaction transaction)
        {
            var result = default(TResult);
            var hasTransaction = transaction != null || Transaction.Current != null;

            connection.EnsureOpen();

            if (hasTransaction == false)
            {
                transaction = connection.BeginTransaction();
            }

            try
            {
                if (execute != null)
                {
                    result = execute(transaction);
                }

                if (hasTransaction == false)
                {
                    transaction.Commit();
                }
            }
            catch
            {
                if (hasTransaction == false)
                {
                    transaction.Rollback();
                }

                throw;
            }
            finally
            {
                if (hasTransaction == false)
                {
                    transaction.Dispose();
                }
            }

            return result;
        }

        /// <summary>
        /// Asynchronous counterpart of <see cref="TransactionalExecute{TResult}"/>.
        /// </summary>
        public static async Task<TResult> TransactionalExecuteAsync<TResult>(this OracleConnection connection,
            Func<OracleTransaction, Task<TResult>> executeAsync,
            OracleTransaction transaction,
            CancellationToken cancellationToken = default)
        {
            var result = default(TResult);
            var hasTransaction = transaction != null || Transaction.Current != null;

            await connection.EnsureOpenAsync(cancellationToken);

            if (hasTransaction == false)
            {
                transaction = connection.BeginTransaction();
            }

            try
            {
                if (executeAsync != null)
                {
                    result = await executeAsync(transaction);
                }

                if (hasTransaction == false)
                {
                    transaction.Commit();
                }
            }
            catch
            {
                if (hasTransaction == false)
                {
                    transaction.Rollback();
                }

                throw;
            }
            finally
            {
                if (hasTransaction == false)
                {
                    transaction.Dispose();
                }
            }

            return result;
        }
    }
}
