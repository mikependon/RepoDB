using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using RepoDb.Attributes;
using RepoDb.Enumerations;

namespace RepoDb.Extensions
{
    /// <summary>
    /// Contains the extension methods for <i>RepoDb.DataEntity</i> object.
    /// </summary>
    public static class DataEntityExtension
    {
        /// <summary>
        /// Converts the value to the type of the primary property of the target data entity.
        /// </summary>
        /// <typeparam name="T">The type of the <i>DataEntity</i>.</typeparam>
        /// <param name="value">The value to be converted.</param>
        /// <returns>The converted value to primary property type.</returns>
        internal static object ValueToPrimaryType<T>(object value) where T : DataEntity
        {
            if (value == null || value == DBNull.Value)
            {
                return null;
            }
            var primary = GetPrimaryProperty<T>();
            if (primary != null)
            {
                if (primary.PropertyType == typeof(Guid))
                {
                    value = Guid.Parse(value.ToString());
                }
                else
                {
                    value = Convert.ChangeType(value, primary.PropertyType);
                }
            }
            return value;
        }

        // GetDataEntityChildrenData

        /// <summary>
        /// Gets the recursive data of the target <i>DataEntity</i> object.
        /// </summary>
        /// <param name="type">The type of the target <i>DataEntity</i>.</param>
        /// <returns>An enumerable list of <i>RecursiveData</i> object.</returns>
        internal static IEnumerable<DataEntityChildListData> GetDataEntityChildrenData(Type type)
        {
            return type?
                .GetProperties()
                .Where(property => property.IsRecursive())
                .Select(property => new DataEntityChildListData()
                {
                    ChildListType = property.PropertyType.GetGenericArguments().First(),
                    ParentDataEntityType = type,
                    ChildListProperty = property
                });
        }

        /// <summary>
        /// Gets the recursive data of the target <i>DataEntity</i> object.
        /// </summary>
        /// <typeparam name="T">The type of the target <i>DataEntity</i>.</typeparam>
        /// <returns>An enumerable list of <i>RecursiveData</i> object.</returns>
        internal static IEnumerable<DataEntityChildListData> GetDataEntityChildrenData<T>() where T : DataEntity
        {
            return GetDataEntityChildrenData(typeof(T));
        }

        /// <summary>
        /// Gets the recursive data of the target <i>DataEntity</i> object.
        /// </summary>
        /// <param name="dataEntity">The target <i>DataEntity</i> object.</param>
        /// <returns>An enumerable list of <i>RecursiveData</i> object.</returns>
        internal static IEnumerable<DataEntityChildListData> GetDataEntityChildrenData(this DataEntity dataEntity)
        {
            return GetDataEntityChildrenData(dataEntity.GetType());
        }

        // GetPropertiesFor
        internal static IEnumerable<PropertyInfo> GetPropertiesFor(Type type, Command command)
        {
            return type
                .GetProperties()
                .Where(property => !property.IsIgnored(command) && !property.IsRecursive());
        }

        /// <summary>
        /// Gets the list of <i>System.Reflection.PropertyInfo</i> objects from the data entity class based on the
        /// target command.
        /// </summary>
        /// <typeparam name="T">The type of the data entity where to get the list of the properties.</typeparam>
        /// <param name="command">The target command.</param>
        /// <returns>The list of data entity properties based on the target command.</returns>
        public static IEnumerable<PropertyInfo> GetPropertiesFor<T>(Command command)
            where T : DataEntity
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
        public static IEnumerable<PropertyInfo> GetPropertiesFor(this DataEntity dataEntity, Command command)
        {
            return GetPropertiesFor(dataEntity.GetType(), command);
        }

        // GetCommandType
        internal static CommandType GetCommandType(Type type, Command command)
        {
            return DataEntityMapper.For(type)?.Get(command)?.CommandType ?? CommandType.Text;
        }

        /// <summary>
        /// Gets a mapped command type of the <i>DataEntity</i> object based on the target command.
        /// </summary>
        /// <typeparam name="T">The entity type where to get the mapped command type.</typeparam>
        /// <param name="command">The target command.</param>
        /// <returns>A command type object used by the data entity for the target command.</returns>
        public static CommandType GetCommandType<T>(Command command)
            where T : DataEntity
        {
            return GetCommandType(typeof(T), command);
        }

