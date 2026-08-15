using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MySqlConnector;
using RepoDb.Enumerations.MySqlConnector;
using RepoDb.Extensions;
using RepoDb.Interfaces;

namespace RepoDb.MySqlConnector.BulkOperations.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    internal static class MySqlConnectorExecution
    {
        #region Shared

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="pseudoTableName"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="qualifierField"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        public static void CreatePseudoTable(MySqlConnection connection,
            string tableName,
            string pseudoTableName,
            MySqlConnectorBulkImportPseudoTableType pseudoTableType,
            Field qualifierField = null,
            ITrace trace = null,
            string traceKey = null,
            MySqlTransaction transaction = null)
        {
            var dbSetting = connection.GetDbSetting();
            var commandText = MySqlConnectorText.GetCreatePseudoTableSql(tableName, pseudoTableName, pseudoTableType, dbSetting, qualifierField);
            connection.ExecuteNonQuery(commandText, transaction: transaction);
        }

        /// <summary>
        /// 
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
        public static async Task CreatePseudoTableAsync(MySqlConnection connection,
            string tableName,
            string pseudoTableName,
            MySqlConnectorBulkImportPseudoTableType pseudoTableType,
            Field qualifierField = null,
            ITrace trace = null,
            string traceKey = null,
            MySqlTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            var dbSetting = connection.GetDbSetting();
            var commandText = MySqlConnectorText.GetCreatePseudoTableSql(tableName, pseudoTableName, pseudoTableType, dbSetting, qualifierField);
            await connection.ExecuteNonQueryAsync(commandText, transaction: transaction, cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Creates an index on a pseudo table's qualifier columns. Must run right after
        /// <see cref="CreatePseudoTable"/> and before the client bulk-loads data into the pseudo table - see
        /// <see cref="MySqlConnectorText.GetCreatePseudoTableIndexSql"/> for why. No-ops when
        /// <paramref name="qualifiers"/> is null or empty (e.g. a plain <c>BulkInsert</c>, which has no
        /// qualifiers to index).
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="pseudoTableName"></param>
        /// <param name="qualifiers"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        public static void CreatePseudoTableIndex(MySqlConnection connection,
            string pseudoTableName,
            IEnumerable<Field> qualifiers,
            ITrace trace = null,
            string traceKey = null,
            MySqlTransaction transaction = null)
        {
            if (qualifiers?.Any() != true)
            {
                return;
            }

            var dbSetting = connection.GetDbSetting();
            var commandText = MySqlConnectorText.GetCreatePseudoTableIndexSql(pseudoTableName, qualifiers, dbSetting);
            connection.ExecuteNonQuery(commandText, transaction: transaction);
        }

        /// <summary>
        /// Creates an index on a pseudo table's qualifier columns. Must run right after
        /// <see cref="CreatePseudoTableAsync"/> and before the client bulk-loads data into the pseudo table -
        /// see <see cref="MySqlConnectorText.GetCreatePseudoTableIndexSql"/> for why. No-ops when
        /// <paramref name="qualifiers"/> is null or empty (e.g. a plain <c>BulkInsert</c>, which has no
        /// qualifiers to index).
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="pseudoTableName"></param>
        /// <param name="qualifiers"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task CreatePseudoTableIndexAsync(MySqlConnection connection,
            string pseudoTableName,
            IEnumerable<Field> qualifiers,
            ITrace trace = null,
            string traceKey = null,
            MySqlTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            if (qualifiers?.Any() != true)
            {
                return;
            }

            var dbSetting = connection.GetDbSetting();
            var commandText = MySqlConnectorText.GetCreatePseudoTableIndexSql(pseudoTableName, qualifiers, dbSetting);
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
        public static void TruncatePseudoTable(MySqlConnection connection,
            string pseudoTableName,
            ITrace trace = null,
            string traceKey = null,
            MySqlTransaction transaction = null)
        {
            var dbSetting = connection.GetDbSetting();
            var commandText = MySqlConnectorText.GetTruncatePseudoTableSql(pseudoTableName, dbSetting);
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
        public static async Task TruncatePseudoTableAsync(MySqlConnection connection,
            string pseudoTableName,
            ITrace trace = null,
            string traceKey = null,
            MySqlTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            var dbSetting = connection.GetDbSetting();
            var commandText = MySqlConnectorText.GetTruncatePseudoTableSql(pseudoTableName, dbSetting);
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
        public static void DropPseudoTable(MySqlConnection connection,
            string pseudoTableName,
            ITrace trace = null,
            string traceKey = null,
            MySqlTransaction transaction = null)
        {
            var dbSetting = connection.GetDbSetting();
            var commandText = MySqlConnectorText.GetDropPseudoTableSql(pseudoTableName, dbSetting);
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
        public static async Task DropPseudoTableAsync(MySqlConnection connection,
            string pseudoTableName,
            ITrace trace = null,
            string traceKey = null,
            MySqlTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            var dbSetting = connection.GetDbSetting();
            var commandText = MySqlConnectorText.GetDropPseudoTableSql(pseudoTableName, dbSetting);
            await connection.ExecuteNonQueryAsync(commandText, transaction: transaction, cancellationToken: cancellationToken);
        }

        #endregion

        #region Insert

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="pseudoTableName"></param>
        /// <param name="columnName"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        public static void AllowNullForColumn(MySqlConnection connection,
            string pseudoTableName,
            string columnName,
            ITrace trace = null,
            string traceKey = null,
            MySqlTransaction transaction = null)
        {
            var dbSetting = connection.GetDbSetting();
            var commandText = MySqlConnectorText.GetAllowNullForColumnSql(pseudoTableName, columnName, dbSetting);
            connection.ExecuteNonQuery(commandText, transaction: transaction);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="pseudoTableName"></param>
        /// <param name="columnName"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task AllowNullForColumnAsync(MySqlConnection connection,
            string pseudoTableName,
            string columnName,
            ITrace trace = null,
            string traceKey = null,
            MySqlTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            var dbSetting = connection.GetDbSetting();
            var commandText = MySqlConnectorText.GetAllowNullForColumnSql(pseudoTableName, columnName, dbSetting);
            await connection.ExecuteNonQueryAsync(commandText, transaction: transaction, cancellationToken: cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="identityField"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private static (string SequenceName, bool IsAlwaysGenerated) GetIdentitySequenceMetadata(MySqlConnection connection,
            string tableName,
            Field identityField,
            ITrace trace = null,
            string traceKey = null,
            MySqlTransaction transaction = null)
        {
            var dbSetting = connection.GetDbSetting();
            var commandText = MySqlConnectorText.GetIdentitySequenceMetadataSql(tableName, identityField, dbSetting);

            using var reader = (DbDataReader)connection.ExecuteReader(commandText, transaction: transaction);
            reader.Read();
            return (reader.GetString(0), string.Equals(reader.GetString(1), "ALWAYS", StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="identityField"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private static async Task<(string SequenceName, bool IsAlwaysGenerated)> GetIdentitySequenceMetadataAsync(MySqlConnection connection,
            string tableName,
            Field identityField,
            ITrace trace = null,
            string traceKey = null,
            MySqlTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            var dbSetting = connection.GetDbSetting();
            var commandText = MySqlConnectorText.GetIdentitySequenceMetadataSql(tableName, identityField, dbSetting);

            using var reader = (DbDataReader)await connection.ExecuteReaderAsync(commandText, transaction: transaction, cancellationToken: cancellationToken);
            await reader.ReadAsync(cancellationToken);
            return (reader.GetString(0), string.Equals(reader.GetString(1), "ALWAYS", StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="pseudoTableName"></param>
        /// <param name="fields"></param>
        /// <param name="identityField"></param>
        /// <param name="entities"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public static int InsertFromPseudoTableForReturnIdentity<TEntity>(MySqlConnection connection,
            string tableName,
            string pseudoTableName,
            IEnumerable<Field> fields,
            Field identityField,
            IList<TEntity> entities,
            ITrace trace = null,
            string traceKey = null,
            MySqlTransaction transaction = null)
            where TEntity : class
        {
            var dbSetting = connection.GetDbSetting();
            var (sequenceName, isAlwaysGenerated) = GetIdentitySequenceMetadata(connection, tableName, identityField, trace, traceKey, transaction);
            var commandText = MySqlConnectorText.GetInsertFromPseudoTableForReturnIdentitySql(tableName, pseudoTableName, fields, identityField, sequenceName, isAlwaysGenerated, dbSetting);
            var setter = FunctionCache.GetDataEntityPropertySetterCompiledFunction(typeof(TEntity), identityField);

            using var reader = (DbDataReader)connection.ExecuteReader(commandText, transaction: transaction);
            var result = 0;

            while (reader.Read())
            {
                setter(entities[result], Converter.DbNullToNull(reader.GetValue(0)));
                result++;
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="pseudoTableName"></param>
        /// <param name="fields"></param>
        /// <param name="identityField"></param>
        /// <param name="entities"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<int> InsertFromPseudoTableForReturnIdentityAsync<TEntity>(MySqlConnection connection,
            string tableName,
            string pseudoTableName,
            IEnumerable<Field> fields,
            Field identityField,
            IList<TEntity> entities,
            ITrace trace = null,
            string traceKey = null,
            MySqlTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            var dbSetting = connection.GetDbSetting();
            var (sequenceName, isAlwaysGenerated) = await GetIdentitySequenceMetadataAsync(connection, tableName, identityField, trace, traceKey, transaction, cancellationToken);
            var commandText = MySqlConnectorText.GetInsertFromPseudoTableForReturnIdentitySql(tableName, pseudoTableName, fields, identityField, sequenceName, isAlwaysGenerated, dbSetting);
            var setter = FunctionCache.GetDataEntityPropertySetterCompiledFunction(typeof(TEntity), identityField);

            using var reader = (DbDataReader)await connection.ExecuteReaderAsync(commandText, transaction: transaction, cancellationToken: cancellationToken);
            var result = 0;

            while (await reader.ReadAsync(cancellationToken))
            {
                setter(entities[result], Converter.DbNullToNull(reader.GetValue(0)));
                result++;
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="pseudoTableName"></param>
        /// <param name="fields"></param>
        /// <param name="identityField"></param>
        /// <param name="rows"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public static int InsertFromPseudoTableForReturnIdentityForDataTable(MySqlConnection connection,
            string tableName,
            string pseudoTableName,
            IEnumerable<Field> fields,
            Field identityField,
            IList<DataRow> rows,
            ITrace trace = null,
            string traceKey = null,
            MySqlTransaction transaction = null)
        {
            var dbSetting = connection.GetDbSetting();
            var (sequenceName, isAlwaysGenerated) = GetIdentitySequenceMetadata(connection, tableName, identityField, trace, traceKey, transaction);
            var commandText = MySqlConnectorText.GetInsertFromPseudoTableForReturnIdentitySql(tableName, pseudoTableName, fields, identityField, sequenceName, isAlwaysGenerated, dbSetting);

            using var reader = (DbDataReader)connection.ExecuteReader(commandText, transaction: transaction);
            var result = 0;

            while (reader.Read())
            {
                rows[result][identityField.Name] = Converter.DbNullToNull(reader.GetValue(0));
                result++;
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="pseudoTableName"></param>
        /// <param name="fields"></param>
        /// <param name="identityField"></param>
        /// <param name="rows"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<int> InsertFromPseudoTableForReturnIdentityForDataTableAsync(MySqlConnection connection,
            string tableName,
            string pseudoTableName,
            IEnumerable<Field> fields,
            Field identityField,
            IList<DataRow> rows,
            ITrace trace = null,
            string traceKey = null,
            MySqlTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            var dbSetting = connection.GetDbSetting();
            var (sequenceName, isAlwaysGenerated) = await GetIdentitySequenceMetadataAsync(connection, tableName, identityField, trace, traceKey, transaction, cancellationToken);
            var commandText = MySqlConnectorText.GetInsertFromPseudoTableForReturnIdentitySql(tableName, pseudoTableName, fields, identityField, sequenceName, isAlwaysGenerated, dbSetting);

            using var reader = (DbDataReader)await connection.ExecuteReaderAsync(commandText, transaction: transaction, cancellationToken: cancellationToken);
            var result = 0;

            while (await reader.ReadAsync(cancellationToken))
            {
                rows[result][identityField.Name] = Converter.DbNullToNull(reader.GetValue(0));
                result++;
            }

            return result;
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
        /// <param name="identityField"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public static int MergeFromPseudoTable(MySqlConnection connection,
            string tableName,
            string pseudoTableName,
            IEnumerable<Field> fields,
            IEnumerable<Field> qualifiers,
            Field identityField,
            ITrace trace = null,
            string traceKey = null,
            MySqlTransaction transaction = null)
        {
            var dbSetting = connection.GetDbSetting();
            var commandText = MySqlConnectorText.GetMergeFromPseudoTableSql(tableName, pseudoTableName, fields, qualifiers, identityField, dbSetting);
            return connection.ExecuteNonQuery(commandText, transaction: transaction);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="pseudoTableName"></param>
        /// <param name="fields"></param>
        /// <param name="qualifiers"></param>
        /// <param name="identityField"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<int> MergeFromPseudoTableAsync(MySqlConnection connection,
            string tableName,
            string pseudoTableName,
            IEnumerable<Field> fields,
            IEnumerable<Field> qualifiers,
            Field identityField,
            ITrace trace = null,
            string traceKey = null,
            MySqlTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            var dbSetting = connection.GetDbSetting();
            var commandText = MySqlConnectorText.GetMergeFromPseudoTableSql(tableName, pseudoTableName, fields, qualifiers, identityField, dbSetting);
            return await connection.ExecuteNonQueryAsync(commandText, transaction: transaction, cancellationToken: cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="pseudoTableName"></param>
        /// <param name="fields"></param>
        /// <param name="identityField"></param>
        /// <param name="qualifiers"></param>
        /// <param name="entities"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public static int MergeFromPseudoTableForReturnIdentity<TEntity>(MySqlConnection connection,
            string tableName,
            string pseudoTableName,
            IEnumerable<Field> fields,
            Field identityField,
            IEnumerable<Field> qualifiers,
            IList<TEntity> entities,
            ITrace trace = null,
            string traceKey = null,
            MySqlTransaction transaction = null)
            where TEntity : class
        {
            var dbSetting = connection.GetDbSetting();
            var (sequenceName, isAlwaysGenerated) = GetIdentitySequenceMetadata(connection, tableName, identityField, trace, traceKey, transaction);
            var commandText = MySqlConnectorText.GetMergeFromPseudoTableForReturnIdentitySql(tableName, pseudoTableName, fields, identityField, qualifiers, sequenceName, isAlwaysGenerated, dbSetting);
            var setter = FunctionCache.GetDataEntityPropertySetterCompiledFunction(typeof(TEntity), identityField);

            using var reader = (DbDataReader)connection.ExecuteReader(commandText, transaction: transaction);
            var result = 0;

            while (reader.Read())
            {
                setter?.Invoke(entities[result], Converter.DbNullToNull(reader.GetValue(0)));
                result++;
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="pseudoTableName"></param>
        /// <param name="fields"></param>
        /// <param name="identityField"></param>
        /// <param name="qualifiers"></param>
        /// <param name="entities"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<int> MergeFromPseudoTableForReturnIdentityAsync<TEntity>(MySqlConnection connection,
            string tableName,
            string pseudoTableName,
            IEnumerable<Field> fields,
            Field identityField,
            IEnumerable<Field> qualifiers,
            IList<TEntity> entities,
            ITrace trace = null,
            string traceKey = null,
            MySqlTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            var setter = FunctionCache.GetDataEntityPropertySetterCompiledFunction(typeof(TEntity), identityField);
            var dbSetting = connection.GetDbSetting();
            var (sequenceName, isAlwaysGenerated) = await GetIdentitySequenceMetadataAsync(connection, tableName, identityField, trace, traceKey, transaction, cancellationToken);
            var commandText = MySqlConnectorText.GetMergeFromPseudoTableForReturnIdentitySql(tableName, pseudoTableName, fields, identityField, qualifiers, sequenceName, isAlwaysGenerated, dbSetting);

            using var reader = (DbDataReader)await connection.ExecuteReaderAsync(commandText, transaction: transaction, cancellationToken: cancellationToken);
            var result = 0;

            while (await reader.ReadAsync(cancellationToken))
            {
                setter?.Invoke(entities[result], Converter.DbNullToNull(reader.GetValue(0)));
                result++;
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="pseudoTableName"></param>
        /// <param name="fields"></param>
        /// <param name="identityField"></param>
        /// <param name="qualifiers"></param>
        /// <param name="rows"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public static int MergeFromPseudoTableForReturnIdentityForDataTable(MySqlConnection connection,
            string tableName,
            string pseudoTableName,
            IEnumerable<Field> fields,
            Field identityField,
            IEnumerable<Field> qualifiers,
            IList<DataRow> rows,
            ITrace trace = null,
            string traceKey = null,
            MySqlTransaction transaction = null)
        {
            var dbSetting = connection.GetDbSetting();
            var (sequenceName, isAlwaysGenerated) = GetIdentitySequenceMetadata(connection, tableName, identityField, trace, traceKey, transaction);
            var commandText = MySqlConnectorText.GetMergeFromPseudoTableForReturnIdentitySql(tableName, pseudoTableName, fields, identityField, qualifiers, sequenceName, isAlwaysGenerated, dbSetting);

            using var reader = (DbDataReader)connection.ExecuteReader(commandText, transaction: transaction);
            var result = 0;

            while (reader.Read())
            {
                rows[result][identityField.Name] = Converter.DbNullToNull(reader.GetValue(0));
                result++;
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="pseudoTableName"></param>
        /// <param name="fields"></param>
        /// <param name="identityField"></param>
        /// <param name="qualifiers"></param>
        /// <param name="rows"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<int> MergeFromPseudoTableForReturnIdentityForDataTableAsync(MySqlConnection connection,
            string tableName,
            string pseudoTableName,
            IEnumerable<Field> fields,
            Field identityField,
            IEnumerable<Field> qualifiers,
            IList<DataRow> rows,
            ITrace trace = null,
            string traceKey = null,
            MySqlTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            var dbSetting = connection.GetDbSetting();
            var (sequenceName, isAlwaysGenerated) = await GetIdentitySequenceMetadataAsync(connection, tableName, identityField, trace, traceKey, transaction, cancellationToken);
            var commandText = MySqlConnectorText.GetMergeFromPseudoTableForReturnIdentitySql(tableName, pseudoTableName, fields, identityField, qualifiers, sequenceName, isAlwaysGenerated, dbSetting);

            using var reader = (DbDataReader)await connection.ExecuteReaderAsync(commandText, transaction: transaction, cancellationToken: cancellationToken);
            var result = 0;

            while (await reader.ReadAsync(cancellationToken))
            {
                rows[result][identityField.Name] = Converter.DbNullToNull(reader.GetValue(0));
                result++;
            }

            return result;
        }

        #endregion

        #region Update

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
        /// <returns></returns>
        public static int UpdateFromPseudoTable(MySqlConnection connection,
            string tableName,
            string pseudoTableName,
            IEnumerable<Field> fields,
            IEnumerable<Field> qualifiers,
            ITrace trace = null,
            string traceKey = null,
            MySqlTransaction transaction = null)
        {
            var dbSetting = connection.GetDbSetting();
            var commandText = MySqlConnectorText.GetUpdateFromPseudoTableSql(tableName, pseudoTableName, fields, qualifiers, dbSetting);
            return connection.ExecuteNonQuery(commandText, transaction: transaction);
        }

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
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<int> UpdateFromPseudoTableAsync(MySqlConnection connection,
            string tableName,
            string pseudoTableName,
            IEnumerable<Field> fields,
            IEnumerable<Field> qualifiers,
            ITrace trace = null,
            string traceKey = null,
            MySqlTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            var dbSetting = connection.GetDbSetting();
            var commandText = MySqlConnectorText.GetUpdateFromPseudoTableSql(tableName, pseudoTableName, fields, qualifiers, dbSetting);
            return await connection.ExecuteNonQueryAsync(commandText, transaction: transaction, cancellationToken: cancellationToken);
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
        public static int DeleteFromPseudoTable(MySqlConnection connection,
            string tableName,
            string pseudoTableName,
            IEnumerable<Field> qualifiers,
            ITrace trace = null,
            string traceKey = null,
            MySqlTransaction transaction = null)
        {
            var dbSetting = connection.GetDbSetting();
            var commandText = MySqlConnectorText.GetDeleteFromPseudoTableSql(tableName, pseudoTableName, qualifiers, dbSetting);
            return connection.ExecuteNonQuery(commandText, transaction: transaction);
        }

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
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<int> DeleteFromPseudoTableAsync(MySqlConnection connection,
            string tableName,
            string pseudoTableName,
            IEnumerable<Field> qualifiers,
            ITrace trace = null,
            string traceKey = null,
            MySqlTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            var dbSetting = connection.GetDbSetting();
            var commandText = MySqlConnectorText.GetDeleteFromPseudoTableSql(tableName, pseudoTableName, qualifiers, dbSetting);
            return await connection.ExecuteNonQueryAsync(commandText, transaction: transaction, cancellationToken: cancellationToken);
        }

        #endregion
    }
}
