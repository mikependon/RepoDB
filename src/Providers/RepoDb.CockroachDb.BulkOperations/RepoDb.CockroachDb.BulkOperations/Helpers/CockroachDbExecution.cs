#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using RepoDb.Connector.CockroachDb;
using RepoDb.Enumerations.CockroachDb;
using RepoDb.Extensions;
using RepoDb.Interfaces;

namespace RepoDb.CockroachDb.BulkOperations.Extensions
{
    /// <summary>
    ///
    /// </summary>
    internal static class CockroachDbExecution
    {
        #region Shared

        /// <summary>
        /// 
        /// </summary>
        /// <param name="row"></param>
        /// <param name="columnName"></param>
        /// <param name="value"></param>
        private static void SetIdentityValue(DataRow row,
            string columnName,
            object value)
        {
            var column = row.Table.Columns[columnName];
            var wasReadOnly = column.ReadOnly;
            column.ReadOnly = false;
            try
            {
                row[columnName] = value;
            }
            finally
            {
                column.ReadOnly = wasReadOnly;
            }
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
        public static void CreatePseudoTable(CockroachDbConnection connection,
            string tableName,
            string pseudoTableName,
            CockroachDbBulkImportPseudoTableType pseudoTableType,
            Field qualifierField = null,
            ITrace trace = null,
            string traceKey = null,
            CockroachDbTransaction transaction = null)
        {
            var dbSetting = connection.GetDbSetting();
            var commandText = CockroachDbText.GetCreatePseudoTableSql(tableName, pseudoTableName, pseudoTableType, dbSetting, qualifierField);
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
        public static async Task CreatePseudoTableAsync(CockroachDbConnection connection,
            string tableName,
            string pseudoTableName,
            CockroachDbBulkImportPseudoTableType pseudoTableType,
            Field qualifierField = null,
            ITrace trace = null,
            string traceKey = null,
            CockroachDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            var dbSetting = connection.GetDbSetting();
            var commandText = CockroachDbText.GetCreatePseudoTableSql(tableName, pseudoTableName, pseudoTableType, dbSetting, qualifierField);
            await connection.ExecuteNonQueryAsync(commandText, transaction: transaction, cancellationToken: cancellationToken).ConfigureAwait(false);
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
        public static void CreatePseudoTableIndex(CockroachDbConnection connection,
            string pseudoTableName,
            IEnumerable<Field> qualifiers,
            ITrace trace = null,
            string traceKey = null,
            CockroachDbTransaction transaction = null)
        {
            if (qualifiers?.Any() != true)
            {
                return;
            }

            var dbSetting = connection.GetDbSetting();
            var commandText = CockroachDbText.GetCreatePseudoTableIndexSql(pseudoTableName, qualifiers, dbSetting);
            connection.ExecuteNonQuery(commandText, transaction: transaction);
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
        public static async Task CreatePseudoTableIndexAsync(CockroachDbConnection connection,
            string pseudoTableName,
            IEnumerable<Field> qualifiers,
            ITrace trace = null,
            string traceKey = null,
            CockroachDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            if (qualifiers?.Any() != true)
            {
                return;
            }

            var dbSetting = connection.GetDbSetting();
            var commandText = CockroachDbText.GetCreatePseudoTableIndexSql(pseudoTableName, qualifiers, dbSetting);
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
        public static void TruncatePseudoTable(CockroachDbConnection connection,
            string pseudoTableName,
            ITrace trace = null,
            string traceKey = null,
            CockroachDbTransaction transaction = null)
        {
            var dbSetting = connection.GetDbSetting();
            var commandText = CockroachDbText.GetTruncatePseudoTableSql(pseudoTableName, dbSetting);
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
        public static async Task TruncatePseudoTableAsync(CockroachDbConnection connection,
            string pseudoTableName,
            ITrace trace = null,
            string traceKey = null,
            CockroachDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            var dbSetting = connection.GetDbSetting();
            var commandText = CockroachDbText.GetTruncatePseudoTableSql(pseudoTableName, dbSetting);
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
        public static void DropPseudoTable(CockroachDbConnection connection,
            string pseudoTableName,
            ITrace trace = null,
            string traceKey = null,
            CockroachDbTransaction transaction = null)
        {
            var dbSetting = connection.GetDbSetting();
            var commandText = CockroachDbText.GetDropPseudoTableSql(pseudoTableName, dbSetting);
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
        public static async Task DropPseudoTableAsync(CockroachDbConnection connection,
            string pseudoTableName,
            ITrace trace = null,
            string traceKey = null,
            CockroachDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            var dbSetting = connection.GetDbSetting();
            var commandText = CockroachDbText.GetDropPseudoTableSql(pseudoTableName, dbSetting);
            await connection.ExecuteNonQueryAsync(commandText, transaction: transaction, cancellationToken: cancellationToken).ConfigureAwait(false);
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
        public static int InsertFromPseudoTableForReturnIdentity<TEntity>(CockroachDbConnection connection,
            string tableName,
            string pseudoTableName,
            IEnumerable<Field> fields,
            Field identityField,
            IList<TEntity> entities,
            ITrace trace = null,
            string traceKey = null,
            CockroachDbTransaction transaction = null)
            where TEntity : class
        {
            var dbSetting = connection.GetDbSetting();
            var commandText = CockroachDbText.GetInsertFromPseudoTableForReturnIdentitySql(tableName, pseudoTableName, fields, identityField, dbSetting);
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
        public static async Task<int> InsertFromPseudoTableForReturnIdentityAsync<TEntity>(CockroachDbConnection connection,
            string tableName,
            string pseudoTableName,
            IEnumerable<Field> fields,
            Field identityField,
            IList<TEntity> entities,
            ITrace trace = null,
            string traceKey = null,
            CockroachDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            var dbSetting = connection.GetDbSetting();
            var commandText = CockroachDbText.GetInsertFromPseudoTableForReturnIdentitySql(tableName, pseudoTableName, fields, identityField, dbSetting);
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
        public static int InsertFromPseudoTableForReturnIdentityForDataTable(CockroachDbConnection connection,
            string tableName,
            string pseudoTableName,
            IEnumerable<Field> fields,
            Field identityField,
            IList<DataRow> rows,
            ITrace trace = null,
            string traceKey = null,
            CockroachDbTransaction transaction = null)
        {
            var dbSetting = connection.GetDbSetting();
            var commandText = CockroachDbText.GetInsertFromPseudoTableForReturnIdentitySql(tableName, pseudoTableName, fields, identityField, dbSetting);

            using var reader = (DbDataReader)connection.ExecuteReader(commandText, transaction: transaction);
            var result = 0;

            while (reader.Read())
            {
                SetIdentityValue(rows[result], identityField.Name, Converter.DbNullToNull(reader.GetValue(0)));
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
        public static async Task<int> InsertFromPseudoTableForReturnIdentityForDataTableAsync(CockroachDbConnection connection,
            string tableName,
            string pseudoTableName,
            IEnumerable<Field> fields,
            Field identityField,
            IList<DataRow> rows,
            ITrace trace = null,
            string traceKey = null,
            CockroachDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            var dbSetting = connection.GetDbSetting();
            var commandText = CockroachDbText.GetInsertFromPseudoTableForReturnIdentitySql(tableName, pseudoTableName, fields, identityField, dbSetting);

            using var reader = (DbDataReader)await connection.ExecuteReaderAsync(commandText, transaction: transaction, cancellationToken: cancellationToken).ConfigureAwait(false);
            var result = 0;

            while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
            {
                SetIdentityValue(rows[result], identityField.Name, Converter.DbNullToNull(reader.GetValue(0)));
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
        public static int MergeFromPseudoTable(CockroachDbConnection connection,
            string tableName,
            string pseudoTableName,
            IEnumerable<Field> fields,
            IEnumerable<Field> qualifiers,
            Field identityField,
            ITrace trace = null,
            string traceKey = null,
            CockroachDbTransaction transaction = null)
        {
            var dbSetting = connection.GetDbSetting();
            var commandText = CockroachDbText.GetMergeFromPseudoTableSql(tableName, pseudoTableName, fields, qualifiers, identityField, dbSetting);
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
        public static async Task<int> MergeFromPseudoTableAsync(CockroachDbConnection connection,
            string tableName,
            string pseudoTableName,
            IEnumerable<Field> fields,
            IEnumerable<Field> qualifiers,
            Field identityField,
            ITrace trace = null,
            string traceKey = null,
            CockroachDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            var dbSetting = connection.GetDbSetting();
            var commandText = CockroachDbText.GetMergeFromPseudoTableSql(tableName, pseudoTableName, fields, qualifiers, identityField, dbSetting);
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
        public static int MergeFromPseudoTableForReturnIdentity<TEntity>(CockroachDbConnection connection,
            string tableName,
            string pseudoTableName,
            IEnumerable<Field> fields,
            Field identityField,
            IEnumerable<Field> qualifiers,
            IList<TEntity> entities,
            ITrace trace = null,
            string traceKey = null,
            CockroachDbTransaction transaction = null)
            where TEntity : class
        {
            var dbSetting = connection.GetDbSetting();
            var commandText = CockroachDbText.GetMergeFromPseudoTableForReturnIdentitySql(tableName, pseudoTableName, fields, identityField, qualifiers, dbSetting);
            var setter = FunctionCache.GetDataEntityPropertySetterCompiledFunction(typeof(TEntity), identityField);

            using var reader = (DbDataReader)connection.ExecuteReader(commandText, transaction: transaction);
            var result = 0;

            do
            {
                while (reader.Read())
                {
                    setter?.Invoke(entities[result], Converter.DbNullToNull(reader.GetValue(0)));
                    result++;
                }
            }
            while (reader.NextResult());

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
        public static async Task<int> MergeFromPseudoTableForReturnIdentityAsync<TEntity>(CockroachDbConnection connection,
            string tableName,
            string pseudoTableName,
            IEnumerable<Field> fields,
            Field identityField,
            IEnumerable<Field> qualifiers,
            IList<TEntity> entities,
            ITrace trace = null,
            string traceKey = null,
            CockroachDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            var setter = FunctionCache.GetDataEntityPropertySetterCompiledFunction(typeof(TEntity), identityField);
            var dbSetting = connection.GetDbSetting();
            var commandText = CockroachDbText.GetMergeFromPseudoTableForReturnIdentitySql(tableName, pseudoTableName, fields, identityField, qualifiers, dbSetting);

            using var reader = (DbDataReader)await connection.ExecuteReaderAsync(commandText, transaction: transaction, cancellationToken: cancellationToken).ConfigureAwait(false);
            var result = 0;

            do
            {
                while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
                {
                    setter?.Invoke(entities[result], Converter.DbNullToNull(reader.GetValue(0)));
                    result++;
                }
            }
            while (await reader.NextResultAsync(cancellationToken).ConfigureAwait(false));

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
        public static int MergeFromPseudoTableForReturnIdentityForDataTable(CockroachDbConnection connection,
            string tableName,
            string pseudoTableName,
            IEnumerable<Field> fields,
            Field identityField,
            IEnumerable<Field> qualifiers,
            IList<DataRow> rows,
            ITrace trace = null,
            string traceKey = null,
            CockroachDbTransaction transaction = null)
        {
            var dbSetting = connection.GetDbSetting();
            var commandText = CockroachDbText.GetMergeFromPseudoTableForReturnIdentitySql(tableName, pseudoTableName, fields, identityField, qualifiers, dbSetting);

            using var reader = (DbDataReader)connection.ExecuteReader(commandText, transaction: transaction);
            var result = 0;

            do
            {
                while (reader.Read())
                {
                    SetIdentityValue(rows[result], identityField.Name, Converter.DbNullToNull(reader.GetValue(0)));
                    result++;
                }
            }
            while (reader.NextResult());

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
        public static async Task<int> MergeFromPseudoTableForReturnIdentityForDataTableAsync(CockroachDbConnection connection,
            string tableName,
            string pseudoTableName,
            IEnumerable<Field> fields,
            Field identityField,
            IEnumerable<Field> qualifiers,
            IList<DataRow> rows,
            ITrace trace = null,
            string traceKey = null,
            CockroachDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            var dbSetting = connection.GetDbSetting();
            var commandText = CockroachDbText.GetMergeFromPseudoTableForReturnIdentitySql(tableName, pseudoTableName, fields, identityField, qualifiers, dbSetting);

            using var reader = (DbDataReader)await connection.ExecuteReaderAsync(commandText, transaction: transaction, cancellationToken: cancellationToken).ConfigureAwait(false);
            var result = 0;

            do
            {
                while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
                {
                    SetIdentityValue(rows[result], identityField.Name, Converter.DbNullToNull(reader.GetValue(0)));
                    result++;
                }
            }
            while (await reader.NextResultAsync(cancellationToken).ConfigureAwait(false));

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
        /// <param name="identityField"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public static int UpdateFromPseudoTable(CockroachDbConnection connection,
            string tableName,
            string pseudoTableName,
            IEnumerable<Field> fields,
            IEnumerable<Field> qualifiers,
            Field identityField = null,
            ITrace trace = null,
            string traceKey = null,
            CockroachDbTransaction transaction = null)
        {
            var dbSetting = connection.GetDbSetting();
            var commandText = CockroachDbText.GetUpdateFromPseudoTableSql(tableName, pseudoTableName, fields, qualifiers, identityField, dbSetting);
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
        public static async Task<int> UpdateFromPseudoTableAsync(CockroachDbConnection connection,
            string tableName,
            string pseudoTableName,
            IEnumerable<Field> fields,
            IEnumerable<Field> qualifiers,
            Field identityField = null,
            ITrace trace = null,
            string traceKey = null,
            CockroachDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            var dbSetting = connection.GetDbSetting();
            var commandText = CockroachDbText.GetUpdateFromPseudoTableSql(tableName, pseudoTableName, fields, qualifiers, identityField, dbSetting);
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
        public static int DeleteFromPseudoTable(CockroachDbConnection connection,
            string tableName,
            string pseudoTableName,
            IEnumerable<Field> qualifiers,
            ITrace trace = null,
            string traceKey = null,
            CockroachDbTransaction transaction = null)
        {
            var dbSetting = connection.GetDbSetting();
            var commandText = CockroachDbText.GetDeleteFromPseudoTableSql(tableName, pseudoTableName, qualifiers, dbSetting);
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
        public static async Task<int> DeleteFromPseudoTableAsync(CockroachDbConnection connection,
            string tableName,
            string pseudoTableName,
            IEnumerable<Field> qualifiers,
            ITrace trace = null,
            string traceKey = null,
            CockroachDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            var dbSetting = connection.GetDbSetting();
            var commandText = CockroachDbText.GetDeleteFromPseudoTableSql(tableName, pseudoTableName, qualifiers, dbSetting);
            return await connection.ExecuteNonQueryAsync(commandText, transaction: transaction, cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        #endregion
    }
}