        /// <summary>
        /// Gets a mapped command type of the <i>DataEntity</i> object based on the target command.
        /// </summary>
        /// <param name="dataEntity">The instance of data entity where to get the mapped command type.</param>
        /// <param name="command">The target command.</param>
        /// <returns>A command type object used by the data entity for the target command.</returns>
        public static CommandType GetCommandType(this DataEntity dataEntity, Command command)
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
            where T : DataEntity
        {
            return GetPropertyByAttribute(typeof(T), attributeType);
        }

        internal static PropertyInfo GetPropertyByAttribute(this DataEntity dataEntity, Type attributeType)
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
            property = type.GetProperties().FirstOrDefault(p => p.Name.ToLower() == StringConstant.Id.ToLower());
            if (property != null)
            {
                return property;
            }

            // Type.Name + Id
            property = type.GetProperties().FirstOrDefault(p => p.Name.ToLower() == $"{type.Name}{StringConstant.Id}".ToLower());
            if (property != null)
            {
                return property;
            }

            // Mapping.Name + Id
            property = type.GetProperties().FirstOrDefault(p => p.Name.ToLower() == $"{GetMappedName(type, Command.Query).AsUnquoted()}{StringConstant.Id}".ToLower());
            if (property != null)
            {
                return property;
            }

            // Not found
            return null;
        }

        /// <summary>
        /// Gets the primary key property of the <i>DataEntity</i> object. The identification of the primary key will be based on the availability of certain
        /// attributes and naming convention.
        /// The identification process:
        /// 1. Checks the <i>RepoDb.Attributes.PrimaryKeyAttribute</i>.
        /// 2. If #1 is not present, then it checks the name of the property must be equal to <i>Id</i>.
        /// 3. If #2 is not present, then it checks the type name of the class appended by the word <i>Id</i>.
        /// 4. If #3 is not present, then it checks the mapped name of the class appended by the word <i>Id</i>.
        /// </summary>
        /// <typeparam name="T">The type of the data entity where to get the primary key property.</typeparam>
        /// <returns>An instance of <i>System.Reflection.PropertyInfo</i> that corresponds to as a primary property of the data entity.</returns>
        public static PropertyInfo GetPrimaryProperty<T>()
            where T : DataEntity
        {
            return GetPrimaryProperty(typeof(T));
        }

        /// <summary>
        /// Gets the primary key property of the <i>DataEntity</i> object. The identification of the primary key will be based on the availability of certain
        /// attributes and naming convention.
        /// The identification process:
        /// 1. Checks the <i>RepoDb.Attributes.PrimaryKeyAttribute</i>.
        /// 2. If #1 is not present, then it checks the name of the property must be equal to <i>Id</i>.
        /// 3. If #2 is not present, then it checks the type name of the class appended by the word <i>Id</i>.
        /// 4. If #3 is not present, then it checks the mapped name of the class appended by the word <i>Id</i>
        /// </summary>
        /// <param name="dataEntity">The instance of data entity where to get the primary key property.</param>
        /// <returns>An instance of <i>System.Reflection.PropertyInfo</i> that corresponds to as a primary property of the data entity.</returns>
        public static PropertyInfo GetPrimaryProperty(this DataEntity dataEntity)
        {
            return GetPrimaryProperty(dataEntity.GetType());
        }

        // GetMappedName
        internal static string GetMappedName(Type type, Command command)
        {
            return DataEntityMapper.For(type)?.Get(command)?.Name ??
                type.GetCustomAttribute<MapAttribute>()?.Name ?? type.Name;
        }

        /// <summary>
        /// Gets the mapped name of the data entity type on a target command. The identification process it to check the <i>RepoDb.Attributes.MapAttribute</i>
        /// and get the value of the <i>Name</i> property.
        /// </summary>
        /// <typeparam name="T">The type of the data entity where to get the mapped name.</typeparam>
        /// <param name="command">The target command.</param>
        /// <returns>A string that contains the mapped name for the target command.</returns>
        public static string GetMappedName<T>(Command command)
            where T : DataEntity
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
        public static string GetMappedName(this DataEntity dataEntity, Command command)
        {
            return GetMappedName(dataEntity.GetType(), command);
        }

