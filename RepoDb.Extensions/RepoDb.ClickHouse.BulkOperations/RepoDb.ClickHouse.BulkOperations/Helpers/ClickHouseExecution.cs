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
    /// 
    /// </summary>
    internal static class ClickHouseExecution
    {
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
        /// 
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
            }
            connection.ExecuteNonQuery(ClickHouseText.GetInsertUnmatchedFromPseudoTableSql(tableName, pseudoTableName, fieldList, qualifierList, dbSetting), transaction: transaction);
            connection.WaitForMutations(tableName, transaction);
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
            await connection.WaitForMutationsAsync(tableName, transaction, cancellationToken: cancellationToken);
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
            connection.WaitForMutations(tableName, transaction);
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
            await connection.WaitForMutationsAsync(tableName, transaction, cancellationToken: cancellationToken);
        }

        #endregion

        #region Delete

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="pseudoTableName"></param>
        /// <param name="qualifiers"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
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
            var countText = ClickHouseText.GetCountMatchedByPseudoTableSql(tableName, pseudoTableName, qualifierList, dbSetting);
            var result = connection.ExecuteScalar<int>(countText, transaction: transaction);
            var commandText = ClickHouseText.GetDeleteFromPseudoTableSql(tableName, pseudoTableName, qualifierList, dbSetting);
            connection.ExecuteNonQuery(commandText, transaction: transaction);
            connection.WaitForMutations(tableName, transaction);
            return result;
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
            var countText = ClickHouseText.GetCountMatchedByPseudoTableSql(tableName, pseudoTableName, qualifierList, dbSetting);
            var result = await connection.ExecuteScalarAsync<int>(countText, transaction: transaction, cancellationToken: cancellationToken);
            var commandText = ClickHouseText.GetDeleteFromPseudoTableSql(tableName, pseudoTableName, qualifierList, dbSetting);
            await connection.ExecuteNonQueryAsync(commandText, transaction: transaction, cancellationToken: cancellationToken);
            await connection.WaitForMutationsAsync(tableName, transaction: transaction, cancellationToken: cancellationToken);
            return result;
        }

        #endregion

        #region Helpers

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fields"></param>
        /// <param name="qualifiers"></param>
        /// <returns></returns>
        private static bool HasUpdatableFields(IList<Field> fields,
            IList<Field> qualifiers) =>
            fields.Any(field =>
                qualifiers.Any(qualifier => string.Equals(qualifier.Name, field.Name, System.StringComparison.OrdinalIgnoreCase)) == false);

        #endregion
    }
}
