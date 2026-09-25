#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using DuckDB.NET.Data;
using RepoDb.Enumerations.DuckDb;
using RepoDb.Interfaces;

namespace RepoDb.DuckDb.BulkOperations.Extensions
{
    /// <summary>
    /// Executes the SQL statements of the DuckDB bulk operations.
    /// </summary>
    internal static class DuckDbExecution
    {
        #region Shared

        /// <summary>
        /// Creates the pseudo table.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="pseudoTableName">The name of the pseudo table.</param>
        /// <param name="pseudoTableType">The type of the pseudo table.</param>
        /// <param name="fields">The fields to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        public static void CreatePseudoTable(DuckDBConnection connection,
            string tableName,
            string pseudoTableName,
            DuckDbBulkImportPseudoTableType pseudoTableType,
            IEnumerable<Field> fields = null,
            ITrace trace = null,
            string traceKey = null,
            DuckDBTransaction transaction = null) =>
            connection.ExecuteNonQuery(DuckDbText.GetCreatePseudoTableSql(tableName, pseudoTableName, pseudoTableType, connection.GetDbSetting(), fields),
                trace: trace, traceKey: traceKey, transaction: transaction);

        /// <summary>
        /// Creates the pseudo table in an asynchronous way.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="pseudoTableName">The name of the pseudo table.</param>
        /// <param name="pseudoTableType">The type of the pseudo table.</param>
        /// <param name="fields">The fields to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public static Task CreatePseudoTableAsync(DuckDBConnection connection,
            string tableName,
            string pseudoTableName,
            DuckDbBulkImportPseudoTableType pseudoTableType,
            IEnumerable<Field> fields = null,
            ITrace trace = null,
            string traceKey = null,
            DuckDBTransaction transaction = null,
            CancellationToken cancellationToken = default) =>
            connection.ExecuteNonQueryAsync(DuckDbText.GetCreatePseudoTableSql(tableName, pseudoTableName, pseudoTableType, connection.GetDbSetting(), fields),
                trace: trace, traceKey: traceKey, transaction: transaction, cancellationToken: cancellationToken);

        /// <summary>
        /// Drops the pseudo table.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="pseudoTableName">The name of the pseudo table.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        public static void DropPseudoTable(DuckDBConnection connection,
            string pseudoTableName,
            ITrace trace = null,
            string traceKey = null,
            DuckDBTransaction transaction = null) =>
            connection.ExecuteNonQuery(DuckDbText.GetDropPseudoTableSql(pseudoTableName, connection.GetDbSetting()),
                trace: trace, traceKey: traceKey, transaction: transaction);

        /// <summary>
        /// Drops the pseudo table in an asynchronous way.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="pseudoTableName">The name of the pseudo table.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public static Task DropPseudoTableAsync(DuckDBConnection connection,
            string pseudoTableName,
            ITrace trace = null,
            string traceKey = null,
            DuckDBTransaction transaction = null,
            CancellationToken cancellationToken = default) =>
            connection.ExecuteNonQueryAsync(DuckDbText.GetDropPseudoTableSql(pseudoTableName, connection.GetDbSetting()),
                trace: trace, traceKey: traceKey, transaction: transaction, cancellationToken: cancellationToken);

        /// <summary>
        /// Gets the action that sets the identity of an entity.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entities.</typeparam>
        /// <param name="entities">The entities to set the identities to.</param>
        /// <param name="identityField">The identity field.</param>
        /// <returns>The identity setter.</returns>
        private static Action<int, object> GetEntitySetter<TEntity>(IList<TEntity> entities,
            Field identityField)
            where TEntity : class
        {
            var setter = FunctionCache.GetDataEntityPropertySetterCompiledFunction(typeof(TEntity), identityField);
            return (index, value) => setter?.Invoke(entities[index], value);
        }

        /// <summary>
        /// Gets the action that sets the identity of a data row.
        /// </summary>
        /// <param name="rows">The data rows to set the identities to.</param>
        /// <param name="identityField">The identity field.</param>
        /// <returns>The identity setter.</returns>
        private static Action<int, object> GetRowSetter(IList<DataRow> rows,
            Field identityField) =>
            (index, value) => rows[index][identityField.Name] = value ?? DBNull.Value;

        /// <summary>
        /// Reads the identities and sets them back to the rows.
        /// </summary>
        /// <param name="reader">The data reader of the identities.</param>
        /// <param name="getIndex">The function that maps the read index to the row index.</param>
        /// <param name="setIdentity">The action that sets the identity of a row.</param>
        /// <returns>The number of identities read.</returns>
        private static int ReadIdentities(DbDataReader reader,
            Func<int, int> getIndex,
            Action<int, object> setIdentity)
        {
            var result = 0;
            while (reader.Read())
            {
                setIdentity(getIndex(result++), Converter.DbNullToNull(reader.GetValue(0)));
            }
            return result;
        }

        /// <summary>
        /// Reads the identities and sets them back to the rows in an asynchronous way.
        /// </summary>
        /// <param name="reader">The data reader of the identities.</param>
        /// <param name="getIndex">The function that maps the read index to the row index.</param>
        /// <param name="setIdentity">The action that sets the identity of a row.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The number of identities read.</returns>
        private static async Task<int> ReadIdentitiesAsync(DbDataReader reader,
            Func<int, int> getIndex,
            Action<int, object> setIdentity,
            CancellationToken cancellationToken)
        {
            var result = 0;
            while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
            {
                setIdentity(getIndex(result++), Converter.DbNullToNull(reader.GetValue(0)));
            }
            return result;
        }

        #endregion

        #region Insert

        /// <summary>
        /// Inserts the pseudo table rows and sets the generated identities back to the entities.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entities.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="pseudoTableName">The name of the pseudo table.</param>
        /// <param name="fields">The fields to be used.</param>
        /// <param name="identityField">The identity field.</param>
        /// <param name="entities">The entities to set the identities to.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of affected rows.</returns>
        public static int InsertFromPseudoTableForReturnIdentity<TEntity>(DuckDBConnection connection,
            string tableName,
            string pseudoTableName,
            IEnumerable<Field> fields,
            Field identityField,
            IList<TEntity> entities,
            ITrace trace = null,
            string traceKey = null,
            DuckDBTransaction transaction = null)
            where TEntity : class =>
            InsertFromPseudoTableForReturnIdentity(connection, tableName, pseudoTableName, fields, identityField, GetEntitySetter(entities, identityField), trace, traceKey, transaction);

        /// <summary>
        /// Inserts the pseudo table rows and sets the generated identities back to the entities in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entities.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="pseudoTableName">The name of the pseudo table.</param>
        /// <param name="fields">The fields to be used.</param>
        /// <param name="identityField">The identity field.</param>
        /// <param name="entities">The entities to set the identities to.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The number of affected rows.</returns>
        public static Task<int> InsertFromPseudoTableForReturnIdentityAsync<TEntity>(DuckDBConnection connection,
            string tableName,
            string pseudoTableName,
            IEnumerable<Field> fields,
            Field identityField,
            IList<TEntity> entities,
            ITrace trace = null,
            string traceKey = null,
            DuckDBTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class =>
            InsertFromPseudoTableForReturnIdentityAsync(connection, tableName, pseudoTableName, fields, identityField, GetEntitySetter(entities, identityField), trace, traceKey, transaction, cancellationToken);

        /// <summary>
        /// Inserts the pseudo table rows and sets the generated identities back to the data rows.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="pseudoTableName">The name of the pseudo table.</param>
        /// <param name="fields">The fields to be used.</param>
        /// <param name="identityField">The identity field.</param>
        /// <param name="rows">The data rows to set the identities to.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of affected rows.</returns>
        public static int InsertFromPseudoTableForReturnIdentityForDataTable(DuckDBConnection connection,
            string tableName,
            string pseudoTableName,
            IEnumerable<Field> fields,
            Field identityField,
            IList<DataRow> rows,
            ITrace trace = null,
            string traceKey = null,
            DuckDBTransaction transaction = null) =>
            InsertFromPseudoTableForReturnIdentity(connection, tableName, pseudoTableName, fields, identityField, GetRowSetter(rows, identityField), trace, traceKey, transaction);

        /// <summary>
        /// Inserts the pseudo table rows and sets the generated identities back to the data rows in an asynchronous way.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="pseudoTableName">The name of the pseudo table.</param>
        /// <param name="fields">The fields to be used.</param>
        /// <param name="identityField">The identity field.</param>
        /// <param name="rows">The data rows to set the identities to.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The number of affected rows.</returns>
        public static Task<int> InsertFromPseudoTableForReturnIdentityForDataTableAsync(DuckDBConnection connection,
            string tableName,
            string pseudoTableName,
            IEnumerable<Field> fields,
            Field identityField,
            IList<DataRow> rows,
            ITrace trace = null,
            string traceKey = null,
            DuckDBTransaction transaction = null,
            CancellationToken cancellationToken = default) =>
            InsertFromPseudoTableForReturnIdentityAsync(connection, tableName, pseudoTableName, fields, identityField, GetRowSetter(rows, identityField), trace, traceKey, transaction, cancellationToken);

        /// <summary>
        /// Inserts the pseudo table rows and sets the generated identities back to the rows.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="pseudoTableName">The name of the pseudo table.</param>
        /// <param name="fields">The fields to be used.</param>
        /// <param name="identityField">The identity field.</param>
        /// <param name="setIdentity">The action that sets the identity of a row.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of affected rows.</returns>
        private static int InsertFromPseudoTableForReturnIdentity(DuckDBConnection connection,
            string tableName,
            string pseudoTableName,
            IEnumerable<Field> fields,
            Field identityField,
            Action<int, object> setIdentity,
            ITrace trace,
            string traceKey,
            DuckDBTransaction transaction)
        {
            var commandText = DuckDbText.GetInsertFromPseudoTableForReturnIdentitySql(tableName, pseudoTableName, fields, identityField, connection.GetDbSetting());
            using var reader = (DbDataReader)connection.ExecuteReader(commandText, trace: trace, traceKey: traceKey, transaction: transaction);
            return ReadIdentities(reader, index => index, setIdentity);
        }

        /// <summary>
        /// Inserts the pseudo table rows and sets the generated identities back to the rows in an asynchronous way.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="pseudoTableName">The name of the pseudo table.</param>
        /// <param name="fields">The fields to be used.</param>
        /// <param name="identityField">The identity field.</param>
        /// <param name="setIdentity">The action that sets the identity of a row.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The number of affected rows.</returns>
        private static async Task<int> InsertFromPseudoTableForReturnIdentityAsync(DuckDBConnection connection,
            string tableName,
            string pseudoTableName,
            IEnumerable<Field> fields,
            Field identityField,
            Action<int, object> setIdentity,
            ITrace trace,
            string traceKey,
            DuckDBTransaction transaction,
            CancellationToken cancellationToken)
        {
            var commandText = DuckDbText.GetInsertFromPseudoTableForReturnIdentitySql(tableName, pseudoTableName, fields, identityField, connection.GetDbSetting());
            using var reader = (DbDataReader)await connection.ExecuteReaderAsync(commandText, trace: trace, traceKey: traceKey, transaction: transaction, cancellationToken: cancellationToken).ConfigureAwait(false);
            return await ReadIdentitiesAsync(reader, index => index, setIdentity, cancellationToken).ConfigureAwait(false);
        }

        #endregion

        #region Merge

        /// <summary>
        /// Merges the pseudo table rows into the table.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="pseudoTableName">The name of the pseudo table.</param>
        /// <param name="fields">The fields to be used.</param>
        /// <param name="qualifiers">The qualifier fields to be used.</param>
        /// <param name="identityField">The identity field.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of affected rows.</returns>
        public static int MergeFromPseudoTable(DuckDBConnection connection,
            string tableName,
            string pseudoTableName,
            IEnumerable<Field> fields,
            IEnumerable<Field> qualifiers,
            Field identityField,
            ITrace trace = null,
            string traceKey = null,
            DuckDBTransaction transaction = null) =>
            connection.ExecuteNonQuery(DuckDbText.GetMergeFromPseudoTableSql(tableName, pseudoTableName, fields, qualifiers, identityField, connection.GetDbSetting()),
                trace: trace, traceKey: traceKey, transaction: transaction);

        /// <summary>
        /// Merges the pseudo table rows into the table in an asynchronous way.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="pseudoTableName">The name of the pseudo table.</param>
        /// <param name="fields">The fields to be used.</param>
        /// <param name="qualifiers">The qualifier fields to be used.</param>
        /// <param name="identityField">The identity field.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The number of affected rows.</returns>
        public static Task<int> MergeFromPseudoTableAsync(DuckDBConnection connection,
            string tableName,
            string pseudoTableName,
            IEnumerable<Field> fields,
            IEnumerable<Field> qualifiers,
            Field identityField,
            ITrace trace = null,
            string traceKey = null,
            DuckDBTransaction transaction = null,
            CancellationToken cancellationToken = default) =>
            connection.ExecuteNonQueryAsync(DuckDbText.GetMergeFromPseudoTableSql(tableName, pseudoTableName, fields, qualifiers, identityField, connection.GetDbSetting()),
                trace: trace, traceKey: traceKey, transaction: transaction, cancellationToken: cancellationToken);

        /// <summary>
        /// Merges the pseudo table rows and sets the identities back to the entities.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entities.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="pseudoTableName">The name of the pseudo table.</param>
        /// <param name="fields">The fields to be used.</param>
        /// <param name="identityField">The identity field.</param>
        /// <param name="qualifiers">The qualifier fields to be used.</param>
        /// <param name="entities">The entities to set the identities to.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of affected rows.</returns>
        public static int MergeFromPseudoTableForReturnIdentity<TEntity>(DuckDBConnection connection,
            string tableName,
            string pseudoTableName,
            IEnumerable<Field> fields,
            Field identityField,
            IEnumerable<Field> qualifiers,
            IList<TEntity> entities,
            ITrace trace = null,
            string traceKey = null,
            DuckDBTransaction transaction = null)
            where TEntity : class =>
            MergeFromPseudoTableForReturnIdentity(connection, tableName, pseudoTableName, fields, identityField, qualifiers, GetEntitySetter(entities, identityField), trace, traceKey, transaction);

        /// <summary>
        /// Merges the pseudo table rows and sets the identities back to the entities in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entities.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="pseudoTableName">The name of the pseudo table.</param>
        /// <param name="fields">The fields to be used.</param>
        /// <param name="identityField">The identity field.</param>
        /// <param name="qualifiers">The qualifier fields to be used.</param>
        /// <param name="entities">The entities to set the identities to.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The number of affected rows.</returns>
        public static Task<int> MergeFromPseudoTableForReturnIdentityAsync<TEntity>(DuckDBConnection connection,
            string tableName,
            string pseudoTableName,
            IEnumerable<Field> fields,
            Field identityField,
            IEnumerable<Field> qualifiers,
            IList<TEntity> entities,
            ITrace trace = null,
            string traceKey = null,
            DuckDBTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class =>
            MergeFromPseudoTableForReturnIdentityAsync(connection, tableName, pseudoTableName, fields, identityField, qualifiers, GetEntitySetter(entities, identityField), trace, traceKey, transaction, cancellationToken);

        /// <summary>
        /// Merges the pseudo table rows and sets the identities back to the data rows.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="pseudoTableName">The name of the pseudo table.</param>
        /// <param name="fields">The fields to be used.</param>
        /// <param name="identityField">The identity field.</param>
        /// <param name="qualifiers">The qualifier fields to be used.</param>
        /// <param name="rows">The data rows to set the identities to.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of affected rows.</returns>
        public static int MergeFromPseudoTableForReturnIdentityForDataTable(DuckDBConnection connection,
            string tableName,
            string pseudoTableName,
            IEnumerable<Field> fields,
            Field identityField,
            IEnumerable<Field> qualifiers,
            IList<DataRow> rows,
            ITrace trace = null,
            string traceKey = null,
            DuckDBTransaction transaction = null) =>
            MergeFromPseudoTableForReturnIdentity(connection, tableName, pseudoTableName, fields, identityField, qualifiers, GetRowSetter(rows, identityField), trace, traceKey, transaction);

        /// <summary>
        /// Merges the pseudo table rows and sets the identities back to the data rows in an asynchronous way.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="pseudoTableName">The name of the pseudo table.</param>
        /// <param name="fields">The fields to be used.</param>
        /// <param name="identityField">The identity field.</param>
        /// <param name="qualifiers">The qualifier fields to be used.</param>
        /// <param name="rows">The data rows to set the identities to.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The number of affected rows.</returns>
        public static Task<int> MergeFromPseudoTableForReturnIdentityForDataTableAsync(DuckDBConnection connection,
            string tableName,
            string pseudoTableName,
            IEnumerable<Field> fields,
            Field identityField,
            IEnumerable<Field> qualifiers,
            IList<DataRow> rows,
            ITrace trace = null,
            string traceKey = null,
            DuckDBTransaction transaction = null,
            CancellationToken cancellationToken = default) =>
            MergeFromPseudoTableForReturnIdentityAsync(connection, tableName, pseudoTableName, fields, identityField, qualifiers, GetRowSetter(rows, identityField), trace, traceKey, transaction, cancellationToken);

        /// <summary>
        /// Merges the pseudo table rows and sets the identities back to the rows.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="pseudoTableName">The name of the pseudo table.</param>
        /// <param name="fields">The fields to be used.</param>
        /// <param name="identityField">The identity field.</param>
        /// <param name="qualifiers">The qualifier fields to be used.</param>
        /// <param name="setIdentity">The action that sets the identity of a row.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of affected rows.</returns>
        private static int MergeFromPseudoTableForReturnIdentity(DuckDBConnection connection,
            string tableName,
            string pseudoTableName,
            IEnumerable<Field> fields,
            Field identityField,
            IEnumerable<Field> qualifiers,
            Action<int, object> setIdentity,
            ITrace trace,
            string traceKey,
            DuckDBTransaction transaction)
        {
            var dbSetting = connection.GetDbSetting();
            var unmatched = new List<int>();
            var result = 0;

            // Set the identities of the matched rows
            using (var reader = (DbDataReader)connection.ExecuteReader(DuckDbText.GetMergeMatchSnapshotSql(tableName, pseudoTableName, identityField, qualifiers, dbSetting),
                trace: trace, traceKey: traceKey, transaction: transaction))
            {
                while (reader.Read())
                {
                    result += SetMatchedIdentity(reader, unmatched, setIdentity);
                }
            }

            // Update the matched rows
            var updateSql = result > 0 ? DuckDbText.GetMergeUpdateOnlySql(tableName, pseudoTableName, fields, identityField, qualifiers, dbSetting) : null;
            if (updateSql != null)
            {
                connection.ExecuteNonQuery(updateSql, trace: trace, traceKey: traceKey, transaction: transaction);
            }

            // Insert the unmatched rows
            if (unmatched.Count > 0)
            {
                using var reader = (DbDataReader)connection.ExecuteReader(DuckDbText.GetMergeInsertOnlyForReturnIdentitySql(tableName, pseudoTableName, fields, identityField, qualifiers, dbSetting),
                    trace: trace, traceKey: traceKey, transaction: transaction);
                result += ReadIdentities(reader, index => unmatched[index], setIdentity);
            }

            return result;
        }

        /// <summary>
        /// Merges the pseudo table rows and sets the identities back to the rows in an asynchronous way.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="pseudoTableName">The name of the pseudo table.</param>
        /// <param name="fields">The fields to be used.</param>
        /// <param name="identityField">The identity field.</param>
        /// <param name="qualifiers">The qualifier fields to be used.</param>
        /// <param name="setIdentity">The action that sets the identity of a row.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The number of affected rows.</returns>
        private static async Task<int> MergeFromPseudoTableForReturnIdentityAsync(DuckDBConnection connection,
            string tableName,
            string pseudoTableName,
            IEnumerable<Field> fields,
            Field identityField,
            IEnumerable<Field> qualifiers,
            Action<int, object> setIdentity,
            ITrace trace,
            string traceKey,
            DuckDBTransaction transaction,
            CancellationToken cancellationToken)
        {
            var dbSetting = connection.GetDbSetting();
            var unmatched = new List<int>();
            var result = 0;

            // Set the identities of the matched rows
            using (var reader = (DbDataReader)await connection.ExecuteReaderAsync(DuckDbText.GetMergeMatchSnapshotSql(tableName, pseudoTableName, identityField, qualifiers, dbSetting),
                trace: trace, traceKey: traceKey, transaction: transaction, cancellationToken: cancellationToken).ConfigureAwait(false))
            {
                while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
                {
                    result += SetMatchedIdentity(reader, unmatched, setIdentity);
                }
            }

            // Update the matched rows
            var updateSql = result > 0 ? DuckDbText.GetMergeUpdateOnlySql(tableName, pseudoTableName, fields, identityField, qualifiers, dbSetting) : null;
            if (updateSql != null)
            {
                await connection.ExecuteNonQueryAsync(updateSql, trace: trace, traceKey: traceKey, transaction: transaction, cancellationToken: cancellationToken).ConfigureAwait(false);
            }

            // Insert the unmatched rows
            if (unmatched.Count > 0)
            {
                using var reader = (DbDataReader)await connection.ExecuteReaderAsync(DuckDbText.GetMergeInsertOnlyForReturnIdentitySql(tableName, pseudoTableName, fields, identityField, qualifiers, dbSetting),
                    trace: trace, traceKey: traceKey, transaction: transaction, cancellationToken: cancellationToken).ConfigureAwait(false);
                result += await ReadIdentitiesAsync(reader, index => unmatched[index], setIdentity, cancellationToken).ConfigureAwait(false);
            }

            return result;
        }

        /// <summary>
        /// Sets the identity of a matched snapshot row, otherwise records the row as unmatched.
        /// </summary>
        /// <param name="reader">The data reader of the merge snapshot.</param>
        /// <param name="unmatched">The row indexes of the unmatched rows.</param>
        /// <param name="setIdentity">The action that sets the identity of a row.</param>
        /// <returns><c>1</c> if the row is matched; otherwise, <c>0</c>.</returns>
        private static int SetMatchedIdentity(DbDataReader reader,
            List<int> unmatched,
            Action<int, object> setIdentity)
        {
            var index = (int)(Convert.ToInt64(reader.GetValue(0), CultureInfo.InvariantCulture) - 1);
            if (reader.IsDBNull(1))
            {
                unmatched.Add(index);
                return 0;
            }
            setIdentity(index, Converter.DbNullToNull(reader.GetValue(1)));
            return 1;
        }

        #endregion

        #region Update

        /// <summary>
        /// Updates the table from the pseudo table rows.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="pseudoTableName">The name of the pseudo table.</param>
        /// <param name="fields">The fields to be used.</param>
        /// <param name="qualifiers">The qualifier fields to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of affected rows.</returns>
        public static int UpdateFromPseudoTable(DuckDBConnection connection,
            string tableName,
            string pseudoTableName,
            IEnumerable<Field> fields,
            IEnumerable<Field> qualifiers,
            ITrace trace = null,
            string traceKey = null,
            DuckDBTransaction transaction = null) =>
            connection.ExecuteNonQuery(DuckDbText.GetUpdateFromPseudoTableSql(tableName, pseudoTableName, fields, qualifiers, connection.GetDbSetting()),
                trace: trace, traceKey: traceKey, transaction: transaction);

        /// <summary>
        /// Updates the table from the pseudo table rows in an asynchronous way.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="pseudoTableName">The name of the pseudo table.</param>
        /// <param name="fields">The fields to be used.</param>
        /// <param name="qualifiers">The qualifier fields to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The number of affected rows.</returns>
        public static Task<int> UpdateFromPseudoTableAsync(DuckDBConnection connection,
            string tableName,
            string pseudoTableName,
            IEnumerable<Field> fields,
            IEnumerable<Field> qualifiers,
            ITrace trace = null,
            string traceKey = null,
            DuckDBTransaction transaction = null,
            CancellationToken cancellationToken = default) =>
            connection.ExecuteNonQueryAsync(DuckDbText.GetUpdateFromPseudoTableSql(tableName, pseudoTableName, fields, qualifiers, connection.GetDbSetting()),
                trace: trace, traceKey: traceKey, transaction: transaction, cancellationToken: cancellationToken);

        #endregion

        #region Delete

        /// <summary>
        /// Deletes the table rows that match the pseudo table rows.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="pseudoTableName">The name of the pseudo table.</param>
        /// <param name="qualifiers">The qualifier fields to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of affected rows.</returns>
        public static int DeleteFromPseudoTable(DuckDBConnection connection,
            string tableName,
            string pseudoTableName,
            IEnumerable<Field> qualifiers,
            ITrace trace = null,
            string traceKey = null,
            DuckDBTransaction transaction = null) =>
            connection.ExecuteNonQuery(DuckDbText.GetDeleteFromPseudoTableSql(tableName, pseudoTableName, qualifiers, connection.GetDbSetting()),
                trace: trace, traceKey: traceKey, transaction: transaction);

        /// <summary>
        /// Deletes the table rows that match the pseudo table rows in an asynchronous way.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="pseudoTableName">The name of the pseudo table.</param>
        /// <param name="qualifiers">The qualifier fields to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The number of affected rows.</returns>
        public static Task<int> DeleteFromPseudoTableAsync(DuckDBConnection connection,
            string tableName,
            string pseudoTableName,
            IEnumerable<Field> qualifiers,
            ITrace trace = null,
            string traceKey = null,
            DuckDBTransaction transaction = null,
            CancellationToken cancellationToken = default) =>
            connection.ExecuteNonQueryAsync(DuckDbText.GetDeleteFromPseudoTableSql(tableName, pseudoTableName, qualifiers, connection.GetDbSetting()),
                trace: trace, traceKey: traceKey, transaction: transaction, cancellationToken: cancellationToken);

        #endregion
    }
}
