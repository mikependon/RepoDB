
using System.Threading;
using Oracle.ManagedDataAccess.Client;

namespace RepoDb.Oracle.BulkOperations.Extensions
{
    internal class OracleExecution
    {
        #region Shared

        public static void CreatePseudoTable(OracleConnection connection,
            string tableName,
            string pseudoTableName)
        {
            var commandText = OracleText.GetCreatePseudoTableSql(tableName, pseudoTableName);
            connection.ExecuteNonQuery(commandText);
        }

        public static async void CreatePseudoTableAsync(OracleConnection connection,
            string tableName,
            string pseudoTableName,
            CancellationToken cancellationToken = default)
        {
            var commandText = OracleText.GetCreatePseudoTableSql(tableName, pseudoTableName);
            await connection.ExecuteNonQueryAsync(commandText, cancellationToken: cancellationToken);
        }

        #endregion

        #region Merge

        public static int MergeFromPseudoTable(OracleConnection connection,
            string tableName,
            string pseudoTableName)
        {
            var commandText = OracleText.GetMergeFromPseudoTableSql(tableName, pseudoTableName);
            return connection.ExecuteNonQuery(commandText);
        }

        #endregion
    }
}
