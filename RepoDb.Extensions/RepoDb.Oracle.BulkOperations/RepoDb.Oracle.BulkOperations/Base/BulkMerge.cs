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

        #region BulkMergeBase<TEntity>

        private static int BulkMergeBase<TEntity>(this OracleConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<Field> qualifiers = null,
            int? bulkCopyTimeout = null,
            OracleBulkImportIdentityBehavior identityBehavior = default,
            OracleBulkImportPseudoTableType pseudoTableType = default,
            OracleTransaction transaction = null)
            where TEntity : class
        {
            var entityList = entities.AsList();
            var entityType = entityList?.FirstOrDefault()?.GetType() ?? typeof(TEntity);
            var isDictionary = TypeCache.Get(entityType).IsDictionaryStringObject();
            var dbSetting = connection.GetDbSetting();
            var dbFields = DbFieldCache.Get(connection, tableName, transaction);
            var primaryField = dbFields.GetPrimary()?.AsField();
            var identityField = dbFields.GetIdentity();
            var stagingTableName = OracleStagingTable.GetStagingTableName(tableName, pseudoTableType);
            var fields = isDictionary ?
                OracleHelpers.GetDictionaryFields(entityList.First() as IDictionary<string, object>, dbFields, dbSetting) :
                OracleHelpers.GetEntityFields(entityType, dbFields, dbSetting);
            var gettersByMappedName = isDictionary ? null : Compiler.GetPropertyGettersByMappedName(entityType);
            var rows = OracleHelpers.BuildRows(entityList, fields, isDictionary, gettersByMappedName, true);
            var stagingInsertText = OracleText.GetStagingInsertCommandText(stagingTableName, fields, true, dbSetting);
            var stagingParameterNames = fields
                .Select(field => OracleText.GetParameterName(field, dbSetting))
                .Concat(new[] { OracleStagingTable.OrderColumnName })
                .AsList();
            var stagingOracleDbTypes = OracleHelpers.GetOracleDbTypes(fields, entityType, isDictionary)
                .Concat(new OracleDbType?[] { null })
                .AsList();
            var mergeText = OracleText.GetMergeCommandText(tableName, stagingTableName, fields, qualifiers, primaryField, identityField?.AsField(), identityBehavior, dbSetting);
            var returnIdentity = identityBehavior == OracleBulkImportIdentityBehavior.ReturnIdentity && identityField != null;
            var resolvedQualifiers = OracleText.ResolveQualifiers(qualifiers, primaryField);

            return connection.TransactionalExecute(transaction =>
            {
                OracleStagingTable.EnsureStagingTable(connection, tableName, stagingTableName, dbFields, pseudoTableType, dbSetting, transaction);
                OracleStagingTable.ClearStagingTable(connection, stagingTableName, dbSetting, transaction);
                OracleStagingTable.ExecuteArrayBind(connection, stagingInsertText, stagingParameterNames, rows, stagingOracleDbTypes, null, null, bulkCopyTimeout, transaction);

                var affected = connection.ExecuteNonQuery(mergeText, bulkCopyTimeout, transaction: transaction);

                if (returnIdentity)
                {
                    var lookupText = OracleText.GetMergeIdentityLookupCommandText(tableName, stagingTableName, resolvedQualifiers, identityField.AsField(), dbSetting);
                    var identityResults = connection.ExecuteQuery<IdentityResult>(lookupText, transaction: transaction);
                    var byIndex = identityResults.ToDictionary(r => r.Index, r => (object)r.Identity);
                    OracleHelpers.SetIdentities(entityType, entityList, identityField, byIndex, dbSetting);
                }

                return affected;
            }, transaction);
        }

        #endregion

        #region BulkMergeBase<DataTable>

        private static int BulkMergeBase(this OracleConnection connection,
            string tableName,
            DataTable table,
            IEnumerable<Field> qualifiers = null,
            DataRowState? rowState = null,
            int? bulkCopyTimeout = null,
            OracleBulkImportIdentityBehavior identityBehavior = default,
            OracleBulkImportPseudoTableType pseudoTableType = default,
            OracleTransaction transaction = null)
        {
            var dbSetting = connection.GetDbSetting();
            var dbFields = DbFieldCache.Get(connection, tableName, transaction);
            var primaryField = dbFields.GetPrimary()?.AsField();
            var identityField = dbFields.GetIdentity();
            var stagingTableName = OracleStagingTable.GetStagingTableName(tableName, pseudoTableType);

            var fields = OracleHelpers.GetDataTableFields(table, dbFields, dbSetting);
            var dataRows = (rowState.HasValue ?
                table.Rows.Cast<DataRow>().Where(row => row.RowState == rowState.Value) :
                table.Rows.Cast<DataRow>()).AsList();
            var rows = OracleHelpers.BuildRows(dataRows, fields, true);

            var stagingInsertText = OracleText.GetStagingInsertCommandText(stagingTableName, fields, true, dbSetting);
            var stagingParameterNames = fields
                .Select(field => OracleText.GetParameterName(field, dbSetting))
                .Concat(new[] { OracleStagingTable.OrderColumnName })
                .AsList();

            var mergeText = OracleText.GetMergeCommandText(tableName, stagingTableName, fields, qualifiers, primaryField, identityField?.AsField(), identityBehavior, dbSetting);
            var returnIdentity = identityBehavior == OracleBulkImportIdentityBehavior.ReturnIdentity && identityField != null;
            var resolvedQualifiers = OracleText.ResolveQualifiers(qualifiers, primaryField);

            return connection.TransactionalExecute(transaction =>
            {
                OracleStagingTable.EnsureStagingTable(connection, tableName, stagingTableName, dbFields, pseudoTableType, dbSetting, transaction);
                OracleStagingTable.ClearStagingTable(connection, stagingTableName, dbSetting, transaction);
                OracleStagingTable.ExecuteArrayBind(connection, stagingInsertText, stagingParameterNames, rows, null, null, null, bulkCopyTimeout, transaction);

                var affected = connection.ExecuteNonQuery(mergeText, bulkCopyTimeout, transaction: transaction);

                if (returnIdentity)
                {
                    var lookupText = OracleText.GetMergeIdentityLookupCommandText(tableName, stagingTableName, resolvedQualifiers, identityField.AsField(), dbSetting);
                    var identityResults = connection.ExecuteQuery<IdentityResult>(lookupText, transaction: transaction);
                    var byIndex = identityResults.ToDictionary(r => r.Index, r => (object)r.Identity);
                    OracleHelpers.SetDataTableIdentities(table, identityField, byIndex, dbSetting);
                }

                return affected;
            }, transaction);
        }

        #endregion

        #endregion

        #region Async

        #region BulkMergeBaseAsync<TEntity>

        private static async Task<int> BulkMergeBaseAsync<TEntity>(this OracleConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<Field> qualifiers = null,
            int? bulkCopyTimeout = null,
            OracleBulkImportIdentityBehavior identityBehavior = default,
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
            var identityField = dbFields.GetIdentity();
            var stagingTableName = OracleStagingTable.GetStagingTableName(tableName, pseudoTableType);

            var fields = isDictionary ?
                OracleHelpers.GetDictionaryFields(entityList.First() as IDictionary<string, object>, dbFields, dbSetting) :
                OracleHelpers.GetEntityFields(entityType, dbFields, dbSetting);
            var gettersByMappedName = isDictionary ? null : Compiler.GetPropertyGettersByMappedName(entityType);
            var rows = OracleHelpers.BuildRows(entityList, fields, isDictionary, gettersByMappedName, true);

            var stagingInsertText = OracleText.GetStagingInsertCommandText(stagingTableName, fields, true, dbSetting);
            var stagingParameterNames = fields
                .Select(field => OracleText.GetParameterName(field, dbSetting))
                .Concat(new[] { OracleStagingTable.OrderColumnName })
                .AsList();
            var stagingOracleDbTypes = OracleHelpers.GetOracleDbTypes(fields, entityType, isDictionary)
                .Concat(new OracleDbType?[] { null })
                .AsList();

            var mergeText = OracleText.GetMergeCommandText(tableName, stagingTableName, fields, qualifiers, primaryField, identityField?.AsField(), identityBehavior, dbSetting);
            var returnIdentity = identityBehavior == OracleBulkImportIdentityBehavior.ReturnIdentity && identityField != null;
            var resolvedQualifiers = OracleText.ResolveQualifiers(qualifiers, primaryField);

            return await connection.TransactionalExecuteAsync(async transaction =>
            {
                await OracleStagingTable.EnsureStagingTableAsync(connection, tableName, stagingTableName, dbFields, pseudoTableType, dbSetting, transaction, cancellationToken);
                await OracleStagingTable.ClearStagingTableAsync(connection, stagingTableName, dbSetting, transaction, cancellationToken);
                await OracleStagingTable.ExecuteArrayBindAsync(connection, stagingInsertText, stagingParameterNames, rows, stagingOracleDbTypes, null, null, bulkCopyTimeout, transaction, cancellationToken);

                var affected = await connection.ExecuteNonQueryAsync(mergeText, bulkCopyTimeout, transaction: transaction, cancellationToken: cancellationToken);

                if (returnIdentity)
                {
                    var lookupText = OracleText.GetMergeIdentityLookupCommandText(tableName, stagingTableName, resolvedQualifiers, identityField.AsField(), dbSetting);
                    var identityResults = await connection.ExecuteQueryAsync<IdentityResult>(lookupText, transaction: transaction, cancellationToken: cancellationToken);
                    var byIndex = identityResults.ToDictionary(r => r.Index, r => (object)r.Identity);
                    OracleHelpers.SetIdentities(entityType, entityList, identityField, byIndex, dbSetting);
                }

                return affected;
            }, transaction, cancellationToken);
        }

        #endregion

        #region BulkMergeBaseAsync<DataTable>

        private static async Task<int> BulkMergeBaseAsync(this OracleConnection connection,
            string tableName,
            DataTable table,
            IEnumerable<Field> qualifiers = null,
            DataRowState? rowState = null,
            int? bulkCopyTimeout = null,
            OracleBulkImportIdentityBehavior identityBehavior = default,
            OracleBulkImportPseudoTableType pseudoTableType = default,
            OracleTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            var dbSetting = connection.GetDbSetting();
            var dbFields = await DbFieldCache.GetAsync(connection, tableName, transaction, cancellationToken);
            var primaryField = dbFields.GetPrimary()?.AsField();
            var identityField = dbFields.GetIdentity();
            var stagingTableName = OracleStagingTable.GetStagingTableName(tableName, pseudoTableType);

            var fields = OracleHelpers.GetDataTableFields(table, dbFields, dbSetting);
            var dataRows = (rowState.HasValue ?
                table.Rows.Cast<DataRow>().Where(row => row.RowState == rowState.Value) :
                table.Rows.Cast<DataRow>()).AsList();
            var rows = OracleHelpers.BuildRows(dataRows, fields, true);

            var stagingInsertText = OracleText.GetStagingInsertCommandText(stagingTableName, fields, true, dbSetting);
            var stagingParameterNames = fields
                .Select(field => OracleText.GetParameterName(field, dbSetting))
                .Concat(new[] { OracleStagingTable.OrderColumnName })
                .AsList();

            var mergeText = OracleText.GetMergeCommandText(tableName, stagingTableName, fields, qualifiers, primaryField, identityField?.AsField(), identityBehavior, dbSetting);
            var returnIdentity = identityBehavior == OracleBulkImportIdentityBehavior.ReturnIdentity && identityField != null;
            var resolvedQualifiers = OracleText.ResolveQualifiers(qualifiers, primaryField);

            return await connection.TransactionalExecuteAsync(async transaction =>
            {
                await OracleStagingTable.EnsureStagingTableAsync(connection, tableName, stagingTableName, dbFields, pseudoTableType, dbSetting, transaction, cancellationToken);
                await OracleStagingTable.ClearStagingTableAsync(connection, stagingTableName, dbSetting, transaction, cancellationToken);
                await OracleStagingTable.ExecuteArrayBindAsync(connection, stagingInsertText, stagingParameterNames, rows, null, null, null, bulkCopyTimeout, transaction, cancellationToken);

                var affected = await connection.ExecuteNonQueryAsync(mergeText, bulkCopyTimeout, transaction: transaction, cancellationToken: cancellationToken);

                if (returnIdentity)
                {
                    var lookupText = OracleText.GetMergeIdentityLookupCommandText(tableName, stagingTableName, resolvedQualifiers, identityField.AsField(), dbSetting);
                    var identityResults = await connection.ExecuteQueryAsync<IdentityResult>(lookupText, transaction: transaction, cancellationToken: cancellationToken);
                    var byIndex = identityResults.ToDictionary(r => r.Index, r => (object)r.Identity);
                    OracleHelpers.SetDataTableIdentities(table, identityField, byIndex, dbSetting);
                }

                return affected;
            }, transaction, cancellationToken);
        }

        #endregion

        #endregion
    }
}
