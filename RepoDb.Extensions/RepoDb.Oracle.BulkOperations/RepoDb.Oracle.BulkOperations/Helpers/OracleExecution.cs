using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;
using RepoDb;
using RepoDb.Enumerations.Oracle;
using RepoDb.Extensions;

namespace RepoDb.Oracle.BulkOperations.Extensions
{
    /// <summary>
    /// Thin execution layer over <see cref="OracleText"/> - builds the SQL text for a step and runs it
    /// against <paramref name="connection"/>, optionally enlisted in <paramref name="transaction"/>.
    /// </summary>
    internal static class OracleExecution
    {
        #region Shared

        /// <summary>
        /// Creates the staging/pseudo table for <paramref name="tableName"/>, if it does not already
        /// exist. See the remarks on <see cref="OracleText.GetCreatePseudoTableSql"/>.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the real, target table the pseudo table is modeled after.</param>
        /// <param name="pseudoTableName">The name of the staging/pseudo table to create.</param>
        /// <param name="pseudoTableType">Whether the pseudo table is a <c>Physical</c> heap table or a <c>Memory</c> (Global Temporary Table) one. <c>Auto</c> must already be resolved to one of these by the caller.</param>
        /// <param name="qualifierField">When provided, the pseudo table is projected down to just this one column - see <see cref="OracleText.GetCreatePseudoTableSql"/>.</param>
        /// <param name="transaction">The transaction to be used.</param>
        public static void CreatePseudoTable(OracleConnection connection,
            string tableName,
            string pseudoTableName,
            OracleBulkImportPseudoTableType pseudoTableType,
            Field qualifierField = null,
            OracleTransaction transaction = null)
        {
            var dbSetting = connection.GetDbSetting();
            var commandText = OracleText.GetCreatePseudoTableSql(tableName, pseudoTableName, pseudoTableType, dbSetting, qualifierField);
            connection.ExecuteNonQuery(commandText, transaction: transaction);
        }

