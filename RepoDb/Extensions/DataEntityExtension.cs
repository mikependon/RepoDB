using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Reflection;
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

        public static IEnumerable<PropertyInfo> GetPropertiesFor<T>(Command command)
            where T : IDataEntity
        {
            return GetPropertiesFor(typeof(T), command);
        }

        public static IEnumerable<PropertyInfo> GetPropertiesFor(this IDataEntity dataEntity, Command command)
        {
            return GetPropertiesFor(dataEntity.GetType(), command);
        }

        // GetCommandType
        internal static CommandType GetCommandType(Type type)
        {
            var attribute = ClassMapCache.Get(type);
            return attribute?.CommandType ?? CommandType.Text;
        }

        public static CommandType GetCommandType<T>()
            where T : IDataEntity
        {
            return GetCommandType(typeof(T));
        }

        public static CommandType GetCommandType(this IDataEntity dataEntity)
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

        public static PropertyInfo GetPropertyByAttribute<T>(Type attributeType)
            where T : IDataEntity
        {
            return GetPropertyByAttribute(typeof(T), attributeType);
        }

        public static PropertyInfo GetPropertyByAttribute(this IDataEntity dataEntity, Type attributeType)
        {
            return GetPropertyByAttribute(dataEntity.GetType(), attributeType);
        }

        // GetPrimaryProperty
        internal static PropertyInfo GetPrimaryProperty(Type type)
        {
            // Primary Attribute
            var property = GetPropertyByAttribute(type, typeof(PrimaryAttribute));
            if (property != null)
            {
                return property;
            }

            // Id Property
            property = type.GetProperties().FirstOrDefault(p => p.Name.ToLower() == Constant.Id.ToLower());
            if (property != null)
            {
                return property;
            }

            // Type.Name + Id
            property = type.GetProperties().FirstOrDefault(p => p.Name.ToLower() == $"{type.Name}{Constant.Id}".ToLower());
            if (property != null)
            {
                return property;
            }

            // Mapping.Name + Id
            property = type.GetProperties().FirstOrDefault(p => p.Name.ToLower() == $"{GetMappedName(type, Command.Select).AsUnquoted()}{Constant.Id}".ToLower());
            if (property != null)
            {
                return property;
            }

            // Not found
            return null;
        }

        public static PropertyInfo GetPrimaryProperty<T>()
            where T : IDataEntity
        {
            return GetPrimaryProperty(typeof(T));
        }

        public static PropertyInfo GetPrimaryProperty(this IDataEntity dataEntity)
        {
            return GetPrimaryProperty(dataEntity.GetType());
        }

        // GetIdentityProperty
        internal static PropertyInfo GetIdentityProperty(Type type)
        {
            var property = GetPropertyByAttribute(type, typeof(PrimaryAttribute));
            return property != null ? property.IsIdentity() ? property : null : null;
        }

        public static PropertyInfo GetIdentityProperty<T>()
            where T : IDataEntity
        {
            return GetIdentityProperty(typeof(T));
        }

        public static PropertyInfo GetIdentityProperty(this IDataEntity dataEntity)
        {
            return GetIdentityProperty(dataEntity.GetType());
        }

        // GetMappedName
        internal static string GetMappedName(Type type, Command command)
        {
            return DataEntityMapper.For(type)?.Get(command)?.Name ??
                ClassMapCache.Get(type)?.Name ?? type.Name;
        }

        public static string GetMappedName<T>(Command command)
            where T : IDataEntity
        {
            return GetMappedName(typeof(T), command);
        }

        public static string GetMappedName(this IDataEntity dataEntity, Command command)
        {
            return GetMappedName(dataEntity.GetType(), command);
        }

        // AsObject
        public static object AsObject(this IDataEntity dataEntity, IQueryGroup queryGroup)
        {
            var expandObject = new ExpandoObject() as IDictionary<string, object>;
            dataEntity.GetType()
                .GetProperties()
                .ToList()
                .ForEach(property =>
                {
                    expandObject[property.GetMappedName()] = property.GetValue(dataEntity);
                });
            queryGroup?
                .Fix()
                .GetAllQueryFields()?
                .ToList()
                .ForEach(queryField =>
                {
                    expandObject[queryField.Parameter.Name] = queryField.Parameter.Value;
                });
            return (ExpandoObject)expandObject;
        }

        public static object AsObject(this IDataEntity dataEntity)
        {
            return AsObject(dataEntity, null);
        }

        // AsDataTable
        public static DataTable AsDataTable<T>(this IEnumerable<T> entities, IDbConnection connection, Command command = Command.None)
            where T : IDataEntity
        {
            var mappedName = GetMappedName<T>(Command.None);
            var table = new DataTable(mappedName);
            var properties = PropertyCache.Get<T>(command);
            var dict = new Dictionary<DataColumn, PropertyInfo>();
            using (var cmd = connection.CreateCommand($"SELECT TOP 1 * FROM {mappedName} WHERE 1 = 0;"))
            {
                using (var reader = cmd.ExecuteReader(CommandBehavior.SchemaOnly))
                {
                    var schemaTable = reader.GetSchemaTable();
                    foreach (DataRow row in schemaTable.Rows)
                    {
                        var columnName = Convert.ToString(row[Constant.ColumnName]);
                        var dataType = Type.GetType(Convert.ToString(row[Constant.DataType]));
                        var dataColumn = new DataColumn(columnName, dataType);
                        var property = properties.FirstOrDefault(p =>
                        {
                            return columnName.ToLower() == p.GetMappedName().ToLower();
                        });
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
                    row[kvp.Key] = kvp.Value?.GetValue(entity) ?? DBNull.Value;
                }
                table.Rows.Add(row);
            });
            return table;
        }

        // IsQueryble
        internal static bool IsQueryable(Type type)
        {
            return true;
        }

        public static bool IsQueryable<T>()
            where T : IDataEntity
        {
            return IsQueryable(typeof(T));
        }

        public static bool IsQueryable(this IDataEntity dataEntity)
        {
            return IsQueryable(dataEntity.GetType());
        }

        // IsBatchQueryble
        internal static bool IsBatchQueryble(Type type)
        {
            var attribute = ClassMapCache.Get(type);
            return attribute == null ? true : attribute.CommandType == CommandType.Text;
        }

        public static bool IsBatchQueryble<T>()
            where T : IDataEntity
        {
            return IsBatchQueryble(typeof(T));
        }

        public static bool IsBatchQueryble(this IDataEntity dataEntity)
        {
            return IsQueryable(dataEntity.GetType());
        }

        // InInsertable
        internal static bool InInsertable(Type type)
        {
            var attribute = ClassMapCache.Get(type);
            return attribute?.CommandType != CommandType.StoredProcedure;
        }

        public static bool InInsertable<T>()
            where T : IDataEntity
        {
            return InInsertable(typeof(T));
        }

        public static bool InInsertable(this IDataEntity dataEntity)
        {
            return InInsertable(dataEntity.GetType());
        }

        // IsUpdateable
        internal static bool IsUpdateable(Type type)
        {
            var attribute = ClassMapCache.Get(type);
            return attribute?.CommandType != CommandType.StoredProcedure;
        }

        public static bool IsUpdateable<T>()
            where T : IDataEntity
        {
            return IsUpdateable(typeof(T));
        }

        public static bool IsUpdateable(this IDataEntity dataEntity)
        {
            return IsUpdateable(dataEntity.GetType());
        }

        // IsDeletable
        internal static bool IsDeletable(Type type)
        {
            var attribute = ClassMapCache.Get(type);
            return attribute?.CommandType != CommandType.StoredProcedure;
        }

        public static bool IsDeletable<T>()
            where T : IDataEntity
        {
            return IsDeletable(typeof(T));
        }

        public static bool IsDeletable(this IDataEntity dataEntity)
        {
            return IsDeletable(dataEntity.GetType());
        }

        // IsInsertable
        internal static bool IsInsertable(Type type)
        {
            var attribute = type.GetCustomAttribute<MapAttribute>();
            return attribute?.CommandType != CommandType.StoredProcedure;
        }

        public static bool IsInsertable<T>()
            where T : IDataEntity
        {
            return IsInsertable(typeof(T));
        }

        public static bool IsInsertable(this IDataEntity dataEntity)
        {
            return IsInsertable(dataEntity.GetType());
        }

        // IsMergeable
        internal static bool IsMergeable(Type type)
        {
            var attribute = type.GetCustomAttribute<MapAttribute>();
            return attribute?.CommandType != CommandType.StoredProcedure;
        }

        public static bool IsMergeable<T>()
            where T : IDataEntity
        {
            return IsMergeable(typeof(T));
        }

        public static bool IsMergeable(this IDataEntity dataEntity)
        {
            return IsMergeable(dataEntity.GetType());
        }

        // GetValue
        public static object GetValue(this IDataEntity dataEntity, string property)
        {
            return dataEntity?.GetType()
                .GetProperty(property)
                .GetValue(dataEntity);
        }

        public static T GetValue<T>(this IDataEntity dataEntity, string property)
        {
            return (T)GetValue(dataEntity, property);
        }
    }
}
