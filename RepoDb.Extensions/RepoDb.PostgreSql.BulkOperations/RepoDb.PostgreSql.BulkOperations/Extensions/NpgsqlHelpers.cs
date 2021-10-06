using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.PostgreSql.BulkOperations;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace RepoDb
{
    public static partial class NpgsqlConnectionExtension
    {

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
            bool keepIdentity = false,
            IDbSetting dbSetting = null)
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dictionary"></param>
        /// <returns></returns>
        private static IEnumerable<NpgsqlBulkInsertMapItem> GetMappings(IDictionary<string, object> dictionary) =>
            dictionary?.Keys.Select(key => new NpgsqlBulkInsertMapItem(key, key, dictionary[key]?.GetType().GetUnderlyingType()));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        private static IEnumerable<NpgsqlBulkInsertMapItem> GetMappings(DataTable table)
        {
            foreach (DataColumn column in table.Columns)
            {
                yield return new NpgsqlBulkInsertMapItem(column.ColumnName, column.ColumnName, column.DataType.GetUnderlyingType());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private static IEnumerable<NpgsqlBulkInsertMapItem> GetMappings(DbDataReader reader)
        {
            for (var i = 0; i < reader.FieldCount; i++)
            {
                var name = reader.GetName(i);
                yield return new NpgsqlBulkInsertMapItem(name, name, reader.GetFieldType(i).GetUnderlyingType());
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
            list.Add(new NpgsqlBulkInsertMapItem("__RepoDb_OrderColumn", "__RepoDb_OrderColumn", NpgsqlTypes.NpgsqlDbType.Integer));
            return list;
        }
    }
}
