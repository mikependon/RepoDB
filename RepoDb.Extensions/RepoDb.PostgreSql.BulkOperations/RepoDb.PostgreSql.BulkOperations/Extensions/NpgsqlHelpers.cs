﻿using NpgsqlTypes;
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
using System.Reflection;

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
        internal static IEnumerable<ClassProperty> GetMatchedProperties(DbFieldCollection dbFields,
            IEnumerable<ClassProperty> properties,
            bool includePrimary,
            bool includeIdentity,
            IDbSetting dbSetting)
        {
            var matchedProperties = properties?
                .Where(property =>
                {
                    var dbField = dbFields?.GetByUnquotedName(property.GetMappedName().AsUnquoted(true, dbSetting));

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
            DbFieldCollection dbFields,
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
            DbFieldCollection dbFields,
            bool includePrimary,
            bool includeIdentity,
            IDbSetting dbSetting)
        {
            // Get
            var dbField = dbFields?.GetByUnquotedName(name.AsUnquoted(true, dbSetting));

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
        private static IEnumerable<NpgsqlBulkInsertMapItem> GetMappings(DbFieldCollection dbFields,
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

            foreach (var property in matchedProperties)
            {
                var mapping = GetMapping(property.PropertyInfo.Name,
                    property.GetMappedName(),
                    dbFields,
                    includePrimary,
                    includeIdentity,
                    property.PropertyInfo.PropertyType,
                    GetMappedNpgsqlDbTypeFromAttributes(property.GetPropertyValueAttributes()),
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
        /// <param name="dictionary"></param>
        /// <param name="dbFields"></param>
        /// <param name="includePrimary"></param>
        /// <param name="includeIdentity"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        private static IEnumerable<NpgsqlBulkInsertMapItem> GetMappings(IDictionary<string, object> dictionary,
            DbFieldCollection dbFields,
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
                    TypeCache.Get(kvp.Value?.GetType()).GetUnderlyingType(),
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
            DbFieldCollection dbFields,
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
                    TypeCache.Get(column.DataType).GetUnderlyingType(),
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
            DbFieldCollection dbFields,
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
                    TypeCache.Get(reader.GetFieldType(i)).GetUnderlyingType(),
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
            var list = new List<NpgsqlBulkInsertMapItem>(mappings);
            list.Insert(0,
                new NpgsqlBulkInsertMapItem("__RepoDb_OrderColumn", "__RepoDb_OrderColumn", NpgsqlDbType.Integer));
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
            DbFieldCollection dbFields,
            IEnumerable<IdentityResult> identityResults,
            IDbSetting dbSetting)
            where TEntity : class
        {
            if (TypeCache.Get(entityType).IsDictionaryStringObject())
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
            DbFieldCollection dbFields,
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
            if (TypeCache.Get(entityType).IsClassType() != true)
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

            var targetPropertyType = PropertyCache.Get<TEntity>().FirstOrDefault(p => p.PropertyInfo.Name == identityField.Name);
            var propType = targetPropertyType.PropertyInfo.PropertyType;

            foreach (var result in identityResults)
            {
                var entity = entityList[result.Index == bulkInsertIndex ? index : result.Index];
                object identityValue = result.Identity;

                //When using Return Identity operations the IdentityResult class returns the Identity as long
                //even though the Identity can be of type short or int so we need to convert it to the correct type
                if (identityValue is long longValue)
                {
                    if (propType == typeof(short))
                    {
                        identityValue = Convert.ToInt16(longValue);
                    }
                    else if (propType == typeof(int))
                    {
                        identityValue = Convert.ToInt32(longValue);
                    }
                    else if (propType == typeof(short?))
                    {
                        identityValue = (short?)Convert.ToInt16(longValue);
                    }
                    else if (propType == typeof(int?))
                    {
                        identityValue = (int?)Convert.ToInt32(longValue);
                    }
                }

                func(entity, identityValue);
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
            DbFieldCollection dbFields,
            IEnumerable<IdentityResult> identityResults,
            IDbSetting dbSetting)
        {
            var identityField = dbFields?.GetIdentity().AsField();

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
            DbFieldCollection dbFields,
            IEnumerable<IdentityResult> identityResults,
            IDbSetting dbSetting)
        {
            var identityField = dbFields?.GetIdentity().AsField();
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
        private static Field GetEntityIdentityField<TEntity>(DbFieldCollection dbFields,
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
        private static ClassProperty GetEntityIdentityProperty<TEntity>(DbFieldCollection dbFields,
            IDbSetting dbSetting)
            where TEntity : class
        {
            var identityDbField = dbFields?.GetIdentity();
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
        private static bool IsPrimaryAnIdentity(DbFieldCollection dbFields) =>
            IsPrimaryAnIdentity(dbFields?.GetPrimary());

        /// <summary>
        /// 
        /// </summary>
        /// <param name="primary"></param>
        /// <returns></returns>
        private static bool IsPrimaryAnIdentity(DbField primary) =>
            primary?.IsPrimary == true && primary?.IsIdentity == true;

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