        /// <summary>
        /// Converts the <i>DataEntity</i> object into a dynamic object. During the conversion, the passed query groups are being merged.
        /// </summary>
        /// <param name="dataEntity">The <i>DataEntity</i> object to be converted.</param>
        /// <param name="queryGroup">The query group to be merged.</param>
        /// <returns>An instance of converted dynamic object.</returns>
        public static object AsObject(this DataEntity dataEntity, QueryGroup queryGroup)
        {
            return AsObject(dataEntity, queryGroup, Command.None);
        }

        /// <summary>
        /// Converts the <i>DataEntity</i> object into a dynamic object. During the conversion, the passed query groups are being merged.
        /// </summary>
        /// <param name="dataEntity">The <i>DataEntity</i> object to be converted.</param>
        /// <param name="queryGroup">The query group to be merged.</param>
        /// <param name="command">The target command type to be used for the object transformation.</param>
        /// <returns>An instance of converted dynamic object.</returns>
        public static object AsObject(this DataEntity dataEntity, QueryGroup queryGroup, Command command)
        {
            var expandObject = new ExpandoObject() as IDictionary<string, object>;
            var properties = GetPropertiesFor(dataEntity.GetType(), command);
            properties?
                .ToList()
                .ForEach(property =>
                {
                    expandObject[property.GetMappedName()] = property.GetValue(dataEntity);
                });
            queryGroup?
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
        /// Converts the <i>DataEntity</i> object into a dynamic object.
        /// </summary>
        /// <param name="dataEntity">The <i>DataEntity</i> object to be converted.</param>
        /// <returns>An instance of converted dynamic object.</returns>
        public static object AsObject(this DataEntity dataEntity)
        {
            return AsObject(dataEntity, null, Command.None);
        }

        /// <summary>
        /// Converts the <i>DataEntity</i> object into a dynamic object.
        /// </summary>
        /// <param name="dataEntity">The <i>DataEntity</i> object to be converted.</param>
        /// <param name="command">The target command type to be used for the object transformation.</param>
        /// <returns>An instance of converted dynamic object.</returns>
        public static object AsObject(this DataEntity dataEntity, Command command)
        {
            return AsObject(dataEntity, null, command);
        }

        // IsBatchQueryable
        internal static bool IsBatchQueryable(Type type)
        {
            var commandType = GetCommandType(type, Command.BatchQuery);
            return commandType != CommandType.TableDirect;
        }

        /// <summary>
        /// Checks whether the data entity is batch queryable.
        /// </summary>
        /// <typeparam name="T">The data entity type to be checked.</typeparam>
        /// <returns>A boolean value signifies whether the data entity is batch queryable.</returns>
        public static bool IsBatchQueryable<T>()
            where T : DataEntity
        {
            return IsBatchQueryable(typeof(T));
        }

        /// <summary>
        /// Checks whether the data entity is batch queryable.
        /// </summary>
        /// <param name="dataEntity">The data entity instance to be checked.</param>
        /// <returns>A boolean value signifies whether the data entity is batch queryable.</returns>
        public static bool IsBatchQueryable(this DataEntity dataEntity)
        {
            return IsQueryable(dataEntity.GetType());
        }

        // IsBulkInsertable

        internal static bool IsBulkInsertable(Type type)
        {
            var commandType = GetCommandType(type, Command.BulkInsert);
            return commandType != CommandType.StoredProcedure;
        }

        /// <summary>
        /// Checks whether the data entity is bulk insertable.
        /// </summary>
        /// <typeparam name="T">The data entity type to be checked.</typeparam>
        /// <returns>A boolean value signifies whether the data entity is bulk insertable.</returns>
        public static bool IsBulkInsertable<T>()
            where T : DataEntity
        {
            return IsBulkInsertable(typeof(T));
        }

