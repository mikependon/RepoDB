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
        /// <param name="tableName"></param>
        /// <param name="pseudoTableName"></param>
        /// <param name="mappings"></param>
        /// <param name="dbFields"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="dbSetting"></param>
        /// <param name="transaction"></param>
        private static IEnumerable<long> InsertPseudoTable(NpgsqlConnection connection,
            string tableName,
            string pseudoTableName,
            IEnumerable<NpgsqlBulkInsertMapItem> mappings,
            IEnumerable<DbField> dbFields,
            int? bulkCopyTimeout = null,
            IDbSetting dbSetting = null,
            NpgsqlTransaction transaction = null)
        {
            var identityField = dbFields.FirstOrDefault(dbField => dbField.IsIdentity)?.AsField();
            var commandText = GetInsertCommand(pseudoTableName,
                tableName,
                mappings.Select(mapping => new Field(mapping.DestinationColumn)),
                identityField,
                dbSetting);

            return connection.ExecuteQuery<long>(commandText,
                bulkCopyTimeout,
                transaction: transaction);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="pseudoTableName"></param>
        /// <param name="mappings"></param>
        /// <param name="dbFields"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="dbSetting"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        private static async Task<IEnumerable<long>> InsertPseudoTableAsync(NpgsqlConnection connection,
            string tableName,
            string pseudoTableName,
            IEnumerable<NpgsqlBulkInsertMapItem> mappings,
            IEnumerable<DbField> dbFields,
            int? bulkCopyTimeout = null,
            IDbSetting dbSetting = null,
            NpgsqlTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            var identityField = dbFields.FirstOrDefault(dbField => dbField.IsIdentity)?.AsField();
            var commandText = GetInsertCommand(pseudoTableName,
                tableName,
                mappings.Select(mapping => new Field(mapping.DestinationColumn)),
                identityField,
                dbSetting);

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

            connection.ExecuteNonQuery(commandText, bulkCopyTimeout, transaction: transaction);
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
