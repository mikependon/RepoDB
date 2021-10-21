using Npgsql;
using RepoDb.Enumerations.PostgreSql;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.PostgreSql.BulkOperations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RepoDb
{
    public static partial class NpgsqlConnectionExtension
    {
        #region Others

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="getPseudoTableName"></param>
        /// <param name="mappings"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="identityBehavior"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="dbSetting"></param>
        /// <param name="transaction"></param>
        private static void CreatePseudoTable(NpgsqlConnection connection,
            string tableName,
            Func<string> getPseudoTableName,
            IEnumerable<NpgsqlBulkInsertMapItem> mappings,
            int? bulkCopyTimeout = null,
            BulkImportIdentityBehavior identityBehavior = default,
            BulkImportPseudoTableType pseudoTableType = default,
            IDbSetting dbSetting = null,
            NpgsqlTransaction transaction = null)
        {
            var commandText = pseudoTableType == BulkImportPseudoTableType.Physical ?
                GetCreatePseudoTableCommandText(tableName, getPseudoTableName(), mappings, identityBehavior, dbSetting) :
                GetCreatePseudoTemporaryTableCommandText(tableName, getPseudoTableName(), mappings, identityBehavior, dbSetting);

            connection.ExecuteNonQuery(commandText,
                bulkCopyTimeout,
                transaction: transaction);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="getPseudoTableName"></param>
        /// <param name="mappings"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="identityBehavior"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="dbSetting"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        private static async Task CreatePseudoTableAsync(NpgsqlConnection connection,
            string tableName,
            Func<string> getPseudoTableName,
            IEnumerable<NpgsqlBulkInsertMapItem> mappings,
            int? bulkCopyTimeout = null,
            BulkImportIdentityBehavior identityBehavior = default,
            BulkImportPseudoTableType pseudoTableType = default,
            IDbSetting dbSetting = null,
            NpgsqlTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            var commandText = pseudoTableType == BulkImportPseudoTableType.Physical ?
                GetCreatePseudoTableCommandText(tableName, getPseudoTableName(), mappings, identityBehavior, dbSetting) :
                GetCreatePseudoTemporaryTableCommandText(tableName, getPseudoTableName(), mappings, identityBehavior, dbSetting);

            await connection.ExecuteNonQueryAsync(commandText,
                bulkCopyTimeout,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="getMergeToPseudoCommandText"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private static IEnumerable<long> MergeToPseudoTable(NpgsqlConnection connection,
            Func<string> getMergeToPseudoCommandText,
            int? bulkCopyTimeout = null,
            NpgsqlTransaction transaction = null)
        {
            var commandText = getMergeToPseudoCommandText();

            return connection.ExecuteQuery<long>(commandText,
                bulkCopyTimeout,
                transaction: transaction);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="getMergeToPseudoCommandText"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        private static async Task<IEnumerable<long>> MergeToPseudoTableAsync(NpgsqlConnection connection,
            Func<string> getMergeToPseudoCommandText,
            int? bulkCopyTimeout = null,
            NpgsqlTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            var commandText = getMergeToPseudoCommandText();

            return await connection.ExecuteQueryAsync<long>(commandText,
                bulkCopyTimeout,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="transaction"></param>
        private static void DropPseudoTable(NpgsqlConnection connection,
            string tableName,
            int? bulkCopyTimeout = null,
            NpgsqlTransaction transaction = null)
        {
            if (string.IsNullOrWhiteSpace(tableName))
            {
                return;
            }

            var dbSetting = connection.GetDbSetting();
            var commandText = GetDropPseudoTemporaryTableCommandText(tableName, dbSetting);

            connection.ExecuteNonQuery(commandText,
                bulkCopyTimeout,
                transaction: transaction);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        private static async Task DropPseudoTableAsync(NpgsqlConnection connection,
            string tableName,
            int? bulkCopyTimeout = null,
            NpgsqlTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(tableName))
            {
                return;
            }

            var dbSetting = connection.GetDbSetting();
            var commandText = GetDropPseudoTemporaryTableCommandText(tableName, dbSetting);

            await connection.ExecuteNonQueryAsync(commandText,
                bulkCopyTimeout,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="fields"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="dbSetting"></param>
        /// <param name="transaction"></param>
        private static void CreatePseudoTableIndex(NpgsqlConnection connection,
            string tableName,
            IEnumerable<Field> fields,
            int? bulkCopyTimeout = null,
            IDbSetting dbSetting = null,
            NpgsqlTransaction transaction = null)
        {
            if (fields?.Any()!=true)
            {
                return;
            }

            var commandText = GetCreatePseudoTableIndexCommandText(tableName, fields, dbSetting);

            connection.ExecuteNonQuery(commandText,
                bulkCopyTimeout,
                transaction: transaction);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="fields"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="dbSetting"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private static async Task CreatePseudoTableIndexAsync(NpgsqlConnection connection,
            string tableName,
            IEnumerable<Field> fields,
            int? bulkCopyTimeout = null,
            IDbSetting dbSetting = null,
            NpgsqlTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            if (fields?.Any() != true)
            {
                return;
            }

            var commandText = GetCreatePseudoTableIndexCommandText(tableName, fields, dbSetting);

            await connection.ExecuteNonQueryAsync(commandText,
                bulkCopyTimeout,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="fields"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        private static string GetCreatePseudoTableIndexCommandText(string tableName,
            IEnumerable<Field> fields,
            IDbSetting dbSetting)
        {
            var indexName = $"{tableName}_{fields.Select(field => field.Name).Join("")}_IDX".AsQuoted(true, dbSetting);
            var columns = fields.Select(field => field.Name.AsQuoted(true, dbSetting)).Join(", ");

            return $"CREATE INDEX IF NOT EXISTS {indexName} " +
                $"ON {tableName.AsQuoted(true, dbSetting)} ({columns}); ";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="pseudoTableName"></param>
        /// <param name="mappings"></param>
        /// <param name="identityBehavior"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        private static string GetCreatePseudoTableCommandText(string tableName,
            string pseudoTableName,
            IEnumerable<NpgsqlBulkInsertMapItem> mappings,
            BulkImportIdentityBehavior identityBehavior,
            IDbSetting dbSetting) =>
            $"SELECT {GetCreatePseudoTableQueryColumns(mappings, identityBehavior, dbSetting)} " +
            $"INTO {pseudoTableName.AsQuoted(true, dbSetting)} " +
            $"FROM {tableName.AsQuoted(true, dbSetting)} " +
            $"WHERE (1 = 0);";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="pseudoTableName"></param>
        /// <param name="mappings"></param>
        /// <param name="identityBehavior"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        private static string GetCreatePseudoTemporaryTableCommandText(string tableName,
            string pseudoTableName,
            IEnumerable<NpgsqlBulkInsertMapItem> mappings,
            BulkImportIdentityBehavior identityBehavior,
            IDbSetting dbSetting) =>
            $"SELECT {GetCreatePseudoTableQueryColumns(mappings, identityBehavior, dbSetting)} " +
            $"INTO TEMPORARY {pseudoTableName.AsQuoted(true, dbSetting)} " +
            $"FROM {tableName.AsQuoted(true, dbSetting)} " +
            $"WHERE (1 = 0);";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mappings"></param>
        /// <param name="identityBehavior"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        private static string GetCreatePseudoTableQueryColumns(IEnumerable<NpgsqlBulkInsertMapItem> mappings,
            BulkImportIdentityBehavior identityBehavior,
            IDbSetting dbSetting) =>
            identityBehavior != BulkImportIdentityBehavior.ReturnIdentity ?
                mappings.Select(field => field.DestinationColumn.AsQuoted(true, dbSetting)).Join(", ") :
                $"0 AS {"__RepoDb_OrderColumn".AsQuoted(dbSetting)}, " +
                    $"{mappings.Select(field => field.DestinationColumn.AsQuoted(true, dbSetting)).Join(", ")}";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        private static string GetDropPseudoTemporaryTableCommandText(string tableName,
            IDbSetting dbSetting) =>
            $"DROP TABLE IF EXISTS {tableName.AsQuoted(true, dbSetting)};";

        #endregion
    }
}
