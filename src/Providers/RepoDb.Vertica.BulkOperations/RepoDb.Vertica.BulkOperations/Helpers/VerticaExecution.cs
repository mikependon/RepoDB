#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
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
        /// Inserts the data from the pseudo table into the target table and returns the identity value of the last inserted row.
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
            var commandText = VerticaText.GetInsertFromPseudoTableForReturnIdentitySql(tableName, pseudoTableName, fields, connection.GetDbSetting());
            connection.ExecuteNonQuery(commandText, trace: trace, traceKey: traceKey, transaction: transaction);

            if (entities.Count == 0)
            {
                return 0;
            }

            var lastIdentity = Convert.ToInt64(connection.GetDbHelper().GetScopeIdentity<object>(connection, transaction));
            var setter = FunctionCache.GetDataEntityPropertySetterCompiledFunction(typeof(TEntity), identityField);

            for (var i = 0; i < entities.Count; i++)
            {
                setter?.Invoke(entities[i], lastIdentity - (entities.Count - 1 - i));
            }

            return entities.Count;
        }

        /// <summary>
        /// Asynchronous counterpart of <see cref="InsertFromPseudoTableForReturnIdentity{TEntity}"/>.
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
            var commandText = VerticaText.GetInsertFromPseudoTableForReturnIdentitySql(tableName, pseudoTableName, fields, connection.GetDbSetting());
            await connection.ExecuteNonQueryAsync(commandText, trace: trace, traceKey: traceKey, transaction: transaction, cancellationToken: cancellationToken);

            if (entities.Count == 0)
            {
                return 0;
            }

            var lastIdentity = Convert.ToInt64(await connection.GetDbHelper().GetScopeIdentityAsync<object>(connection, transaction, cancellationToken));
            var setter = FunctionCache.GetDataEntityPropertySetterCompiledFunction(typeof(TEntity), identityField);

            for (var i = 0; i < entities.Count; i++)
            {
                setter?.Invoke(entities[i], lastIdentity - (entities.Count - 1 - i));
            }

            return entities.Count;
        }

        /// <summary>
        /// The <see cref="DataRow"/> counterpart of <see cref="InsertFromPseudoTableForReturnIdentity{TEntity}"/>.
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
            var commandText = VerticaText.GetInsertFromPseudoTableForReturnIdentitySql(tableName, pseudoTableName, fields, connection.GetDbSetting());
            connection.ExecuteNonQuery(commandText, trace: trace, traceKey: traceKey, transaction: transaction);

            if (rows.Count == 0)
            {
                return 0;
            }

            var lastIdentity = Convert.ToInt64(connection.GetDbHelper().GetScopeIdentity<object>(connection, transaction));

            SetIdentityValues(rows, identityField, i => lastIdentity - (rows.Count - 1 - i));

            return rows.Count;
        }

        /// <summary>
        /// Asynchronous counterpart of <see cref="InsertFromPseudoTableForReturnIdentityForDataTable"/>.
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
            var commandText = VerticaText.GetInsertFromPseudoTableForReturnIdentitySql(tableName, pseudoTableName, fields, connection.GetDbSetting());
            await connection.ExecuteNonQueryAsync(commandText, trace: trace, traceKey: traceKey, transaction: transaction, cancellationToken: cancellationToken);

            if (rows.Count == 0)
            {
                return 0;
            }

            var lastIdentity = Convert.ToInt64(await connection.GetDbHelper().GetScopeIdentityAsync<object>(connection, transaction, cancellationToken));

            SetIdentityValues(rows, identityField, i => lastIdentity - (rows.Count - 1 - i));

            return rows.Count;
        }

        /// <summary>
        /// Writes the server-generated identity values back into the source <see cref="DataRow"/> instances.
        /// The identity column is temporarily unmarked as read-only for the duration of the write - <see cref="DataTable.Load(IDataReader)"/>
        /// (e.g. loading from a SELECT against an existing table) marks an identity/computed column as read-only based on the reader's
        /// schema, which would otherwise make every assignment below throw a <see cref="ReadOnlyException"/>.
        /// </summary>
        /// <param name="rows"></param>
        /// <param name="identityField"></param>
        /// <param name="valueSelector">Resolves the identity value to assign for the row at the given index.</param>
        private static void SetIdentityValues(IList<DataRow> rows,
            Field identityField,
            Func<int, object> valueSelector)
        {
            if (rows.Count == 0)
            {
                return;
            }

            var column = rows[0].Table.Columns[identityField.Name];
            var wasReadOnly = column.ReadOnly;

            if (wasReadOnly)
            {
                column.ReadOnly = false;
            }

            try
            {
                for (var i = 0; i < rows.Count; i++)
                {
                    rows[i][identityField.Name] = valueSelector(i);
                }
            }
            finally
            {
                if (wasReadOnly)
                {
                    column.ReadOnly = true;
                }
            }
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
            var updateText = VerticaText.GetMergeUpdateFromPseudoTableSql(tableName, pseudoTableName, fields, qualifiers, identityField, connection.GetDbSetting());
            if (updateText != null)
            {
                connection.ExecuteNonQuery(updateText, trace: trace, traceKey: traceKey, transaction: transaction);
            }
            var insertText = VerticaText.GetMergeInsertFromPseudoTableSql(tableName, pseudoTableName, fields, qualifiers, identityField, connection.GetDbSetting());
            connection.ExecuteNonQuery(insertText, trace: trace, traceKey: traceKey, transaction: transaction);
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
            var updateText = VerticaText.GetMergeUpdateFromPseudoTableSql(tableName, pseudoTableName, fields, qualifiers, identityField, connection.GetDbSetting());
            if (updateText != null)
            {
                await connection.ExecuteNonQueryAsync(updateText, trace: trace, traceKey: traceKey, transaction: transaction, cancellationToken: cancellationToken);
            }
            var insertText = VerticaText.GetMergeInsertFromPseudoTableSql(tableName, pseudoTableName, fields, qualifiers, identityField, connection.GetDbSetting());
            await connection.ExecuteNonQueryAsync(insertText, trace: trace, traceKey: traceKey, transaction: transaction, cancellationToken: cancellationToken);
            var countText = VerticaText.GetPseudoTableRowCountSql(pseudoTableName, connection.GetDbSetting());
            return await connection.ExecuteScalarAsync<int>(countText, transaction: transaction, cancellationToken: cancellationToken);
        }

        /// <summary>
        /// True when the identity column is itself one of the merge qualifiers - the common "merge by primary
        /// key" shape. In that shape, a row's original identity value doubles as the caller's intent: a real,
        /// already-known value means "update this existing row" and the unset 0/null sentinel means "insert a
        /// new row, generate its identity" - see <see cref="IsUnsetIdentityValue"/> and <see cref="ComputeNewRowIdentities"/>.
        /// When the identity column is not a qualifier, its value carries no such meaning and every row's
        /// identity is instead read back via <see cref="VerticaText.GetSelectIdentityAfterMergeSql"/>.
        /// </summary>
        private static bool IsIdentityQualifier(IEnumerable<Field> qualifiers, Field identityField) =>
            qualifiers.Any(q => string.Equals(q.Name, identityField.Name, StringComparison.OrdinalIgnoreCase));

        /// <summary>
        /// True when a value read from a row's identity property/column is the "please generate one" sentinel.
        /// </summary>
        private static bool IsUnsetIdentityValue(object value) =>
            value == null || value is DBNull || Convert.ToInt64(value) == 0;

        /// <summary>
        /// Computes, in insertion order, the identity values Vertica assigned to the rows inserted by the merge's
        /// INSERT statement. Vertica assigns IDENTITY/AUTO_INCREMENT values contiguously in the order rows are
        /// inserted (the INSERT is itself ordered by the pseudo table's row-order column), so the last value of
        /// the underlying sequence minus a descending offset reconstructs every inserted row's value - the same
        /// technique <see cref="InsertFromPseudoTableForReturnIdentity{TEntity}"/> already relies on.
        /// </summary>
        private static long[] ComputeNewRowIdentities(VerticaConnection connection,
            int insertedCount,
            VerticaTransaction transaction)
        {
            if (insertedCount == 0)
            {
                return [];
            }

            var lastIdentity = Convert.ToInt64(connection.GetDbHelper().GetScopeIdentity<object>(connection, transaction));
            var identities = new long[insertedCount];

            for (var i = 0; i < insertedCount; i++)
            {
                identities[i] = lastIdentity - (insertedCount - 1 - i);
            }

            return identities;
        }

        /// <summary>
        /// Asynchronous counterpart of <see cref="ComputeNewRowIdentities"/>.
        /// </summary>
        private static async Task<long[]> ComputeNewRowIdentitiesAsync(VerticaConnection connection,
            int insertedCount,
            VerticaTransaction transaction,
            CancellationToken cancellationToken)
        {
            if (insertedCount == 0)
            {
                return [];
            }

            var lastIdentity = Convert.ToInt64(await connection.GetDbHelper().GetScopeIdentityAsync<object>(connection, transaction, cancellationToken));
            var identities = new long[insertedCount];

            for (var i = 0; i < insertedCount; i++)
            {
                identities[i] = lastIdentity - (insertedCount - 1 - i);
            }

            return identities;
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
            var dbSetting = connection.GetDbSetting();
            var qualifierList = qualifiers.AsList();
            var identityIsQualifier = IsIdentityQualifier(qualifierList, identityField);

            var updateText = VerticaText.GetMergeUpdateFromPseudoTableSql(tableName, pseudoTableName, fields, qualifierList, identityField, dbSetting);
            if (updateText != null)
            {
                connection.ExecuteNonQuery(updateText, trace: trace, traceKey: traceKey, transaction: transaction);
            }
            var insertText = VerticaText.GetMergeInsertFromPseudoTableSql(tableName, pseudoTableName, fields, qualifierList, identityField, dbSetting);
            var insertedCount = connection.ExecuteNonQuery(insertText, trace: trace, traceKey: traceKey, transaction: transaction);

            var setter = FunctionCache.GetDataEntityPropertySetterCompiledFunction(typeof(TEntity), identityField);

            if (identityIsQualifier)
            {
                var getter = PropertyCache.Get(typeof(TEntity), identityField, true)?.PropertyInfo;
                var newIdentities = ComputeNewRowIdentities(connection, insertedCount, transaction);
                var newIndex = 0;

                for (var i = 0; i < entities.Count; i++)
                {
                    if (IsUnsetIdentityValue(getter?.GetValue(entities[i])))
                    {
                        setter?.Invoke(entities[i], newIdentities[newIndex]);
                        newIndex++;
                    }
                }

                return entities.Count;
            }

            var selectText = VerticaText.GetSelectIdentityAfterMergeSql(tableName, pseudoTableName, qualifierList, identityField, dbSetting);

            using var command = CreateReaderCommand(connection, selectText, transaction);
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
            var dbSetting = connection.GetDbSetting();
            var qualifierList = qualifiers.AsList();
            var identityIsQualifier = IsIdentityQualifier(qualifierList, identityField);

            var updateText = VerticaText.GetMergeUpdateFromPseudoTableSql(tableName, pseudoTableName, fields, qualifierList, identityField, dbSetting);
            if (updateText != null)
            {
                await connection.ExecuteNonQueryAsync(updateText, trace: trace, traceKey: traceKey, transaction: transaction, cancellationToken: cancellationToken);
            }
            var insertText = VerticaText.GetMergeInsertFromPseudoTableSql(tableName, pseudoTableName, fields, qualifierList, identityField, dbSetting);
            var insertedCount = await connection.ExecuteNonQueryAsync(insertText, trace: trace, traceKey: traceKey, transaction: transaction, cancellationToken: cancellationToken);

            var setter = FunctionCache.GetDataEntityPropertySetterCompiledFunction(typeof(TEntity), identityField);

            if (identityIsQualifier)
            {
                var getter = PropertyCache.Get(typeof(TEntity), identityField, true)?.PropertyInfo;
                var newIdentities = await ComputeNewRowIdentitiesAsync(connection, insertedCount, transaction, cancellationToken);
                var newIndex = 0;

                for (var i = 0; i < entities.Count; i++)
                {
                    if (IsUnsetIdentityValue(getter?.GetValue(entities[i])))
                    {
                        setter?.Invoke(entities[i], newIdentities[newIndex]);
                        newIndex++;
                    }
                }

                return entities.Count;
            }

            var selectText = VerticaText.GetSelectIdentityAfterMergeSql(tableName, pseudoTableName, qualifierList, identityField, dbSetting);

            using var command = CreateReaderCommand(connection, selectText, transaction);
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
            var dbSetting = connection.GetDbSetting();
            var qualifierList = qualifiers.AsList();
            var identityIsQualifier = IsIdentityQualifier(qualifierList, identityField);

            var updateText = VerticaText.GetMergeUpdateFromPseudoTableSql(tableName, pseudoTableName, fields, qualifierList, identityField, dbSetting);
            if (updateText != null)
            {
                connection.ExecuteNonQuery(updateText, trace: trace, traceKey: traceKey, transaction: transaction);
            }
            var insertText = VerticaText.GetMergeInsertFromPseudoTableSql(tableName, pseudoTableName, fields, qualifierList, identityField, dbSetting);
            var insertedCount = connection.ExecuteNonQuery(insertText, trace: trace, traceKey: traceKey, transaction: transaction);

            if (identityIsQualifier)
            {
                var newIdentities = ComputeNewRowIdentities(connection, insertedCount, transaction);
                var newIndex = 0;

                SetIdentityValues(rows, identityField, i =>
                {
                    var current = rows[i][identityField.Name];
                    return IsUnsetIdentityValue(current) ? newIdentities[newIndex++] : current;
                });

                return rows.Count;
            }

            var selectText = VerticaText.GetSelectIdentityAfterMergeSql(tableName, pseudoTableName, qualifierList, identityField, dbSetting);
            var values = new List<object>();

            using (var command = CreateReaderCommand(connection, selectText, transaction))
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    values.Add(Converter.DbNullToNull(reader.GetValue(0)));
                }
            }

            SetIdentityValues(rows, identityField, i => values[i]);

            return values.Count;
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
            var dbSetting = connection.GetDbSetting();
            var qualifierList = qualifiers.AsList();
            var identityIsQualifier = IsIdentityQualifier(qualifierList, identityField);

            var updateText = VerticaText.GetMergeUpdateFromPseudoTableSql(tableName, pseudoTableName, fields, qualifierList, identityField, dbSetting);
            if (updateText != null)
            {
                await connection.ExecuteNonQueryAsync(updateText, trace: trace, traceKey: traceKey, transaction: transaction, cancellationToken: cancellationToken);
            }
            var insertText = VerticaText.GetMergeInsertFromPseudoTableSql(tableName, pseudoTableName, fields, qualifierList, identityField, dbSetting);
            var insertedCount = await connection.ExecuteNonQueryAsync(insertText, trace: trace, traceKey: traceKey, transaction: transaction, cancellationToken: cancellationToken);

            if (identityIsQualifier)
            {
                var newIdentities = await ComputeNewRowIdentitiesAsync(connection, insertedCount, transaction, cancellationToken);
                var newIndex = 0;

                SetIdentityValues(rows, identityField, i =>
                {
                    var current = rows[i][identityField.Name];
                    return IsUnsetIdentityValue(current) ? newIdentities[newIndex++] : current;
                });

                return rows.Count;
            }

            var selectText = VerticaText.GetSelectIdentityAfterMergeSql(tableName, pseudoTableName, qualifierList, identityField, dbSetting);
            var values = new List<object>();

            using (var command = CreateReaderCommand(connection, selectText, transaction))
            using (var reader = await command.ExecuteReaderAsync(cancellationToken))
            {
                while (await reader.ReadAsync(cancellationToken))
                {
                    values.Add(Converter.DbNullToNull(reader.GetValue(0)));
                }
            }

            SetIdentityValues(rows, identityField, i => values[i]);

            return values.Count;
        }

        #endregion

        #region Update

        /// <summary>
        /// Updates the target table from the pseudo table's rows. Reuses <see cref="VerticaText.GetMergeUpdateFromPseudoTableSql"/> -
        /// a bulk update is just the update half of a merge, and Vertica's identity-column restrictions apply here too.
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
        public static int UpdateFromPseudoTable(VerticaConnection connection,
            string tableName,
            string pseudoTableName,
            IEnumerable<Field> fields,
            IEnumerable<Field> qualifiers,
            Field identityField,
            ITrace trace = null,
            string traceKey = null,
            VerticaTransaction transaction = null)
        {
            var commandText = VerticaText.GetMergeUpdateFromPseudoTableSql(tableName, pseudoTableName, fields, qualifiers, identityField, connection.GetDbSetting());
            return commandText == null ? 0 : connection.ExecuteNonQuery(commandText, trace: trace, traceKey: traceKey, transaction: transaction);
        }

        /// <summary>
        /// Asynchronous counterpart of <see cref="UpdateFromPseudoTable"/>.
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
        public static async Task<int> UpdateFromPseudoTableAsync(VerticaConnection connection,
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
            var commandText = VerticaText.GetMergeUpdateFromPseudoTableSql(tableName, pseudoTableName, fields, qualifiers, identityField, connection.GetDbSetting());
            return commandText == null ? 0 : await connection.ExecuteNonQueryAsync(commandText, trace: trace, traceKey: traceKey, transaction: transaction, cancellationToken: cancellationToken);
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
