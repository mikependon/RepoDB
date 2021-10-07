using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.PostgreSql.BulkOperations;
using RepoDb.Resolvers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace RepoDb
{
    public static partial class NpgsqlConnectionExtension
    {
        private readonly static PostgreSqlDbTypeNameToNpgsqlDbTypeResolver dbTypeNameToNpgsqlDbTypeResolver = new();
        private readonly static ClientTypeToNpgsqlDbTypeResolver clientTypeToNpgsqlDbTypeResolver = new();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbFields"></param>
        /// <param name="properties"></param>
        /// <param name="includeIdentity"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        internal static IEnumerable<ClassProperty> GetMatchedProperties(IEnumerable<DbField> dbFields,
            IEnumerable<ClassProperty> properties,
            bool includeIdentity = false,
            IDbSetting dbSetting = null)
        {
            var matchedProperties = properties?
                .Where(property =>
                    dbFields?.FirstOrDefault(dbField =>
                        (dbField.IsIdentity == false || (includeIdentity && dbField.IsIdentity)) &&
                        string.Equals(property.GetMappedName().AsUnquoted(true, dbSetting),
                            dbField.Name.AsUnquoted(true, dbSetting), StringComparison.OrdinalIgnoreCase)) != null);

            if (matchedProperties?.Any() != true)
            {
                throw new InvalidOperationException($"There are no matching properties/columns found between the " +
                    $"entity model and the underlying table.");
            }

            return matchedProperties;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceName"></param>
        /// <param name="destinationName"></param>
        /// <param name="dbFields"></param>
        /// <param name="alternativeType"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        private static NpgsqlBulkInsertMapItem GetMapping(string sourceName,
            string destinationName,
            IEnumerable<DbField> dbFields,
            Type alternativeType,
            IDbSetting dbSetting)
        {
            var dbField = dbFields?.First(df => string.Equals(destinationName.AsUnquoted(true, dbSetting),
                df.Name.AsUnquoted(true, dbSetting), StringComparison.OrdinalIgnoreCase));
            var dbType = !string.IsNullOrWhiteSpace(dbField?.DatabaseType) ?
                dbTypeNameToNpgsqlDbTypeResolver.Resolve(dbField.DatabaseType) :
                clientTypeToNpgsqlDbTypeResolver.Resolve(dbField?.Type ?? alternativeType);

            return new NpgsqlBulkInsertMapItem(sourceName, destinationName, dbType);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbFields"></param>
        /// <param name="properties"></param>
        /// <param name="includeIdentity"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        private static IEnumerable<NpgsqlBulkInsertMapItem> GetMappings(IEnumerable<DbField> dbFields,
            IEnumerable<ClassProperty> properties,
            bool includeIdentity = false,
            IDbSetting dbSetting = null)
        {
            var matchedProperties = GetMatchedProperties(dbFields, properties, includeIdentity, dbSetting);
            return matchedProperties
                .Select(property => GetMapping(property.PropertyInfo.Name, property.GetMappedName(),
                    dbFields, property.PropertyInfo.PropertyType, dbSetting));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dictionary"></param>
        /// <param name="dbFields"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        private static IEnumerable<NpgsqlBulkInsertMapItem> GetMappings(IDictionary<string, object> dictionary,
            IEnumerable<DbField> dbFields,
            IDbSetting dbSetting) =>
            dictionary?.Keys.Select(key =>
                GetMapping(key, key, dbFields, dictionary[key]?.GetType().GetUnderlyingType(), dbSetting));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="table"></param>
        /// <param name="dbFields"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        private static IEnumerable<NpgsqlBulkInsertMapItem> GetMappings(DataTable table,
            IEnumerable<DbField> dbFields,
            IDbSetting dbSetting)
        {
            foreach (DataColumn column in table.Columns)
            {
                yield return GetMapping(column.ColumnName, column.ColumnName, dbFields,
                    column.DataType.GetUnderlyingType(), dbSetting);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="dbFields"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        private static IEnumerable<NpgsqlBulkInsertMapItem> GetMappings(DbDataReader reader,
            IEnumerable<DbField> dbFields,
            IDbSetting dbSetting)
        {
            for (var i = 0; i < reader.FieldCount; i++)
            {
                var name = reader.GetName(i);
                yield return GetMapping(name, name, dbFields,
                    reader.GetFieldType(i).GetUnderlyingType(), dbSetting);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="table"></param>
        /// <param name="rowState"></param>
        /// <returns></returns>
        private static IEnumerable<DataRow> GetRows(DataTable table,
            DataRowState? rowState)
        {
            foreach (DataRow row in table.Rows)
            {
                if (rowState == null || row.RowState == rowState)
                {
                    yield return row;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mappings"></param>
        private static IEnumerable<NpgsqlBulkInsertMapItem> AddOrderColumnMapping(IEnumerable<NpgsqlBulkInsertMapItem> mappings)
        {
            var list = mappings.AsList();
            list.Insert(0, new NpgsqlBulkInsertMapItem("__RepoDb_OrderColumn", "__RepoDb_OrderColumn", NpgsqlTypes.NpgsqlDbType.Integer));
            return list;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="tableName"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        private static string GetBinaryInsertPseudoTableName<TEntity>(string tableName,
            IDbSetting dbSetting) =>
            $"_RepoDb_BinaryBulkInsert_{(tableName ?? ClassMappedNameCache.Get<TEntity>()).AsUnquoted(true, dbSetting)}";
    }
}
