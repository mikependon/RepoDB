using Npgsql;
using RepoDb.Enumerations.PostgreSql;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using System;

namespace RepoDb
{
    public static partial class NpgsqlConnectionExtension
    {
        #region Others

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="getPseudoTableName"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="identityBehavior"></param>
        /// <param name="usePhysicalPseudoTempTable"></param>
        /// <param name="transaction"></param>
        private static void EnsurePseudoTable<TEntity>(NpgsqlConnection connection,
            string tableName,
            Func<string> getPseudoTableName,
            int? bulkCopyTimeout = null,
            BulkImportIdentityBehavior identityBehavior = default,
            bool? usePhysicalPseudoTempTable = null,
            NpgsqlTransaction transaction = null)
            where TEntity : class
        {
            if (identityBehavior != BulkImportIdentityBehavior.ReturnIdentity)
            {
                return;
            }

            var dbSetting = connection.GetDbSetting();
            var commandText = usePhysicalPseudoTempTable == true ?
                GetCreatePseudoTableCommandText<TEntity>(tableName, getPseudoTableName(), dbSetting) :
                GetCreatePseudoTemporaryTableCommandText<TEntity>(tableName, getPseudoTableName(), dbSetting);

            connection.ExecuteNonQuery(commandText, bulkCopyTimeout, transaction: transaction);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="tableName"></param>
        /// <param name="pseudoTableName"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        private static string GetCreatePseudoTableCommandText<TEntity>(string tableName,
            string pseudoTableName,
            IDbSetting dbSetting)
            where TEntity : class =>
            $"SELECT * " +
            $"INTO {pseudoTableName.AsQuoted(true, dbSetting)} " +
            $"FROM {tableName.AsQuoted(true, dbSetting)} " +
            $"WHERE (1 = 0);";

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="tableName"></param>
        /// <param name="pseudoTableName"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        private static string GetCreatePseudoTemporaryTableCommandText<TEntity>(string tableName,
            string pseudoTableName,
            IDbSetting dbSetting)
            where TEntity : class =>
            $"SELECT * " +
            $"INTO TEMPORARY {pseudoTableName.AsQuoted(true, dbSetting)} " +
            $"FROM {tableName.AsQuoted(true, dbSetting)} " +
            $"WHERE (1 = 0);";

        #endregion
    }
}
