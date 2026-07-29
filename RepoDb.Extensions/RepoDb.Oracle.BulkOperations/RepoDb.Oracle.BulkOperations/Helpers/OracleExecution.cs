using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;
using RepoDb;
using RepoDb.Enumerations.Oracle;

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
        /// Runs the <c>INSERT ... SELECT ... RETURNING ... BULK COLLECT INTO ...</c> statement that moves
        /// every row currently staged in <paramref name="pseudoTableName"/> into <paramref name="tableName"/>,
        /// assigning each generated <paramref name="identityField"/> value back onto the matching element of
        /// <paramref name="entities"/> - position-for-position, in the order returned (see the remarks on
        /// <see cref="OracleText.GetInsertFromPseudoTableForReturnIdentitySql"/> for how that lines up with
        /// the original bulk-write order). Returns the number of rows inserted.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the real, target table.</param>
        /// <param name="pseudoTableName">The name of the staging table that was bulk-written to.</param>
        /// <param name="fields">Every field that was staged and should be inserted.</param>
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
            var commandText = OracleText.GetInsertFromPseudoTableForReturnIdentitySql(tableName, pseudoTableName, fields, identityField, dbSetting);
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
        /// <param name="fields">Every field that was staged and should be inserted.</param>
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
            var commandText = OracleText.GetInsertFromPseudoTableForReturnIdentitySql(tableName, pseudoTableName, fields, identityField, dbSetting);
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
        /// <param name="fields">Every field that was staged and should be inserted.</param>
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
            var commandText = OracleText.GetInsertFromPseudoTableForReturnIdentitySql(tableName, pseudoTableName, fields, identityField, dbSetting);

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
        /// <param name="fields">Every field that was staged and should be inserted.</param>
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
            var commandText = OracleText.GetInsertFromPseudoTableForReturnIdentitySql(tableName, pseudoTableName, fields, identityField, dbSetting);

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
