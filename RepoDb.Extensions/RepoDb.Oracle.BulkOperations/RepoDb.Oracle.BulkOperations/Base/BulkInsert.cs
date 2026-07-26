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

        #region BulkInsertBase<TEntity>

        private static int BulkInsertBase<TEntity>(this OracleConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<OracleBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            BulkImportIdentityBehavior identityBehavior = default,
            OracleTransaction transaction = null)
            where TEntity : class
        {
            var entityList = entities as IList<TEntity> ?? entities.ToList();
            var entityType = entityList.FirstOrDefault()?.GetType() ?? typeof(TEntity);
            var isDictionary = TypeCache.Get(entityType).IsDictionaryStringObject();
            var dbSetting = connection.GetDbSetting();
            var dbFields = DbFieldCache.Get(connection, tableName, transaction);
            var identityField = dbFields.GetIdentity();
            var includeIdentity = identityBehavior == BulkImportIdentityBehavior.KeepIdentity;

            mappings = mappings?.Any() == true ? mappings :
                isDictionary ?
                    OracleHelpers.GetMappings(entityList.First() as IDictionary<string, object>, dbFields, includeIdentity, dbSetting) :
                    OracleHelpers.GetMappings(dbFields, entityType, includeIdentity, dbSetting);
            var mappingList = mappings.AsList();

            var fields = mappingList.Select(m => new Field(m.DestinationColumn)).AsList();
            var parameterNames = fields.Select(f => OracleText.GetParameterName(f, dbSetting)).AsList();
            var oracleDbTypes = mappingList.Select(m => m.OracleDbType).AsList();
            var sourceFields = mappingList.Select(m => new Field(m.SourceColumn)).AsList();
            var gettersByMappedName = isDictionary ? null : Compiler.GetPropertyGettersByMappedName(entityType);
            var rows = OracleHelpers.BuildRows(entityList, sourceFields, isDictionary, gettersByMappedName, false);

            var returnIdentity = identityBehavior == BulkImportIdentityBehavior.ReturnIdentity && identityField != null;
            var commandText = OracleText.GetInsertCommandText(tableName, fields, identityField?.AsField(), identityBehavior, dbSetting);

            return connection.TransactionalExecute(txn =>
            {
                var (affected, returned) = OracleStagingTable.ExecuteArrayBind(connection, commandText, parameterNames, rows,
                    oracleDbTypes, returnIdentity ? identityField.Name : null, null, bulkCopyTimeout, txn);

                if (returnIdentity && returned != null)
                {
                    OracleHelpers.SetIdentities(entityType, entityList, identityField, ToIndexMap(returned), dbSetting);
                }

                return affected;
            }, transaction);
        }

        #endregion

        #region BulkInsertBase<DataTable>

        private static int BulkInsertBase(this OracleConnection connection,
            string tableName,
            DataTable table,
            DataRowState? rowState = null,
            IEnumerable<OracleBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            BulkImportIdentityBehavior identityBehavior = default,
            OracleTransaction transaction = null)
        {
            var dbSetting = connection.GetDbSetting();
            var dbFields = DbFieldCache.Get(connection, tableName, transaction);
            var identityField = dbFields.GetIdentity();
            var includeIdentity = identityBehavior == BulkImportIdentityBehavior.KeepIdentity;

            mappings = mappings?.Any() == true ? mappings :
                OracleHelpers.GetMappings(table, dbFields, includeIdentity, dbSetting);
            var mappingList = mappings.AsList();

            var fields = mappingList.Select(m => new Field(m.DestinationColumn)).AsList();
            var parameterNames = fields.Select(f => OracleText.GetParameterName(f, dbSetting)).AsList();
            var oracleDbTypes = mappingList.Select(m => m.OracleDbType).AsList();
            var sourceFields = mappingList.Select(m => new Field(m.SourceColumn)).AsList();

            var dataRows = (rowState.HasValue ?
                table.Rows.Cast<DataRow>().Where(row => row.RowState == rowState.Value) :
                table.Rows.Cast<DataRow>()).AsList();
            var rows = OracleHelpers.BuildRows(dataRows, sourceFields, false);

            var returnIdentity = identityBehavior == BulkImportIdentityBehavior.ReturnIdentity && identityField != null;
            var commandText = OracleText.GetInsertCommandText(tableName, fields, identityField?.AsField(), identityBehavior, dbSetting);

            return connection.TransactionalExecute(txn =>
            {
                var (affected, returned) = OracleStagingTable.ExecuteArrayBind(connection, commandText, parameterNames, rows,
                    oracleDbTypes, returnIdentity ? identityField.Name : null, null, bulkCopyTimeout, txn);

                if (returnIdentity && returned != null)
                {
                    OracleHelpers.SetDataTableIdentities(table, identityField, ToIndexMap(returned), dbSetting);
                }

                return affected;
            }, transaction);
        }

        #endregion

        #endregion

        #region Async

        #region BulkInsertBaseAsync<TEntity>

        private static async Task<int> BulkInsertBaseAsync<TEntity>(this OracleConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<OracleBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            BulkImportIdentityBehavior identityBehavior = default,
            OracleTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            var entityList = entities as IList<TEntity> ?? entities.ToList();
            var entityType = entityList.FirstOrDefault()?.GetType() ?? typeof(TEntity);
            var isDictionary = TypeCache.Get(entityType).IsDictionaryStringObject();
            var dbSetting = connection.GetDbSetting();
            var dbFields = await DbFieldCache.GetAsync(connection, tableName, transaction, cancellationToken);
            var identityField = dbFields.GetIdentity();
            var includeIdentity = identityBehavior == BulkImportIdentityBehavior.KeepIdentity;

            mappings = mappings?.Any() == true ? mappings :
                isDictionary ?
                    OracleHelpers.GetMappings(entityList.First() as IDictionary<string, object>, dbFields, includeIdentity, dbSetting) :
                    OracleHelpers.GetMappings(dbFields, entityType, includeIdentity, dbSetting);
            var mappingList = mappings.AsList();

            var fields = mappingList.Select(m => new Field(m.DestinationColumn)).AsList();
            var parameterNames = fields.Select(f => OracleText.GetParameterName(f, dbSetting)).AsList();
            var oracleDbTypes = mappingList.Select(m => m.OracleDbType).AsList();
            var sourceFields = mappingList.Select(m => new Field(m.SourceColumn)).AsList();
            var gettersByMappedName = isDictionary ? null : Compiler.GetPropertyGettersByMappedName(entityType);
            var rows = OracleHelpers.BuildRows(entityList, sourceFields, isDictionary, gettersByMappedName, false);

            var returnIdentity = identityBehavior == BulkImportIdentityBehavior.ReturnIdentity && identityField != null;
            var commandText = OracleText.GetInsertCommandText(tableName, fields, identityField?.AsField(), identityBehavior, dbSetting);

            return await connection.TransactionalExecuteAsync(async txn =>
            {
                var (affected, returned) = await OracleStagingTable.ExecuteArrayBindAsync(connection, commandText, parameterNames, rows,
                    oracleDbTypes, returnIdentity ? identityField.Name : null, null, bulkCopyTimeout, txn, cancellationToken);

                if (returnIdentity && returned != null)
                {
                    OracleHelpers.SetIdentities(entityType, entityList, identityField, ToIndexMap(returned), dbSetting);
                }

                return affected;
            }, transaction, cancellationToken);
        }

        #endregion

        #region BulkInsertBaseAsync<DataTable>

        private static async Task<int> BulkInsertBaseAsync(this OracleConnection connection,
            string tableName,
            DataTable table,
            DataRowState? rowState = null,
            IEnumerable<OracleBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            BulkImportIdentityBehavior identityBehavior = default,
            OracleTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            var dbSetting = connection.GetDbSetting();
            var dbFields = await DbFieldCache.GetAsync(connection, tableName, transaction, cancellationToken);
            var identityField = dbFields.GetIdentity();
            var includeIdentity = identityBehavior == BulkImportIdentityBehavior.KeepIdentity;

            mappings = mappings?.Any() == true ? mappings :
                OracleHelpers.GetMappings(table, dbFields, includeIdentity, dbSetting);
            var mappingList = mappings.AsList();

            var fields = mappingList.Select(m => new Field(m.DestinationColumn)).AsList();
            var parameterNames = fields.Select(f => OracleText.GetParameterName(f, dbSetting)).AsList();
            var oracleDbTypes = mappingList.Select(m => m.OracleDbType).AsList();
            var sourceFields = mappingList.Select(m => new Field(m.SourceColumn)).AsList();

            var dataRows = (rowState.HasValue ?
                table.Rows.Cast<DataRow>().Where(row => row.RowState == rowState.Value) :
                table.Rows.Cast<DataRow>()).AsList();
            var rows = OracleHelpers.BuildRows(dataRows, sourceFields, false);

            var returnIdentity = identityBehavior == BulkImportIdentityBehavior.ReturnIdentity && identityField != null;
            var commandText = OracleText.GetInsertCommandText(tableName, fields, identityField?.AsField(), identityBehavior, dbSetting);

            return await connection.TransactionalExecuteAsync(async txn =>
            {
                var (affected, returned) = await OracleStagingTable.ExecuteArrayBindAsync(connection, commandText, parameterNames, rows,
                    oracleDbTypes, returnIdentity ? identityField.Name : null, null, bulkCopyTimeout, txn, cancellationToken);

                if (returnIdentity && returned != null)
                {
                    OracleHelpers.SetDataTableIdentities(table, identityField, ToIndexMap(returned), dbSetting);
                }

                return affected;
            }, transaction, cancellationToken);
        }

        #endregion

        #endregion

        #region Helpers

        private static IReadOnlyDictionary<int, object> ToIndexMap(object[] values)
        {
            var map = new Dictionary<int, object>(values.Length);

            for (var i = 0; i < values.Length; i++)
            {
                map[i] = values[i];
            }

            return map;
        }

        #endregion
    }
}
