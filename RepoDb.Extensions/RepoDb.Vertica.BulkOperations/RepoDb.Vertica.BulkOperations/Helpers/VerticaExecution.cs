#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Vertica.Data.VerticaClient;
using RepoDb.Enumerations.Vertica;
using RepoDb.Extensions;
using RepoDb.Interfaces;

namespace RepoDb.Vertica.BulkOperations.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    internal static class VerticaExecution
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
        public static void CreatePseudoTable(VerticaConnection connection,
            string pseudoTableName,
            IEnumerable<Field> fields,
            DbFieldCollection dbFields,
            VerticaBulkImportPseudoTableType pseudoTableType,
            ITrace trace = null,
            string traceKey = null,
            VerticaTransaction transaction = null)
        {
            var commandText = VerticaText.GetCreatePseudoTableSql(pseudoTableName, fields, dbFields, pseudoTableType, connection.GetDbSetting());
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
        public static async Task CreatePseudoTableAsync(VerticaConnection connection,
            string pseudoTableName,
            IEnumerable<Field> fields,
            DbFieldCollection dbFields,
            VerticaBulkImportPseudoTableType pseudoTableType,
            ITrace trace = null,
            string traceKey = null,
            VerticaTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            var commandText = VerticaText.GetCreatePseudoTableSql(pseudoTableName, fields, dbFields, pseudoTableType, connection.GetDbSetting());
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
        public static void CreatePseudoTableIndex(VerticaConnection connection,
            string pseudoTableName,
            IEnumerable<Field> qualifiers,
            ITrace trace = null,
            string traceKey = null,
            VerticaTransaction transaction = null)
        {
            var commandText = VerticaText.GetCreatePseudoTableIndexSql(pseudoTableName, qualifiers, connection.GetDbSetting());
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
        public static async Task CreatePseudoTableIndexAsync(VerticaConnection connection,
            string pseudoTableName,
            IEnumerable<Field> qualifiers,
            ITrace trace = null,
            string traceKey = null,
            VerticaTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            var commandText = VerticaText.GetCreatePseudoTableIndexSql(pseudoTableName, qualifiers, connection.GetDbSetting());
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
        public static void DropPseudoTable(VerticaConnection connection,
            string pseudoTableName,
            ITrace trace = null,
            string traceKey = null,
            VerticaTransaction transaction = null)
        {
            var commandText = VerticaText.GetDropPseudoTableSql(pseudoTableName, connection.GetDbSetting());
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
        public static async Task DropPseudoTableAsync(VerticaConnection connection,
            string pseudoTableName,
            ITrace trace = null,
            string traceKey = null,
            VerticaTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            var commandText = VerticaText.GetDropPseudoTableSql(pseudoTableName, connection.GetDbSetting());
            await connection.ExecuteNonQueryAsync(commandText, trace: trace, traceKey: traceKey, transaction: transaction, cancellationToken: cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="commandText"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private static VerticaCommand CreateReaderCommand(VerticaConnection connection,
            string commandText,
            VerticaTransaction transaction) =>
            (VerticaCommand)connection.CreateCommand(commandText, CommandType.Text, null, transaction);

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
        public static int InsertFromPseudoTableForReturnIdentity<TEntity>(VerticaConnection connection,
            string tableName,
            string pseudoTableName,
            IEnumerable<Field> fields,
            Field identityField,
            IList<TEntity> entities,
            ITrace trace = null,
            string traceKey = null,
            VerticaTransaction transaction = null)
            where TEntity : class
        {
            var commandText = VerticaText.GetInsertFromPseudoTableForReturnIdentitySql(tableName, pseudoTableName, fields, identityField, connection.GetDbSetting());
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
        public static async Task<int> InsertFromPseudoTableForReturnIdentityAsync<TEntity>(VerticaConnection connection,
            string tableName,
            string pseudoTableName,
            IEnumerable<Field> fields,
            Field identityField,
            IList<TEntity> entities,
            ITrace trace = null,
            string traceKey = null,
            VerticaTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            var commandText = VerticaText.GetInsertFromPseudoTableForReturnIdentitySql(tableName, pseudoTableName, fields, identityField, connection.GetDbSetting());
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
        public static int InsertFromPseudoTableForReturnIdentityForDataTable(VerticaConnection connection,
            string tableName,
            string pseudoTableName,
            IEnumerable<Field> fields,
            Field identityField,
            IList<DataRow> rows,
            ITrace trace = null,
            string traceKey = null,
            VerticaTransaction transaction = null)
        {
            var commandText = VerticaText.GetInsertFromPseudoTableForReturnIdentitySql(tableName, pseudoTableName, fields, identityField, connection.GetDbSetting());

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
        public static async Task<int> InsertFromPseudoTableForReturnIdentityForDataTableAsync(VerticaConnection connection,
            string tableName,
            string pseudoTableName,
            IEnumerable<Field> fields,
            Field identityField,
            IList<DataRow> rows,
            ITrace trace = null,
            string traceKey = null,
            VerticaTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            var commandText = VerticaText.GetInsertFromPseudoTableForReturnIdentitySql(tableName, pseudoTableName, fields, identityField, connection.GetDbSetting());

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
        public static int MergeFromPseudoTable(VerticaConnection connection,
            string tableName,
            string pseudoTableName,
            IEnumerable<Field> fields,
            IEnumerable<Field> qualifiers,
            Field identityField,
            ITrace trace = null,
            string traceKey = null,
            VerticaTransaction transaction = null)
        {
            var commandText = VerticaText.GetMergeFromPseudoTableSql(tableName, pseudoTableName, fields, qualifiers, identityField, false, connection.GetDbSetting());
            connection.ExecuteNonQuery(commandText, trace: trace, traceKey: traceKey, transaction: transaction);
            var countText = VerticaText.GetPseudoTableRowCountSql(pseudoTableName, connection.GetDbSetting());
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
        public static async Task<int> MergeFromPseudoTableAsync(VerticaConnection connection,
            string tableName,
            string pseudoTableName,
            IEnumerable<Field> fields,
            IEnumerable<Field> qualifiers,
            Field identityField,
            ITrace trace = null,
            string traceKey = null,
            VerticaTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            var commandText = VerticaText.GetMergeFromPseudoTableSql(tableName, pseudoTableName, fields, qualifiers, identityField, false, connection.GetDbSetting());
            await connection.ExecuteNonQueryAsync(commandText, trace: trace, traceKey: traceKey, transaction: transaction, cancellationToken: cancellationToken);
            var countText = VerticaText.GetPseudoTableRowCountSql(pseudoTableName, connection.GetDbSetting());
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
        public static int MergeFromPseudoTableForReturnIdentity<TEntity>(VerticaConnection connection,
            string tableName,
            string pseudoTableName,
            IEnumerable<Field> fields,
            IEnumerable<Field> qualifiers,
            Field identityField,
            IList<TEntity> entities,
            ITrace trace = null,
            string traceKey = null,
            VerticaTransaction transaction = null)
            where TEntity : class
        {
            var commandText = VerticaText.GetMergeFromPseudoTableSql(tableName, pseudoTableName, fields, qualifiers, identityField, true, connection.GetDbSetting());
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
        public static async Task<int> MergeFromPseudoTableForReturnIdentityAsync<TEntity>(VerticaConnection connection,
            string tableName,
            string pseudoTableName,
            IEnumerable<Field> fields,
            IEnumerable<Field> qualifiers,
            Field identityField,
            IList<TEntity> entities,
            ITrace trace = null,
            string traceKey = null,
            VerticaTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            var commandText = VerticaText.GetMergeFromPseudoTableSql(tableName, pseudoTableName, fields, qualifiers, identityField, true, connection.GetDbSetting());
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
        public static int MergeFromPseudoTableForReturnIdentityForDataTable(VerticaConnection connection,
            string tableName,
            string pseudoTableName,
            IEnumerable<Field> fields,
            IEnumerable<Field> qualifiers,
            Field identityField,
            IList<DataRow> rows,
            ITrace trace = null,
            string traceKey = null,
            VerticaTransaction transaction = null)
        {
            var commandText = VerticaText.GetMergeFromPseudoTableSql(tableName, pseudoTableName, fields, qualifiers, identityField, true, connection.GetDbSetting());

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
        public static async Task<int> MergeFromPseudoTableForReturnIdentityForDataTableAsync(VerticaConnection connection,
            string tableName,
            string pseudoTableName,
            IEnumerable<Field> fields,
            IEnumerable<Field> qualifiers,
            Field identityField,
            IList<DataRow> rows,
            ITrace trace = null,
            string traceKey = null,
            VerticaTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            var commandText = VerticaText.GetMergeFromPseudoTableSql(tableName, pseudoTableName, fields, qualifiers, identityField, true, connection.GetDbSetting());

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
        public static int UpdateFromPseudoTable(VerticaConnection connection,
            string tableName,
            string pseudoTableName,
            IEnumerable<Field> fields,
            IEnumerable<Field> qualifiers,
            ITrace trace = null,
            string traceKey = null,
            VerticaTransaction transaction = null)
        {
            var commandText = VerticaText.GetUpdateFromPseudoTableSql(tableName, pseudoTableName, fields, qualifiers, connection.GetDbSetting());
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
        public static async Task<int> UpdateFromPseudoTableAsync(VerticaConnection connection,
            string tableName,
            string pseudoTableName,
            IEnumerable<Field> fields,
            IEnumerable<Field> qualifiers,
            ITrace trace = null,
            string traceKey = null,
            VerticaTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            var commandText = VerticaText.GetUpdateFromPseudoTableSql(tableName, pseudoTableName, fields, qualifiers, connection.GetDbSetting());
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
        public static int DeleteFromPseudoTable(VerticaConnection connection,
            string tableName,
            string pseudoTableName,
            IEnumerable<Field> qualifiers,
            ITrace trace = null,
            string traceKey = null,
            VerticaTransaction transaction = null)
        {
            var commandText = VerticaText.GetDeleteFromPseudoTableSql(tableName, pseudoTableName, qualifiers, connection.GetDbSetting());
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
        public static async Task<int> DeleteFromPseudoTableAsync(VerticaConnection connection,
            string tableName,
            string pseudoTableName,
            IEnumerable<Field> qualifiers,
            ITrace trace = null,
            string traceKey = null,
            VerticaTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            var commandText = VerticaText.GetDeleteFromPseudoTableSql(tableName, pseudoTableName, qualifiers, connection.GetDbSetting());
            return await connection.ExecuteNonQueryAsync(commandText, trace: trace, traceKey: traceKey, transaction: transaction, cancellationToken: cancellationToken);
        }

        #endregion
    }
}
