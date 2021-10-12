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
        /// <param name="includeIdentity"></param>
        /// <param name="alternativeType"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        private static NpgsqlBulkInsertMapItem GetMapping(string sourceName,
            string destinationName,
            IEnumerable<DbField> dbFields,
            bool includeIdentity,
            Type alternativeType,
            IDbSetting dbSetting)
        {
            if (includeIdentity == false)
            {
                var identity = dbFields?.FirstOrDefault(dbField => dbField.IsIdentity);
                if (identity != null && string.Equals(identity.Name.AsUnquoted(true, dbSetting), destinationName.AsUnquoted(true, dbSetting)))
                {
                    return null;
                }
            }

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
                .Select(property => GetMapping(property.PropertyInfo.Name, property.GetMappedName(), dbFields,
                    includeIdentity, property.PropertyInfo.PropertyType, dbSetting));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dictionary"></param>
        /// <param name="dbFields"></param>
        /// <param name="includeIdentity"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        private static IEnumerable<NpgsqlBulkInsertMapItem> GetMappings(IDictionary<string, object> dictionary,
            IEnumerable<DbField> dbFields,
            bool includeIdentity,
            IDbSetting dbSetting)
        {
            foreach (var kvp in dictionary)
            {
                var mapping = GetMapping(kvp.Key,
                    kvp.Key,
                    dbFields,
                    includeIdentity,
                    kvp.Value?.GetType().GetUnderlyingType(), dbSetting);

                if (mapping != null)
                {
                    yield return mapping;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="table"></param>
        /// <param name="dbFields"></param>
        /// <param name="includeIdentity"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        private static IEnumerable<NpgsqlBulkInsertMapItem> GetMappings(DataTable table,
            IEnumerable<DbField> dbFields,
            bool includeIdentity,
            IDbSetting dbSetting)
        {
            foreach (DataColumn column in table.Columns)
            {
                var mapping = GetMapping(column.ColumnName,
                    column.ColumnName,
                    dbFields,
                    includeIdentity,
                    column.DataType.GetUnderlyingType(),
                    dbSetting);

                if (mapping != null)
                {
                    yield return mapping;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="dbFields"></param>
        /// <param name="includeIdentity"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        private static IEnumerable<NpgsqlBulkInsertMapItem> GetMappings(DbDataReader reader,
            IEnumerable<DbField> dbFields,
            bool includeIdentity,
            IDbSetting dbSetting)
        {
            for (var i = 0; i < reader.FieldCount; i++)
            {
                var name = reader.GetName(i);
                var mapping = GetMapping(name,
                    name,
                    dbFields,
                    includeIdentity,
                    reader.GetFieldType(i).GetUnderlyingType(),
                    dbSetting);

                if (mapping != null)
                {
                    yield return mapping;
                }
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
        /// <param name="entityType"></param>
        /// <param name="entities"></param>
        /// <param name="dbFields"></param>
        /// <param name="identities"></param>
        /// <param name="dbSetting"></param>
        private static void SetIdentities<TEntity>(Type entityType,
            IEnumerable<TEntity> entities,
            IEnumerable<DbField> dbFields,
            IEnumerable<long> identities,
            IDbSetting dbSetting)
            where TEntity : class
        {
            if (entityType.IsDictionaryStringObject())
            {
                var dictionaries = entities.Select(item => item as IDictionary<string, object>);
                SetDictionaryIdentities(dictionaries, dbFields, identities, dbSetting);
            }
            else
            {
                SetEntityIdentities(entities, dbFields, identities, dbSetting);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entities"></param>
        /// <param name="dbFields"></param>
        /// <param name="identities"></param>
        /// <param name="dbSetting"></param>
        private static void SetEntityIdentities<TEntity>(IEnumerable<TEntity> entities,
            IEnumerable<DbField> dbFields,
            IEnumerable<long> identities,
            IDbSetting dbSetting)
            where TEntity : class =>
            SetEntityIdentities<TEntity>(entities, GetEntityIdentityField<TEntity>(dbFields, dbSetting), identities);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entities"></param>
        /// <param name="identityField"></param>
        /// <param name="identities"></param>
        private static void SetEntityIdentities<TEntity>(IEnumerable<TEntity> entities,
            Field identityField,
            IEnumerable<long> identities)
            where TEntity : class
        {
            if (identityField == null)
            {
                return;
            }

            var entityType = (entities?.FirstOrDefault().GetType() ?? typeof(TEntity));
            if (entityType?.IsClassType() != true)
            {
                return;
            }

            var func = Compiler.GetPropertySetterFunc<TEntity>(identityField.Name);
            if (func == null)
            {
                return;
            }

            var entityList = entities.AsList();
            var identityList = identities.AsList();

            for (var i = 0; i < identityList.Count; i++)
            {
                func(entityList[i], identityList[i]);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entities"></param>
        /// <param name="dbFields"></param>
        /// <param name="identities"></param>
        /// <param name="dbSetting"></param>
        private static void SetDictionaryIdentities(IEnumerable<IDictionary<string, object>> entities,
            IEnumerable<DbField> dbFields,
            IEnumerable<long> identities,
            IDbSetting dbSetting)
        {
            var identityField = dbFields?.FirstOrDefault(dbField => dbField.IsIdentity).AsField();

            if (identityField == null)
            {
                return;
            }

            var entityList = entities.AsList();
            var identityList = identities.AsList();

            for (var i = 0; i < identityList.Count; i++)
            {
                entityList[i][identityField.Name.AsUnquoted(true, dbSetting)] = identityList[i];
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="table"></param>
        /// <param name="dbFields"></param>
        /// <param name="identities"></param>
        /// <param name="dbSetting"></param>
        private static void SetDataTableIdentities(DataTable table,
            IEnumerable<DbField> dbFields,
            IEnumerable<long> identities,
            IDbSetting dbSetting)
        {
            var identityField = dbFields?.FirstOrDefault(dbField => dbField.IsIdentity).AsField();
            if (identityField == null)
            {
                return;
            }

            var identityColumn = GetDataTableIdentityColumn(table, identityField, dbSetting);
            if (identityColumn == null)
            {
                identityColumn = table.Columns.Add(identityField.Name, identityField.Type ?? typeof(object));
            }

            var identityList = identities.ToList();
            for (var i = 0; i < table.Rows.Count; i++)
            {
                table.Rows[i][identityColumn] = identityList[i];
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="dbFields"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        private static Field GetEntityIdentityField<TEntity>(IEnumerable<DbField> dbFields,
            IDbSetting dbSetting)
            where TEntity : class
        {
            // We cannot use the ClassProperty.AsField() and PropertyInfo.AsField() as it is
            // using the mappings. We need to get the identity class property name instead

            var property = IdentityCache.Get<TEntity>() ??
                    GetEntityIdentityProperty<TEntity>(dbFields, dbSetting);

            if (property != null)
            {
                return new Field(property.PropertyInfo.Name);
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="dbFields"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        private static ClassProperty GetEntityIdentityProperty<TEntity>(IEnumerable<DbField> dbFields,
            IDbSetting dbSetting)
            where TEntity : class
        {
            var identityDbField = dbFields?.FirstOrDefault(dbField => dbField.IsIdentity);
            ClassProperty property = null;

            if (identityDbField != null)
            {
                property = PropertyCache
                   .Get<TEntity>()
                   .FirstOrDefault(p =>
                       string.Equals(p.GetMappedName().AsUnquoted(true, dbSetting),
                           identityDbField.Name.AsUnquoted(true, dbSetting), StringComparison.OrdinalIgnoreCase));
            }

            return property;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="table"></param>
        /// <param name="identityField"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        private static DataColumn GetDataTableIdentityColumn(DataTable table,
            Field identityField,
            IDbSetting dbSetting)
        {
            if (identityField == null)
            {
                return null;
            }

            foreach (DataColumn column in table.Columns)
            {
                if (string.Equals(column.ColumnName.AsUnquoted(true, dbSetting),
                    identityField.Name.AsUnquoted(true, dbSetting), StringComparison.OrdinalIgnoreCase))
                {
                    return column;
                }
            }

            return null;
        }
    }
}
