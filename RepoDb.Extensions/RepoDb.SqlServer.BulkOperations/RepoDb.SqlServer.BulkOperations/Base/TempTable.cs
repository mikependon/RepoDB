using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using RepoDb.Extensions;
using RepoDb.Interfaces;

namespace RepoDb
{
    /// <summary>
    /// 
    /// </summary>
    public static partial class SqlConnectionExtension
    {
        #region CreateTemporaryTable

        private static string GetCreateTemporaryTableSqlText(string tableName,
            string tempTableName,
            IEnumerable<Field> fields,
            IDbSetting dbSetting,
            bool isReturnIdentity)
        {
            var builder = new QueryBuilder();

            // Compose the statement
            builder
                .Clear()
                .Select()
                .FieldsFrom(fields, dbSetting);

            // Return Identity
            if (isReturnIdentity)
            {
                builder.WriteText(", CONVERT(INT, NULL) AS [__RepoDb_OrderColumn]");
            };

            // Continuation
            builder
                .Into()
                .WriteText(tempTableName.AsQuoted(dbSetting))
                .From()
                .TableNameFrom(tableName, dbSetting)
                .Where()
                .WriteText("(1 = 0)")
                .End();

            // Return the text
            return builder.ToString();
        }

        /// <summary>
        /// Creates the temporary (pseudo) table that mirrors <paramref name="tableName"/>.
        /// </summary>
        private static void CreateTemporaryTable(SqlConnection connection,
            string tableName,
            string tempTableName,
            IEnumerable<Field> fields,
            IDbSetting dbSetting,
            bool isReturnIdentity,
            SqlTransaction transaction,
            ITrace trace)
        {
            var sql = GetCreateTemporaryTableSqlText(tableName, tempTableName, fields, dbSetting, isReturnIdentity);
            connection.ExecuteNonQuery(sql, transaction: transaction, trace: trace);
        }

        /// <summary>
        /// Creates the temporary (pseudo) table that mirrors <paramref name="tableName"/>.
        /// </summary>
        private static async Task CreateTemporaryTableAsync(SqlConnection connection,
            string tableName,
            string tempTableName,
            IEnumerable<Field> fields,
            IDbSetting dbSetting,
            bool isReturnIdentity,
            SqlTransaction transaction,
            ITrace trace,
            CancellationToken cancellationToken)
        {
            var sql = GetCreateTemporaryTableSqlText(tableName, tempTableName, fields, dbSetting, isReturnIdentity);
            await connection.ExecuteNonQueryAsync(sql, transaction: transaction, trace: trace, cancellationToken: cancellationToken);
        }

        #endregion

        #region CreateTemporaryTableClusteredIndex

        private static string GetCreateTemporaryTableClusteredIndexSqlText(string tempTableName,
            IEnumerable<Field> qualifiers,
            IDbSetting dbSetting)
        {
            // Validate the presence
            if (qualifiers?.Any() != true)
            {
                throw new MissingFieldException("There is no qualifier field(s) defined.");
            }

            // Variables needed
            var clusteredIndexFields = qualifiers
                .Select(f => $"{f.Name.AsQuoted(dbSetting)} ASC")
                .Join(", ");
            var builder = new QueryBuilder();

            // Compose the statement
            builder
                .Clear()
                .WriteText("CREATE CLUSTERED INDEX")
                .WriteText($"IX_{tempTableName}".AsQuoted(dbSetting))
                .On()
                .WriteText(tempTableName.AsQuoted(dbSetting))
                .OpenParen()
                .WriteText(clusteredIndexFields)
                .CloseParen()
                .End();

            // Return the sql
            return builder.ToString();
        }

        /// <summary>
        /// Creates the clustered index on the temporary table's qualifier columns.
        /// </summary>
        private static void CreateTemporaryTableClusteredIndex(SqlConnection connection,
            string tempTableName,
            IEnumerable<Field> qualifiers,
            IDbSetting dbSetting,
            SqlTransaction transaction,
            ITrace trace)
        {
            var sql = GetCreateTemporaryTableClusteredIndexSqlText(tempTableName, qualifiers, dbSetting);
            connection.ExecuteNonQuery(sql, transaction: transaction, trace: trace);
        }

