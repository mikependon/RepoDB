using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
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
        /// <param name="rowCount"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="dbFields"></param>
        /// <param name="getPseudoTableName"></param>
        /// <param name="getMappings"></param>
        /// <param name="binaryImport"></param>
        /// <param name="getMergeToPseudoCommandText"></param>
        /// <param name="setIdentities"></param>
        /// <param name="qualifiers"></param>
        /// <param name="isBulkInsert"></param>
        /// <param name="identityBehavior"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="dbSetting"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private static int PseudoBasedBinaryImport(this NpgsqlConnection connection,
            string tableName,
            int rowCount,
            int? bulkCopyTimeout,
            DbFieldCollection dbFields,
            Func<string> getPseudoTableName,
            Func<IEnumerable<PostgreSqlBulkInsertMapItem>> getMappings,
            Func<string, int> binaryImport,
            Func<string> getMergeToPseudoCommandText,
            Action<IEnumerable<IdentityResult>> setIdentities,
            IEnumerable<Field> qualifiers,
            bool isBulkInsert,
            PostgreSqlBulkImportIdentityBehavior identityBehavior,
            PostgreSqlBulkImportPseudoTableType pseudoTableType,
            IDbSetting dbSetting,
            ITrace trace,
            string traceKey,
            NpgsqlTransaction transaction)
        {
            string pseudoTableName = null;
            var withPseudoTable = identityBehavior == PostgreSqlBulkImportIdentityBehavior.ReturnIdentity ||
                isBulkInsert == false;

            try
            {
                // Before Execution
                using var command = CreateTraceCommand(connection, tableName, traceKey, bulkCopyTimeout, transaction);
                var traceResult = Tracer
                    .InvokeBeforeExecution(traceKey, trace, command);

                // Silent cancellation
                if (traceResult?.CancellableTraceLog?.IsCancelled == true)
                {
                    return default;
                }

                // Mappings
                var mappings = getMappings?.Invoke();

                // Create (TEMP)
                if (withPseudoTable)
                {
                    pseudoTableName = getPseudoTableName?.Invoke();

                    DropPseudoTable(connection,
                        pseudoTableName,
                        bulkCopyTimeout,
                        trace,
                        traceKey,
                        transaction);

                    CreatePseudoTable(connection,
                        tableName,
                        pseudoTableName,
                        rowCount,
                        mappings,
                        bulkCopyTimeout,
                        identityBehavior,
                        pseudoTableType,
                        dbSetting,
                        trace,
                        traceKey,
                        transaction);
                }

                // Create Index
                if (isBulkInsert == false && withPseudoTable)
                {
                    qualifiers = qualifiers?.Any() == true ? qualifiers :
                        dbFields?.GetPrimary().AsField().AsEnumerable();

                    CreatePseudoTableIndex(connection,
                        pseudoTableName,
                        qualifiers,
                        bulkCopyTimeout,
                        dbSetting,
                        trace,
                        traceKey,
                        transaction);
                }

                // Import
                var result = binaryImport?.Invoke(pseudoTableName ?? tableName);

                // Merge/Update/Delete
                if (withPseudoTable)
                {
                    var identityResults = MergeToPseudoTableWithIdentityResults(connection,
                        getMergeToPseudoCommandText,
                        bulkCopyTimeout,
                        transaction)?.AsList();

                    if (identityBehavior == PostgreSqlBulkImportIdentityBehavior.ReturnIdentity)
                    {
                        setIdentities?.Invoke(identityResults);
                    }

                    result = identityResults.Count();
                }

                // After Execution
                Tracer
                    .InvokeAfterExecution(traceResult, trace, result);

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
                        trace,
                        traceKey,
                        transaction);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="rowCount"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="dbFields"></param>
        /// <param name="getPseudoTableName"></param>
        /// <param name="getMappings"></param>
        /// <param name="binaryImportAsync"></param>
        /// <param name="getMergeToPseudoCommandText"></param>
        /// <param name="setIdentities"></param>
        /// <param name="qualifiers"></param>
        /// <param name="isBulkInsert"></param>
        /// <param name="identityBehavior"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="dbSetting"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private static async Task<int> PseudoBasedBinaryImportAsync(this NpgsqlConnection connection,
            string tableName,
            int rowCount,
            int? bulkCopyTimeout,
            DbFieldCollection dbFields,
            Func<string> getPseudoTableName,
            Func<IEnumerable<PostgreSqlBulkInsertMapItem>> getMappings,
            Func<string, Task<int>> binaryImportAsync,
            Func<string> getMergeToPseudoCommandText,
            Action<IEnumerable<IdentityResult>> setIdentities,
            IEnumerable<Field> qualifiers,
            bool isBulkInsert,
            PostgreSqlBulkImportIdentityBehavior identityBehavior,
            PostgreSqlBulkImportPseudoTableType pseudoTableType,
            IDbSetting dbSetting,
            ITrace trace,
            string traceKey,
            NpgsqlTransaction transaction,
            CancellationToken cancellationToken = default)
        {
            string pseudoTableName = null;
            var withPseudoTable = identityBehavior == PostgreSqlBulkImportIdentityBehavior.ReturnIdentity ||
                isBulkInsert == false;

            try
            {
                // Before Execution
                using var command = CreateTraceCommand(connection, tableName, traceKey, bulkCopyTimeout, transaction);
                var traceResult = await Tracer
                    .InvokeBeforeExecutionAsync(traceKey, trace, command, cancellationToken);

                // Silent cancellation
                if (traceResult?.CancellableTraceLog?.IsCancelled == true)
                {
                    return default;
                }

                // Mappings
                var mappings = getMappings?.Invoke();

                // Create (TEMP)
                if (withPseudoTable)
                {
                    pseudoTableName = getPseudoTableName?.Invoke();

                    await DropPseudoTableAsync(connection,
                        pseudoTableName,
                        bulkCopyTimeout,
                        trace,
                        traceKey,
                        transaction,
                        cancellationToken);

                    await CreatePseudoTableAsync(connection,
                        tableName,
                        pseudoTableName,
                        rowCount,
                        mappings,
                        bulkCopyTimeout,
                        identityBehavior,
                        pseudoTableType,
                        dbSetting,
                        trace,
                        traceKey,
                        transaction,
                        cancellationToken);
                }

                // Create Index
                if (isBulkInsert == false && withPseudoTable)
                {
                    qualifiers = qualifiers?.Any() == true ? qualifiers :
                        dbFields?.GetPrimary().AsField().AsEnumerable();

                    await CreatePseudoTableIndexAsync(connection,
                        pseudoTableName,
                        qualifiers,
                        bulkCopyTimeout,
                        dbSetting,
                        trace,
                        traceKey,
                        transaction,
                        cancellationToken);
                }

                // Import
                var result = await binaryImportAsync?.Invoke(pseudoTableName ?? tableName);

                // Insert (INTO)
                if (withPseudoTable)
                {
                    var identityResults = (await MergeToPseudoTableWithIdentityResultsAsync(connection,
                        getMergeToPseudoCommandText,
                        bulkCopyTimeout,
                        transaction))?.AsList();

                    if (identityBehavior == PostgreSqlBulkImportIdentityBehavior.ReturnIdentity)
                    {
                        setIdentities?.Invoke(identityResults);
                    }

                    result = identityResults.Count;
                }

                // After Execution
                await Tracer
                    .InvokeAfterExecutionAsync(traceResult, trace, result, cancellationToken);

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
                        trace,
                        traceKey,
                        transaction,
                        cancellationToken);
                }
            }
        }

        /// <summary>
        /// Builds a lightweight <see cref="NpgsqlCommand"/> that is never executed - it exists purely to
        /// carry a descriptive <c>CommandText</c> into <see cref="Tracer.InvokeBeforeExecution"/>/
        /// <see cref="Tracer.InvokeBeforeExecutionAsync"/> (see <see cref="PseudoBasedBinaryImport"/>/
        /// <see cref="PseudoBasedBinaryImportAsync"/> above, which is the single place shared by all of the
        /// BulkInsert/BulkMerge/BulkUpdate/BulkDelete/BulkDeleteByKey operations that calls this). A bulk
        /// operation's actual data movement goes through <see cref="NpgsqlBinaryImporter"/> (plus a handful
        /// of pseudo/staging table DDL/DML statements) rather than a single <see cref="DbCommand"/> - so
        /// unlike <see cref="RepoDb.Extensions.DbConnectionExtension.ExecuteNonQuery"/> and friends, there
        /// is no command for the trace machinery to pick up for free; this synthesizes one, mirroring the
        /// approach taken by <c>RepoDb.Oracle.BulkOperations</c>'s <c>CreateTraceCommand</c>.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName">The target table being bulk-operated on (not executed, used for the description only).</param>
        /// <param name="traceKey">Identifies which bulk operation is being traced, used to pick the descriptive <c>CommandText</c> (not executed).</param>
        /// <param name="commandTimeout"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private static DbCommand CreateTraceCommand(NpgsqlConnection connection,
            string tableName,
            string traceKey,
            int? commandTimeout = null,
            NpgsqlTransaction transaction = null) =>
            (DbCommand)connection.CreateCommand(GetTraceCommandText(traceKey, tableName), CommandType.Text, commandTimeout, transaction);

        /// <summary>
        /// Resolves the descriptive (never executed) SQL-like text used as the <c>CommandText</c> of the
        /// command synthesized by <see cref="CreateTraceCommand"/>, keyed off the operation's <see cref="PostgreSqlTraceKeys"/>
        /// value.
        /// </summary>
        /// <param name="traceKey"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        private static string GetTraceCommandText(string traceKey,
            string tableName) =>
            traceKey switch
            {
                PostgreSqlTraceKeys.PostgreSqlBulkDelete => $"BULK DELETE FROM {tableName}",
                PostgreSqlTraceKeys.PostgreSqlBulkDeleteByKey => $"BULK DELETE BY KEY FROM {tableName}",
                PostgreSqlTraceKeys.PostgreSqlBulkInsert => $"BULK INSERT INTO {tableName}",
                PostgreSqlTraceKeys.PostgreSqlBulkMerge => $"BULK MERGE INTO {tableName}",
                PostgreSqlTraceKeys.PostgreSqlBulkUpdate => $"BULK UPDATE INTO {tableName}",
                _ => $"{traceKey}: {tableName}",
            };

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
                transaction = await connection.BeginTransactionAsync(cancellationToken);
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
