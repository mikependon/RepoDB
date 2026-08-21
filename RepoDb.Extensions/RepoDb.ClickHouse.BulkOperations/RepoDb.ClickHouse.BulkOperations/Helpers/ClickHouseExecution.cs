using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using RepoDb.Enumerations.ClickHouse;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using ClickHouse.Driver.ADO;

namespace RepoDb.ClickHouse.BulkOperations.Extensions
{
    /// <summary>
    /// Issues every ADO.NET round trip the ClickHouse bulk-operations pseudo-table pipeline needs, built
    /// against the SQL text <see cref="ClickHouseText"/> builds. Unlike the MySQL provider's
    /// <c>MySqlExecution</c>, none of the merge/update/delete methods here return a meaningful affected-row
    /// count: ClickHouse's <c>ALTER TABLE ... UPDATE</c>/<c>DELETE</c> mutations are asynchronous (registered
    /// immediately, applied by a background merge afterward - see
    /// <c>RepoDb.StatementBuilders.ClickHouseStatementBuilder.CreateUpdate</c> in <c>RepoDb.ClickHouse</c> for
    /// the same caveat at the single-row level), and ClickHouse.Driver's <c>ExecuteNonQuery</c> has no
    /// reliable "rows affected" figure for either a mutation or a plain <c>INSERT</c>. These methods are
    /// therefore <see langword="void"/>/non-generic <see cref="Task"/> - they fire the statement(s) and let it
    /// go - and every <c>Base/*.cs</c> caller reports back the number of rows it staged into the pseudo table
    /// (already known synchronously from the bulk-copy step) as the operation's result instead.
    ///
    /// <para>
    /// Every mutation method below (<see cref="UpdateFromPseudoTable"/>, <see cref="DeleteFromPseudoTable"/>,
    /// and the update half of <see cref="MergeFromPseudoTable"/>, plus their async counterparts) blocks until
    /// the mutation it just issued has actually finished applying (see <see cref="WaitForMutations"/>) before
    /// returning - not merely until ClickHouse acknowledges the mutation was queued. This matters because every
    /// <c>Base/*.cs</c> caller drops the pseudo table in a <c>finally</c> block immediately after calling into
    /// this class; without waiting here first, that drop races the mutation's own asynchronous execution
    /// (ClickHouse evaluates an <c>ALTER TABLE ... UPDATE/DELETE</c> mutation's <c>WHERE</c>/<c>JOIN</c>
    /// predicate - here, a reference to the pseudo table - when the mutation actually runs in the background,
    /// not when it's submitted). Whichever rows the mutation had not yet processed by the time the pseudo table
    /// disappeared underneath it are silently left unmutated - observed directly as a bulk delete/update that
    /// reports success but leaves some rows behind, intermittently, worse under load or on the async call path
    /// where there's less incidental blocking time between submission and the pseudo table's drop.
    /// </para>
    /// </summary>
    internal static class ClickHouseExecution
    {
        /// <summary>
        /// How long <see cref="WaitForMutations"/>/<see cref="WaitForMutationsAsync"/> will poll
        /// <c>system.mutations</c> before giving up and throwing.
        /// </summary>
        private static readonly TimeSpan MutationWaitTimeout = TimeSpan.FromSeconds(30);

        /// <summary>
        /// The delay between successive <c>system.mutations</c> polls in <see cref="WaitForMutations"/>/
        /// <see cref="WaitForMutationsAsync"/>.
        /// </summary>
        private static readonly TimeSpan MutationPollInterval = TimeSpan.FromMilliseconds(100);

