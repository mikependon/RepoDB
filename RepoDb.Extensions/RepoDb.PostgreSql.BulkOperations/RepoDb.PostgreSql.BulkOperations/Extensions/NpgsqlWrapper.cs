using Npgsql;
using RepoDb.Enumerations.PostgreSql;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.PostgreSql.BulkOperations;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RepoDb
{
    /// <summary>
    /// 
    /// </summary>
    public static partial class NpgsqlConnectionExtension
    {
        #region PseudoBasedBinaryImport

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="getPseudoTableName"></param>
        /// <param name="getMappings"></param>
        /// <param name="dbFields"></param>
        /// <param name="binaryImport"></param>
        /// <param name="setIdentities"></param>
        /// <param name="identityBehavior"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="dbSetting"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private static int PseudoBasedBinaryImport(this NpgsqlConnection connection,
            string tableName,
            int? bulkCopyTimeout,
            Func<string> getPseudoTableName,
            Func<IEnumerable<NpgsqlBulkInsertMapItem>> getMappings,
            IEnumerable<DbField> dbFields,
            Func<string, int> binaryImport,
            Action<IEnumerable<long>> setIdentities,
            BulkImportIdentityBehavior identityBehavior,
            BulkImportPseudoTableType pseudoTableType,
            IDbSetting dbSetting,
            NpgsqlTransaction transaction)
        {
            string pseudoTableName = null;

            try
            {
                // Mappings
                var mappings = getMappings?.Invoke();

                // Create (TEMP)
                if (identityBehavior == BulkImportIdentityBehavior.ReturnIdentity)
                {
                    CreatePseudoTable(connection,
                        tableName,
                        () => (pseudoTableName = getPseudoTableName?.Invoke()),
                        mappings,
                        bulkCopyTimeout,
                        identityBehavior,
                        pseudoTableType,
                        dbSetting,
                        transaction);
                }

                // Import
                var result = binaryImport?.Invoke(pseudoTableName ?? tableName);

                // Insert (INTO)
                if (identityBehavior == BulkImportIdentityBehavior.ReturnIdentity)
                {
                    var identities = InsertPseudoTable(connection,
                        tableName,
                        pseudoTableName,
                        mappings,
                        dbFields,
                        bulkCopyTimeout,
                        dbSetting,
                        transaction).AsList();

                    setIdentities?.Invoke(identities);
                }

                // Return
                return result.GetValueOrDefault();
            }
            finally
            {
                if (identityBehavior == BulkImportIdentityBehavior.ReturnIdentity)
                {
                    DropPseudoTable(connection, pseudoTableName, bulkCopyTimeout, transaction);
                }
            }
        }

        #endregion

        #region TransactionalExecute

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="connection"></param>
        /// <param name="execute"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private static TResult TransactionalExecute<TResult>(this NpgsqlConnection connection,
            Func<TResult> execute,
            NpgsqlTransaction transaction)
        {
            // Variables
            var result = default(TResult);
            var hasTransaction = transaction != null;

            // Open
            connection.EnsureOpen();

            // Ensure transaction
            if (hasTransaction == false)
            {
                transaction = connection.BeginTransaction();
            }

            try
            {
                // Execute
                if (execute != null)
                {
                    result = execute.Invoke();
                }

                // Commit
                if (hasTransaction == false)
                {
                    transaction.Commit();
                }
            }
            catch
            {
                // Rollback
                if (hasTransaction == false)
                {
                    transaction.Rollback();
                }

                // Throw
                throw;
            }
            finally
            {
                // Dispose
                if (hasTransaction == false)
                {
                    transaction.Dispose();
                }
            }

            // Return
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="connection"></param>
        /// <param name="executeAsync"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private static async Task<TResult> TransactionalExecuteAsync<TResult>(this NpgsqlConnection connection,
            Func<Task<TResult>> executeAsync,
            NpgsqlTransaction transaction,
            CancellationToken cancellationToken = default)
        {
            // Variables
            var result = default(TResult);
            var hasTransaction = transaction != null;

            // Open
            connection.EnsureOpen();

            // Ensure transaction
            if (hasTransaction == false)
            {
#if NET5_0
                transaction = await connection.BeginTransactionAsync(cancellationToken);
#else
                transaction = connection.BeginTransaction();
#endif
            }

            try
            {
                // Execute
                if (executeAsync != null)
                {
                    result = await executeAsync();
                }

                // Commit
                if (hasTransaction == false)
                {
                    await transaction.CommitAsync(cancellationToken);
                }
            }
            catch
            {
                // Rollback
                if (hasTransaction == false)
                {
                    await transaction.RollbackAsync(cancellationToken);
                }

                // Throw
                throw;
            }
            finally
            {
                // Dispose
                if (hasTransaction == false)
                {
                    await transaction.DisposeAsync();
                }
            }

            // Return
            return result;
        }

        #endregion
    }
}