        /// <summary>
        /// Creates the clustered index on the temporary table's qualifier columns.
        /// </summary>
        private static async Task CreateTemporaryTableClusteredIndexAsync(SqlConnection connection,
            string tempTableName,
            IEnumerable<Field> qualifiers,
            IDbSetting dbSetting,
            SqlTransaction transaction,
            ITrace trace,
            CancellationToken cancellationToken)
        {
            var sql = GetCreateTemporaryTableClusteredIndexSqlText(tempTableName, qualifiers, dbSetting);
            await connection.ExecuteNonQueryAsync(sql, transaction: transaction, trace: trace, cancellationToken: cancellationToken);
        }

        #endregion

        #region CreateTemporaryTableWithIndex

        /// <summary>
        /// Creates the temporary table and its qualifier clustered index, in that order, so the index
        /// exists before the staging data is bulk-loaded (<see cref="WriteToServerInternal"/>).
        /// </summary>
        private static void CreateTemporaryTableWithIndex(SqlConnection connection,
            string tableName,
            string tempTableName,
            IEnumerable<Field> fields,
            IEnumerable<Field> qualifiers,
            IDbSetting dbSetting,
            bool isReturnIdentity,
            SqlTransaction transaction,
            ITrace trace)
        {
            CreateTemporaryTable(connection, tableName, tempTableName, fields, dbSetting, isReturnIdentity, transaction, trace);
            CreateTemporaryTableClusteredIndex(connection, tempTableName, qualifiers, dbSetting, transaction, trace);
        }

        /// <summary>
        /// Creates the temporary table and its qualifier clustered index, in that order, so the index
        /// exists before the staging data is bulk-loaded (<see cref="WriteToServerAsyncInternal"/>).
        /// </summary>
        private static async Task CreateTemporaryTableWithIndexAsync(SqlConnection connection,
            string tableName,
            string tempTableName,
            IEnumerable<Field> fields,
            IEnumerable<Field> qualifiers,
            IDbSetting dbSetting,
            bool isReturnIdentity,
            SqlTransaction transaction,
            ITrace trace,
            CancellationToken cancellationToken)
        {
            await CreateTemporaryTableAsync(connection, tableName, tempTableName, fields, dbSetting, isReturnIdentity, transaction, trace, cancellationToken);
            await CreateTemporaryTableClusteredIndexAsync(connection, tempTableName, qualifiers, dbSetting, transaction, trace, cancellationToken);
        }

        #endregion

        #region DropTemporaryTable

        private static string GetDropTemporaryTableSqlText(string tempTableName,
            IDbSetting dbSetting) =>
            $"DROP TABLE {tempTableName.AsQuoted(dbSetting)};";

        /// <summary>
        /// Drops the temporary (pseudo) table.
        /// </summary>
        private static void DropTemporaryTable(SqlConnection connection,
            string tempTableName,
            IDbSetting dbSetting,
            SqlTransaction transaction,
            ITrace trace)
        {
            var sql = GetDropTemporaryTableSqlText(tempTableName, dbSetting);
            connection.ExecuteNonQuery(sql, transaction: transaction, trace: trace);
        }

        /// <summary>
        /// Drops the temporary (pseudo) table.
        /// </summary>
        private static async Task DropTemporaryTableAsync(SqlConnection connection,
            string tempTableName,
            IDbSetting dbSetting,
            SqlTransaction transaction,
            ITrace trace,
            CancellationToken cancellationToken)
        {
            var sql = GetDropTemporaryTableSqlText(tempTableName, dbSetting);
            await connection.ExecuteNonQueryAsync(sql, transaction: transaction, trace: trace, cancellationToken: cancellationToken);
        }

        #endregion
    }
}
