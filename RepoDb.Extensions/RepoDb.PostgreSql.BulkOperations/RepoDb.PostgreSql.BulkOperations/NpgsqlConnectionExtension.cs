﻿using Npgsql;
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
    /// <summary>
    /// Contains the extension methods for NpgsqlConnection object.
    /// </summary>
    public static partial class NpgsqlConnectionExtension
    {
        /*
         * GetBinaryImportCopyCommand
         */

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="entityType"></param>
        /// <param name="mappings"></param>
        /// <param name="keepIdentity"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private static string GetBinaryImportCopyCommand(NpgsqlConnection connection,
            string tableName,
            Type entityType,
            IEnumerable<NpgsqlBulkInsertMapItem> mappings,
            bool keepIdentity,
            NpgsqlTransaction transaction)
        {
            var targetTableName = (tableName ?? ClassMappedNameCache.Get(entityType)).AsQuoted(true, connection.GetDbSetting());
            var textColumns = GetTextColumns(connection,
                tableName,
                entityType,
                mappings,
                keepIdentity,
                transaction);

            return $"COPY {targetTableName} ({textColumns}) FROM STDIN (FORMAT BINARY)";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="entityType"></param>
        /// <param name="mappings"></param>
        /// <param name="keepIdentity"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private static async Task<string> GetBinaryImportCopyCommandAsync(NpgsqlConnection connection,
            string tableName,
            Type entityType,
            IEnumerable<NpgsqlBulkInsertMapItem> mappings,
            bool keepIdentity,
            NpgsqlTransaction transaction,
            CancellationToken cancellationToken = default)
        {
            var targetTableName = (tableName ?? ClassMappedNameCache.Get(entityType)).AsQuoted(true, connection.GetDbSetting());
            var textColumns = await GetTextColumnsAsync(connection,
                tableName,
                entityType,
                mappings,
                keepIdentity,
                transaction,
                cancellationToken);

            return $"COPY {targetTableName} ({textColumns}) FROM STDIN (FORMAT BINARY)";
        }

        /*
         * GetTextColumns
         */

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="entityType"></param>
        /// <param name="mappings"></param>
        /// <param name="keepIdentity"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private static string GetTextColumns(NpgsqlConnection connection,
            string tableName,
            Type entityType,
            IEnumerable<NpgsqlBulkInsertMapItem> mappings,
            bool keepIdentity,
            NpgsqlTransaction transaction)
        {
            var dbSetting = connection.GetDbSetting();

            if (mappings?.Any() == true)
            {
                return GetTextColumns(mappings, dbSetting);
            }
            else
            {
                var dbFields = DbFieldCache.Get(connection,
                    (tableName ?? ClassMappedNameCache.Get(entityType)),
                    transaction);
                return GetTextColumns(dbFields, PropertyCache.Get(entityType), keepIdentity, dbSetting);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="entityType"></param>
        /// <param name="mappings"></param>
        /// <param name="keepIdentity"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private static async Task<string> GetTextColumnsAsync(NpgsqlConnection connection,
            string tableName,
            Type entityType,
            IEnumerable<NpgsqlBulkInsertMapItem> mappings,
            bool keepIdentity,
            NpgsqlTransaction transaction,
            CancellationToken cancellationToken = default)
        {
            var dbSetting = connection.GetDbSetting();

            if (mappings?.Any() == true)
            {
                return GetTextColumns(mappings, dbSetting);
            }
            else
            {
                var dbFields = await DbFieldCache.GetAsync(connection,
                    (tableName ?? ClassMappedNameCache.Get(entityType)),
                    transaction,
                    cancellationToken);
                return GetTextColumns(dbFields, PropertyCache.Get(entityType), keepIdentity, dbSetting);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mappings"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        private static string GetTextColumns(IEnumerable<NpgsqlBulkInsertMapItem> mappings,
            IDbSetting dbSetting) =>
            mappings.Select(mapping => mapping.DestinationColumn.AsQuoted(true, dbSetting)).Join(", ");

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbFields"></param>
        /// <param name="properties"></param>
        /// <param name="keepIdentity"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        private static string GetTextColumns(IEnumerable<DbField> dbFields,
            IEnumerable<ClassProperty> properties,
            bool keepIdentity,
            IDbSetting dbSetting) =>
            GetMatchedProperties(dbFields, properties, keepIdentity, dbSetting)
                .Select(property => property.GetMappedName().AsQuoted(true, dbSetting)).Join(", ");

        /*
         * GetMatchedProperties
         */

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbFields"></param>
        /// <param name="properties"></param>
        /// <param name="keepIdentity"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        internal static IEnumerable<ClassProperty> GetMatchedProperties(IEnumerable<DbField> dbFields,
            IEnumerable<ClassProperty> properties,
            bool keepIdentity,
            IDbSetting dbSetting)
        {
            var matchedProperties = properties?
                .Where(property =>
                    dbFields?.FirstOrDefault(dbField =>
                        (dbField.IsIdentity == false || (keepIdentity && dbField.IsIdentity)) &&
                        string.Equals(property.GetMappedName().AsUnquoted(true, dbSetting),
                            dbField.Name.AsUnquoted(true, dbSetting), StringComparison.OrdinalIgnoreCase)) != null);

            if (matchedProperties?.Any() != true)
            {
                throw new InvalidOperationException($"There are no matching properties/columns found between the " +
                    $"entity model and the underlying table.");
            }

            return matchedProperties;
        }
    }
}