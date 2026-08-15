using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IBM.Data.Db2;
using RepoDb.Enumerations.Db2;
using RepoDb.Extensions;
using RepoDb.Interfaces;

namespace RepoDb.Db2.BulkOperations.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    internal static class Db2Execution
    {
        #region Shared

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="pseudoTableName"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="qualifierFields"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        /// <param name="nullableFields">See the remarks on <see cref="Db2Text.GetCreatePseudoTableSql"/>.</param>
        public static void CreatePseudoTable(DB2Connection connection,
            string tableName,
            string pseudoTableName,
            Db2BulkImportPseudoTableType pseudoTableType,
            IEnumerable<Field> qualifierFields = null,
            ITrace trace = null,
            string traceKey = null,
            DB2Transaction transaction = null,
            IEnumerable<Field> nullableFields = null)
        {
            var dbSetting = connection.GetDbSetting();
            var commandText = Db2Text.GetCreatePseudoTableSql(tableName, pseudoTableName, dbSetting, qualifierFields, nullableFields);
            connection.ExecuteNonQuery(commandText, trace: trace, traceKey: traceKey, transaction: transaction);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="pseudoTableName"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="qualifierFields"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="nullableFields">See the remarks on <see cref="Db2Text.GetCreatePseudoTableSql"/>.</param>
        /// <returns></returns>
        public static async Task CreatePseudoTableAsync(DB2Connection connection,
            string tableName,
            string pseudoTableName,
            Db2BulkImportPseudoTableType pseudoTableType,
            IEnumerable<Field> qualifierFields = null,
            ITrace trace = null,
            string traceKey = null,
            DB2Transaction transaction = null,
            CancellationToken cancellationToken = default,
            IEnumerable<Field> nullableFields = null)
        {
            var dbSetting = connection.GetDbSetting();
            var commandText = Db2Text.GetCreatePseudoTableSql(tableName, pseudoTableName, dbSetting, qualifierFields, nullableFields);
            await connection.ExecuteNonQueryAsync(commandText, trace: trace, traceKey: traceKey, transaction: transaction, cancellationToken: cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="pseudoTableName"></param>
        /// <param name="qualifiers"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        public static void CreatePseudoTableIndex(DB2Connection connection,
            string pseudoTableName,
            IEnumerable<Field> qualifiers,
            ITrace trace = null,
            string traceKey = null,
            DB2Transaction transaction = null)
        {
            if (qualifiers?.Any() != true)
            {
                return;
            }

            var dbSetting = connection.GetDbSetting();
            var commandText = Db2Text.GetCreatePseudoTableIndexSql(pseudoTableName, qualifiers, dbSetting);
            connection.ExecuteNonQuery(commandText, trace: trace, traceKey: traceKey, transaction: transaction);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="pseudoTableName"></param>
        /// <param name="qualifiers"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task CreatePseudoTableIndexAsync(DB2Connection connection,
            string pseudoTableName,
            IEnumerable<Field> qualifiers,
            ITrace trace = null,
            string traceKey = null,
            DB2Transaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            if (qualifiers?.Any() != true)
            {
                return;
            }

            var dbSetting = connection.GetDbSetting();
            var commandText = Db2Text.GetCreatePseudoTableIndexSql(pseudoTableName, qualifiers, dbSetting);
            await connection.ExecuteNonQueryAsync(commandText, trace: trace, traceKey: traceKey, transaction: transaction, cancellationToken: cancellationToken);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="pseudoTableName"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        public static void TruncatePseudoTable(DB2Connection connection,
            string pseudoTableName,
            ITrace trace = null,
            string traceKey = null,
            DB2Transaction transaction = null)
        {
            var dbSetting = connection.GetDbSetting();
            var commandText = Db2Text.GetTruncatePseudoTableSql(pseudoTableName, dbSetting);
            connection.ExecuteNonQuery(commandText, trace: trace, traceKey: traceKey, transaction: transaction);
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
        public static async Task TruncatePseudoTableAsync(DB2Connection connection,
            string pseudoTableName,
            ITrace trace = null,
            string traceKey = null,
            DB2Transaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            var dbSetting = connection.GetDbSetting();
            var commandText = Db2Text.GetTruncatePseudoTableSql(pseudoTableName, dbSetting);
            await connection.ExecuteNonQueryAsync(commandText, trace: trace, traceKey: traceKey, transaction: transaction, cancellationToken: cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="pseudoTableName"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        public static void DropPseudoTable(DB2Connection connection,
            string pseudoTableName,
            ITrace trace = null,
            string traceKey = null,
            DB2Transaction transaction = null)
        {
            var dbSetting = connection.GetDbSetting();
            var commandText = Db2Text.GetDropPseudoTableSql(pseudoTableName, dbSetting);
            connection.ExecuteNonQuery(commandText, trace: trace, traceKey: traceKey, transaction: transaction);
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
        public static async Task DropPseudoTableAsync(DB2Connection connection,
            string pseudoTableName,
            ITrace trace = null,
            string traceKey = null,
            DB2Transaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            var dbSetting = connection.GetDbSetting();
            var commandText = Db2Text.GetDropPseudoTableSql(pseudoTableName, dbSetting);
            await connection.ExecuteNonQueryAsync(commandText, trace: trace, traceKey: traceKey, transaction: transaction, cancellationToken: cancellationToken);
        }

        #endregion

        #region Insert

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
        public static int InsertFromPseudoTableForReturnIdentity<TEntity>(DB2Connection connection,
            string tableName,
            string pseudoTableName,
            IEnumerable<Field> fields,
            Field identityField,
            IList<TEntity> entities,
            ITrace trace = null,
            string traceKey = null,
            DB2Transaction transaction = null)
            where TEntity : class
        {
            var dbSetting = connection.GetDbSetting();
            var commandText = Db2Text.GetInsertFromPseudoTableForReturnIdentitySql(tableName, pseudoTableName, fields, identityField, dbSetting);
            var setter = FunctionCache.GetDataEntityPropertySetterCompiledFunction(typeof(TEntity), identityField);

            using var reader = (DbDataReader)connection.ExecuteReader(commandText, trace: trace, traceKey: traceKey, transaction: transaction);
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
        public static async Task<int> InsertFromPseudoTableForReturnIdentityAsync<TEntity>(DB2Connection connection,
            string tableName,
            string pseudoTableName,
            IEnumerable<Field> fields,
            Field identityField,
            IList<TEntity> entities,
            ITrace trace = null,
            string traceKey = null,
            DB2Transaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            var dbSetting = connection.GetDbSetting();
            var commandText = Db2Text.GetInsertFromPseudoTableForReturnIdentitySql(tableName, pseudoTableName, fields, identityField, dbSetting);
            var setter = FunctionCache.GetDataEntityPropertySetterCompiledFunction(typeof(TEntity), identityField);

            using var reader = (DbDataReader)await connection.ExecuteReaderAsync(commandText, trace: trace, traceKey: traceKey, transaction: transaction, cancellationToken: cancellationToken);
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
        public static int InsertFromPseudoTableForReturnIdentityForDataTable(DB2Connection connection,
            string tableName,
            string pseudoTableName,
            IEnumerable<Field> fields,
            Field identityField,
            IList<DataRow> rows,
            ITrace trace = null,
            string traceKey = null,
            DB2Transaction transaction = null)
        {
            var dbSetting = connection.GetDbSetting();
            var commandText = Db2Text.GetInsertFromPseudoTableForReturnIdentitySql(tableName, pseudoTableName, fields, identityField, dbSetting);

            using var reader = (DbDataReader)connection.ExecuteReader(commandText, trace: trace, traceKey: traceKey, transaction: transaction);
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
        public static async Task<int> InsertFromPseudoTableForReturnIdentityForDataTableAsync(DB2Connection connection,
            string tableName,
            string pseudoTableName,
            IEnumerable<Field> fields,
            Field identityField,
            IList<DataRow> rows,
            ITrace trace = null,
            string traceKey = null,
            DB2Transaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            var dbSetting = connection.GetDbSetting();
            var commandText = Db2Text.GetInsertFromPseudoTableForReturnIdentitySql(tableName, pseudoTableName, fields, identityField, dbSetting);

            using var reader = (DbDataReader)await connection.ExecuteReaderAsync(commandText, trace: trace, traceKey: traceKey, transaction: transaction, cancellationToken: cancellationToken);
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
        public static int MergeFromPseudoTable(DB2Connection connection,
            string tableName,
            string pseudoTableName,
            IEnumerable<Field> fields,
            IEnumerable<Field> qualifiers,
            Field identityField,
            ITrace trace = null,
            string traceKey = null,
            DB2Transaction transaction = null)
        {
            var dbSetting = connection.GetDbSetting();
            var commandText = Db2Text.GetMergeFromPseudoTableSql(tableName, pseudoTableName, fields, qualifiers, identityField, dbSetting);
            return connection.ExecuteNonQuery(commandText, trace: trace, traceKey: traceKey, transaction: transaction);
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
        public static async Task<int> MergeFromPseudoTableAsync(DB2Connection connection,
            string tableName,
            string pseudoTableName,
            IEnumerable<Field> fields,
            IEnumerable<Field> qualifiers,
            Field identityField,
            ITrace trace = null,
            string traceKey = null,
            DB2Transaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            var dbSetting = connection.GetDbSetting();
            var commandText = Db2Text.GetMergeFromPseudoTableSql(tableName, pseudoTableName, fields, qualifiers, identityField, dbSetting);
            return await connection.ExecuteNonQueryAsync(commandText, trace: trace, traceKey: traceKey, transaction: transaction, cancellationToken: cancellationToken);
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
        public static int MergeFromPseudoTableForReturnIdentity<TEntity>(DB2Connection connection,
            string tableName,
            string pseudoTableName,
            IEnumerable<Field> fields,
            Field identityField,
            IEnumerable<Field> qualifiers,
            IList<TEntity> entities,
            ITrace trace = null,
            string traceKey = null,
            DB2Transaction transaction = null)
            where TEntity : class
        {
            var dbSetting = connection.GetDbSetting();
            var setter = FunctionCache.GetDataEntityPropertySetterCompiledFunction(typeof(TEntity), identityField);
            var result = 0;
            var unmatchedRowOrders = new List<long>();
            var snapshotSql = Db2Text.GetMergeMatchSnapshotSql(tableName, pseudoTableName, identityField, qualifiers, dbSetting);
            using (var snapshotReader = (DbDataReader)connection.ExecuteReader(snapshotSql, trace: trace, traceKey: traceKey, transaction: transaction))
            {
                while (snapshotReader.Read())
                {
                    var rowOrder = Convert.ToInt64(snapshotReader.GetValue(0));
                    if (snapshotReader.IsDBNull(1))
                    {
                        unmatchedRowOrders.Add(rowOrder);
                    }
                    else
                    {
                        setter?.Invoke(entities[(int)(rowOrder - 1)], Converter.DbNullToNull(snapshotReader.GetValue(1)));
                        result++;
                    }
                }
            }

            // Step 2: update the matched rows.
            if (result > 0)
            {
                var updateSql = Db2Text.GetMergeUpdateOnlySql(tableName, pseudoTableName, fields, identityField, qualifiers, dbSetting);
                if (updateSql != null)
                {
                    connection.ExecuteNonQuery(updateSql, trace: trace, traceKey: traceKey, transaction: transaction);
                }
            }

            if (unmatchedRowOrders.Count > 0)
            {
                var insertSql = Db2Text.GetMergeInsertOnlyForReturnIdentitySql(tableName, pseudoTableName, fields, identityField, qualifiers, dbSetting);
                using var insertReader = (DbDataReader)connection.ExecuteReader(insertSql, trace: trace, traceKey: traceKey, transaction: transaction);
                var index = 0;
                while (insertReader.Read())
                {
                    setter?.Invoke(entities[(int)(unmatchedRowOrders[index] - 1)], Converter.DbNullToNull(insertReader.GetValue(0)));
                    index++;
                    result++;
                }
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
        public static async Task<int> MergeFromPseudoTableForReturnIdentityAsync<TEntity>(DB2Connection connection,
            string tableName,
            string pseudoTableName,
            IEnumerable<Field> fields,
            Field identityField,
            IEnumerable<Field> qualifiers,
            IList<TEntity> entities,
            ITrace trace = null,
            string traceKey = null,
            DB2Transaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            var dbSetting = connection.GetDbSetting();
            var setter = FunctionCache.GetDataEntityPropertySetterCompiledFunction(typeof(TEntity), identityField);
            var result = 0;

            var unmatchedRowOrders = new List<long>();
            var snapshotSql = Db2Text.GetMergeMatchSnapshotSql(tableName, pseudoTableName, identityField, qualifiers, dbSetting);
            using (var snapshotReader = (DbDataReader)await connection.ExecuteReaderAsync(snapshotSql, trace: trace, traceKey: traceKey, transaction: transaction, cancellationToken: cancellationToken))
            {
                while (await snapshotReader.ReadAsync(cancellationToken))
                {
                    var rowOrder = Convert.ToInt64(snapshotReader.GetValue(0));
                    if (snapshotReader.IsDBNull(1))
                    {
                        unmatchedRowOrders.Add(rowOrder);
                    }
                    else
                    {
                        setter?.Invoke(entities[(int)(rowOrder - 1)], Converter.DbNullToNull(snapshotReader.GetValue(1)));
                        result++;
                    }
                }
            }

            if (result > 0)
            {
                var updateSql = Db2Text.GetMergeUpdateOnlySql(tableName, pseudoTableName, fields, identityField, qualifiers, dbSetting);
                if (updateSql != null)
                {
                    await connection.ExecuteNonQueryAsync(updateSql, trace: trace, traceKey: traceKey, transaction: transaction, cancellationToken: cancellationToken);
                }
            }

            if (unmatchedRowOrders.Count > 0)
            {
                var insertSql = Db2Text.GetMergeInsertOnlyForReturnIdentitySql(tableName, pseudoTableName, fields, identityField, qualifiers, dbSetting);
                using var insertReader = (DbDataReader)await connection.ExecuteReaderAsync(insertSql, trace: trace, traceKey: traceKey, transaction: transaction, cancellationToken: cancellationToken);
                var index = 0;
                while (await insertReader.ReadAsync(cancellationToken))
                {
                    setter?.Invoke(entities[(int)(unmatchedRowOrders[index] - 1)], Converter.DbNullToNull(insertReader.GetValue(0)));
                    index++;
                    result++;
                }
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
        public static int MergeFromPseudoTableForReturnIdentityForDataTable(DB2Connection connection,
            string tableName,
            string pseudoTableName,
            IEnumerable<Field> fields,
            Field identityField,
            IEnumerable<Field> qualifiers,
            IList<DataRow> rows,
            ITrace trace = null,
            string traceKey = null,
            DB2Transaction transaction = null)
        {
            var dbSetting = connection.GetDbSetting();
            var result = 0;

            var unmatchedRowOrders = new List<long>();
            var snapshotSql = Db2Text.GetMergeMatchSnapshotSql(tableName, pseudoTableName, identityField, qualifiers, dbSetting);
            using (var snapshotReader = (DbDataReader)connection.ExecuteReader(snapshotSql, trace: trace, traceKey: traceKey, transaction: transaction))
            {
                while (snapshotReader.Read())
                {
                    var rowOrder = Convert.ToInt64(snapshotReader.GetValue(0));
                    if (snapshotReader.IsDBNull(1))
                    {
                        unmatchedRowOrders.Add(rowOrder);
                    }
                    else
                    {
                        rows[(int)(rowOrder - 1)][identityField.Name] = Converter.DbNullToNull(snapshotReader.GetValue(1));
                        result++;
                    }
                }
            }

            if (result > 0)
            {
                var updateSql = Db2Text.GetMergeUpdateOnlySql(tableName, pseudoTableName, fields, identityField, qualifiers, dbSetting);
                if (updateSql != null)
                {
                    connection.ExecuteNonQuery(updateSql, trace: trace, traceKey: traceKey, transaction: transaction);
                }
            }

            if (unmatchedRowOrders.Count > 0)
            {
                var insertSql = Db2Text.GetMergeInsertOnlyForReturnIdentitySql(tableName, pseudoTableName, fields, identityField, qualifiers, dbSetting);
                using var insertReader = (DbDataReader)connection.ExecuteReader(insertSql, trace: trace, traceKey: traceKey, transaction: transaction);
                var index = 0;
                while (insertReader.Read())
                {
                    rows[(int)(unmatchedRowOrders[index] - 1)][identityField.Name] = Converter.DbNullToNull(insertReader.GetValue(0));
                    index++;
                    result++;
                }
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
        public static async Task<int> MergeFromPseudoTableForReturnIdentityForDataTableAsync(DB2Connection connection,
            string tableName,
            string pseudoTableName,
            IEnumerable<Field> fields,
            Field identityField,
            IEnumerable<Field> qualifiers,
            IList<DataRow> rows,
            ITrace trace = null,
            string traceKey = null,
            DB2Transaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            var dbSetting = connection.GetDbSetting();
            var result = 0;

            var unmatchedRowOrders = new List<long>();
            var snapshotSql = Db2Text.GetMergeMatchSnapshotSql(tableName, pseudoTableName, identityField, qualifiers, dbSetting);
            using (var snapshotReader = (DbDataReader)await connection.ExecuteReaderAsync(snapshotSql, trace: trace, traceKey: traceKey, transaction: transaction, cancellationToken: cancellationToken))
            {
                while (await snapshotReader.ReadAsync(cancellationToken))
                {
                    var rowOrder = Convert.ToInt64(snapshotReader.GetValue(0));
                    if (snapshotReader.IsDBNull(1))
                    {
                        unmatchedRowOrders.Add(rowOrder);
                    }
                    else
                    {
                        rows[(int)(rowOrder - 1)][identityField.Name] = Converter.DbNullToNull(snapshotReader.GetValue(1));
                        result++;
                    }
                }
            }

            if (result > 0)
            {
                var updateSql = Db2Text.GetMergeUpdateOnlySql(tableName, pseudoTableName, fields, identityField, qualifiers, dbSetting);
                if (updateSql != null)
                {
                    await connection.ExecuteNonQueryAsync(updateSql, trace: trace, traceKey: traceKey, transaction: transaction, cancellationToken: cancellationToken);
                }
            }

            if (unmatchedRowOrders.Count > 0)
            {
                var insertSql = Db2Text.GetMergeInsertOnlyForReturnIdentitySql(tableName, pseudoTableName, fields, identityField, qualifiers, dbSetting);
                using var insertReader = (DbDataReader)await connection.ExecuteReaderAsync(insertSql, trace: trace, traceKey: traceKey, transaction: transaction, cancellationToken: cancellationToken);
                var index = 0;
                while (await insertReader.ReadAsync(cancellationToken))
                {
                    rows[(int)(unmatchedRowOrders[index] - 1)][identityField.Name] = Converter.DbNullToNull(insertReader.GetValue(0));
                    index++;
                    result++;
                }
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
        public static int UpdateFromPseudoTable(DB2Connection connection,
            string tableName,
            string pseudoTableName,
            IEnumerable<Field> fields,
            IEnumerable<Field> qualifiers,
            ITrace trace = null,
            string traceKey = null,
            DB2Transaction transaction = null)
        {
            var dbSetting = connection.GetDbSetting();
            var commandText = Db2Text.GetUpdateFromPseudoTableSql(tableName, pseudoTableName, fields, qualifiers, dbSetting);
            return connection.ExecuteNonQuery(commandText, trace: trace, traceKey: traceKey, transaction: transaction);
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
        public static async Task<int> UpdateFromPseudoTableAsync(DB2Connection connection,
            string tableName,
            string pseudoTableName,
            IEnumerable<Field> fields,
            IEnumerable<Field> qualifiers,
            ITrace trace = null,
            string traceKey = null,
            DB2Transaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            var dbSetting = connection.GetDbSetting();
            var commandText = Db2Text.GetUpdateFromPseudoTableSql(tableName, pseudoTableName, fields, qualifiers, dbSetting);
            return await connection.ExecuteNonQueryAsync(commandText, trace: trace, traceKey: traceKey, transaction: transaction, cancellationToken: cancellationToken);
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
        public static int DeleteFromPseudoTable(DB2Connection connection,
            string tableName,
            string pseudoTableName,
            IEnumerable<Field> qualifiers,
            ITrace trace = null,
            string traceKey = null,
            DB2Transaction transaction = null)
        {
            var dbSetting = connection.GetDbSetting();
            var commandText = Db2Text.GetDeleteFromPseudoTableSql(tableName, pseudoTableName, qualifiers, dbSetting);
            return connection.ExecuteNonQuery(commandText, trace: trace, traceKey: traceKey, transaction: transaction);
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
        public static async Task<int> DeleteFromPseudoTableAsync(DB2Connection connection,
            string tableName,
            string pseudoTableName,
            IEnumerable<Field> qualifiers,
            ITrace trace = null,
            string traceKey = null,
            DB2Transaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            var dbSetting = connection.GetDbSetting();
            var commandText = Db2Text.GetDeleteFromPseudoTableSql(tableName, pseudoTableName, qualifiers, dbSetting);
            return await connection.ExecuteNonQueryAsync(commandText, trace: trace, traceKey: traceKey, transaction: transaction, cancellationToken: cancellationToken);
        }

        #endregion
    }
}
