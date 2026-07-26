using Oracle.ManagedDataAccess.Client;
using RepoDb.Enumerations.Oracle;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.Oracle.BulkOperations;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RepoDb
{
    /// <summary>
    /// Owns the lifecycle of the per-table Global Temporary Table (GTT) used to stage rows for
    /// BulkMerge/BulkUpdate/BulkDelete, plus the array-bind primitive used both to load that staging
    /// table and (for BulkInsert, which needs no staging table at all - see the package README) to write
    /// directly into the real table in a single round trip.
    /// <para>
    /// Why a GTT instead of PostgreSQL's per-call physical/TEMP pseudo table: Oracle's CREATE TABLE and
    /// DROP TABLE are DDL and cause an implicit COMMIT, so creating/dropping a staging table on every
    /// bulk call - the way the PostgreSQL package does - would silently commit whatever the caller's
    /// transaction had pending. Instead, the staging table is created once per (connection database,
    /// table name) the first time it is needed with <c>ON COMMIT PRESERVE ROWS</c> (so its rows survive
    /// across commits within a session) and is then simply cleared with a plain <c>DELETE</c> - which is
    /// DML, not DDL - before every subsequent load.
    /// </para>
    /// </summary>
    internal static class OracleStagingTable
    {
        private static readonly ConcurrentDictionary<string, bool> ensuredTables = new(StringComparer.Ordinal);

        internal const string OrderColumnName = "__RepoDb_OrderColumn";

        /// <summary>
        /// The bind-variable name used for the <c>RETURNING ... INTO</c> output parameter on a direct
        /// (staging-table-free) array-bound BulkInsert. Shared with <see cref="OracleText.GetInsertCommandText"/>,
        /// which must emit the exact same name into the command text.
        /// </summary>
        internal const string ReturningParameterName = "__out_identity";

        #region Naming

        /// <summary>
        /// Deterministically derives the staging table's name from the real table's name and the
        /// requested <see cref="OracleBulkImportPseudoTableType"/>. Kept short (well within Oracle's
        /// pre-12.2 30-byte identifier limit) since it is always quoted and never needs to be
        /// human-readable. The type-specific suffix ensures the Temporary and Physical staging tables for
        /// the same real table never collide by name or in the <see cref="ensuredTables"/> cache.
        /// </summary>
        public static string GetStagingTableName(string tableName, OracleBulkImportPseudoTableType pseudoTableType)
        {
            var suffix = pseudoTableType == OracleBulkImportPseudoTableType.Physical ? "P" : "T";
            return "RB$" + unchecked((uint)tableName.GetHashCode()).ToString("X8", CultureInfo.InvariantCulture) + suffix;
        }

        #endregion

        #region Ensure / Clear (Sync)

        /// <summary>
        /// Creates the staging table if it does not already exist for this table (idempotent, cached
        /// per-process after the first successful check so steady-state calls never round-trip to
        /// USER_TABLES). No-ops after the first call for a given table within the process lifetime.
        /// </summary>
        public static void EnsureStagingTable(OracleConnection connection,
            string tableName,
            string stagingTableName,
            DbFieldCollection dbFields,
            OracleBulkImportPseudoTableType pseudoTableType,
            IDbSetting dbSetting,
            OracleTransaction transaction)
        {
            var cacheKey = GetCacheKey(connection, stagingTableName);

            if (ensuredTables.ContainsKey(cacheKey))
            {
                return;
            }

            if (StagingTableExists(connection, stagingTableName, transaction) == false)
            {
                var commandText = OracleText.GetCreateStagingTableCommandText(tableName, stagingTableName, dbFields, pseudoTableType, dbSetting);
                connection.ExecuteNonQuery(commandText, transaction: transaction);
            }

            ensuredTables.TryAdd(cacheKey, true);
        }

        /// <summary>
        /// Clears any rows left over from a previous call (plain DML - transaction-safe, unlike TRUNCATE).
        /// </summary>
        public static void ClearStagingTable(OracleConnection connection,
            string stagingTableName,
            IDbSetting dbSetting,
            OracleTransaction transaction) =>
            connection.ExecuteNonQuery(
                string.Concat("DELETE FROM ", stagingTableName.AsQuoted(true, dbSetting)),
                transaction: transaction);

        private static bool StagingTableExists(OracleConnection connection,
            string stagingTableName,
            OracleTransaction transaction)
        {
            var unquotedName = stagingTableName.AsUnquoted(true, connection.GetDbSetting());
            var count = connection.ExecuteScalar<int>(
                "SELECT COUNT(*) FROM USER_TABLES WHERE TABLE_NAME = :TableName",
                param: new { TableName = unquotedName },
                transaction: transaction);

            return count > 0;
        }

        #endregion

        #region Ensure / Clear (Async)

        /// <summary>
        /// Asynchronous counterpart of <see cref="EnsureStagingTable"/>.
        /// </summary>
        public static async Task EnsureStagingTableAsync(OracleConnection connection,
            string tableName,
            string stagingTableName,
            DbFieldCollection dbFields,
            OracleBulkImportPseudoTableType pseudoTableType,
            IDbSetting dbSetting,
            OracleTransaction transaction,
            CancellationToken cancellationToken = default)
        {
            var cacheKey = GetCacheKey(connection, stagingTableName);

            if (ensuredTables.ContainsKey(cacheKey))
            {
                return;
            }

            if (await StagingTableExistsAsync(connection, stagingTableName, transaction, cancellationToken) == false)
            {
                var commandText = OracleText.GetCreateStagingTableCommandText(tableName, stagingTableName, dbFields, pseudoTableType, dbSetting);
                await connection.ExecuteNonQueryAsync(commandText, transaction: transaction, cancellationToken: cancellationToken);
            }

            ensuredTables.TryAdd(cacheKey, true);
        }

        /// <summary>
        /// Asynchronous counterpart of <see cref="ClearStagingTable"/>.
        /// </summary>
        public static Task ClearStagingTableAsync(OracleConnection connection,
            string stagingTableName,
            IDbSetting dbSetting,
            OracleTransaction transaction,
            CancellationToken cancellationToken = default) =>
            connection.ExecuteNonQueryAsync(
                string.Concat("DELETE FROM ", stagingTableName.AsQuoted(true, dbSetting)),
                transaction: transaction,
                cancellationToken: cancellationToken);

        private static async Task<bool> StagingTableExistsAsync(OracleConnection connection,
            string stagingTableName,
            OracleTransaction transaction,
            CancellationToken cancellationToken)
        {
            var unquotedName = stagingTableName.AsUnquoted(true, connection.GetDbSetting());
            var count = await connection.ExecuteScalarAsync<int>(
                "SELECT COUNT(*) FROM USER_TABLES WHERE TABLE_NAME = :TableName",
                param: new { TableName = unquotedName },
                transaction: transaction,
                cancellationToken: cancellationToken);

            return count > 0;
        }

        #endregion

        #region Array-Bind Load/Insert

        /// <summary>
        /// Executes an array-bound INSERT (into either the staging table or, for plain BulkInsert, the
        /// real table directly) in a single round trip, optionally reading back a <c>RETURNING ... INTO</c>
        /// array of generated/matched identity values aligned 1:1 with <paramref name="rows"/>'s order.
        /// </summary>
        /// <param name="connection">The connection to use.</param>
        /// <param name="commandText">The INSERT command text, produced by <see cref="OracleText"/>.</param>
        /// <param name="columnOrder">The destination column names, in the same order as each row in <paramref name="rows"/>.</param>
        /// <param name="rows">One <see cref="object"/> array per row, values aligned to <paramref name="columnOrder"/>.</param>
        /// <param name="columnOracleDbTypes">Optional explicit <see cref="OracleDbType"/> per column (by position); null entries let ODP.NET infer.</param>
        /// <param name="returningColumn">When set, appends a <c>RETURNING [column] INTO :__out_identity</c> array read-back.</param>
        /// <param name="returningOracleDbType">The <see cref="OracleDbType"/> to bind the RETURNING output array as. Required when <paramref name="returningColumn"/> is set.</param>
        /// <param name="bulkCopyTimeout">Optional command timeout override, in seconds.</param>
        /// <param name="transaction">The transaction to enlist in, if any.</param>
        /// <returns>The affected row count and, when <paramref name="returningColumn"/> was set, the returned values in row order.</returns>
        public static (int AffectedRows, object[] ReturnedValues) ExecuteArrayBind(OracleConnection connection,
            string commandText,
            IReadOnlyList<string> columnOrder,
            IReadOnlyList<object[]> rows,
            IReadOnlyList<OracleDbType?> columnOracleDbTypes,
            string returningColumn,
            OracleDbType? returningOracleDbType,
            int? bulkCopyTimeout,
            OracleTransaction transaction)
        {
            // ArrayBindCount == 0 is not a valid array-bind execution - nothing to do.
            if (rows.Count == 0)
            {
                return (0, string.IsNullOrEmpty(returningColumn) ? null : Array.Empty<object>());
            }

            connection.EnsureOpen();

            using var command = CreateArrayBindCommand(connection, commandText, columnOrder, rows, columnOracleDbTypes,
                returningColumn, returningOracleDbType, bulkCopyTimeout, transaction, out var returningParameter);

            var affected = command.ExecuteNonQuery();
            var returned = returningParameter == null ? null : ExtractReturnedValues(returningParameter, rows.Count);

            return (affected, returned);
        }

        /// <summary>
        /// Asynchronous counterpart of <see cref="ExecuteArrayBind"/>.
        /// </summary>
        public static async Task<(int AffectedRows, object[] ReturnedValues)> ExecuteArrayBindAsync(OracleConnection connection,
            string commandText,
            IReadOnlyList<string> columnOrder,
            IReadOnlyList<object[]> rows,
            IReadOnlyList<OracleDbType?> columnOracleDbTypes,
            string returningColumn,
            OracleDbType? returningOracleDbType,
            int? bulkCopyTimeout,
            OracleTransaction transaction,
            CancellationToken cancellationToken = default)
        {
            // ArrayBindCount == 0 is not a valid array-bind execution - nothing to do.
            if (rows.Count == 0)
            {
                return (0, string.IsNullOrEmpty(returningColumn) ? null : Array.Empty<object>());
            }

            await connection.EnsureOpenAsync(cancellationToken);

            using var command = CreateArrayBindCommand(connection, commandText, columnOrder, rows, columnOracleDbTypes,
                returningColumn, returningOracleDbType, bulkCopyTimeout, transaction, out var returningParameter);

            var affected = await command.ExecuteNonQueryAsync(cancellationToken);
            var returned = returningParameter == null ? null : ExtractReturnedValues(returningParameter, rows.Count);

            return (affected, returned);
        }

        private static OracleCommand CreateArrayBindCommand(OracleConnection connection,
            string commandText,
            IReadOnlyList<string> columnOrder,
            IReadOnlyList<object[]> rows,
            IReadOnlyList<OracleDbType?> columnOracleDbTypes,
            string returningColumn,
            OracleDbType? returningOracleDbType,
            int? bulkCopyTimeout,
            OracleTransaction transaction,
            out OracleParameter returningParameter)
        {
            var rowCount = rows.Count;
            var command = connection.CreateCommand();

            command.CommandText = commandText;
            command.CommandType = CommandType.Text;
            command.Transaction = transaction;
            command.BindByName = true;
            command.ArrayBindCount = rowCount;

            if (bulkCopyTimeout.HasValue)
            {
                command.CommandTimeout = bulkCopyTimeout.Value;
            }

            for (var columnIndex = 0; columnIndex < columnOrder.Count; columnIndex++)
            {
                var values = new object[rowCount];

                for (var rowIndex = 0; rowIndex < rowCount; rowIndex++)
                {
                    values[rowIndex] = rows[rowIndex][columnIndex] ?? DBNull.Value;
                }

                var parameter = new OracleParameter(columnOrder[columnIndex], values)
                {
                    Direction = ParameterDirection.Input
                };

                var explicitType = columnOracleDbTypes?[columnIndex];
                if (explicitType.HasValue)
                {
                    parameter.OracleDbType = explicitType.Value;
                }

                command.Parameters.Add(parameter);
            }

            if (string.IsNullOrEmpty(returningColumn) == false)
            {
                returningParameter = new OracleParameter(ReturningParameterName, returningOracleDbType ?? OracleDbType.Decimal, rowCount, null, ParameterDirection.Output);
                command.Parameters.Add(returningParameter);
            }
            else
            {
                returningParameter = null;
            }

            return command;
        }

        private static object[] ExtractReturnedValues(OracleParameter returningParameter,
            int expectedCount)
        {
            // For array-bound DML with a RETURNING ... INTO clause, ODP.NET populates the output
            // parameter's .Value with an array (one element per array-bind iteration, in the same order
            // the rows were bound in) rather than the single scalar it would produce for a non-array-bound
            // command. This is documented ODP.NET behavior for "Array Binding" combined with "RETURNING
            // INTO" - it has not been exercised against a live Oracle instance as part of this change; see
            // the package README's verification note.
            if (returningParameter.Value is Array array)
            {
                var result = new object[array.Length];
                Array.Copy(array, result, array.Length);
                return result;
            }

            // Defensive fallback in case ODP.NET ever returns a single value for a single-row batch.
            return new[] { returningParameter.Value };
        }

        #endregion

        #region Helpers

        private static string GetCacheKey(OracleConnection connection,
            string stagingTableName) =>
            string.Concat(connection.DataSource, "::", connection.Database, "::", stagingTableName);

        #endregion
    }
}
