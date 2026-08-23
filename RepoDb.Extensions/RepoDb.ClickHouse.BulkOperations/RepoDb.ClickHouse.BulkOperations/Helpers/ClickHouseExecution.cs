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
    /// <c>MySqlExecution</c>, none of the merge/update/delete mutations here report a meaningful affected-row
    /// count of their own: ClickHouse's <c>ALTER TABLE ... UPDATE</c>/<c>DELETE</c> mutations are asynchronous
    /// (registered immediately, applied by a background merge afterward - see
    /// <c>RepoDb.StatementBuilders.ClickHouseStatementBuilder.CreateUpdate</c> in <c>RepoDb.ClickHouse</c> for
    /// the same caveat at the single-row level), and ClickHouse.Driver's <c>ExecuteNonQuery</c> has no
    /// reliable "rows affected" figure for either a mutation or a plain <c>INSERT</c>.
    /// <see cref="DeleteFromPseudoTable"/>/<see cref="DeleteFromPseudoTableAsync"/> compensate by running a
    /// plain <c>SELECT count(*)</c> with the identical matching predicate <i>before</i> issuing the mutation
    /// (see <see cref="ClickHouseText.GetCountMatchedByPseudoTableSql"/>) and returning that; every other
    /// mutation method below (<see cref="UpdateFromPseudoTable"/>, and the update half of
    /// <see cref="MergeFromPseudoTable"/>) is still <see langword="void"/>/non-generic <see cref="Task"/> -
    /// they fire the statement(s) and let it go - and their <c>Base/*.cs</c> callers still report back the
    /// number of rows staged into the pseudo table (already known synchronously from the bulk-copy step) as
    /// the operation's result instead, which is liable to the same "reports more than it actually matched"
    /// bug the delete path had (see <see cref="DeleteFromPseudoTable"/>'s remarks) - not yet fixed there.
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
        }

        #endregion

        #region Delete

        /// <summary>
        /// Deletes every row of <paramref name="tableName"/> matched by <paramref name="pseudoTableName"/> via
        /// a single <c>ALTER TABLE ... DELETE WHERE</c> mutation, and returns how many rows actually matched
        /// (and were therefore deleted) - <b>not</b> the number of rows staged into <paramref name="pseudoTableName"/>,
        /// which may be larger (e.g. keys that don't exist in <paramref name="tableName"/>, as with a delete
        /// against an empty or partially-populated table). The mutation itself reports no affected-row count
        /// of its own (see the type-level remarks on <see cref="ClickHouseText"/>), so the count is captured
        /// with <see cref="ClickHouseText.GetCountMatchedByPseudoTableSql"/> <i>before</i> the mutation runs -
        /// counting afterward would always see 0, since by then the matching rows are gone. This is a
        /// best-effort snapshot: nothing prevents a concurrent write from changing which rows match between
        /// the count and the delete that follows it.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="pseudoTableName"></param>
        /// <param name="qualifiers"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        /// <returns>The number of rows matched (and deleted).</returns>
        public static int DeleteFromPseudoTable(ClickHouseConnection connection,
            string tableName,
            string pseudoTableName,
            IEnumerable<Field> qualifiers,
            ITrace trace = null,
            string traceKey = null,
            DbTransaction transaction = null)
        {
            var dbSetting = connection.GetDbSetting();
            var qualifierList = qualifiers.AsList();
            var commandText = ClickHouseText.GetDeleteFromPseudoTableSql(tableName, pseudoTableName, qualifierList, dbSetting);
            return connection.ExecuteNonQuery(commandText, transaction: transaction);
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
        /// <returns>The number of rows matched (and deleted).</returns>
        public static async Task<int> DeleteFromPseudoTableAsync(ClickHouseConnection connection,
            string tableName,
            string pseudoTableName,
            IEnumerable<Field> qualifiers,
            ITrace trace = null,
            string traceKey = null,
            DbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            var dbSetting = connection.GetDbSetting();
            var qualifierList = qualifiers.AsList();
            var commandText = ClickHouseText.GetDeleteFromPseudoTableSql(tableName, pseudoTableName, qualifierList, dbSetting);
            return await connection.ExecuteNonQueryAsync(commandText, transaction: transaction, cancellationToken: cancellationToken);
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