        /// <summary>
        /// Blocks until every mutation ClickHouse has queued for <paramref name="tableName"/> has finished (or
        /// <see cref="MutationWaitTimeout"/> elapses). See the type-level remarks on why every pseudo-table
        /// mutation method in this class calls this before returning.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="transaction"></param>
        private static void WaitForMutations(ClickHouseConnection connection,
            string tableName,
            DbTransaction transaction)
        {
            var deadline = DateTime.UtcNow.Add(MutationWaitTimeout);

            while (true)
            {
                var pending = connection.ExecuteScalar<long>(
                    "SELECT count(*) FROM system.mutations WHERE database = @Database AND table = @Table AND is_done = 0;",
                    new { Database = connection.Database, Table = tableName },
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
        /// Asynchronous counterpart of <see cref="WaitForMutations"/>.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        private static async Task WaitForMutationsAsync(ClickHouseConnection connection,
            string tableName,
            DbTransaction transaction,
            CancellationToken cancellationToken)
        {
            var deadline = DateTime.UtcNow.Add(MutationWaitTimeout);

            while (true)
            {
                var pending = await connection.ExecuteScalarAsync<long>(
                    "SELECT count(*) FROM system.mutations WHERE database = @Database AND table = @Table AND is_done = 0;",
                    new { Database = connection.Database, Table = tableName },
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

                await Task.Delay(MutationPollInterval, cancellationToken);
            }
        }

        #region Shared

        /// <summary>
        /// Creates a fresh pseudo table, first dropping any table left over under the same name (see the
        /// remarks on <see cref="ClickHouseText.GetCreatePseudoTableSql"/> for why a leftover table is never
        /// reused). Two separate statements/round trips, since ClickHouse.Driver does not support
        /// multi-statement execution (see the type-level remarks on <see cref="ClickHouseText"/>).
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="pseudoTableName"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="qualifierField"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        public static void CreatePseudoTable(ClickHouseConnection connection,
            string tableName,
            string pseudoTableName,
            ClickHouseBulkImportPseudoTableType pseudoTableType,
            Field qualifierField = null,
            ITrace trace = null,
            string traceKey = null,
            DbTransaction transaction = null)
        {
            var dbSetting = connection.GetDbSetting();
            connection.ExecuteNonQuery(ClickHouseText.GetDropPseudoTableSql(pseudoTableName, dbSetting), transaction: transaction);
            connection.ExecuteNonQuery(ClickHouseText.GetCreatePseudoTableSql(tableName, pseudoTableName, pseudoTableType, dbSetting, qualifierField), transaction: transaction);
        }

        /// <summary>
        /// Asynchronous counterpart of <see cref="CreatePseudoTable"/>.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="pseudoTableName"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="qualifierField"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task CreatePseudoTableAsync(ClickHouseConnection connection,
            string tableName,
            string pseudoTableName,
            ClickHouseBulkImportPseudoTableType pseudoTableType,
            Field qualifierField = null,
            ITrace trace = null,
            string traceKey = null,
            DbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            var dbSetting = connection.GetDbSetting();
            await connection.ExecuteNonQueryAsync(ClickHouseText.GetDropPseudoTableSql(pseudoTableName, dbSetting), transaction: transaction, cancellationToken: cancellationToken);
            await connection.ExecuteNonQueryAsync(ClickHouseText.GetCreatePseudoTableSql(tableName, pseudoTableName, pseudoTableType, dbSetting, qualifierField), transaction: transaction, cancellationToken: cancellationToken);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="pseudoTableName"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        public static void TruncatePseudoTable(ClickHouseConnection connection,
            string pseudoTableName,
            ITrace trace = null,
            string traceKey = null,
            DbTransaction transaction = null)
        {
            var dbSetting = connection.GetDbSetting();
            var commandText = ClickHouseText.GetTruncatePseudoTableSql(pseudoTableName, dbSetting);
            connection.ExecuteNonQuery(commandText, transaction: transaction);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="pseudoTableName"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task TruncatePseudoTableAsync(ClickHouseConnection connection,
            string pseudoTableName,
            ITrace trace = null,
            string traceKey = null,
            DbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            var dbSetting = connection.GetDbSetting();
            var commandText = ClickHouseText.GetTruncatePseudoTableSql(pseudoTableName, dbSetting);
            await connection.ExecuteNonQueryAsync(commandText, transaction: transaction, cancellationToken: cancellationToken);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="pseudoTableName"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        public static void DropPseudoTable(ClickHouseConnection connection,
            string pseudoTableName,
            ITrace trace = null,
            string traceKey = null,
            DbTransaction transaction = null)
        {
            var dbSetting = connection.GetDbSetting();
            var commandText = ClickHouseText.GetDropPseudoTableSql(pseudoTableName, dbSetting);
            connection.ExecuteNonQuery(commandText, transaction: transaction);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="pseudoTableName"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task DropPseudoTableAsync(ClickHouseConnection connection,
            string pseudoTableName,
            ITrace trace = null,
            string traceKey = null,
            DbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            var dbSetting = connection.GetDbSetting();
            var commandText = ClickHouseText.GetDropPseudoTableSql(pseudoTableName, dbSetting);
            await connection.ExecuteNonQueryAsync(commandText, transaction: transaction, cancellationToken: cancellationToken);
        }

        #endregion

        #region Merge

        /// <summary>
        /// Merges every row of <paramref name="pseudoTableName"/> into <paramref name="tableName"/>: an
        /// <c>ALTER TABLE ... UPDATE</c> mutation for matched rows (only issued when there is at least one
        /// non-qualifier field to update), followed unconditionally by an <c>INSERT ... SELECT</c> anti-join
        /// for unmatched rows. Two separate statements/round trips - see the type-level remarks on
        /// <see cref="ClickHouseExecution"/> for why neither is expected to return a meaningful affected-row
        /// count.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="pseudoTableName"></param>
        /// <param name="fields"></param>
        /// <param name="qualifiers"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        public static void MergeFromPseudoTable(ClickHouseConnection connection,
            string tableName,
            string pseudoTableName,
            IEnumerable<Field> fields,
            IEnumerable<Field> qualifiers,
            ITrace trace = null,
            string traceKey = null,
            DbTransaction transaction = null)
        {
            var dbSetting = connection.GetDbSetting();
            var fieldList = fields.AsList();
            var qualifierList = qualifiers.AsList();

            if (HasUpdatableFields(fieldList, qualifierList))
            {
                connection.ExecuteNonQuery(ClickHouseText.GetUpdateFromPseudoTableSql(tableName, pseudoTableName, fieldList, qualifierList, dbSetting), transaction: transaction);

                // The UPDATE above is an asynchronous mutation whose WHERE/JOIN predicate still references
                // pseudoTableName when it actually runs - wait for it before the caller drops that table
                // (see the type-level remarks on this class).
                WaitForMutations(connection, tableName, transaction);
            }

            connection.ExecuteNonQuery(ClickHouseText.GetInsertUnmatchedFromPseudoTableSql(tableName, pseudoTableName, fieldList, qualifierList, dbSetting), transaction: transaction);
        }

        /// <summary>
        /// Asynchronous counterpart of <see cref="MergeFromPseudoTable"/>.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="pseudoTableName"></param>
        /// <param name="fields"></param>
        /// <param name="qualifiers"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task MergeFromPseudoTableAsync(ClickHouseConnection connection,
            string tableName,
            string pseudoTableName,
            IEnumerable<Field> fields,
            IEnumerable<Field> qualifiers,
            ITrace trace = null,
            string traceKey = null,
            DbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            var dbSetting = connection.GetDbSetting();
            var fieldList = fields.AsList();
            var qualifierList = qualifiers.AsList();

            if (HasUpdatableFields(fieldList, qualifierList))
            {
                await connection.ExecuteNonQueryAsync(ClickHouseText.GetUpdateFromPseudoTableSql(tableName, pseudoTableName, fieldList, qualifierList, dbSetting), transaction: transaction, cancellationToken: cancellationToken);

                // The UPDATE above is an asynchronous mutation whose WHERE/JOIN predicate still references
                // pseudoTableName when it actually runs - wait for it before the caller drops that table
                // (see the type-level remarks on this class).
                await WaitForMutationsAsync(connection, tableName, transaction, cancellationToken);
            }

            await connection.ExecuteNonQueryAsync(ClickHouseText.GetInsertUnmatchedFromPseudoTableSql(tableName, pseudoTableName, fieldList, qualifierList, dbSetting), transaction: transaction, cancellationToken: cancellationToken);
        }

        #endregion

        #region Update

        /// <summary>
        /// Updates every row of <paramref name="tableName"/> matched by <paramref name="pseudoTableName"/> via
        /// a single <c>ALTER TABLE ... UPDATE</c> mutation.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="pseudoTableName"></param>
        /// <param name="fields"></param>
        /// <param name="qualifiers"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        public static void UpdateFromPseudoTable(ClickHouseConnection connection,
            string tableName,
            string pseudoTableName,
            IEnumerable<Field> fields,
            IEnumerable<Field> qualifiers,
            ITrace trace = null,
            string traceKey = null,
            DbTransaction transaction = null)
        {
            var dbSetting = connection.GetDbSetting();
            var commandText = ClickHouseText.GetUpdateFromPseudoTableSql(tableName, pseudoTableName, fields, qualifiers, dbSetting);
            connection.ExecuteNonQuery(commandText, transaction: transaction);

            // The UPDATE above is an asynchronous mutation whose WHERE/JOIN predicate still references
            // pseudoTableName when it actually runs - wait for it before the caller drops that table (see
            // the type-level remarks on this class).
            WaitForMutations(connection, tableName, transaction);
        }

        /// <summary>
        /// Asynchronous counterpart of <see cref="UpdateFromPseudoTable"/>.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="pseudoTableName"></param>
        /// <param name="fields"></param>
        /// <param name="qualifiers"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task UpdateFromPseudoTableAsync(ClickHouseConnection connection,
            string tableName,
            string pseudoTableName,
            IEnumerable<Field> fields,
            IEnumerable<Field> qualifiers,
            ITrace trace = null,
            string traceKey = null,
            DbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            var dbSetting = connection.GetDbSetting();
            var commandText = ClickHouseText.GetUpdateFromPseudoTableSql(tableName, pseudoTableName, fields, qualifiers, dbSetting);
            await connection.ExecuteNonQueryAsync(commandText, transaction: transaction, cancellationToken: cancellationToken);

            // The UPDATE above is an asynchronous mutation whose WHERE/JOIN predicate still references
            // pseudoTableName when it actually runs - wait for it before the caller drops that table (see
            // the type-level remarks on this class).
            await WaitForMutationsAsync(connection, tableName, transaction, cancellationToken);
        }

        #endregion

        #region Delete

        /// <summary>
        /// Deletes every row of <paramref name="tableName"/> matched by <paramref name="pseudoTableName"/> via
        /// a single <c>ALTER TABLE ... DELETE WHERE</c> mutation.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="pseudoTableName"></param>
        /// <param name="qualifiers"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        public static void DeleteFromPseudoTable(ClickHouseConnection connection,
            string tableName,
            string pseudoTableName,
            IEnumerable<Field> qualifiers,
            ITrace trace = null,
            string traceKey = null,
            DbTransaction transaction = null)
        {
            var dbSetting = connection.GetDbSetting();
            var commandText = ClickHouseText.GetDeleteFromPseudoTableSql(tableName, pseudoTableName, qualifiers, dbSetting);
            connection.ExecuteNonQuery(commandText, transaction: transaction);

            // The DELETE above is an asynchronous mutation whose WHERE predicate still references
            // pseudoTableName when it actually runs - wait for it before the caller drops that table (see
            // the type-level remarks on this class). Without this, rows the mutation hadn't gotten to yet by
            // the time the pseudo table disappeared are silently left undeleted.
            WaitForMutations(connection, tableName, transaction);
        }

        /// <summary>
        /// Asynchronous counterpart of <see cref="DeleteFromPseudoTable"/>.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="pseudoTableName"></param>
        /// <param name="qualifiers"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task DeleteFromPseudoTableAsync(ClickHouseConnection connection,
            string tableName,
            string pseudoTableName,
            IEnumerable<Field> qualifiers,
            ITrace trace = null,
            string traceKey = null,
            DbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            var dbSetting = connection.GetDbSetting();
            var commandText = ClickHouseText.GetDeleteFromPseudoTableSql(tableName, pseudoTableName, qualifiers, dbSetting);
            await connection.ExecuteNonQueryAsync(commandText, transaction: transaction, cancellationToken: cancellationToken);

            // The DELETE above is an asynchronous mutation whose WHERE predicate still references
            // pseudoTableName when it actually runs - wait for it before the caller drops that table (see
            // the type-level remarks on this class). Without this, rows the mutation hadn't gotten to yet by
            // the time the pseudo table disappeared are silently left undeleted.
            await WaitForMutationsAsync(connection, tableName, transaction, cancellationToken);
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Determines whether <paramref name="fields"/> contains at least one field that isn't also a
        /// qualifier - i.e. whether there is anything for the matched-rows half of a merge to actually update.
        /// </summary>
        private static bool HasUpdatableFields(IList<Field> fields,
            IList<Field> qualifiers) =>
            fields.Any(field =>
                qualifiers.Any(qualifier => string.Equals(qualifier.Name, field.Name, System.StringComparison.OrdinalIgnoreCase)) == false);

        #endregion
    }
}
