using NpgsqlTypes;
using RepoDb.Attributes.Parameter;
using RepoDb.Attributes.Parameter.Npgsql;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.PostgreSql.BulkOperations;
using RepoDb.Resolvers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Dynamic;
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
        /// <param name="includePrimary"></param>
        /// <param name="includeIdentity"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        internal static IEnumerable<ClassProperty> GetMatchedProperties(IEnumerable<DbField> dbFields,
            IEnumerable<ClassProperty> properties,
            bool includePrimary,
            bool includeIdentity,
            IDbSetting dbSetting)
        {
            var matchedProperties = properties?
                .Where(property =>
                {
                    var dbField = dbFields?.FirstOrDefault(dbField =>
                        string.Equals(property.GetMappedName().AsUnquoted(true, dbSetting),
                            dbField.Name.AsUnquoted(true, dbSetting), StringComparison.OrdinalIgnoreCase));

                    return (dbField != null && dbField.IsPrimary == false && dbField.IsIdentity == false) ||
                        (includePrimary && dbField?.IsPrimary == true) ||
                        (includeIdentity && dbField?.IsIdentity == true);
                });

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
        /// <param name="includePrimary"></param>
        /// <param name="includeIdentity"></param>
        /// <param name="alternativeType"></param>
        /// <param name="npgsqlDbType"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        private static NpgsqlBulkInsertMapItem GetMapping(string sourceName,
            string destinationName,
            IEnumerable<DbField> dbFields,
            bool includePrimary,
            bool includeIdentity,
            Type alternativeType,
            NpgsqlDbType? npgsqlDbType,
            IDbSetting dbSetting)
        {
            if (npgsqlDbType == null)
            {
                var dbField = GetMappingDbField(destinationName,
                    dbFields,
                    includePrimary,
                    includeIdentity,
                    dbSetting);

                // Check
                if (dbField == null)
                {
                    return default;
                }

                // Resolve
                npgsqlDbType = !string.IsNullOrWhiteSpace(dbField?.DatabaseType) ?
                    dbTypeNameToNpgsqlDbTypeResolver.Resolve(dbField.DatabaseType) :
                    clientTypeToNpgsqlDbTypeResolver.Resolve(dbField?.Type ?? alternativeType);
            }

            // Return
            return new NpgsqlBulkInsertMapItem(sourceName, destinationName, npgsqlDbType);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="dbFields"></param>
        /// <param name="includePrimary"></param>
        /// <param name="includeIdentity"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        private static DbField GetMappingDbField(string name,
            IEnumerable<DbField> dbFields,
            bool includePrimary,
            bool includeIdentity,
            IDbSetting dbSetting)
        {
            // Get
            var dbField = dbFields?.First(df => string.Equals(name.AsUnquoted(true, dbSetting),
                df.Name.AsUnquoted(true, dbSetting), StringComparison.OrdinalIgnoreCase));

            // Check
            if (dbField == null)
            {
                return dbField;
            }

            // Primary
            if ((dbField.IsPrimary && includePrimary) || // Primary
                (dbField.IsIdentity && includePrimary) || // Identity
                (dbField.IsPrimary == false && dbField.IsIdentity == false)) // Others
            {
                return dbField;
            }

            // Return
            return default;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbFields"></param>
        /// <param name="properties"></param>
        /// <param name="includePrimary"></param>
        /// <param name="includeIdentity"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        private static IEnumerable<NpgsqlBulkInsertMapItem> GetMappings(IEnumerable<DbField> dbFields,
            IEnumerable<ClassProperty> properties,
            bool includePrimary,
            bool includeIdentity,
            IDbSetting dbSetting)
        {
            var matchedProperties = GetMatchedProperties(dbFields,
                properties,
                includePrimary,
                includeIdentity,
                dbSetting);

            return matchedProperties
                .Select(property =>
                    GetMapping(property.PropertyInfo.Name,
                        property.GetMappedName(),
                        dbFields,
                        includePrimary,
                        includeIdentity,
                        property.PropertyInfo.PropertyType,
                        GetMappedNpgsqlDbTypeFromAttributes(property.GetPropertyValueAttributes()),
                        dbSetting))
                .Where(property => property != null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dictionary"></param>
        /// <param name="dbFields"></param>
        /// <param name="includePrimary"></param>
        /// <param name="includeIdentity"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        private static IEnumerable<NpgsqlBulkInsertMapItem> GetMappings(IDictionary<string, object> dictionary,
            IEnumerable<DbField> dbFields,
            bool includePrimary,
            bool includeIdentity,
            IDbSetting dbSetting)
        {
            foreach (var kvp in dictionary)
            {
                var mapping = GetMapping(kvp.Key,
                    kvp.Key,
                    dbFields,
                    includePrimary,
                    includeIdentity,
                    kvp.Value?.GetType().GetUnderlyingType(),
                    null,
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
        /// <param name="dbFields"></param>
        /// <param name="includePrimary"></param>
        /// <param name="includeIdentity"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        private static IEnumerable<NpgsqlBulkInsertMapItem> GetMappings(DataTable table,
            IEnumerable<DbField> dbFields,
            bool includePrimary,
            bool includeIdentity,
            IDbSetting dbSetting)
        {
            foreach (DataColumn column in table.Columns)
            {
                var mapping = GetMapping(column.ColumnName,
                    column.ColumnName,
                    dbFields,
                    includePrimary,
                    includeIdentity,
                    column.DataType.GetUnderlyingType(),
                    null,
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
        /// <param name="includePrimary"></param>
        /// <param name="includeIdentity"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        private static IEnumerable<NpgsqlBulkInsertMapItem> GetMappings(DbDataReader reader,
            IEnumerable<DbField> dbFields,
            bool includePrimary,
            bool includeIdentity,
            IDbSetting dbSetting)
        {
            for (var i = 0; i < reader.FieldCount; i++)
            {
                var name = reader.GetName(i);
                var mapping = GetMapping(name,
                    name,
                    dbFields,
                    includePrimary,
                    includeIdentity,
                    reader.GetFieldType(i).GetUnderlyingType(),
                    null,
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
        /// <param name="propertyValueAttributes"></param>
        /// <returns></returns>
        private static NpgsqlDbType? GetMappedNpgsqlDbTypeFromAttributes(IEnumerable<PropertyValueAttribute> propertyValueAttributes)
        {
            // In purpose, the PropertyValueAttribute.Value is not exposed, therefore we cannot use this.
            // -> string.Equals(propertyValueAttribute.PropertyName, nameof(NpgsqlDbTypeAttribute.PropertyName), StringComparison.OrdinalIgnoreCase))

            var attribute = (NpgsqlDbTypeAttribute)propertyValueAttributes?
                .FirstOrDefault(propertyValueAttribute => propertyValueAttribute is NpgsqlDbTypeAttribute);

            return attribute?.NpgsqlDbType;
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
        /// <typeparam name="TData"></typeparam>
        /// <param name="data"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        private static IEnumerable<ExpandoObject> GetExpandoObjectData<TData>(IEnumerable<TData> data,
            Field field) =>
            GetExpandoObjectData<TData>(data, field.Name);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <param name="data"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        private static IEnumerable<ExpandoObject> GetExpandoObjectData<TData>(IEnumerable<TData> data,
            string column)
        {
            foreach (var item in data)
            {
                var expandoObject = new ExpandoObject() as IDictionary<string, object>;
                expandoObject[column] = item;
                yield return (ExpandoObject)expandoObject;
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
        /// <param name="identityResults"></param>
        /// <param name="dbSetting"></param>
        private static void SetIdentities<TEntity>(Type entityType,
            IEnumerable<TEntity> entities,
            IEnumerable<DbField> dbFields,
            IEnumerable<IdentityResult> identityResults,
            IDbSetting dbSetting)
            where TEntity : class
        {
            if (entityType.IsDictionaryStringObject())
            {
                var dictionaries = entities.Select(item => item as IDictionary<string, object>);
                SetDictionaryIdentities(dictionaries, dbFields, identityResults, dbSetting);
            }
            else
            {
                SetEntityIdentities(entities, dbFields, identityResults, dbSetting);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entities"></param>
        /// <param name="dbFields"></param>
        /// <param name="identityResults"></param>
        /// <param name="dbSetting"></param>
        private static void SetEntityIdentities<TEntity>(IEnumerable<TEntity> entities,
            IEnumerable<DbField> dbFields,
            IEnumerable<IdentityResult> identityResults,
            IDbSetting dbSetting)
            where TEntity : class =>
            SetEntityIdentities<TEntity>(entities, GetEntityIdentityField<TEntity>(dbFields, dbSetting), identityResults);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entities"></param>
        /// <param name="identityField"></param>
        /// <param name="identityResults"></param>
        private static void SetEntityIdentities<TEntity>(IEnumerable<TEntity> entities,
            Field identityField,
            IEnumerable<IdentityResult> identityResults)
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
            var bulkInsertIndex = -1;
            var index = 0;

            foreach (var result in identityResults)
            {
                var entity = entityList[result.Index == bulkInsertIndex ? index : result.Index];
                func(entity, result.Identity);
                index++;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entities"></param>
        /// <param name="dbFields"></param>
        /// <param name="identityResults"></param>
        /// <param name="dbSetting"></param>
        private static void SetDictionaryIdentities(IEnumerable<IDictionary<string, object>> entities,
            IEnumerable<DbField> dbFields,
            IEnumerable<IdentityResult> identityResults,
            IDbSetting dbSetting)
        {
            var identityField = dbFields?.FirstOrDefault(dbField => dbField.IsIdentity).AsField();

            if (identityField == null)
            {
                return;
            }

            var entityList = entities.AsList();
            var bulkInsertIndex = -1;
            var index = 0;

            foreach (var result in identityResults)
            {
                var entity = entityList[result.Index == bulkInsertIndex ? index : result.Index];
                entity[identityField.Name.AsUnquoted(true, dbSetting)] = result.Identity;
                index++;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="table"></param>
        /// <param name="dbFields"></param>
        /// <param name="identityResults"></param>
        /// <param name="dbSetting"></param>
        private static void SetDataTableIdentities(DataTable table,
            IEnumerable<DbField> dbFields,
            IEnumerable<IdentityResult> identityResults,
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

            var identityList = identityResults.ToList();
            var bulkInsertIndex = -1;
            var index = 0;

            foreach (var result in identityResults)
            {
                var row = table.Rows[result.Index == bulkInsertIndex ? index : result.Index];
                row[identityColumn] = result.Identity;
                index++;
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbFields"></param>
        /// <returns></returns>
        private static bool IsPrimaryAnIdentity(IEnumerable<DbField> dbFields) =>
            IsPrimaryAnIdentity(dbFields?.FirstOrDefault(dbField => dbField.IsPrimary));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="primary"></param>
        /// <returns></returns>
        private static bool IsPrimaryAnIdentity(DbField primary) =>
            primary.IsPrimary && primary.IsIdentity;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable"></param>
        /// <returns></returns>
        private static int EnumerableGetHashCode<T>(IEnumerable<T> enumerable)
        {
            var hashCode = 0;

            if (enumerable?.Any() != true)
            {
                return hashCode;
            }

            foreach (var item in enumerable)
            {
                hashCode = HashCode.Combine(hashCode, item.GetHashCode());
            }

            return hashCode;
        }
    }
}
