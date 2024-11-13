using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Npgsql;
using RepoDb.Enumerations.PostgreSql;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.PostgreSql.BulkOperations;

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
        /// <param name="dbFields"></param>
        /// <param name="getPseudoTableName"></param>
        /// <param name="getMappings"></param>
        /// <param name="binaryImport"></param>
        /// <param name="getMergeToPseudoCommandText"></param>
        /// <param name="setIdentities"></param>
        /// <param name="qualifiers"></param>
        /// <param name="isBinaryBulkInsert"></param>
        /// <param name="identityBehavior"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="dbSetting"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private static int PseudoBasedBinaryImport(this NpgsqlConnection connection,
            string tableName,
            int? bulkCopyTimeout,
            DbFieldCollection dbFields,
            Func<string> getPseudoTableName,
            Func<IEnumerable<NpgsqlBulkInsertMapItem>> getMappings,
            Func<string, int> binaryImport,
            Func<string> getMergeToPseudoCommandText,
            Action<IEnumerable<IdentityResult>> setIdentities,
            IEnumerable<Field> qualifiers,
            bool isBinaryBulkInsert,
            BulkImportIdentityBehavior identityBehavior,
            BulkImportPseudoTableType pseudoTableType,
            IDbSetting dbSetting,
            NpgsqlTransaction transaction)
        {
            string pseudoTableName = null;
            var withPseudoTable = identityBehavior == BulkImportIdentityBehavior.ReturnIdentity ||
                isBinaryBulkInsert == false;

            try
            {
                // Mappings
                var mappings = getMappings?.Invoke();

                // Create (TEMP)
                if (withPseudoTable)
                {
                    pseudoTableName = getPseudoTableName?.Invoke();

                    DropPseudoTable(connection,
                        pseudoTableName,
                        bulkCopyTimeout,
                        transaction);

                    CreatePseudoTable(connection,
                        tableName,
                        pseudoTableName,
                        mappings,
                        bulkCopyTimeout,
                        identityBehavior,
                        pseudoTableType,
                        dbSetting,
                        transaction);
                }

                // Import
                var result = binaryImport?.Invoke(pseudoTableName ?? tableName);

                // Create Index
                if (isBinaryBulkInsert == false && withPseudoTable)
                {
                    qualifiers = qualifiers?.Any() == true ? qualifiers :
                        dbFields?.GetPrimary().AsField().AsEnumerable();

                    CreatePseudoTableIndex(connection,
                        pseudoTableName,
                        qualifiers,
                        bulkCopyTimeout,
                        dbSetting,
                        transaction);
                }

                // Merge/Update/Delete
                if (withPseudoTable)
                {
                    var identityResults = MergeToPseudoTableWithIdentityResults(connection,
                        getMergeToPseudoCommandText,
                        bulkCopyTimeout,
                        transaction)?.AsList();

                    if (identityBehavior == BulkImportIdentityBehavior.ReturnIdentity)
                    {
                        setIdentities?.Invoke(identityResults);
                    }

                    result = identityResults.Count();
                }

                // Return
                return result.GetValueOrDefault();
            }
            finally
            {
                if (withPseudoTable)
                {
                    DropPseudoTable(connection,
                        pseudoTableName,
                        bulkCopyTimeout,
                        transaction);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="dbFields"></param>
        /// <param name="getPseudoTableName"></param>
        /// <param name="getMappings"></param>
        /// <param name="binaryImportAsync"></param>
        /// <param name="getMergeToPseudoCommandText"></param>
        /// <param name="setIdentities"></param>
        /// <param name="qualifiers"></param>
        /// <param name="isBinaryBulkInsert"></param>
        /// <param name="identityBehavior"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="dbSetting"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private static async Task<int> PseudoBasedBinaryImportAsync(this NpgsqlConnection connection,
            string tableName,
            int? bulkCopyTimeout,
            DbFieldCollection dbFields,
            Func<string> getPseudoTableName,
            Func<IEnumerable<NpgsqlBulkInsertMapItem>> getMappings,
            Func<string, Task<int>> binaryImportAsync,
            Func<string> getMergeToPseudoCommandText,
            Action<IEnumerable<IdentityResult>> setIdentities,
            IEnumerable<Field> qualifiers,
            bool isBinaryBulkInsert,
            BulkImportIdentityBehavior identityBehavior,
            BulkImportPseudoTableType pseudoTableType,
            IDbSetting dbSetting,
            NpgsqlTransaction transaction,
            CancellationToken cancellationToken = default)
        {
            string pseudoTableName = null;
            var withPseudoTable = identityBehavior == BulkImportIdentityBehavior.ReturnIdentity ||
                isBinaryBulkInsert == false;

            try
            {
                // Mappings
                var mappings = getMappings?.Invoke();

                // Create (TEMP)
                if (withPseudoTable)
                {
                    pseudoTableName = getPseudoTableName?.Invoke();

                    await DropPseudoTableAsync(connection,
                        pseudoTableName,
                        bulkCopyTimeout,
                        transaction,
                        cancellationToken);

                    await CreatePseudoTableAsync(connection,
                        tableName,
                        pseudoTableName,
                        mappings,
                        bulkCopyTimeout,
                        identityBehavior,
                        pseudoTableType,
                        dbSetting,
                        transaction,
                        cancellationToken);
                }

                // Import
                var result = await binaryImportAsync?.Invoke(pseudoTableName ?? tableName);

                // Create Index
                if (isBinaryBulkInsert == false && withPseudoTable)
                {
                    qualifiers = qualifiers?.Any() == true ? qualifiers :
                        dbFields?.GetPrimary().AsField().AsEnumerable();

                    await CreatePseudoTableIndexAsync(connection,
                        pseudoTableName,
                        qualifiers,
                        bulkCopyTimeout,
                        dbSetting,
                        transaction,
                        cancellationToken);
                }

                // Insert (INTO)
                if (withPseudoTable)
                {
                    var identityResults = (await MergeToPseudoTableWithIdentityResultsAsync(connection,
                        getMergeToPseudoCommandText,
                        bulkCopyTimeout,
                        transaction))?.AsList();

                    if (identityBehavior == BulkImportIdentityBehavior.ReturnIdentity)
                    {
                        setIdentities?.Invoke(identityResults);
                    }

                    result = identityResults.Count;
                }

                // Return
                return result;
            }
            finally
            {
                if (withPseudoTable)
                {
                    await DropPseudoTableAsync(connection,
                        pseudoTableName,
                        bulkCopyTimeout,
                        transaction,
                        cancellationToken);
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
            var hasTransaction = (transaction != null || Transaction.Current != null);

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
                    result = execute();
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
            var hasTransaction = (transaction != null || Transaction.Current != null);

            // Open
            await connection.EnsureOpenAsync(cancellationToken);

            // Ensure transaction
            if (hasTransaction == false)
            {
#if NET
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
