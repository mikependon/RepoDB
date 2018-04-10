using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using RepoDb.Attributes;
using RepoDb.Enumerations;
using RepoDb.Interfaces;

namespace RepoDb.Extensions
{
    public static class DataEntityExtension
    {
        // GetPropertiesFor
        internal static IEnumerable<PropertyInfo> GetPropertiesFor(Type type, Command command)
        {
            return type
                .GetProperties()
                .Where(property => !property.IsIgnored(command));
        }

        internal static IEnumerable<PropertyInfo> GetPropertiesFor<T>(Command command)
            where T : IDataEntity
        {
            return GetPropertiesFor(typeof(T), command);
        }

        internal static IEnumerable<PropertyInfo> GetPropertiesFor(this IDataEntity dataEntity, Command command)
        {
            return GetPropertiesFor(dataEntity.GetType(), command);
        }

        // GetCommandType
        internal static CommandType GetCommandType(Type type)
        {
            var attribute = type.GetCustomAttribute<MapAttribute>();
            return attribute?.CommandType ?? CommandType.Text;
        }

        internal static CommandType GetCommandType<T>()
            where T : IDataEntity
        {
            return GetCommandType(typeof(T));
        }

        internal static CommandType GetCommandType(this IDataEntity dataEntity)
        {
            return GetCommandType(dataEntity.GetType());
        }

        // GetPropertyByAttribute
        internal static PropertyInfo GetPropertyByAttribute(Type type, Type attributeType)
        {
            return type
                .GetProperties()
                .FirstOrDefault(property => property.GetCustomAttribute(attributeType) != null);
        }

        internal static PropertyInfo GetPropertyByAttribute<T>(Type attributeType)
            where T : IDataEntity
        {
            return GetPropertyByAttribute(typeof(T), attributeType);
        }

        internal static PropertyInfo GetPropertyByAttribute(this IDataEntity dataEntity, Type attributeType)
        {
            return GetPropertyByAttribute(dataEntity.GetType(), attributeType);
        }

        // GetPrimaryProperty
        internal static PropertyInfo GetPrimaryProperty(Type type)
        {
            return GetPropertyByAttribute(type, typeof(PrimaryAttribute)) ??
                type.GetProperties()
                    .FirstOrDefault(
                        property =>
                            string.Equals(property.Name, Constant.Id, StringComparison.InvariantCultureIgnoreCase)) ??
                type.GetProperties()
                    .FirstOrDefault(
                        property =>
                            string.Equals(property.Name, $"{type.Name}{Constant.Id}", StringComparison.InvariantCultureIgnoreCase));
        }

        internal static PropertyInfo GetPrimaryProperty<T>()
            where T : IDataEntity
        {
            return GetPrimaryProperty(typeof(T));
        }

        internal static PropertyInfo GetPrimaryProperty(this IDataEntity dataEntity)
        {
            return GetPrimaryProperty(dataEntity.GetType());
        }

        // GetIdentityProperty
        internal static PropertyInfo GetIdentityProperty(Type type)
        {
            var property = GetPropertyByAttribute(type, typeof(PrimaryAttribute));
            return property != null ? property.IsIdentity() ? property : null : null;
        }

        internal static PropertyInfo GetIdentityProperty<T>()
            where T : IDataEntity
        {
            return GetPrimaryProperty(typeof(T));
        }

        internal static PropertyInfo GetIdentityProperty(this IDataEntity dataEntity)
        {
            return GetPrimaryProperty(dataEntity.GetType());
        }

        // GetMappedName
        internal static string GetMappedName(Type type)
        {
            var attribute = type.GetCustomAttribute<MapAttribute>();
            return attribute?.Name ?? type.Name;
        }

        internal static string GetMappedName<T>()
            where T : IDataEntity
        {
            return GetMappedName(typeof(T));
        }

        internal static string GetMappedName(this IDataEntity dataEntity)
        {
            return GetMappedName(dataEntity.GetType());
        }

        // AsObject
        internal static object AsObject(this IDataEntity dataEntity, IQueryGroup queryGroup)
        {
            var expandObject = new ExpandoObject() as IDictionary<string, object>;
            dataEntity.GetType()
                .GetProperties()
                .ToList()
                .ForEach(property =>
                {
                    expandObject[property.Name] = property.GetValue(dataEntity);
                });
            if (queryGroup != null)
            {
                queryGroup
                    .GetAllQueryFields()?
                    .ToList()
                    .ForEach(queryField =>
                    {
                        expandObject[queryField.Parameter.Name] = queryField.Parameter.Value;
                    });
            }
            return (ExpandoObject)expandObject;
        }

