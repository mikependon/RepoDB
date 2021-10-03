using Npgsql;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.PostgreSql.BulkOperations;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace RepoDb
{
    /// <summary>
    /// Contains the extension methods for NpgsqlConnection object.
    /// </summary>
    public static partial class NpgsqlConnectionExtension
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="mappings"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private static string GetBinaryImportCopyCommand<TEntity>(NpgsqlConnection connection,
            string tableName,
            IEnumerable<NpgsqlBulkInsertMapItem> mappings,
            NpgsqlTransaction transaction)
            where TEntity : class =>
            GetBinaryImportCopyCommand(connection, tableName, typeof(TEntity), mappings, transaction);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="entityType"></param>
        /// <param name="mappings"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private static string GetBinaryImportCopyCommand(NpgsqlConnection connection,
            string tableName,
            Type entityType,
            IEnumerable<NpgsqlBulkInsertMapItem> mappings,
            NpgsqlTransaction transaction)
        {
            var targetTableName = (tableName ?? ClassMappedNameCache.Get(entityType)).AsQuoted(true, connection.GetDbSetting());
            var textColumns = GetTextColumns(connection, tableName, entityType, mappings, transaction);

            return $"COPY {targetTableName} ({textColumns}) FROM STDIN (FORMAT BINARY)";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="entityType"></param>
        /// <param name="mappings"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private static string GetTextColumns(NpgsqlConnection connection,
            string tableName,
            Type entityType,
            IEnumerable<NpgsqlBulkInsertMapItem> mappings,
            NpgsqlTransaction transaction)
        {
            var dbSetting = connection.GetDbSetting();

            if (mappings?.Any() == true)
            {
                // Defined Mappings
                return GetMappingsTextColumns(mappings, dbSetting);
            }
            else
            {
                // DB/Entity Fields
                var dbFields = DbFieldCache.Get(connection, tableName ?? ClassMappedNameCache.Get(entityType), transaction);
                var fields = FieldCache.Get(entityType);

                return GetMatchingTextColumns(dbFields, fields, dbSetting);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mappings"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        private static string GetMappingsTextColumns(IEnumerable<NpgsqlBulkInsertMapItem> mappings,
            IDbSetting dbSetting) =>
            mappings.Select(mapping => mapping.DestinationColumn.AsQuoted(true, dbSetting)).Join(", ");

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fields"></param>
        /// <param name="dbFields"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        private static string GetMatchingTextColumns(IEnumerable<DbField> dbFields,
            IEnumerable<Field> fields,
            IDbSetting dbSetting) =>
            GetMatchedFields(dbFields, fields, dbSetting)?.Select(field => field.Name.AsQuoted(true, dbSetting)).Join(", ");

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbFields"></param>
        /// <param name="fields"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        private static IEnumerable<Field> GetMatchedFields(IEnumerable<DbField> dbFields,
            IEnumerable<Field> fields,
            IDbSetting dbSetting) =>
            dbFields?.Any() != true ? fields : dbFields?
                .Where(dbField =>
                    fields?.FirstOrDefault(field =>
                        string.Equals(field.Name.AsUnquoted(true, dbSetting), dbField.Name.AsUnquoted(true, dbSetting), StringComparison.OrdinalIgnoreCase)) != null)
                .AsFields();
    }
}
