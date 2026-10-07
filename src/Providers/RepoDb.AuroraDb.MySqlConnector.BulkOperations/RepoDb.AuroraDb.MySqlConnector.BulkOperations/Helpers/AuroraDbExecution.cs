#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.Diagnostics.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using RepoDb.Connector.AuroraDb.MySqlConnector;
using RepoDb.Enumerations.AuroraDb;
using RepoDb.Extensions;
using RepoDb.Interfaces;

namespace RepoDb.AuroraDb.MySqlConnector.BulkOperations.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    internal static class AuroraDbExecution
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
        [UnconditionalSuppressMessage("Trimming", "IL2026:RequiresUnreferencedCode", Justification = "No parameter object is passed to the RepoDB execute methods.")]
        public static void CreatePseudoTable(AuroraDbConnection connection,
            string tableName,
            string pseudoTableName,
            AuroraDbBulkImportPseudoTableType pseudoTableType,
            Field qualifierField = null,
            ITrace trace = null,
            string traceKey = null,
            AuroraDbTransaction transaction = null)
        {
            var dbSetting = connection.GetDbSetting();
            var commandText = AuroraDbText.GetCreatePseudoTableSql(tableName, pseudoTableName, pseudoTableType, dbSetting, qualifierField);
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
        [UnconditionalSuppressMessage("Trimming", "IL2026:RequiresUnreferencedCode", Justification = "No parameter object is passed to the RepoDB execute methods.")]
        public static async Task CreatePseudoTableAsync(AuroraDbConnection connection,
            string tableName,
            string pseudoTableName,
            AuroraDbBulkImportPseudoTableType pseudoTableType,
            Field qualifierField = null,
            ITrace trace = null,
            string traceKey = null,
            AuroraDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            var dbSetting = connection.GetDbSetting();
            var commandText = AuroraDbText.GetCreatePseudoTableSql(tableName, pseudoTableName, pseudoTableType, dbSetting, qualifierField);
            await connection.ExecuteNonQueryAsync(commandText, transaction: transaction, cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Creates an index on <paramref name="pseudoTableName"/>'s qualifier columns. Must be called after
        /// <see cref="CreatePseudoTable"/> and before the pseudo table is bulk-loaded, so the index exists
        /// before any data is staged into it rather than being built (or rebuilt) against a populated table.
        /// No-ops when <paramref name="qualifiers"/> is empty (e.g. a plain insert has no qualifier to index).
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="pseudoTableName"></param>
        /// <param name="qualifiers"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        [UnconditionalSuppressMessage("Trimming", "IL2026:RequiresUnreferencedCode", Justification = "No parameter object is passed to the RepoDB execute methods.")]
        public static void CreatePseudoTableIndex(AuroraDbConnection connection,
            string pseudoTableName,
            IEnumerable<Field> qualifiers,
            ITrace trace = null,
            string traceKey = null,
            AuroraDbTransaction transaction = null)
        {
            if (qualifiers?.Any() != true)
            {
                return;
            }

            var dbSetting = connection.GetDbSetting();
            var commandText = AuroraDbText.GetCreatePseudoTableIndexSql(pseudoTableName, qualifiers, dbSetting);
            connection.ExecuteNonQuery(commandText, transaction: transaction);
        }

        /// <summary>
        /// Creates an index on <paramref name="pseudoTableName"/>'s qualifier columns. Must be called after
        /// <see cref="CreatePseudoTableAsync"/> and before the pseudo table is bulk-loaded, so the index exists
        /// before any data is staged into it rather than being built (or rebuilt) against a populated table.
        /// No-ops when <paramref name="qualifiers"/> is empty (e.g. a plain insert has no qualifier to index).
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="pseudoTableName"></param>
        /// <param name="qualifiers"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [UnconditionalSuppressMessage("Trimming", "IL2026:RequiresUnreferencedCode", Justification = "No parameter object is passed to the RepoDB execute methods.")]
        public static async Task CreatePseudoTableIndexAsync(AuroraDbConnection connection,
            string pseudoTableName,
            IEnumerable<Field> qualifiers,
            ITrace trace = null,
            string traceKey = null,
            AuroraDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            if (qualifiers?.Any() != true)
            {
                return;
            }

            var dbSetting = connection.GetDbSetting();
            var commandText = AuroraDbText.GetCreatePseudoTableIndexSql(pseudoTableName, qualifiers, dbSetting);
            await connection.ExecuteNonQueryAsync(commandText, transaction: transaction, cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="pseudoTableName"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        [UnconditionalSuppressMessage("Trimming", "IL2026:RequiresUnreferencedCode", Justification = "No parameter object is passed to the RepoDB execute methods.")]
        public static void TruncatePseudoTable(AuroraDbConnection connection,
            string pseudoTableName,
            ITrace trace = null,
            string traceKey = null,
            AuroraDbTransaction transaction = null)
        {
            var dbSetting = connection.GetDbSetting();
            var commandText = AuroraDbText.GetTruncatePseudoTableSql(pseudoTableName, dbSetting);
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
        [UnconditionalSuppressMessage("Trimming", "IL2026:RequiresUnreferencedCode", Justification = "No parameter object is passed to the RepoDB execute methods.")]
        public static async Task TruncatePseudoTableAsync(AuroraDbConnection connection,
            string pseudoTableName,
            ITrace trace = null,
            string traceKey = null,
            AuroraDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            var dbSetting = connection.GetDbSetting();
            var commandText = AuroraDbText.GetTruncatePseudoTableSql(pseudoTableName, dbSetting);
            await connection.ExecuteNonQueryAsync(commandText, transaction: transaction, cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="pseudoTableName"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        [UnconditionalSuppressMessage("Trimming", "IL2026:RequiresUnreferencedCode", Justification = "No parameter object is passed to the RepoDB execute methods.")]
        public static void DropPseudoTable(AuroraDbConnection connection,
            string pseudoTableName,
            ITrace trace = null,
            string traceKey = null,
            AuroraDbTransaction transaction = null)
        {
            var dbSetting = connection.GetDbSetting();
            var commandText = AuroraDbText.GetDropPseudoTableSql(pseudoTableName, dbSetting);
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
        [UnconditionalSuppressMessage("Trimming", "IL2026:RequiresUnreferencedCode", Justification = "No parameter object is passed to the RepoDB execute methods.")]
        public static async Task DropPseudoTableAsync(AuroraDbConnection connection,
            string pseudoTableName,
            ITrace trace = null,
            string traceKey = null,
            AuroraDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            var dbSetting = connection.GetDbSetting();
            var commandText = AuroraDbText.GetDropPseudoTableSql(pseudoTableName, dbSetting);
            await connection.ExecuteNonQueryAsync(commandText, transaction: transaction, cancellationToken: cancellationToken).ConfigureAwait(false);
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
        [UnconditionalSuppressMessage("Trimming", "IL2026:RequiresUnreferencedCode", Justification = "No parameter object is passed to the RepoDB execute methods.")]
        public static void AllowNullForColumn(AuroraDbConnection connection,
            string pseudoTableName,
            string columnName,
            ITrace trace = null,
            string traceKey = null,
            AuroraDbTransaction transaction = null)
        {
            var dbSetting = connection.GetDbSetting();
            var commandText = AuroraDbText.GetAllowNullForColumnSql(pseudoTableName, columnName, dbSetting);
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
        [UnconditionalSuppressMessage("Trimming", "IL2026:RequiresUnreferencedCode", Justification = "No parameter object is passed to the RepoDB execute methods.")]
        public static async Task AllowNullForColumnAsync(AuroraDbConnection connection,
            string pseudoTableName,
            string columnName,
            ITrace trace = null,
            string traceKey = null,
            AuroraDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            var dbSetting = connection.GetDbSetting();
            var commandText = AuroraDbText.GetAllowNullForColumnSql(pseudoTableName, columnName, dbSetting);
            await connection.ExecuteNonQueryAsync(commandText, transaction: transaction, cancellationToken: cancellationToken).ConfigureAwait(false);
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
        [UnconditionalSuppressMessage("Trimming", "IL2026:RequiresUnreferencedCode", Justification = "No parameter object is passed to the RepoDB execute methods.")]
        private static (string SequenceName, bool IsAlwaysGenerated) GetIdentitySequenceMetadata(AuroraDbConnection connection,
            string tableName,
            Field identityField,
            ITrace trace = null,
            string traceKey = null,
            AuroraDbTransaction transaction = null)
        {
            var dbSetting = connection.GetDbSetting();
            var commandText = AuroraDbText.GetIdentitySequenceMetadataSql(tableName, identityField, dbSetting);

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
        [UnconditionalSuppressMessage("Trimming", "IL2026:RequiresUnreferencedCode", Justification = "No parameter object is passed to the RepoDB execute methods.")]
        private static async Task<(string SequenceName, bool IsAlwaysGenerated)> GetIdentitySequenceMetadataAsync(AuroraDbConnection connection,
            string tableName,
            Field identityField,
            ITrace trace = null,
            string traceKey = null,
            AuroraDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            var dbSetting = connection.GetDbSetting();
            var commandText = AuroraDbText.GetIdentitySequenceMetadataSql(tableName, identityField, dbSetting);

            using var reader = (DbDataReader)await connection.ExecuteReaderAsync(commandText, transaction: transaction, cancellationToken: cancellationToken).ConfigureAwait(false);
            await reader.ReadAsync(cancellationToken).ConfigureAwait(false);
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
        [UnconditionalSuppressMessage("Trimming", "IL2026:RequiresUnreferencedCode", Justification = "No parameter object is passed to the RepoDB execute methods.")]
        public static int InsertFromPseudoTableForReturnIdentity<[DynamicallyAccessedMembers(Trimming.Entity)] TEntity>(AuroraDbConnection connection,
            string tableName,
            string pseudoTableName,
            IEnumerable<Field> fields,
            Field identityField,
            IList<TEntity> entities,
            ITrace trace = null,
            string traceKey = null,
            AuroraDbTransaction transaction = null)
            where TEntity : class
        {
            var dbSetting = connection.GetDbSetting();
            var (sequenceName, isAlwaysGenerated) = GetIdentitySequenceMetadata(connection, tableName, identityField, trace, traceKey, transaction);
            var commandText = AuroraDbText.GetInsertFromPseudoTableForReturnIdentitySql(tableName, pseudoTableName, fields, identityField, sequenceName, isAlwaysGenerated, dbSetting);
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
        [UnconditionalSuppressMessage("Trimming", "IL2026:RequiresUnreferencedCode", Justification = "No parameter object is passed to the RepoDB execute methods.")]
        public static async Task<int> InsertFromPseudoTableForReturnIdentityAsync<[DynamicallyAccessedMembers(Trimming.Entity)] TEntity>(AuroraDbConnection connection,
            string tableName,
            string pseudoTableName,
            IEnumerable<Field> fields,
            Field identityField,
            IList<TEntity> entities,
            ITrace trace = null,
            string traceKey = null,
            AuroraDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            var dbSetting = connection.GetDbSetting();
            var (sequenceName, isAlwaysGenerated) = await GetIdentitySequenceMetadataAsync(connection, tableName, identityField, trace, traceKey, transaction, cancellationToken).ConfigureAwait(false);
            var commandText = AuroraDbText.GetInsertFromPseudoTableForReturnIdentitySql(tableName, pseudoTableName, fields, identityField, sequenceName, isAlwaysGenerated, dbSetting);
            var setter = FunctionCache.GetDataEntityPropertySetterCompiledFunction(typeof(TEntity), identityField);

            using var reader = (DbDataReader)await connection.ExecuteReaderAsync(commandText, transaction: transaction, cancellationToken: cancellationToken).ConfigureAwait(false);
            var result = 0;

            while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
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
        [UnconditionalSuppressMessage("Trimming", "IL2026:RequiresUnreferencedCode", Justification = "No parameter object is passed to the RepoDB execute methods.")]
        public static int InsertFromPseudoTableForReturnIdentityForDataTable(AuroraDbConnection connection,
            string tableName,
            string pseudoTableName,
            IEnumerable<Field> fields,
            Field identityField,
            IList<DataRow> rows,
            ITrace trace = null,
            string traceKey = null,
            AuroraDbTransaction transaction = null)
        {
            var dbSetting = connection.GetDbSetting();
            var (sequenceName, isAlwaysGenerated) = GetIdentitySequenceMetadata(connection, tableName, identityField, trace, traceKey, transaction);
            var commandText = AuroraDbText.GetInsertFromPseudoTableForReturnIdentitySql(tableName, pseudoTableName, fields, identityField, sequenceName, isAlwaysGenerated, dbSetting);

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
        [UnconditionalSuppressMessage("Trimming", "IL2026:RequiresUnreferencedCode", Justification = "No parameter object is passed to the RepoDB execute methods.")]
        public static async Task<int> InsertFromPseudoTableForReturnIdentityForDataTableAsync(AuroraDbConnection connection,
            string tableName,
            string pseudoTableName,
            IEnumerable<Field> fields,
            Field identityField,
            IList<DataRow> rows,
            ITrace trace = null,
            string traceKey = null,
            AuroraDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            var dbSetting = connection.GetDbSetting();
            var (sequenceName, isAlwaysGenerated) = await GetIdentitySequenceMetadataAsync(connection, tableName, identityField, trace, traceKey, transaction, cancellationToken).ConfigureAwait(false);
            var commandText = AuroraDbText.GetInsertFromPseudoTableForReturnIdentitySql(tableName, pseudoTableName, fields, identityField, sequenceName, isAlwaysGenerated, dbSetting);

            using var reader = (DbDataReader)await connection.ExecuteReaderAsync(commandText, transaction: transaction, cancellationToken: cancellationToken).ConfigureAwait(false);
            var result = 0;

            while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
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
        [UnconditionalSuppressMessage("Trimming", "IL2026:RequiresUnreferencedCode", Justification = "No parameter object is passed to the RepoDB execute methods.")]
        public static int MergeFromPseudoTable(AuroraDbConnection connection,
            string tableName,
            string pseudoTableName,
            IEnumerable<Field> fields,
            IEnumerable<Field> qualifiers,
            Field identityField,
            ITrace trace = null,
            string traceKey = null,
            AuroraDbTransaction transaction = null)
        {
            var dbSetting = connection.GetDbSetting();
            var commandText = AuroraDbText.GetMergeFromPseudoTableSql(tableName, pseudoTableName, fields, qualifiers, identityField, dbSetting);
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
        [UnconditionalSuppressMessage("Trimming", "IL2026:RequiresUnreferencedCode", Justification = "No parameter object is passed to the RepoDB execute methods.")]
        public static async Task<int> MergeFromPseudoTableAsync(AuroraDbConnection connection,
            string tableName,
            string pseudoTableName,
            IEnumerable<Field> fields,
            IEnumerable<Field> qualifiers,
            Field identityField,
            ITrace trace = null,
            string traceKey = null,
            AuroraDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            var dbSetting = connection.GetDbSetting();
            var commandText = AuroraDbText.GetMergeFromPseudoTableSql(tableName, pseudoTableName, fields, qualifiers, identityField, dbSetting);
            return await connection.ExecuteNonQueryAsync(commandText, transaction: transaction, cancellationToken: cancellationToken).ConfigureAwait(false);
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
        [UnconditionalSuppressMessage("Trimming", "IL2026:RequiresUnreferencedCode", Justification = "No parameter object is passed to the RepoDB execute methods.")]
        public static int MergeFromPseudoTableForReturnIdentity<[DynamicallyAccessedMembers(Trimming.Entity)] TEntity>(AuroraDbConnection connection,
            string tableName,
            string pseudoTableName,
            IEnumerable<Field> fields,
            Field identityField,
            IEnumerable<Field> qualifiers,
            IList<TEntity> entities,
            ITrace trace = null,
            string traceKey = null,
            AuroraDbTransaction transaction = null)
            where TEntity : class
        {
            var dbSetting = connection.GetDbSetting();
            var (sequenceName, isAlwaysGenerated) = GetIdentitySequenceMetadata(connection, tableName, identityField, trace, traceKey, transaction);
            var commandText = AuroraDbText.GetMergeFromPseudoTableForReturnIdentitySql(tableName, pseudoTableName, fields, identityField, qualifiers, sequenceName, isAlwaysGenerated, dbSetting);
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
        [UnconditionalSuppressMessage("Trimming", "IL2026:RequiresUnreferencedCode", Justification = "No parameter object is passed to the RepoDB execute methods.")]
        public static async Task<int> MergeFromPseudoTableForReturnIdentityAsync<[DynamicallyAccessedMembers(Trimming.Entity)] TEntity>(AuroraDbConnection connection,
            string tableName,
            string pseudoTableName,
            IEnumerable<Field> fields,
            Field identityField,
            IEnumerable<Field> qualifiers,
            IList<TEntity> entities,
            ITrace trace = null,
            string traceKey = null,
            AuroraDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            var setter = FunctionCache.GetDataEntityPropertySetterCompiledFunction(typeof(TEntity), identityField);
            var dbSetting = connection.GetDbSetting();
            var (sequenceName, isAlwaysGenerated) = await GetIdentitySequenceMetadataAsync(connection, tableName, identityField, trace, traceKey, transaction, cancellationToken).ConfigureAwait(false);
            var commandText = AuroraDbText.GetMergeFromPseudoTableForReturnIdentitySql(tableName, pseudoTableName, fields, identityField, qualifiers, sequenceName, isAlwaysGenerated, dbSetting);

            using var reader = (DbDataReader)await connection.ExecuteReaderAsync(commandText, transaction: transaction, cancellationToken: cancellationToken).ConfigureAwait(false);
            var result = 0;

            while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
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
        [UnconditionalSuppressMessage("Trimming", "IL2026:RequiresUnreferencedCode", Justification = "No parameter object is passed to the RepoDB execute methods.")]
        public static int MergeFromPseudoTableForReturnIdentityForDataTable(AuroraDbConnection connection,
            string tableName,
            string pseudoTableName,
            IEnumerable<Field> fields,
            Field identityField,
            IEnumerable<Field> qualifiers,
            IList<DataRow> rows,
            ITrace trace = null,
            string traceKey = null,
            AuroraDbTransaction transaction = null)
        {
            var dbSetting = connection.GetDbSetting();
            var (sequenceName, isAlwaysGenerated) = GetIdentitySequenceMetadata(connection, tableName, identityField, trace, traceKey, transaction);
            var commandText = AuroraDbText.GetMergeFromPseudoTableForReturnIdentitySql(tableName, pseudoTableName, fields, identityField, qualifiers, sequenceName, isAlwaysGenerated, dbSetting);

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
        [UnconditionalSuppressMessage("Trimming", "IL2026:RequiresUnreferencedCode", Justification = "No parameter object is passed to the RepoDB execute methods.")]
        public static async Task<int> MergeFromPseudoTableForReturnIdentityForDataTableAsync(AuroraDbConnection connection,
            string tableName,
            string pseudoTableName,
            IEnumerable<Field> fields,
            Field identityField,
            IEnumerable<Field> qualifiers,
            IList<DataRow> rows,
            ITrace trace = null,
            string traceKey = null,
            AuroraDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            var dbSetting = connection.GetDbSetting();
            var (sequenceName, isAlwaysGenerated) = await GetIdentitySequenceMetadataAsync(connection, tableName, identityField, trace, traceKey, transaction, cancellationToken).ConfigureAwait(false);
            var commandText = AuroraDbText.GetMergeFromPseudoTableForReturnIdentitySql(tableName, pseudoTableName, fields, identityField, qualifiers, sequenceName, isAlwaysGenerated, dbSetting);

            using var reader = (DbDataReader)await connection.ExecuteReaderAsync(commandText, transaction: transaction, cancellationToken: cancellationToken).ConfigureAwait(false);
            var result = 0;

            while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
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
        [UnconditionalSuppressMessage("Trimming", "IL2026:RequiresUnreferencedCode", Justification = "No parameter object is passed to the RepoDB execute methods.")]
        public static int UpdateFromPseudoTable(AuroraDbConnection connection,
            string tableName,
            string pseudoTableName,
            IEnumerable<Field> fields,
            IEnumerable<Field> qualifiers,
            ITrace trace = null,
            string traceKey = null,
            AuroraDbTransaction transaction = null)
        {
            var dbSetting = connection.GetDbSetting();
            var commandText = AuroraDbText.GetUpdateFromPseudoTableSql(tableName, pseudoTableName, fields, qualifiers, dbSetting);
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
        [UnconditionalSuppressMessage("Trimming", "IL2026:RequiresUnreferencedCode", Justification = "No parameter object is passed to the RepoDB execute methods.")]
        public static async Task<int> UpdateFromPseudoTableAsync(AuroraDbConnection connection,
            string tableName,
            string pseudoTableName,
            IEnumerable<Field> fields,
            IEnumerable<Field> qualifiers,
            ITrace trace = null,
            string traceKey = null,
            AuroraDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            var dbSetting = connection.GetDbSetting();
            var commandText = AuroraDbText.GetUpdateFromPseudoTableSql(tableName, pseudoTableName, fields, qualifiers, dbSetting);
            return await connection.ExecuteNonQueryAsync(commandText, transaction: transaction, cancellationToken: cancellationToken).ConfigureAwait(false);
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
        [UnconditionalSuppressMessage("Trimming", "IL2026:RequiresUnreferencedCode", Justification = "No parameter object is passed to the RepoDB execute methods.")]
        public static int DeleteFromPseudoTable(AuroraDbConnection connection,
            string tableName,
            string pseudoTableName,
            IEnumerable<Field> qualifiers,
            ITrace trace = null,
            string traceKey = null,
            AuroraDbTransaction transaction = null)
        {
            var dbSetting = connection.GetDbSetting();
            var commandText = AuroraDbText.GetDeleteFromPseudoTableSql(tableName, pseudoTableName, qualifiers, dbSetting);
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
        [UnconditionalSuppressMessage("Trimming", "IL2026:RequiresUnreferencedCode", Justification = "No parameter object is passed to the RepoDB execute methods.")]
        public static async Task<int> DeleteFromPseudoTableAsync(AuroraDbConnection connection,
            string tableName,
            string pseudoTableName,
            IEnumerable<Field> qualifiers,
            ITrace trace = null,
            string traceKey = null,
            AuroraDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            var dbSetting = connection.GetDbSetting();
            var commandText = AuroraDbText.GetDeleteFromPseudoTableSql(tableName, pseudoTableName, qualifiers, dbSetting);
            return await connection.ExecuteNonQueryAsync(commandText, transaction: transaction, cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        #endregion
    }
}