        internal static object AsObject(this IDataEntity dataEntity)
        {
            return AsObject(dataEntity, null);
        }

        // AsDataTable
        internal static DataTable AsDataTable<T>(this IEnumerable<T> entities, IDbConnection connection)
            where T : IDataEntity
        {
            var mappedName = GetMappedName<T>();
            var table = new DataTable(mappedName);
            var properties = typeof(T).GetProperties();
            var dict = new Dictionary<DataColumn, PropertyInfo>();
            using (var command = connection.CreateCommand($"SELECT TOP 1 * FROM {mappedName} WHERE 1 = 0;"))
            {
                using (var reader = command.ExecuteReader(CommandBehavior.SchemaOnly))
                {
                    var schemaTable = reader.GetSchemaTable();
                    foreach (DataRow row in schemaTable.Rows)
                    {
                        var columnName = Convert.ToString(row[Constant.ColumnName]);
                        var dataType = Type.GetType(Convert.ToString(row[Constant.DataType]));
                        var dataColumn = new DataColumn(columnName, dataType);
                        var property = properties.FirstOrDefault(p =>
                            string.Equals(columnName, p.Name, StringComparison.InvariantCultureIgnoreCase));
                        table.Columns.Add(dataColumn);
                        dict.Add(dataColumn, property);
                    }
                }
            }
            entities?.ToList().ForEach(entity =>
            {
                var row = table.NewRow();
                foreach (var kvp in dict)
                {
                    var value = (object)null;
                    if (kvp.Value != null)
                    {
                        value = kvp.Value.GetValue(entity);
                    }
                    row[kvp.Key] = value ?? DBNull.Value;
                }
                table.Rows.Add(row);
            });
            return table;
        }

        // IsQueryble
        internal static bool IsQueryable(Type type)
        {
            return true; // Always return true for Query
        }

        internal static bool IsQueryable<T>()
            where T : IDataEntity
        {
            return IsQueryable(typeof(T));
        }

        internal static bool IsQueryable(this IDataEntity dataEntity)
        {
            return IsQueryable(dataEntity.GetType());
        }

        // InInsertable
        internal static bool InInsertable(Type type)
        {
            var attribute = type.GetCustomAttribute<MapAttribute>();
            return attribute?.CommandType != CommandType.StoredProcedure;
        }

        internal static bool InInsertable<T>()
            where T : IDataEntity
        {
            return InInsertable(typeof(T));
        }

        internal static bool InInsertable(this IDataEntity dataEntity)
        {
            return InInsertable(dataEntity.GetType());
        }

        // IsUpdateable
        internal static bool IsUpdateable(Type type)
        {
            var attribute = type.GetCustomAttribute<MapAttribute>();
            return attribute?.CommandType != CommandType.StoredProcedure;
        }

        internal static bool IsUpdateable<T>()
            where T : IDataEntity
        {
            return IsUpdateable(typeof(T));
        }

        internal static bool IsUpdateable(this IDataEntity dataEntity)
        {
            return IsUpdateable(dataEntity.GetType());
        }

        // IsDeletable
        internal static bool IsDeletable(Type type)
        {
            var attribute = type.GetCustomAttribute<MapAttribute>();
            return attribute?.CommandType != CommandType.StoredProcedure;
        }

        internal static bool IsDeletable<T>()
            where T : IDataEntity
        {
            return IsDeletable(typeof(T));
        }

        internal static bool IsDeletable(this IDataEntity dataEntity)
        {
            return IsDeletable(dataEntity.GetType());
        }

        // IsInsertable
        internal static bool IsInsertable(Type type)
        {
            var attribute = type.GetCustomAttribute<MapAttribute>();
            return attribute?.CommandType != CommandType.StoredProcedure;
        }

        internal static bool IsInsertable<T>()
            where T : IDataEntity
        {
            return IsInsertable(typeof(T));
        }

        internal static bool IsInsertable(this IDataEntity dataEntity)
        {
            return IsInsertable(dataEntity.GetType());
        }

        // IsMergeable
        internal static bool IsMergeable(Type type)
        {
            var attribute = type.GetCustomAttribute<MapAttribute>();
            return attribute?.CommandType != CommandType.StoredProcedure;
        }

        internal static bool IsMergeable<T>()
            where T : IDataEntity
        {
            return IsMergeable(typeof(T));
        }

        internal static bool IsMergeable(this IDataEntity dataEntity)
        {
            return IsMergeable(dataEntity.GetType());
        }
    }
}
