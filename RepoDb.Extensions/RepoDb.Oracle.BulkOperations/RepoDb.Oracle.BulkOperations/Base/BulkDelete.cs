using Oracle.ManagedDataAccess.Client;
using RepoDb.Enumerations.Oracle;
using RepoDb.Extensions;
using RepoDb.Oracle.BulkOperations;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RepoDb
{
    public static partial class OracleConnectionExtension
    {
        // BulkDelete only ever needs the qualifier columns staged (not the whole row) - the staging
        // table's payload is just "which rows to delete", so this is intentionally the lightest of the
        // four operations.

        #region Sync

        #region BulkDeleteBase<TEntity>

        private static int BulkDeleteBase<TEntity>(this OracleConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<Field> qualifiers = null,
            int? bulkCopyTimeout = null,
            OracleBulkImportPseudoTableType pseudoTableType = default,
            OracleTransaction transaction = null)
            where TEntity : class
        {
            var entityList = entities as IList<TEntity> ?? entities.ToList();
            var entityType = entityList.FirstOrDefault()?.GetType() ?? typeof(TEntity);
            var isDictionary = TypeCache.Get(entityType).IsDictionaryStringObject();
            var dbSetting = connection.GetDbSetting();
            var dbFields = DbFieldCache.Get(connection, tableName, transaction);
            var primaryField = dbFields.GetPrimary()?.AsField();
            var stagingTableName = OracleStagingTable.GetStagingTableName(tableName, pseudoTableType);

            // GetDeleteCommandText validates (and throws a clear error if there are no usable qualifiers)
            // before any of the resolved-qualifiers-dependent code below runs, so a table with neither an
            // explicit qualifier nor a primary key fails fast with a helpful message instead of a bare NRE.
            var deleteText = OracleText.GetDeleteCommandText(tableName, stagingTableName, qualifiers, primaryField, dbSetting);
            var resolvedQualifiers = OracleText.ResolveQualifiers(qualifiers, primaryField).AsList();
            var gettersByMappedName = isDictionary ? null : Compiler.GetPropertyGettersByMappedName(entityType);
            var rows = OracleHelpers.BuildRows(entityList, resolvedQualifiers, isDictionary, gettersByMappedName, false);

            var stagingInsertText = OracleText.GetStagingInsertCommandText(stagingTableName, resolvedQualifiers, false, dbSetting);
            var stagingParameterNames = resolvedQualifiers.Select(field => OracleText.GetParameterName(field, dbSetting)).AsList();

            return connection.TransactionalExecute(txn =>
            {
                OracleStagingTable.EnsureStagingTable(connection, tableName, stagingTableName, dbFields, pseudoTableType, dbSetting, txn);
                OracleStagingTable.ClearStagingTable(connection, stagingTableName, dbSetting, txn);
                OracleStagingTable.ExecuteArrayBind(connection, stagingInsertText, stagingParameterNames, rows, null, null, null, bulkCopyTimeout, txn);

                return connection.ExecuteNonQuery(deleteText, bulkCopyTimeout, transaction: txn);
            }, transaction);
        }

        #endregion

        #region BulkDeleteBase<DataTable>

        private static int BulkDeleteBase(this OracleConnection connection,
            string tableName,
            DataTable table,
            IEnumerable<Field> qualifiers = null,
            DataRowState? rowState = null,
            int? bulkCopyTimeout = null,
            OracleBulkImportPseudoTableType pseudoTableType = default,
            OracleTransaction transaction = null)
        {
            var dbSetting = connection.GetDbSetting();
            var dbFields = DbFieldCache.Get(connection, tableName, transaction);
            var primaryField = dbFields.GetPrimary()?.AsField();
            var stagingTableName = OracleStagingTable.GetStagingTableName(tableName, pseudoTableType);

            var resolvedQualifiers = OracleText.ResolveQualifiers(qualifiers, primaryField).AsList();
            var dataRows = (rowState.HasValue ?
                table.Rows.Cast<DataRow>().Where(row => row.RowState == rowState.Value) :
                table.Rows.Cast<DataRow>()).AsList();
            var rows = OracleHelpers.BuildRows(dataRows, resolvedQualifiers, false);

            var stagingInsertText = OracleText.GetStagingInsertCommandText(stagingTableName, resolvedQualifiers, false, dbSetting);
            var stagingParameterNames = resolvedQualifiers.Select(field => OracleText.GetParameterName(field, dbSetting)).AsList();
            var deleteText = OracleText.GetDeleteCommandText(tableName, stagingTableName, qualifiers, primaryField, dbSetting);

            return connection.TransactionalExecute(txn =>
            {
                OracleStagingTable.EnsureStagingTable(connection, tableName, stagingTableName, dbFields, pseudoTableType, dbSetting, txn);
                OracleStagingTable.ClearStagingTable(connection, stagingTableName, dbSetting, txn);
                OracleStagingTable.ExecuteArrayBind(connection, stagingInsertText, stagingParameterNames, rows, null, null, null, bulkCopyTimeout, txn);

                return connection.ExecuteNonQuery(deleteText, bulkCopyTimeout, transaction: txn);
            }, transaction);
        }

        #endregion

        #endregion

        #region Async

        #region BulkDeleteBaseAsync<TEntity>

        private static async Task<int> BulkDeleteBaseAsync<TEntity>(this OracleConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<Field> qualifiers = null,
            int? bulkCopyTimeout = null,
            OracleBulkImportPseudoTableType pseudoTableType = default,
            OracleTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            var entityList = entities as IList<TEntity> ?? entities.ToList();
            var entityType = entityList.FirstOrDefault()?.GetType() ?? typeof(TEntity);
            var isDictionary = TypeCache.Get(entityType).IsDictionaryStringObject();
            var dbSetting = connection.GetDbSetting();
            var dbFields = await DbFieldCache.GetAsync(connection, tableName, transaction, cancellationToken);
            var primaryField = dbFields.GetPrimary()?.AsField();
            var stagingTableName = OracleStagingTable.GetStagingTableName(tableName, pseudoTableType);

            // GetDeleteCommandText validates (and throws a clear error if there are no usable qualifiers)
            // before any of the resolved-qualifiers-dependent code below runs, so a table with neither an
            // explicit qualifier nor a primary key fails fast with a helpful message instead of a bare NRE.
            var deleteText = OracleText.GetDeleteCommandText(tableName, stagingTableName, qualifiers, primaryField, dbSetting);
            var resolvedQualifiers = OracleText.ResolveQualifiers(qualifiers, primaryField).AsList();
            var gettersByMappedName = isDictionary ? null : Compiler.GetPropertyGettersByMappedName(entityType);
            var rows = OracleHelpers.BuildRows(entityList, resolvedQualifiers, isDictionary, gettersByMappedName, false);

            var stagingInsertText = OracleText.GetStagingInsertCommandText(stagingTableName, resolvedQualifiers, false, dbSetting);
            var stagingParameterNames = resolvedQualifiers.Select(field => OracleText.GetParameterName(field, dbSetting)).AsList();

            return await connection.TransactionalExecuteAsync(async txn =>
            {
                await OracleStagingTable.EnsureStagingTableAsync(connection, tableName, stagingTableName, dbFields, pseudoTableType, dbSetting, txn, cancellationToken);
                await OracleStagingTable.ClearStagingTableAsync(connection, stagingTableName, dbSetting, txn, cancellationToken);
                await OracleStagingTable.ExecuteArrayBindAsync(connection, stagingInsertText, stagingParameterNames, rows, null, null, null, bulkCopyTimeout, txn, cancellationToken);

                return await connection.ExecuteNonQueryAsync(deleteText, bulkCopyTimeout, transaction: txn, cancellationToken: cancellationToken);
            }, transaction, cancellationToken);
        }

        #endregion

        #region BulkDeleteBaseAsync<DataTable>

        private static async Task<int> BulkDeleteBaseAsync(this OracleConnection connection,
            string tableName,
            DataTable table,
            IEnumerable<Field> qualifiers = null,
            DataRowState? rowState = null,
            int? bulkCopyTimeout = null,
            OracleBulkImportPseudoTableType pseudoTableType = default,
            OracleTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            var dbSetting = connection.GetDbSetting();
            var dbFields = await DbFieldCache.GetAsync(connection, tableName, transaction, cancellationToken);
            var primaryField = dbFields.GetPrimary()?.AsField();
            var stagingTableName = OracleStagingTable.GetStagingTableName(tableName, pseudoTableType);

            var resolvedQualifiers = OracleText.ResolveQualifiers(qualifiers, primaryField).AsList();
            var dataRows = (rowState.HasValue ?
                table.Rows.Cast<DataRow>().Where(row => row.RowState == rowState.Value) :
                table.Rows.Cast<DataRow>()).AsList();
            var rows = OracleHelpers.BuildRows(dataRows, resolvedQualifiers, false);

            var stagingInsertText = OracleText.GetStagingInsertCommandText(stagingTableName, resolvedQualifiers, false, dbSetting);
            var stagingParameterNames = resolvedQualifiers.Select(field => OracleText.GetParameterName(field, dbSetting)).AsList();
            var deleteText = OracleText.GetDeleteCommandText(tableName, stagingTableName, qualifiers, primaryField, dbSetting);

            return await connection.TransactionalExecuteAsync(async txn =>
            {
                await OracleStagingTable.EnsureStagingTableAsync(connection, tableName, stagingTableName, dbFields, pseudoTableType, dbSetting, txn, cancellationToken);
                await OracleStagingTable.ClearStagingTableAsync(connection, stagingTableName, dbSetting, txn, cancellationToken);
                await OracleStagingTable.ExecuteArrayBindAsync(connection, stagingInsertText, stagingParameterNames, rows, null, null, null, bulkCopyTimeout, txn, cancellationToken);

                return await connection.ExecuteNonQueryAsync(deleteText, bulkCopyTimeout, transaction: txn, cancellationToken: cancellationToken);
            }, transaction, cancellationToken);
        }

        #endregion

        #endregion
    }
}
