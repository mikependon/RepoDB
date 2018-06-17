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
    /// <summary>
    /// Contains the extension methods for <i>RepoDb.Interfaces.IDataEntity</i> object.
    /// </summary>
    public static class DataEntityExtension
    {
        // GetPropertiesFor
        internal static IEnumerable<PropertyInfo> GetPropertiesFor(Type type, Command command)
        {
            return type
                .GetProperties()
                .Where(property => !property.IsIgnored(command));
        }

        /// <summary>
        /// Gets the list of <i>System.Reflection.PropertyInfo</i> objects from the data entity class based on the
        /// target command.
        /// </summary>
        /// <typeparam name="T">The type of the data entity where to get the list of the properties.</typeparam>
        /// <param name="command">The target command.</param>
        /// <returns>The list of data entity properties based on the target command.</returns>
        public static IEnumerable<PropertyInfo> GetPropertiesFor<T>(Command command)
            where T : IDataEntity
        {
            return GetPropertiesFor(typeof(T), command);
        }

        /// <summary>
        /// Gets the list of <i>System.Reflection.PropertyInfo</i> objects from the data entity class based on the
        /// target command.
        /// </summary>
        /// <param name="dataEntity">The instance of the data entity where to get the list of the properties.</param>
        /// <param name="command">The target command.</param>
        /// <returns>The list of data entity properties based on the target command.</returns>
        public static IEnumerable<PropertyInfo> GetPropertiesFor(this IDataEntity dataEntity, Command command)
        {
            return GetPropertiesFor(dataEntity.GetType(), command);
        }

        // GetCommandType
        internal static CommandType GetCommandType(Type type, Command command)
        {
            return DataEntityMapper.For(type)?.Get(command)?.CommandType ?? CommandType.Text;
        }

        /// <summary>
        /// Gets a mapped command type of the <i>Data Entity</i> object based on the target command.
        /// </summary>
        /// <typeparam name="T">The entity type where to get the mapped command type.</typeparam>
        /// <param name="command">The target command.</param>
        /// <returns>A command type object used by the data entity for the target command.</returns>
        public static CommandType GetCommandType<T>(Command command)
            where T : IDataEntity
        {
            return GetCommandType(typeof(T), command);
        }

        /// <summary>
        /// Gets a mapped command type of the <i>Data Entity</i> object based on the target command.
        /// </summary>
        /// <param name="dataEntity">The instance of data entity where to get the mapped command type.</param>
        /// <param name="command">The target command.</param>
        /// <returns>A command type object used by the data entity for the target command.</returns>
        public static CommandType GetCommandType(this IDataEntity dataEntity, Command command)
        {
            return GetCommandType(dataEntity.GetType(), command);
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
            property = type.GetProperties().FirstOrDefault(p => p.Name.ToLower() == $"{GetMappedName(type, Command.Query).AsUnquoted()}{Constant.Id}".ToLower());
            if (property != null)
            {
                return property;
            }

            // Not found
            return null;
        }

        /// <summary>
        /// Gets the primary key property of the <i>Data Entity</i> object. The identification of the primary key will be based on the availability of certain
        /// attributes and naming convention.
        /// The identification process:
        /// 1. Checks the <i>RepoDb.Attributes.PrimaryKeyAttribute</i>.
        /// 2. If #1 is not present, then it checks the name of the property must be equal to <i>Id</i>.
        /// 3. If #2 is not present, then it checks the type name of the class appended by the word <i>Id</i>.
        /// 4. If #3 is not present, then it checks the mapped name of the class appended by the word <i>Id</i>
        /// </summary>
        /// <typeparam name="T">The type of the data entity where to get the primary key property.</typeparam>
        /// <returns>An instance of <i>System.Reflection.PropertyInfo</i> that corresponds to as a primary property of the data entity.</returns>
        public static PropertyInfo GetPrimaryProperty<T>()
            where T : IDataEntity
        {
            return GetPrimaryProperty(typeof(T));
        }

        /// <summary>
        /// Gets the primary key property of the <i>Data Entity</i> object. The identification of the primary key will be based on the availability of certain
        /// attributes and naming convention.
        /// The identification process:
        /// 1. Checks the <i>RepoDb.Attributes.PrimaryKeyAttribute</i>.
        /// 2. If #1 is not present, then it checks the name of the property must be equal to <i>Id</i>.
        /// 3. If #2 is not present, then it checks the type name of the class appended by the word <i>Id</i>.
        /// 4. If #3 is not present, then it checks the mapped name of the class appended by the word <i>Id</i>
        /// </summary>
        /// <param name="dataEntity">The instance of data entity where to get the primary key property.</param>
        /// <returns>An instance of <i>System.Reflection.PropertyInfo</i> that corresponds to as a primary property of the data entity.</returns>
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

        /// <summary>
        /// Gets the identity property of the data entiy type. The identification process is to check the implemented <i>RepoDb.Attributes.PrimaryKeyAttribute</i>
        /// where the <i>IsIdentity</i> property value is set to <i>True</i>.
        /// </summary>
        /// <typeparam name="T">The type of the data entity where to get the the identity property.</typeparam>
        /// <returns>An instance of <i>System.Reflection.PropertyInfo</i> that corresponds to as a identity property of the data entity.</returns>
        public static PropertyInfo GetIdentityProperty<T>()
            where T : IDataEntity
        {
            return GetIdentityProperty(typeof(T));
        }

        /// <summary>
        /// Gets the identity property of the data entiy type. The identification process is to check the implemented <i>RepoDb.Attributes.PrimaryKeyAttribute</i>
        /// where the <i>IsIdentity</i> property value is set to <i>True</i>.
        /// </summary>
        /// <typeparam name="T">The instance of the data entity where to get the the identity property.</typeparam>
        /// <returns>An instance of <i>System.Reflection.PropertyInfo</i> that corresponds to as a identity property of the data entity.</returns>
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

        /// <summary>
        /// Gets the mapped name of the data entity type on a target command. The identification process it to check the <i>RepoDb.Attributes.MapAttribute</i>
        /// and get the value of the <i>Name</i> property.
        /// </summary>
        /// <typeparam name="T">The type of the data entity where to get the mapped name.</typeparam>
        /// <param name="command">The target command.</param>
        /// <returns>A string that contains the mapped name for the target command.</returns>
        public static string GetMappedName<T>(Command command)
            where T : IDataEntity
        {
            return GetMappedName(typeof(T), command);
        }

        /// <summary>
        /// Gets the mapped name of the data entity type on a target command. The identification process it to check the <i>RepoDb.Attributes.MapAttribute</i>
        /// and get the value of the <i>Name</i> property.
        /// </summary>
        /// <param name="dataEntity">The instance of the data entity where to get the mapped name.</param>
        /// <param name="command">The target command.</param>
        /// <returns>A string that contains the mapped name for the target command.</returns>
        public static string GetMappedName(this IDataEntity dataEntity, Command command)
        {
            return GetMappedName(dataEntity.GetType(), command);
        }

        /// <summary>
        /// Converts the <i>Data Entity</i> object into a dynamic object. During the conversion, the passed query groups are being merged.
        /// </summary>
        /// <param name="dataEntity">The <i>Data Entity</i> object to be converted.</param>
        /// <param name="queryGroup">The query group to be merged.</param>
        /// <returns>An instance of converted dynamic object.</returns>
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
            ((QueryGroup)queryGroup)?
                .FixParameters()
                .GetAllQueryFields()?
                .ToList()
                .ForEach(queryField =>
                {
                    expandObject[queryField.Parameter.Name] = queryField.Parameter.Value;
                });
            return (ExpandoObject)expandObject;
        }

        /// <summary>
        /// Converts the <i>Data Entity</i> object into a dynamic object.
        /// </summary>
        /// <param name="dataEntity">The <i>Data Entity</i> object to be converted.</param>
        /// <returns>An instance of converted dynamic object.</returns>
        public static object AsObject(this IDataEntity dataEntity)
        {
            return AsObject(dataEntity, null);
        }

        // AsDataTable
        internal static DataTable AsDataTable<T>(this IEnumerable<T> entities, IDbConnection connection, Command command = Command.None)
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
                        if (property != null)
                        {
                            dict.Add(dataColumn, property);
                        }
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

        // IsBatchQueryable
        internal static bool IsBatchQueryable(Type type)
        {
            var commandType = CommandTypeCache.Get(type, Command.BatchQuery);
            return commandType == CommandType.Text;
        }

        /// <summary>
        /// Checks whether the data entity is batch queryable.
        /// </summary>
        /// <typeparam name="T">The data entity type to be checked.</typeparam>
        /// <returns>A boolean value signifies whether the data entity is batch queryable.</returns>
        public static bool IsBatchQueryable<T>()
            where T : IDataEntity
        {
            return IsBatchQueryable(typeof(T));
        }

        /// <summary>
        /// Checks whether the data entity is batch queryable.
        /// </summary>
        /// <param name="dataEntity">The data entity instance to be checked.</param>
        /// <returns>A boolean value signifies whether the data entity is batch queryable.</returns>
        public static bool IsBatchQueryable(this IDataEntity dataEntity)
        {
            return IsQueryable(dataEntity.GetType());
        }

        // IsBulkInsertable
        internal static bool IsBulkInsertable(Type type)
        {
            var commandType = CommandTypeCache.Get(type, Command.BulkInsert);
            return commandType != CommandType.StoredProcedure;
        }

        /// <summary>
        /// Checks whether the data entity is bulk insertable.
        /// </summary>
        /// <typeparam name="T">The data entity type to be checked.</typeparam>
        /// <returns>A boolean value signifies whether the data entity is bulk insertable.</returns>
        public static bool IsBulkInsertable<T>()
            where T : IDataEntity
        {
            return IsBulkInsertable(typeof(T));
        }

        /// <summary>
        /// Checks whether the data entity is bulk insertable.
        /// </summary>
        /// <param name="dataEntity">The data entity instance to be checked.</param>
        /// <returns>A boolean value signifies whether the data entity is bulk insertable.</returns>
        public static bool IsBulkInsertable(this IDataEntity dataEntity)
        {
            return IsQueryable(dataEntity.GetType());
        }

        // IsBigCountable
        internal static bool IsBigCountable(Type type)
        {
            var commandType = CommandTypeCache.Get(type, Command.Count);
            return commandType == CommandType.Text;
        }

        /// <summary>
        /// Checks whether the data entity is bulk insertable.
        /// </summary>
        /// <typeparam name="T">The data entity type to be checked.</typeparam>
        /// <returns>A boolean value signifies whether the data entity is bulk insertable.</returns>
        public static bool IsBigCountable<T>()
            where T : IDataEntity
        {
            return IsBigCountable(typeof(T));
        }

        /// <summary>
        /// Checks whether the data entity is bulk insertable.
        /// </summary>
        /// <param name="dataEntity">The data entity instance to be checked.</param>
        /// <returns>A boolean value signifies whether the data entity is bulk insertable.</returns>
        public static bool IsBigCountable(this IDataEntity dataEntity)
        {
            return IsBigCountable(dataEntity.GetType());
        }

        // IsCountable
        internal static bool IsCountable(Type type)
        {
            var commandType = CommandTypeCache.Get(type, Command.Count);
            return commandType == CommandType.Text;
        }

        /// <summary>
        /// Checks whether the data entity is bulk insertable.
        /// </summary>
        /// <typeparam name="T">The data entity type to be checked.</typeparam>
        /// <returns>A boolean value signifies whether the data entity is bulk insertable.</returns>
        public static bool IsCountable<T>()
            where T : IDataEntity
        {
            return IsCountable(typeof(T));
        }

        /// <summary>
        /// Checks whether the data entity is bulk insertable.
        /// </summary>
        /// <param name="dataEntity">The data entity instance to be checked.</param>
        /// <returns>A boolean value signifies whether the data entity is bulk insertable.</returns>
        public static bool IsCountable(this IDataEntity dataEntity)
        {
            return IsCountable(dataEntity.GetType());
        }

        // IsDeletable
        internal static bool IsDeletable(Type type)
        {
            return true;
        }

        /// <summary>
        /// Checks whether the data entity is deletable.
        /// </summary>
        /// <typeparam name="T">The data entity type to be checked.</typeparam>
        /// <returns>A boolean value signifies whether the data entity is deletable.</returns>
        public static bool IsDeletable<T>()
            where T : IDataEntity
        {
            return IsDeletable(typeof(T));
        }

        /// <summary>
        /// Checks whether the data entity is deletable.
        /// </summary>
        /// <param name="dataEntity">The data entity instance to be checked.</param>
        /// <returns>A boolean value signifies whether the data entity is deletable.</returns>
        public static bool IsDeletable(this IDataEntity dataEntity)
        {
            return IsDeletable(dataEntity.GetType());
        }

        // IsInlineUpdateable
        internal static bool IsInlineUpdateable(Type type)
        {
            var commandType = CommandTypeCache.Get(type, Command.InlineUpdate);
            return commandType == CommandType.Text;
        }

        /// <summary>
        /// Checks whether the data entity is inline updateable.
        /// </summary>
        /// <typeparam name="T">The data entity type to be checked.</typeparam>
        /// <returns>A boolean value signifies whether the data entity is inline updateable.</returns>
        public static bool IsInlineUpdateable<T>()
            where T : IDataEntity
        {
            return IsInlineUpdateable(typeof(T));
        }

        /// <summary>
        /// Checks whether the data entity is inline updateable.
        /// </summary>
        /// <param name="dataEntity">The data entity instance to be checked.</param>
        /// <returns>A boolean value signifies whether the data entity is inline updateable.</returns>
        public static bool IsInlineUpdateable(this IDataEntity dataEntity)
        {
            return IsInlineUpdateable(dataEntity.GetType());
        }

        // IsInsertable
        internal static bool IsInsertable(Type type)
        {
            return true;
        }

        /// <summary>
        /// Checks whether the data entity is insertable.
        /// </summary>
        /// <typeparam name="T">The data entity type to be checked.</typeparam>
        /// <returns>A boolean value signifies whether the data entity is insertable.</returns>
        public static bool IsInsertable<T>()
            where T : IDataEntity
        {
            return IsInsertable(typeof(T));
        }

        /// <summary>
        /// Checks whether the data entity is insertable.
        /// </summary>
        /// <param name="dataEntity">The data entity instance to be checked.</param>
        /// <returns>A boolean value signifies whether the data entity is insertable.</returns>
        public static bool IsInsertable(this IDataEntity dataEntity)
        {
            return IsInsertable(dataEntity.GetType());
        }

        // IsMergeable
        internal static bool IsMergeable(Type type)
        {
            var commandType = CommandTypeCache.Get(type, Command.Merge);
            return commandType == CommandType.Text;
        }

        /// <summary>
        /// Checks whether the data entity is mergeable.
        /// </summary>
        /// <typeparam name="T">The data entity type to be checked.</typeparam>
        /// <returns>A boolean value signifies whether the data entity is mergeable.</returns>
        public static bool IsMergeable<T>()
            where T : IDataEntity
        {
            return IsMergeable(typeof(T));
        }

        /// <summary>
        /// Checks whether the data entity is mergeable.
        /// </summary>
        /// <param name="dataEntity">The data entity instance to be checked.</param>
        /// <returns>A boolean value signifies whether the data entity is mergeable.</returns>
        public static bool IsMergeable(this IDataEntity dataEntity)
        {
            return IsMergeable(dataEntity.GetType());
        }

        // IsQueryable
        internal static bool IsQueryable(Type type)
        {
            return true;
        }

        /// <summary>
        /// Checks whether the data entity is queryable.
        /// </summary>
        /// <typeparam name="T">The data entity type to be checked.</typeparam>
        /// <returns>A boolean value signifies whether the data entity is queryable.</returns>
        public static bool IsQueryable<T>()
            where T : IDataEntity
        {
            return IsQueryable(typeof(T));
        }

        /// <summary>
        /// Checks whether the data entity is queryable.
        /// </summary>
        /// <param name="dataEntity">The data entity instance to be checked.</param>
        /// <returns>A boolean value signifies whether the data entity is queryable.</returns>
        public static bool IsQueryable(this IDataEntity dataEntity)
        {
            return IsQueryable(dataEntity.GetType());
        }

        // IsUpdateable
        internal static bool IsUpdateable(Type type)
        {
            return true;
        }

        /// <summary>
        /// Checks whether the data entity is updateable.
        /// </summary>
        /// <typeparam name="T">The data entity type to be checked.</typeparam>
        /// <returns>A boolean value signifies whether the data entity is updateable.</returns>
        public static bool IsUpdateable<T>()
            where T : IDataEntity
        {
            return IsUpdateable(typeof(T));
        }

        /// <summary>
        /// Checks whether the data entity is updateable.
        /// </summary>
        /// <param name="dataEntity">The data entity instance to be checked.</param>
        /// <returns>A boolean value signifies whether the data entity is updateable.</returns>
        public static bool IsUpdateable(this IDataEntity dataEntity)
        {
            return IsUpdateable(dataEntity.GetType());
        }
    }
}
