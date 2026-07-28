using System.Collections.Generic;
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

        public static void CreatePseudoTable(OracleConnection connection,
            string tableName,
            string pseudoTableName,
            OracleBulkImportPseudoTableType pseudoTableType,
            OracleTransaction transaction = null)
        {
            var dbSetting = connection.GetDbSetting();
            var commandText = OracleText.GetCreatePseudoTableSql(tableName, pseudoTableName, pseudoTableType, dbSetting);
            connection.ExecuteNonQuery(commandText, transaction: transaction);
        }

        public static async Task CreatePseudoTableAsync(OracleConnection connection,
            string tableName,
            string pseudoTableName,
            OracleBulkImportPseudoTableType pseudoTableType,
            OracleTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            var dbSetting = connection.GetDbSetting();
            var commandText = OracleText.GetCreatePseudoTableSql(tableName, pseudoTableName, pseudoTableType, dbSetting);
            await connection.ExecuteNonQueryAsync(commandText, transaction: transaction, cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Clears out any rows left over in the staging/pseudo table from a prior bulk operation on the
        /// same session before it is written to again. See the remarks on <see cref="OracleText.GetTruncatePseudoTableSql"/>.
        /// </summary>
        public static void TruncatePseudoTable(OracleConnection connection,
            string pseudoTableName,
            OracleTransaction transaction = null)
        {
            var dbSetting = connection.GetDbSetting();
            var commandText = OracleText.GetTruncatePseudoTableSql(pseudoTableName, dbSetting);
            connection.ExecuteNonQuery(commandText, transaction: transaction);
        }

        public static async Task TruncatePseudoTableAsync(OracleConnection connection,
            string pseudoTableName,
            OracleTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            var dbSetting = connection.GetDbSetting();
            var commandText = OracleText.GetTruncatePseudoTableSql(pseudoTableName, dbSetting);
            await connection.ExecuteNonQueryAsync(commandText, transaction: transaction, cancellationToken: cancellationToken);
        }

        #endregion

        #region Merge

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
    }
}