        /// <summary>
        /// Checks whether the data entity is bulk insertable.
        /// </summary>
        /// <param name="dataEntity">The data entity instance to be checked.</param>
        /// <returns>A boolean value signifies whether the data entity is bulk insertable.</returns>
        public static bool IsBulkInsertable(this DataEntity dataEntity)
        {
            return IsQueryable(dataEntity.GetType());
        }

        // IsCountable

        internal static bool IsCountable(Type type)
        {
            return true;
        }

        /// <summary>
        /// Checks whether the data entity is countable.
        /// </summary>
        /// <typeparam name="T">The data entity type to be checked.</typeparam>
        /// <returns>A boolean value signifies whether the data entity is countable.</returns>
        public static bool IsCountable<T>()
            where T : DataEntity
        {
            return IsCountable(typeof(T));
        }

        /// <summary>
        /// Checks whether the data entity is countable.
        /// </summary>
        /// <param name="dataEntity">The data entity instance to be checked.</param>
        /// <returns>A boolean value signifies whether the data entity is countable.</returns>
        public static bool IsCountable(this DataEntity dataEntity)
        {
            return IsCountable(dataEntity.GetType());
        }

        // IsDeletableAll

        internal static bool IsDeletableAll(Type type)
        {
            return true;
        }

        /// <summary>
        /// Checks whether all data entity is deletable.
        /// </summary>
        /// <typeparam name="T">The data entity type to be checked.</typeparam>
        /// <returns>A boolean value signifies whether all the data entity is deletable.</returns>
        public static bool IsDeletableAll<T>()
            where T : DataEntity
        {
            return IsDeletableAll(typeof(T));
        }

