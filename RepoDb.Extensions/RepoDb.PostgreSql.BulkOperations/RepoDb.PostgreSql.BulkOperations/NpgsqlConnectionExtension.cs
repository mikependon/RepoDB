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
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="mappings"></param>
        /// <returns></returns>
        private static string GetBinaryImportCopyCommand(NpgsqlConnection connection,
            string tableName,
            IEnumerable<NpgsqlBulkInsertMapItem> mappings)
        {
            var dbSetting = connection.GetDbSetting();
            var targetTableName = tableName.AsQuoted(true, dbSetting);
            var textColumns = GetMappingsTextColumns(mappings, dbSetting);

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
                var dbFields = DbFieldCache.Get(connection, tableName ?? ClassMappedNameCache.Get(entityType), transaction);

                // DB/Entity Fields
                var properties = PropertyCache.Get(entityType);
                return GetMatchingTextColumns(dbFields, properties, dbSetting);
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
        /// <param name="dbFields"></param>
        /// <param name="properties"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        private static string GetMatchingTextColumns(IEnumerable<DbField> dbFields,
            IEnumerable<ClassProperty> properties,
            IDbSetting dbSetting) =>
            GetMatchedProperties(dbFields, properties, dbSetting)
                .Select(property => property.GetMappedName().AsQuoted(true, dbSetting)).Join(", ");

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="dbFields"></param>
        ///// <param name="fields"></param>
        ///// <param name="dbSetting"></param>
        ///// <returns></returns>
        //private static string GetMatchingTextColumns(IEnumerable<DbField> dbFields,
        //    IEnumerable<Field> fields,
        //    IDbSetting dbSetting) =>
        //    GetMatchedFields(dbFields, fields, dbSetting)
        //        .Select(field => field.Name.AsQuoted(true, dbSetting)).Join(", ");

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbFields"></param>
        /// <param name="properties"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        internal static IEnumerable<ClassProperty> GetMatchedProperties(IEnumerable<DbField> dbFields,
            IEnumerable<ClassProperty> properties,
            IDbSetting dbSetting)
        {
            var matchedProperties = properties?
                .Where(property =>
                    dbFields?.FirstOrDefault(dbField =>
                        dbField.IsIdentity == false &&
                        string.Equals(property.GetMappedName().AsUnquoted(true, dbSetting), dbField.Name.AsUnquoted(true, dbSetting), StringComparison.OrdinalIgnoreCase)) != null);

            if (matchedProperties?.Any() != true)
            {
                throw new InvalidOperationException($"There are no matching properties/columns found between the " +
                    $"entity model and the underlying table.");
            }

            return matchedProperties;
        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="dbFields"></param>
        ///// <param name="fields"></param>
        ///// <param name="dbSetting"></param>
        ///// <returns></returns>
        //internal static IEnumerable<Field> GetMatchedFields(IEnumerable<DbField> dbFields,
        //    IEnumerable<Field> fields,
        //    IDbSetting dbSetting)
        //{
        //    var matchedFields = fields?
        //        .Where(field =>
        //            dbFields?.FirstOrDefault(dbField =>
        //                dbField.IsIdentity == false &&
        //                string.Equals(field.Name.AsUnquoted(true, dbSetting), dbField.Name.AsUnquoted(true, dbSetting), StringComparison.OrdinalIgnoreCase)) != null);

        //    if (matchedFields?.Any() != true)
        //    {
        //        throw new InvalidOperationException($"There are no matching properties/columns found between the " +
        //            $"dictionary and the underlying table.");
        //    }

        //    return matchedFields;
        //}
    }
}
