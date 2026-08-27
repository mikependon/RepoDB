using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using FirebirdSql.Data.FirebirdClient;
using RepoDb.Enumerations.Firebird;
using RepoDb.Extensions;
using RepoDb.Interfaces;

namespace RepoDb.Firebird.BulkOperations.Extensions
{
    /// <summary>
    /// Executes the SQL text built by <see cref="FirebirdText"/> against the database.
    /// </summary>
    internal static class FirebirdExecution
    {
        #region Shared

        public static void CreatePseudoTable(FbConnection connection,
            string pseudoTableName,
            IEnumerable<Field> fields,
            DbFieldCollection dbFields,
            FirebirdBulkImportPseudoTableType pseudoTableType,
            ITrace trace = null,
            string traceKey = null,
            FbTransaction transaction = null)
        {
            var commandText = FirebirdText.GetCreatePseudoTableSql(pseudoTableName, fields, dbFields, pseudoTableType, connection.GetDbSetting());
            connection.ExecuteNonQuery(commandText, trace: trace, traceKey: traceKey, transaction: transaction);
        }

        public static async Task CreatePseudoTableAsync(FbConnection connection,
            string pseudoTableName,
            IEnumerable<Field> fields,
            DbFieldCollection dbFields,
            FirebirdBulkImportPseudoTableType pseudoTableType,
            ITrace trace = null,
            string traceKey = null,
            FbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            var commandText = FirebirdText.GetCreatePseudoTableSql(pseudoTableName, fields, dbFields, pseudoTableType, connection.GetDbSetting());
            await connection.ExecuteNonQueryAsync(commandText, trace: trace, traceKey: traceKey, transaction: transaction, cancellationToken: cancellationToken);
        }

        public static void CreatePseudoTableIndex(FbConnection connection,
            string pseudoTableName,
            IEnumerable<Field> qualifiers,
            ITrace trace = null,
            string traceKey = null,
            FbTransaction transaction = null)
        {
            var commandText = FirebirdText.GetCreatePseudoTableIndexSql(pseudoTableName, qualifiers, connection.GetDbSetting());
            connection.ExecuteNonQuery(commandText, trace: trace, traceKey: traceKey, transaction: transaction);
        }

        public static async Task CreatePseudoTableIndexAsync(FbConnection connection,
            string pseudoTableName,
            IEnumerable<Field> qualifiers,
            ITrace trace = null,
            string traceKey = null,
            FbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            var commandText = FirebirdText.GetCreatePseudoTableIndexSql(pseudoTableName, qualifiers, connection.GetDbSetting());
            await connection.ExecuteNonQueryAsync(commandText, trace: trace, traceKey: traceKey, transaction: transaction, cancellationToken: cancellationToken);
        }

        public static void DropPseudoTable(FbConnection connection,
            string pseudoTableName,
            ITrace trace = null,
            string traceKey = null,
            FbTransaction transaction = null)
        {
            var commandText = FirebirdText.GetDropPseudoTableSql(pseudoTableName, connection.GetDbSetting());
            connection.ExecuteNonQuery(commandText, trace: trace, traceKey: traceKey, transaction: transaction);
        }

        public static async Task DropPseudoTableAsync(FbConnection connection,
            string pseudoTableName,
            ITrace trace = null,
            string traceKey = null,
            FbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            var commandText = FirebirdText.GetDropPseudoTableSql(pseudoTableName, connection.GetDbSetting());
            await connection.ExecuteNonQueryAsync(commandText, trace: trace, traceKey: traceKey, transaction: transaction, cancellationToken: cancellationToken);
        }

        #endregion

        #region Insert