        /// <summary>
        /// Checks whether all data entity is deletable.
        /// </summary>
        /// <param name="dataEntity">The data entity instance to be checked.</param>
        /// <returns>A boolean value signifies whether all the data entity is deletable.</returns>
        public static bool IsDeletableAll(this DataEntity dataEntity)
        {
            return IsDeletableAll(dataEntity.GetType());
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
            where T : DataEntity
        {
            return IsDeletable(typeof(T));
        }

        /// <summary>
        /// Checks whether the data entity is deletable.
        /// </summary>
        /// <param name="dataEntity">The data entity instance to be checked.</param>
        /// <returns>A boolean value signifies whether the data entity is deletable.</returns>
        public static bool IsDeletable(this DataEntity dataEntity)
        {
            return IsDeletable(dataEntity.GetType());
        }

        // IsTruncatable

        internal static bool IsTruncatable(Type type)
        {
            return true;
        }

        /// <summary>
        /// Checks whether the table can be truncated.
        /// </summary>
        /// <typeparam name="T">The data entity type to be checked.</typeparam>
        /// <returns>A boolean value signifies whether the table is truncatable.</returns>
        public static bool IsTruncatable<T>()
            where T : DataEntity
        {
            return IsTruncatable(typeof(T));
        }

        /// <summary>
        /// Checks whether the table can be truncated.
        /// </summary>
        /// <returns>A boolean value signifies whether the table is truncatable.</returns>
        public static bool IsTruncatable(this DataEntity dataEntity)
        {
            return IsTruncatable(dataEntity.GetType());
        }

        // IsInlineUpdateable

        internal static bool IsInlineUpdateable(Type type)
        {
            var commandType = GetCommandType(type, Command.InlineUpdate);
            return commandType != CommandType.StoredProcedure;
        }

        /// <summary>
        /// Checks whether the data entity is inline updateable.
        /// </summary>
        /// <typeparam name="T">The data entity type to be checked.</typeparam>
        /// <returns>A boolean value signifies whether the data entity is inline updateable.</returns>
        public static bool IsInlineUpdateable<T>()
            where T : DataEntity
        {
            return IsInlineUpdateable(typeof(T));
        }

        /// <summary>
        /// Checks whether the data entity is inline updateable.
        /// </summary>
        /// <param name="dataEntity">The data entity instance to be checked.</param>
        /// <returns>A boolean value signifies whether the data entity is inline updateable.</returns>
        public static bool IsInlineUpdateable(this DataEntity dataEntity)
        {
            return IsInlineUpdateable(dataEntity.GetType());
        }

        // IsInlineInsertable

        internal static bool IsInlineInsertable(Type type)
        {
            var commandType = GetCommandType(type, Command.InlineInsert);
            return commandType != CommandType.StoredProcedure;
        }

        /// <summary>
        /// Checks whether the data entity is inline insertable.
        /// </summary>
        /// <typeparam name="T">The data entity type to be checked.</typeparam>
        /// <returns>A boolean value signifies whether the data entity is inline insertable.</returns>
        public static bool IsInlineInsertable<T>()
            where T : DataEntity
        {
            return IsInlineInsertable(typeof(T));
        }

        /// <summary>
        /// Checks whether the data entity is inline insertable.
        /// </summary>
        /// <param name="dataEntity">The data entity instance to be checked.</param>
        /// <returns>A boolean value signifies whether the data entity is inline insertable.</returns>
        public static bool IsInlineInsertable(this DataEntity dataEntity)
        {
            return IsInlineInsertable(dataEntity.GetType());
        }

        // IsInlineMergeable

        internal static bool IsInlineMergeable(Type type)
        {
            var commandType = GetCommandType(type, Command.InlineUpdate);
            return commandType != CommandType.StoredProcedure;
        }

        /// <summary>
        /// Checks whether the data entity is inline mergeable.
        /// </summary>
        /// <typeparam name="T">The data entity type to be checked.</typeparam>
        /// <returns>A boolean value signifies whether the data entity is inline mergeable.</returns>
        public static bool IsInlineMergeable<T>()
            where T : DataEntity
        {
            return IsInlineMergeable(typeof(T));
        }

        /// <summary>
        /// Checks whether the data entity is inline mergeable.
        /// </summary>
        /// <param name="dataEntity">The data entity instance to be checked.</param>
        /// <returns>A boolean value signifies whether the data entity is inline mergeable.</returns>
        public static bool IsInlineMergeable(this DataEntity dataEntity)
        {
            return IsInlineMergeable(dataEntity.GetType());
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
            where T : DataEntity
        {
            return IsInsertable(typeof(T));
        }

        /// <summary>
        /// Checks whether the data entity is insertable.
        /// </summary>
        /// <param name="dataEntity">The data entity instance to be checked.</param>
        /// <returns>A boolean value signifies whether the data entity is insertable.</returns>
        public static bool IsInsertable(this DataEntity dataEntity)
        {
            return IsInsertable(dataEntity.GetType());
        }

        // IsMergeable
        internal static bool IsMergeable(Type type)
        {
            var commandType = GetCommandType(type, Command.Merge);
            return commandType != CommandType.TableDirect;
        }

        /// <summary>
        /// Checks whether the data entity is mergeable.
        /// </summary>
        /// <typeparam name="T">The data entity type to be checked.</typeparam>
        /// <returns>A boolean value signifies whether the data entity is mergeable.</returns>
        public static bool IsMergeable<T>()
            where T : DataEntity
        {
            return IsMergeable(typeof(T));
        }

        /// <summary>
        /// Checks whether the data entity is mergeable.
        /// </summary>
        /// <param name="dataEntity">The data entity instance to be checked.</param>
        /// <returns>A boolean value signifies whether the data entity is mergeable.</returns>
        public static bool IsMergeable(this DataEntity dataEntity)
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
            where T : DataEntity
        {
            return IsQueryable(typeof(T));
        }

        /// <summary>
        /// Checks whether the data entity is queryable.
        /// </summary>
        /// <param name="dataEntity">The data entity instance to be checked.</param>
        /// <returns>A boolean value signifies whether the data entity is queryable.</returns>
        public static bool IsQueryable(this DataEntity dataEntity)
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
            where T : DataEntity
        {
            return IsUpdateable(typeof(T));
        }

        /// <summary>
        /// Checks whether the data entity is updateable.
        /// </summary>
        /// <param name="dataEntity">The data entity instance to be checked.</param>
        /// <returns>A boolean value signifies whether the data entity is updateable.</returns>
        public static bool IsUpdateable(this DataEntity dataEntity)
        {
            return IsUpdateable(dataEntity.GetType());
        }
    }
}