        /// <summary>
        /// Asynchronous counterpart of <see cref="CreatePseudoTable"/> - see its remarks for the detailed
        /// behavior (identical here).
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the real, target table the pseudo table is modeled after.</param>
        /// <param name="pseudoTableName">The name of the staging/pseudo table to create.</param>
        /// <param name="pseudoTableType">Whether the pseudo table is a <c>Physical</c> heap table or a <c>Memory</c> (Global Temporary Table) one. <c>Auto</c> must already be resolved to one of these by the caller.</param>
        /// <param name="qualifierField">When provided, the pseudo table is projected down to just this one column - see <see cref="OracleText.GetCreatePseudoTableSql"/>.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The token to cancel the asynchronous operation.</param>
        public static async Task CreatePseudoTableAsync(OracleConnection connection,
            string tableName,
            string pseudoTableName,
            OracleBulkImportPseudoTableType pseudoTableType,
            Field qualifierField = null,
            OracleTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            var dbSetting = connection.GetDbSetting();
            var commandText = OracleText.GetCreatePseudoTableSql(tableName, pseudoTableName, pseudoTableType, dbSetting, qualifierField);
            await connection.ExecuteNonQueryAsync(commandText, transaction: transaction, cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Clears out any rows left over in the staging/pseudo table from a prior bulk operation on the
        /// same session before it is written to again. See the remarks on <see cref="OracleText.GetTruncatePseudoTableSql"/>.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="pseudoTableName">The name of the staging/pseudo table to truncate.</param>
        /// <param name="transaction">The transaction to be used.</param>
        public static void TruncatePseudoTable(OracleConnection connection,
            string pseudoTableName,
            OracleTransaction transaction = null)
        {
            var dbSetting = connection.GetDbSetting();
            var commandText = OracleText.GetTruncatePseudoTableSql(pseudoTableName, dbSetting);
            connection.ExecuteNonQuery(commandText, transaction: transaction);
        }

        /// <summary>
        /// Asynchronous counterpart of <see cref="TruncatePseudoTable"/> - see its remarks for the
        /// detailed behavior (identical here).
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="pseudoTableName">The name of the staging/pseudo table to truncate.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The token to cancel the asynchronous operation.</param>
        public static async Task TruncatePseudoTableAsync(OracleConnection connection,
            string pseudoTableName,
            OracleTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            var dbSetting = connection.GetDbSetting();
            var commandText = OracleText.GetTruncatePseudoTableSql(pseudoTableName, dbSetting);
            await connection.ExecuteNonQueryAsync(commandText, transaction: transaction, cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Drops the staging/pseudo table for maximum cleanup once a bulk operation is done with it -
        /// see the remarks on <see cref="OracleText.GetDropPseudoTableSql"/>.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="pseudoTableName">The name of the staging/pseudo table to drop.</param>
        /// <param name="transaction">The transaction to be used.</param>
        public static void DropPseudoTable(OracleConnection connection,
            string pseudoTableName,
            OracleTransaction transaction = null)
        {
            var dbSetting = connection.GetDbSetting();
            var commandText = OracleText.GetDropPseudoTableSql(pseudoTableName, dbSetting);
            connection.ExecuteNonQuery(commandText, transaction: transaction);
        }

        /// <summary>
        /// Asynchronous counterpart of <see cref="DropPseudoTable"/> - see its remarks for the detailed
        /// behavior (identical here).
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="pseudoTableName">The name of the staging/pseudo table to drop.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The token to cancel the asynchronous operation.</param>
        public static async Task DropPseudoTableAsync(OracleConnection connection,
            string pseudoTableName,
            OracleTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            var dbSetting = connection.GetDbSetting();
            var commandText = OracleText.GetDropPseudoTableSql(pseudoTableName, dbSetting);
            await connection.ExecuteNonQueryAsync(commandText, transaction: transaction, cancellationToken: cancellationToken);
        }

        #endregion

        #region Insert

        /// <summary>
        /// Drops the <c>NOT NULL</c> constraint that <see cref="CreatePseudoTable"/> can carry over onto a
        /// staging table column - see the remarks on <see cref="OracleText.GetAllowNullForColumnSql"/> for why
        /// this exists.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="pseudoTableName">The name of the staging/pseudo table.</param>
        /// <param name="columnName">The column to allow <c>NULL</c> for.</param>
        /// <param name="transaction">The transaction to be used.</param>
        public static void AllowNullForColumn(OracleConnection connection,
            string pseudoTableName,
            string columnName,
            OracleTransaction transaction = null)
        {
            var dbSetting = connection.GetDbSetting();
            var commandText = OracleText.GetAllowNullForColumnSql(pseudoTableName, columnName, dbSetting);
            connection.ExecuteNonQuery(commandText, transaction: transaction);
        }

        /// <summary>
        /// Asynchronous counterpart of <see cref="AllowNullForColumn"/> - see its remarks for the detailed
        /// behavior (identical here).
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="pseudoTableName">The name of the staging/pseudo table.</param>
        /// <param name="columnName">The column to allow <c>NULL</c> for.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The token to cancel the asynchronous operation.</param>
        public static async Task AllowNullForColumnAsync(OracleConnection connection,
            string pseudoTableName,
            string columnName,
            OracleTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            var dbSetting = connection.GetDbSetting();
            var commandText = OracleText.GetAllowNullForColumnSql(pseudoTableName, columnName, dbSetting);
            await connection.ExecuteNonQueryAsync(commandText, transaction: transaction, cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Resolves the sequence (and its <c>ALWAYS</c>/<c>BY DEFAULT</c> generation mode) backing
        /// <paramref name="identityField"/> - see the remarks on <see cref="OracleText.GetIdentitySequenceMetadataSql"/>
        /// for why this lookup exists and why it is guaranteed to find a match.
        /// </summary>
        private static (string SequenceName, bool IsAlwaysGenerated) GetIdentitySequenceMetadata(OracleConnection connection,
            string tableName,
            Field identityField,
            OracleTransaction transaction)
        {
            var dbSetting = connection.GetDbSetting();
            var commandText = OracleText.GetIdentitySequenceMetadataSql();
            var param = new
            {
                Schema = DataEntityExtension.GetSchema(tableName, dbSetting)?.AsUnquoted(dbSetting),
                TableName = DataEntityExtension.GetTableName(tableName, dbSetting).AsUnquoted(dbSetting),
                ColumnName = identityField.Name.AsUnquoted(dbSetting)
            };

            using var reader = (DbDataReader)connection.ExecuteReader(commandText, param: param, transaction: transaction);
            reader.Read();
            return (reader.GetString(0), string.Equals(reader.GetString(1), "ALWAYS", StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Asynchronous counterpart of <see cref="GetIdentitySequenceMetadata"/> - see its remarks for the
        /// detailed behavior (identical here).
        /// </summary>
        private static async Task<(string SequenceName, bool IsAlwaysGenerated)> GetIdentitySequenceMetadataAsync(OracleConnection connection,
            string tableName,
            Field identityField,
            OracleTransaction transaction,
            CancellationToken cancellationToken)
        {
            var dbSetting = connection.GetDbSetting();
            var commandText = OracleText.GetIdentitySequenceMetadataSql();
            var param = new
            {
                Schema = DataEntityExtension.GetSchema(tableName, dbSetting)?.AsUnquoted(dbSetting),
                TableName = DataEntityExtension.GetTableName(tableName, dbSetting).AsUnquoted(dbSetting),
                ColumnName = identityField.Name.AsUnquoted(dbSetting)
            };

            using var reader = (DbDataReader)await connection.ExecuteReaderAsync(commandText, param: param, transaction: transaction, cancellationToken: cancellationToken);
            await reader.ReadAsync(cancellationToken);
            return (reader.GetString(0), string.Equals(reader.GetString(1), "ALWAYS", StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Runs the statement that pre-generates an identity value (via the backing sequence's <c>NEXTVAL</c>)
        /// for every row currently staged in <paramref name="pseudoTableName"/>, moves the now fully-populated
        /// rows into <paramref name="tableName"/>, and assigns each generated <paramref name="identityField"/>
        /// value back onto the matching element of <paramref name="entities"/> - position-for-position, in the
        /// order returned (see the remarks on <see cref="OracleText.GetInsertFromPseudoTableForReturnIdentitySql"/>
        /// for how that lines up with the original bulk-write order, and for why this doesn't just use
        /// <c>RETURNING</c>). Returns the number of rows inserted.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the real, target table.</param>
        /// <param name="pseudoTableName">The name of the staging table that was bulk-written to.</param>
        /// <param name="fields">Every field that was staged and should be inserted (including <paramref name="identityField"/>).</param>
        /// <param name="identityField">The identity column whose generated values are assigned back onto <paramref name="entities"/>.</param>
        /// <param name="entities">The entities - in the same order they were bulk-written into <paramref name="pseudoTableName"/> - to assign the generated identity values back onto.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows inserted.</returns>
        public static int InsertFromPseudoTableForReturnIdentity<TEntity>(OracleConnection connection,
            string tableName,
            string pseudoTableName,
            IEnumerable<Field> fields,
            Field identityField,
            IList<TEntity> entities,
            OracleTransaction transaction = null)
            where TEntity : class
        {
            var dbSetting = connection.GetDbSetting();
            var (sequenceName, isAlwaysGenerated) = GetIdentitySequenceMetadata(connection, tableName, identityField, transaction);
            var commandText = OracleText.GetInsertFromPseudoTableForReturnIdentitySql(tableName, pseudoTableName, fields, identityField, sequenceName, isAlwaysGenerated, dbSetting);
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
        /// Asynchronous counterpart of <see cref="InsertFromPseudoTableForReturnIdentity{TEntity}"/> - see
        /// its remarks for the detailed behavior (identical here).
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the real, target table.</param>
        /// <param name="pseudoTableName">The name of the staging table that was bulk-written to.</param>
        /// <param name="fields">Every field that was staged and should be inserted (including <paramref name="identityField"/>).</param>
        /// <param name="identityField">The identity column whose generated values are assigned back onto <paramref name="entities"/>.</param>
        /// <param name="entities">The entities - in the same order they were bulk-written into <paramref name="pseudoTableName"/> - to assign the generated identity values back onto.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The token to cancel the asynchronous operation.</param>
        /// <returns>The number of rows inserted.</returns>
        public static async Task<int> InsertFromPseudoTableForReturnIdentityAsync<TEntity>(OracleConnection connection,
            string tableName,
            string pseudoTableName,
            IEnumerable<Field> fields,
            Field identityField,
            IList<TEntity> entities,
            OracleTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            var dbSetting = connection.GetDbSetting();
            var (sequenceName, isAlwaysGenerated) = await GetIdentitySequenceMetadataAsync(connection, tableName, identityField, transaction, cancellationToken);
            var commandText = OracleText.GetInsertFromPseudoTableForReturnIdentitySql(tableName, pseudoTableName, fields, identityField, sequenceName, isAlwaysGenerated, dbSetting);
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
        /// <see cref="DataRow"/> counterpart of <see cref="InsertFromPseudoTableForReturnIdentity{TEntity}"/> -
        /// see its remarks for the detailed behavior (identical here), except the generated identity values
        /// are assigned back onto <paramref name="rows"/>' <paramref name="identityField"/> column instead of
        /// an entity property (there is no compiled property setter to reuse for a <see cref="DataTable"/> row).
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the real, target table.</param>
        /// <param name="pseudoTableName">The name of the staging table that was bulk-written to.</param>
        /// <param name="fields">Every field that was staged and should be inserted (including <paramref name="identityField"/>).</param>
        /// <param name="identityField">The identity column whose generated values are assigned back onto <paramref name="rows"/>.</param>
        /// <param name="rows">The rows - in the same order they were bulk-written into <paramref name="pseudoTableName"/> - to assign the generated identity values back onto.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows inserted.</returns>
        public static int InsertFromPseudoTableForReturnIdentityForDataTable(OracleConnection connection,
            string tableName,
            string pseudoTableName,
            IEnumerable<Field> fields,
            Field identityField,
            IList<DataRow> rows,
            OracleTransaction transaction = null)
        {
            var dbSetting = connection.GetDbSetting();
            var (sequenceName, isAlwaysGenerated) = GetIdentitySequenceMetadata(connection, tableName, identityField, transaction);
            var commandText = OracleText.GetInsertFromPseudoTableForReturnIdentitySql(tableName, pseudoTableName, fields, identityField, sequenceName, isAlwaysGenerated, dbSetting);

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
        /// Asynchronous counterpart of <see cref="InsertFromPseudoTableForReturnIdentityForDataTable"/> - see
        /// its remarks for the detailed behavior (identical here).
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the real, target table.</param>
        /// <param name="pseudoTableName">The name of the staging table that was bulk-written to.</param>
        /// <param name="fields">Every field that was staged and should be inserted (including <paramref name="identityField"/>).</param>
        /// <param name="identityField">The identity column whose generated values are assigned back onto <paramref name="rows"/>.</param>
        /// <param name="rows">The rows - in the same order they were bulk-written into <paramref name="pseudoTableName"/> - to assign the generated identity values back onto.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The token to cancel the asynchronous operation.</param>
        /// <returns>The number of rows inserted.</returns>
        public static async Task<int> InsertFromPseudoTableForReturnIdentityForDataTableAsync(OracleConnection connection,
            string tableName,
            string pseudoTableName,
            IEnumerable<Field> fields,
            Field identityField,
            IList<DataRow> rows,
            OracleTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            var dbSetting = connection.GetDbSetting();
            var (sequenceName, isAlwaysGenerated) = await GetIdentitySequenceMetadataAsync(connection, tableName, identityField, transaction, cancellationToken);
            var commandText = OracleText.GetInsertFromPseudoTableForReturnIdentitySql(tableName, pseudoTableName, fields, identityField, sequenceName, isAlwaysGenerated, dbSetting);

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
        /// Runs the <c>MERGE</c> statement that upserts every row currently staged in
        /// <paramref name="pseudoTableName"/> into <paramref name="tableName"/>. See the remarks on
        /// <see cref="OracleText.GetMergeFromPseudoTableSql"/>.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the real, target table.</param>
        /// <param name="pseudoTableName">The name of the staging table that was bulk-written to.</param>
        /// <param name="fields">Every field that was staged and should be merged (inserted and/or updated).</param>
        /// <param name="qualifiers">The field(s) used to match an existing row (the <c>ON</c> clause).</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows affected by the <c>MERGE</c>.</returns>
        public static int MergeFromPseudoTable(OracleConnection connection,
            string tableName,
            string pseudoTableName,
            IEnumerable<Field> fields,
            IEnumerable<Field> qualifiers,
            OracleTransaction transaction = null)
        {
            var dbSetting = connection.GetDbSetting();
            var commandText = OracleText.GetMergeFromPseudoTableSql(tableName, pseudoTableName, fields, qualifiers, dbSetting);
            return connection.ExecuteNonQuery(commandText, transaction: transaction);
        }

        /// <summary>
        /// Asynchronous counterpart of <see cref="MergeFromPseudoTable"/> - see its remarks for the
        /// detailed behavior (identical here).
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the real, target table.</param>
        /// <param name="pseudoTableName">The name of the staging table that was bulk-written to.</param>
        /// <param name="fields">Every field that was staged and should be merged (inserted and/or updated).</param>
        /// <param name="qualifiers">The field(s) used to match an existing row (the <c>ON</c> clause).</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The token to cancel the asynchronous operation.</param>
        /// <returns>The number of rows affected by the <c>MERGE</c>.</returns>
        public static async Task<int> MergeFromPseudoTableAsync(OracleConnection connection,
            string tableName,
            string pseudoTableName,
            IEnumerable<Field> fields,
            IEnumerable<Field> qualifiers,
            OracleTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            var dbSetting = connection.GetDbSetting();
            var commandText = OracleText.GetMergeFromPseudoTableSql(tableName, pseudoTableName, fields, qualifiers, dbSetting);
            return await connection.ExecuteNonQueryAsync(commandText, transaction: transaction, cancellationToken: cancellationToken);
        }

        #endregion

        #region Update

        /// <summary>
        /// Runs the <c>MERGE ... WHEN MATCHED THEN UPDATE</c> statement that updates every row on
        /// <paramref name="tableName"/> matched by a row currently staged in <paramref name="pseudoTableName"/>.
        /// See the remarks on <see cref="OracleText.GetUpdateFromPseudoTableSql"/>.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the real, target table.</param>
        /// <param name="pseudoTableName">The name of the staging table that was bulk-written to.</param>
        /// <param name="fields">Every field that was staged (the qualifier(s) plus every field to update).</param>
        /// <param name="qualifiers">The field(s) used to match an existing row (the <c>ON</c> clause).</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows updated.</returns>
        public static int UpdateFromPseudoTable(OracleConnection connection,
            string tableName,
            string pseudoTableName,
            IEnumerable<Field> fields,
            IEnumerable<Field> qualifiers,
            OracleTransaction transaction = null)
        {
            var dbSetting = connection.GetDbSetting();
            var commandText = OracleText.GetUpdateFromPseudoTableSql(tableName, pseudoTableName, fields, qualifiers, dbSetting);
            return connection.ExecuteNonQuery(commandText, transaction: transaction);
        }

        /// <summary>
        /// Asynchronous counterpart of <see cref="UpdateFromPseudoTable"/> - see its remarks for the
        /// detailed behavior (identical here).
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the real, target table.</param>
        /// <param name="pseudoTableName">The name of the staging table that was bulk-written to.</param>
        /// <param name="fields">Every field that was staged (the qualifier(s) plus every field to update).</param>
        /// <param name="qualifiers">The field(s) used to match an existing row (the <c>ON</c> clause).</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The token to cancel the asynchronous operation.</param>
        /// <returns>The number of rows updated.</returns>
        public static async Task<int> UpdateFromPseudoTableAsync(OracleConnection connection,
            string tableName,
            string pseudoTableName,
            IEnumerable<Field> fields,
            IEnumerable<Field> qualifiers,
            OracleTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            var dbSetting = connection.GetDbSetting();
            var commandText = OracleText.GetUpdateFromPseudoTableSql(tableName, pseudoTableName, fields, qualifiers, dbSetting);
            return await connection.ExecuteNonQueryAsync(commandText, transaction: transaction, cancellationToken: cancellationToken);
        }

        #endregion

        #region Delete

        /// <summary>
        /// Runs the <c>DELETE ... WHERE ROWID IN (SELECT ... INNER JOIN ...)</c> statement that removes every row on
        /// <paramref name="tableName"/> matched by a row currently staged in <paramref name="pseudoTableName"/>.
        /// See the remarks on <see cref="OracleText.GetDeleteFromPseudoTableSql"/>.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the real, target table.</param>
        /// <param name="pseudoTableName">The name of the staging table that was bulk-written to.</param>
        /// <param name="qualifiers">The field(s) used to match an existing row for deletion.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows deleted.</returns>
        public static int DeleteFromPseudoTable(OracleConnection connection,
            string tableName,
            string pseudoTableName,
            IEnumerable<Field> qualifiers,
            OracleTransaction transaction = null)
        {
            var dbSetting = connection.GetDbSetting();
            var commandText = OracleText.GetDeleteFromPseudoTableSql(tableName, pseudoTableName, qualifiers, dbSetting);
            return connection.ExecuteNonQuery(commandText, transaction: transaction);
        }

        /// <summary>
        /// Asynchronous counterpart of <see cref="DeleteFromPseudoTable"/> - see its remarks for the
        /// detailed behavior (identical here).
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the real, target table.</param>
        /// <param name="pseudoTableName">The name of the staging table that was bulk-written to.</param>
        /// <param name="qualifiers">The field(s) used to match an existing row for deletion.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The token to cancel the asynchronous operation.</param>
        /// <returns>The number of rows deleted.</returns>
        public static async Task<int> DeleteFromPseudoTableAsync(OracleConnection connection,
            string tableName,
            string pseudoTableName,
            IEnumerable<Field> qualifiers,
            OracleTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            var dbSetting = connection.GetDbSetting();
            var commandText = OracleText.GetDeleteFromPseudoTableSql(tableName, pseudoTableName, qualifiers, dbSetting);
            return await connection.ExecuteNonQueryAsync(commandText, transaction: transaction, cancellationToken: cancellationToken);
        }

        #endregion
    }
}
