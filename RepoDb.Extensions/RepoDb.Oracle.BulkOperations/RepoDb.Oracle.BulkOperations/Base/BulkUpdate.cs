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
        #region Sync

        #region BulkUpdateBase<TEntity>

        private static int BulkUpdateBase<TEntity>(this OracleConnection connection,
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

            var fields = isDictionary ?
                OracleHelpers.GetDictionaryFields(entityList.First() as IDictionary<string, object>, dbFields, dbSetting) :
                OracleHelpers.GetEntityFields(entityType, dbFields, dbSetting);
            var gettersByMappedName = isDictionary ? null : Compiler.GetPropertyGettersByMappedName(entityType);
            var rows = OracleHelpers.BuildRows(entityList, fields, isDictionary, gettersByMappedName, false);

            var stagingInsertText = OracleText.GetStagingInsertCommandText(stagingTableName, fields, false, dbSetting);
            var stagingParameterNames = fields.Select(field => OracleText.GetParameterName(field, dbSetting)).AsList();
            var stagingOracleDbTypes = OracleHelpers.GetOracleDbTypes(fields, entityType, isDictionary);
            var updateText = OracleText.GetUpdateCommandText(tableName, stagingTableName, fields, qualifiers, primaryField, dbSetting);

            return connection.TransactionalExecute(transaction =>
            {
                OracleStagingTable.EnsureStagingTable(connection, tableName, stagingTableName, dbFields, pseudoTableType, dbSetting, transaction);
                OracleStagingTable.ClearStagingTable(connection, stagingTableName, dbSetting, transaction);
                OracleStagingTable.ExecuteArrayBind(connection, stagingInsertText, stagingParameterNames, rows, stagingOracleDbTypes, null, null, bulkCopyTimeout, transaction);

                return connection.ExecuteNonQuery(updateText, bulkCopyTimeout, transaction: transaction);
            }, transaction);
        }

        #endregion

        #region BulkUpdateBase<DataTable>

        private static int BulkUpdateBase(this OracleConnection connection,
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

            var fields = OracleHelpers.GetDataTableFields(table, dbFields, dbSetting);
            var dataRows = (rowState.HasValue ?
                table.Rows.Cast<DataRow>().Where(row => row.RowState == rowState.Value) :
                table.Rows.Cast<DataRow>()).AsList();
            var rows = OracleHelpers.BuildRows(dataRows, fields, false);

            var stagingInsertText = OracleText.GetStagingInsertCommandText(stagingTableName, fields, false, dbSetting);
            var stagingParameterNames = fields.Select(field => OracleText.GetParameterName(field, dbSetting)).AsList();
            var updateText = OracleText.GetUpdateCommandText(tableName, stagingTableName, fields, qualifiers, primaryField, dbSetting);

            return connection.TransactionalExecute(transaction =>
            {
                OracleStagingTable.EnsureStagingTable(connection, tableName, stagingTableName, dbFields, pseudoTableType, dbSetting, transaction);
                OracleStagingTable.ClearStagingTable(connection, stagingTableName, dbSetting, transaction);
                OracleStagingTable.ExecuteArrayBind(connection, stagingInsertText, stagingParameterNames, rows, null, null, null, bulkCopyTimeout, transaction);

                return connection.ExecuteNonQuery(updateText, bulkCopyTimeout, transaction: transaction);
            }, transaction);
        }

        #endregion

        #endregion

        #region Async

        #region BulkUpdateBaseAsync<TEntity>

        private static async Task<int> BulkUpdateBaseAsync<TEntity>(this OracleConnection connection,
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

            var fields = isDictionary ?
                OracleHelpers.GetDictionaryFields(entityList.First() as IDictionary<string, object>, dbFields, dbSetting) :
                OracleHelpers.GetEntityFields(entityType, dbFields, dbSetting);
            var gettersByMappedName = isDictionary ? null : Compiler.GetPropertyGettersByMappedName(entityType);
            var rows = OracleHelpers.BuildRows(entityList, fields, isDictionary, gettersByMappedName, false);

            var stagingInsertText = OracleText.GetStagingInsertCommandText(stagingTableName, fields, false, dbSetting);
            var stagingParameterNames = fields.Select(field => OracleText.GetParameterName(field, dbSetting)).AsList();
            var stagingOracleDbTypes = OracleHelpers.GetOracleDbTypes(fields, entityType, isDictionary);
            var updateText = OracleText.GetUpdateCommandText(tableName, stagingTableName, fields, qualifiers, primaryField, dbSetting);

            return await connection.TransactionalExecuteAsync(async transaction =>
            {
                await OracleStagingTable.EnsureStagingTableAsync(connection, tableName, stagingTableName, dbFields, pseudoTableType, dbSetting, transaction, cancellationToken);
                await OracleStagingTable.ClearStagingTableAsync(connection, stagingTableName, dbSetting, transaction, cancellationToken);
                await OracleStagingTable.ExecuteArrayBindAsync(connection, stagingInsertText, stagingParameterNames, rows, stagingOracleDbTypes, null, null, bulkCopyTimeout, transaction, cancellationToken);

                return await connection.ExecuteNonQueryAsync(updateText, bulkCopyTimeout, transaction: transaction, cancellationToken: cancellationToken);
            }, transaction, cancellationToken);
        }

        #endregion

        #region BulkUpdateBaseAsync<DataTable>

        private static async Task<int> BulkUpdateBaseAsync(this OracleConnection connection,
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

            var fields = OracleHelpers.GetDataTableFields(table, dbFields, dbSetting);
            var dataRows = (rowState.HasValue ?
                table.Rows.Cast<DataRow>().Where(row => row.RowState == rowState.Value) :
                table.Rows.Cast<DataRow>()).AsList();
            var rows = OracleHelpers.BuildRows(dataRows, fields, false);

            var stagingInsertText = OracleText.GetStagingInsertCommandText(stagingTableName, fields, false, dbSetting);
            var stagingParameterNames = fields.Select(field => OracleText.GetParameterName(field, dbSetting)).AsList();
            var updateText = OracleText.GetUpdateCommandText(tableName, stagingTableName, fields, qualifiers, primaryField, dbSetting);

            return await connection.TransactionalExecuteAsync(async transaction =>
            {
                await OracleStagingTable.EnsureStagingTableAsync(connection, tableName, stagingTableName, dbFields, pseudoTableType, dbSetting, transaction, cancellationToken);
                await OracleStagingTable.ClearStagingTableAsync(connection, stagingTableName, dbSetting, transaction, cancellationToken);
                await OracleStagingTable.ExecuteArrayBindAsync(connection, stagingInsertText, stagingParameterNames, rows, null, null, null, bulkCopyTimeout, transaction, cancellationToken);

                return await connection.ExecuteNonQueryAsync(updateText, bulkCopyTimeout, transaction: transaction, cancellationToken: cancellationToken);
            }, transaction, cancellationToken);
        }

        #endregion

        #endregion
    }
}