        /// <summary>
        /// Runs the <c>EXECUTE BLOCK</c> loop built by <see cref="FirebirdText.GetInsertFromPseudoTableForReturnIdentitySql"/>
        /// and assigns each yielded identity value back onto the matching source entity, by position (row
        /// order is guaranteed by the pseudo table's client-assigned row-order column).
        /// </summary>
        public static int InsertFromPseudoTableForReturnIdentity<TEntity>(FbConnection connection,
            string tableName,
            string pseudoTableName,
            IEnumerable<Field> fields,
            Field identityField,
            IList<TEntity> entities,
            ITrace trace = null,
            string traceKey = null,
            FbTransaction transaction = null)
            where TEntity : class
        {
            var commandText = FirebirdText.GetInsertFromPseudoTableForReturnIdentitySql(tableName, pseudoTableName, fields, identityField, connection.GetDbSetting());
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

        public static async Task<int> InsertFromPseudoTableForReturnIdentityAsync<TEntity>(FbConnection connection,
            string tableName,
            string pseudoTableName,
            IEnumerable<Field> fields,
            Field identityField,
            IList<TEntity> entities,
            ITrace trace = null,
            string traceKey = null,
            FbTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            var commandText = FirebirdText.GetInsertFromPseudoTableForReturnIdentitySql(tableName, pseudoTableName, fields, identityField, connection.GetDbSetting());
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

        public static int InsertFromPseudoTableForReturnIdentityForDataTable(FbConnection connection,
            string tableName,
            string pseudoTableName,
            IEnumerable<Field> fields,
            Field identityField,
            IList<DataRow> rows,
            ITrace trace = null,
            string traceKey = null,
            FbTransaction transaction = null)
        {
            var commandText = FirebirdText.GetInsertFromPseudoTableForReturnIdentitySql(tableName, pseudoTableName, fields, identityField, connection.GetDbSetting());

            using var reader = (DbDataReader)connection.ExecuteReader(commandText, trace: trace, traceKey: traceKey, transaction: transaction);
            var result = 0;

            while (reader.Read())
            {
                rows[result][identityField.Name] = Converter.DbNullToNull(reader.GetValue(0));
                result++;
            }

            return result;
        }

        public static async Task<int> InsertFromPseudoTableForReturnIdentityForDataTableAsync(FbConnection connection,
            string tableName,
            string pseudoTableName,
            IEnumerable<Field> fields,
            Field identityField,
            IList<DataRow> rows,
            ITrace trace = null,
            string traceKey = null,
            FbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            var commandText = FirebirdText.GetInsertFromPseudoTableForReturnIdentitySql(tableName, pseudoTableName, fields, identityField, connection.GetDbSetting());

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
        /// Applies a no-return-identity merge (see <see cref="FirebirdText.GetMergeFromPseudoTableSql"/> for
        /// the three possible statement shapes) and returns the number of rows processed.
        /// </summary>
        public static int MergeFromPseudoTable(FbConnection connection,
            string tableName,
            string pseudoTableName,
            IEnumerable<Field> fields,
            IEnumerable<Field> qualifiers,
            Field identityField,
            ITrace trace = null,
            string traceKey = null,
            FbTransaction transaction = null)
        {
            var commandText = FirebirdText.GetMergeFromPseudoTableSql(tableName, pseudoTableName, fields, qualifiers, identityField, false, connection.GetDbSetting());
            return connection.ExecuteNonQuery(commandText, trace: trace, traceKey: traceKey, transaction: transaction);
        }

        public static async Task<int> MergeFromPseudoTableAsync(FbConnection connection,
            string tableName,
            string pseudoTableName,
            IEnumerable<Field> fields,
            IEnumerable<Field> qualifiers,
            Field identityField,
            ITrace trace = null,
            string traceKey = null,
            FbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            var commandText = FirebirdText.GetMergeFromPseudoTableSql(tableName, pseudoTableName, fields, qualifiers, identityField, false, connection.GetDbSetting());
            return await connection.ExecuteNonQueryAsync(commandText, trace: trace, traceKey: traceKey, transaction: transaction, cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Applies a return-identity merge and assigns each yielded identity value back onto the matching
        /// source entity, by position.
        /// </summary>
        public static int MergeFromPseudoTableForReturnIdentity<TEntity>(FbConnection connection,
            string tableName,
            string pseudoTableName,
            IEnumerable<Field> fields,
            IEnumerable<Field> qualifiers,
            Field identityField,
            IList<TEntity> entities,
            ITrace trace = null,
            string traceKey = null,
            FbTransaction transaction = null)
            where TEntity : class
        {
            var commandText = FirebirdText.GetMergeFromPseudoTableSql(tableName, pseudoTableName, fields, qualifiers, identityField, true, connection.GetDbSetting());
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

        public static async Task<int> MergeFromPseudoTableForReturnIdentityAsync<TEntity>(FbConnection connection,
            string tableName,
            string pseudoTableName,
            IEnumerable<Field> fields,
            IEnumerable<Field> qualifiers,
            Field identityField,
            IList<TEntity> entities,
            ITrace trace = null,
            string traceKey = null,
            FbTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            var commandText = FirebirdText.GetMergeFromPseudoTableSql(tableName, pseudoTableName, fields, qualifiers, identityField, true, connection.GetDbSetting());
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

        public static int MergeFromPseudoTableForReturnIdentityForDataTable(FbConnection connection,
            string tableName,
            string pseudoTableName,
            IEnumerable<Field> fields,
            IEnumerable<Field> qualifiers,
            Field identityField,
            IList<DataRow> rows,
            ITrace trace = null,
            string traceKey = null,
            FbTransaction transaction = null)
        {
            var commandText = FirebirdText.GetMergeFromPseudoTableSql(tableName, pseudoTableName, fields, qualifiers, identityField, true, connection.GetDbSetting());

            using var reader = (DbDataReader)connection.ExecuteReader(commandText, trace: trace, traceKey: traceKey, transaction: transaction);
            var result = 0;

            while (reader.Read())
            {
                rows[result][identityField.Name] = Converter.DbNullToNull(reader.GetValue(0));
                result++;
            }

            return result;
        }

        public static async Task<int> MergeFromPseudoTableForReturnIdentityForDataTableAsync(FbConnection connection,
            string tableName,
            string pseudoTableName,
            IEnumerable<Field> fields,
            IEnumerable<Field> qualifiers,
            Field identityField,
            IList<DataRow> rows,
            ITrace trace = null,
            string traceKey = null,
            FbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            var commandText = FirebirdText.GetMergeFromPseudoTableSql(tableName, pseudoTableName, fields, qualifiers, identityField, true, connection.GetDbSetting());

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

        #region Update

        public static int UpdateFromPseudoTable(FbConnection connection,
            string tableName,
            string pseudoTableName,
            IEnumerable<Field> fields,
            IEnumerable<Field> qualifiers,
            ITrace trace = null,
            string traceKey = null,
            FbTransaction transaction = null)
        {
            var commandText = FirebirdText.GetUpdateFromPseudoTableSql(tableName, pseudoTableName, fields, qualifiers, connection.GetDbSetting());
            return connection.ExecuteNonQuery(commandText, trace: trace, traceKey: traceKey, transaction: transaction);
        }

        public static async Task<int> UpdateFromPseudoTableAsync(FbConnection connection,
            string tableName,
            string pseudoTableName,
            IEnumerable<Field> fields,
            IEnumerable<Field> qualifiers,
            ITrace trace = null,
            string traceKey = null,
            FbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            var commandText = FirebirdText.GetUpdateFromPseudoTableSql(tableName, pseudoTableName, fields, qualifiers, connection.GetDbSetting());
            return await connection.ExecuteNonQueryAsync(commandText, trace: trace, traceKey: traceKey, transaction: transaction, cancellationToken: cancellationToken);
        }

        #endregion

        #region Delete

        public static int DeleteFromPseudoTable(FbConnection connection,
            string tableName,
            string pseudoTableName,
            IEnumerable<Field> qualifiers,
            ITrace trace = null,
            string traceKey = null,
            FbTransaction transaction = null)
        {
            var commandText = FirebirdText.GetDeleteFromPseudoTableSql(tableName, pseudoTableName, qualifiers, connection.GetDbSetting());
            return connection.ExecuteNonQuery(commandText, trace: trace, traceKey: traceKey, transaction: transaction);
        }

        public static async Task<int> DeleteFromPseudoTableAsync(FbConnection connection,
            string tableName,
            string pseudoTableName,
            IEnumerable<Field> qualifiers,
            ITrace trace = null,
            string traceKey = null,
            FbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            var commandText = FirebirdText.GetDeleteFromPseudoTableSql(tableName, pseudoTableName, qualifiers, connection.GetDbSetting());
            return await connection.ExecuteNonQueryAsync(commandText, trace: trace, traceKey: traceKey, transaction: transaction, cancellationToken: cancellationToken);
        }

        #endregion
    }
}
