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
    /// 
    /// </summary>
    internal static class FirebirdExecution
    {
        #region Shared

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="pseudoTableName"></param>
        /// <param name="fields"></param>
        /// <param name="dbFields"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="pseudoTableName"></param>
        /// <param name="fields"></param>
        /// <param name="dbFields"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="pseudoTableName"></param>
        /// <param name="qualifiers"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="pseudoTableName"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        public static void DropPseudoTable(FbConnection connection,
            string pseudoTableName,
            ITrace trace = null,
            string traceKey = null,
            FbTransaction transaction = null)
        {
            var commandText = FirebirdText.GetDropPseudoTableSql(pseudoTableName, connection.GetDbSetting());
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="commandText"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private static FbCommand CreateReaderCommand(FbConnection connection,
            string commandText,
            FbTransaction transaction) =>
            (FbCommand)connection.CreateCommand(commandText, CommandType.Text, null, transaction);

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

            using var command = CreateReaderCommand(connection, commandText, transaction);
            using var reader = command.ExecuteReader();
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
        /// <param name="entities"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
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

            using var command = CreateReaderCommand(connection, commandText, transaction);
            using var reader = await command.ExecuteReaderAsync(cancellationToken);
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
        /// <param name="rows"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
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

            using var command = CreateReaderCommand(connection, commandText, transaction);
            using var reader = command.ExecuteReader();
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

            using var command = CreateReaderCommand(connection, commandText, transaction);
            using var reader = await command.ExecuteReaderAsync(cancellationToken);
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
            connection.ExecuteNonQuery(commandText, trace: trace, traceKey: traceKey, transaction: transaction);
            var countText = FirebirdText.GetPseudoTableRowCountSql(pseudoTableName, connection.GetDbSetting());
            return connection.ExecuteScalar<int>(countText, transaction: transaction);
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
            await connection.ExecuteNonQueryAsync(commandText, trace: trace, traceKey: traceKey, transaction: transaction, cancellationToken: cancellationToken);
            var countText = FirebirdText.GetPseudoTableRowCountSql(pseudoTableName, connection.GetDbSetting());
            return await connection.ExecuteScalarAsync<int>(countText, transaction: transaction, cancellationToken: cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="pseudoTableName"></param>
        /// <param name="fields"></param>
        /// <param name="qualifiers"></param>
        /// <param name="identityField"></param>
        /// <param name="entities"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
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

            using var command = CreateReaderCommand(connection, commandText, transaction);
            using var reader = command.ExecuteReader();
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
        /// <param name="qualifiers"></param>
        /// <param name="identityField"></param>
        /// <param name="entities"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
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

            using var command = CreateReaderCommand(connection, commandText, transaction);
            using var reader = await command.ExecuteReaderAsync(cancellationToken);
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
        /// <param name="qualifiers"></param>
        /// <param name="identityField"></param>
        /// <param name="rows"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
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

            using var command = CreateReaderCommand(connection, commandText, transaction);
            using var reader = command.ExecuteReader();
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
        /// <param name="qualifiers"></param>
        /// <param name="identityField"></param>
        /// <param name="rows"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
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

            using var command = CreateReaderCommand(connection, commandText, transaction);
            using var reader = await command.ExecuteReaderAsync(cancellationToken);
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
