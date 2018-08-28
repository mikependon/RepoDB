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
    /// Contains the extension methods for data entity object.
    /// </summary>
    public static class DataEntityExtension
    {
        /// <summary>
        /// Converts the value to the type of the primary property of the target data entity.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="value">The value to be converted.</param>
        /// <returns>The converted value to primary property type.</returns>
        internal static object ValueToPrimaryType<TEntity>(object value)
            where TEntity : class
        {
            if (value == null || value == DBNull.Value)
            {
                return null;
            }
            var primary = PrimaryKeyCache.Get<TEntity>();
            if (primary != null)
            {
                if (primary.PropertyInfo.PropertyType == typeof(Guid))
                {
                    value = Guid.Parse(value.ToString());
                }
                else
                {
                    value = Convert.ChangeType(value, primary.PropertyInfo.PropertyType);
                }
            }
            return value;
        }

        // GetDataEntityChildrenData

        /// <summary>
        /// Gets the recursive data of the target data entity object.
        /// </summary>
        /// <param name="type">The type of the target data entity.</param>
        /// <returns>An enumerable list of <see cref="DataEntityChildListData"/> object.</returns>
        internal static IEnumerable<DataEntityChildListData> GetDataEntityChildrenData(Type type)
        {
            return type?
                .GetTypeInfo()
                .GetProperties()
                .Where(property => property.IsRecursive())
                .Select(property => new DataEntityChildListData()
                {
                    ChildListType = property.PropertyType.GetTypeInfo().GetGenericArguments().First(),
                    ParentDataEntityType = type,
                    ChildListProperty = property
                });
        }

        /// <summary>
        /// Gets the recursive data of the target data entity object.
        /// </summary>
        /// <typeparam name="T">The type of the target data entity.</typeparam>
        /// <returns>An enumerable list of <see cref="DataEntityChildListData"/> object.</returns>
        internal static IEnumerable<DataEntityChildListData> GetDataEntityChildrenData<T>()
        {
            return GetDataEntityChildrenData(typeof(T));
        }

        // GetPropertiesFor
        internal static IEnumerable<ClassProperty> GetPropertiesFor(Type type, Command command)
        {
            return type
                .GetTypeInfo()
                .GetProperties()
                .Select(property => new ClassProperty(property))
                .Where(property =>
                    property.IsIgnored(command) == false &&
                    property.IsRecursive() == false);
        }

        /// <summary>
        /// Gets the list of <see cref="PropertyInfo"/> objects from the data entity class based on the
        /// target command.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity where to get the list of the properties.</typeparam>
        /// <param name="command">The target command.</param>
        /// <returns>The list of data entity properties based on the target command.</returns>
        public static IEnumerable<ClassProperty> GetPropertiesFor<TEntity>(Command command)
            where TEntity : class
        {
            return GetPropertiesFor(typeof(TEntity), command);
        }

        // GetCommandType
        internal static CommandType GetCommandType(Type type, Command command)
        {
            return DataEntityMapper.For(type)?.Get(command)?.CommandType ?? CommandType.Text;
        }

        /// <summary>
        /// Gets a mapped command type of the data entity object based on the target command.
        /// </summary>
        /// <typeparam name="TEntity">The entity type where to get the mapped command type.</typeparam>
        /// <param name="command">The target command.</param>
        /// <returns>A command type object used by the data entity for the target command.</returns>
        public static CommandType GetCommandType<TEntity>(Command command)
            where TEntity : class
        {
            return GetCommandType(typeof(TEntity), command);
        }

        // GetPropertyByAttribute

        internal static PropertyInfo GetPropertyByAttribute(Type type, Type attributeType)
        {
            return type
                .GetTypeInfo()
                .GetProperties()
                .FirstOrDefault(property => property.GetCustomAttribute(attributeType) != null);
        }

        internal static PropertyInfo GetPropertyByAttribute<TEntity>(Type attributeType)
            where TEntity : class
        {
            return GetPropertyByAttribute(typeof(TEntity), attributeType);
        }

        // GetMappedName
        internal static string GetMappedName(Type type, Command command)
        {
            return DataEntityMapper.For(type)?.Get(command)?.Name ??
                type.GetTypeInfo().GetCustomAttribute<MapAttribute>()?.Name ?? type.Name;
        }

        /// <summary>
        /// Gets the mapped name of the data entity type on a target command. The identification process it to check the <see cref="MapAttribute"/>
        /// and get the value of the name property.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity where to get the mapped name.</typeparam>
        /// <param name="command">The target command.</param>
        /// <returns>A string that contains the mapped name for the target command.</returns>
        public static string GetMappedName<TEntity>(Command command)
            where TEntity : class
        {
            return GetMappedName(typeof(TEntity), command);
        }

        /// <summary>
        /// Converts the data entity object into a dynamic object. During the conversion, the passed query groups are being merged.
        /// </summary>
        /// <param name="dataEntity">The data entity object to be converted.</param>
        /// <param name="queryGroup">The query group to be merged.</param>
        /// <param name="command">The target command type to be used for the object transformation.</param>
        /// <returns>An instance of converted dynamic object.</returns>
        internal static object AsObject(this object dataEntity, QueryGroup queryGroup, Command command)
        {
            var expandObject = new ExpandoObject() as IDictionary<string, object>;
            var properties = GetPropertiesFor(dataEntity.GetType(), command);
            properties?
                .ToList()
                .ForEach(property =>
                {
                    expandObject[property.GetMappedName()] = property.PropertyInfo.GetValue(dataEntity);
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
        /// Converts the data entity object into a dynamic object.
        /// </summary>
        /// <param name="dataEntity">The data entity object to be converted.</param>
        /// <returns>An instance of converted dynamic object.</returns>
        internal static object AsObject(this object dataEntity)
        {
            return AsObject(dataEntity, null, Command.None);
        }

        /// <summary>
        /// Converts the data entity object into a dynamic object.
        /// </summary>
        /// <param name="dataEntity">The data entity object to be converted.</param>
        /// <param name="command">The target command type to be used for the object transformation.</param>
        /// <returns>An instance of converted dynamic object.</returns>
        internal static object AsObject(this object dataEntity, Command command)
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
        /// <typeparam name="TEntity">The data entity type to be checked.</typeparam>
        /// <returns>A boolean value signifies whether the data entity is batch queryable.</returns>
        internal static bool IsBatchQueryable<TEntity>()
            where TEntity : class
        {
            return IsBatchQueryable(typeof(TEntity));
        }

        /// <summary>
        /// Checks whether the data entity is batch queryable.
        /// </summary>
        /// <param name="dataEntity">The data entity instance to be checked.</param>
        /// <returns>A boolean value signifies whether the data entity is batch queryable.</returns>
        internal static bool IsBatchQueryable(this object dataEntity)
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
        /// <typeparam name="TEntity">The data entity type to be checked.</typeparam>
        /// <returns>A boolean value signifies whether the data entity is bulk insertable.</returns>
        internal static bool IsBulkInsertable<TEntity>()
            where TEntity : class
        {
            return IsBulkInsertable(typeof(TEntity));
        }

        // IsCountable

        internal static bool IsCountable(Type type)
        {
            return true;
        }

        /// <summary>
        /// Checks whether the data entity is countable.
        /// </summary>
        /// <typeparam name="TEntity">The data entity type to be checked.</typeparam>
        /// <returns>A boolean value signifies whether the data entity is countable.</returns>
        internal static bool IsCountable<TEntity>()
            where TEntity : class
        {
            return IsCountable(typeof(TEntity));
        }

        /// <summary>
        /// Checks whether the data entity is countable.
        /// </summary>
        /// <param name="dataEntity">The data entity instance to be checked.</param>
        /// <returns>A boolean value signifies whether the data entity is countable.</returns>
        internal static bool IsCountable(this object dataEntity)
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
        /// <typeparam name="TEntity">The data entity type to be checked.</typeparam>
        /// <returns>A boolean value signifies whether all the data entity is deletable.</returns>
        internal static bool IsDeletableAll<TEntity>()
            where TEntity : class
        {
            return IsDeletableAll(typeof(TEntity));
        }

        // IsDeletable

        internal static bool IsDeletable(Type type)
        {
            return true;
        }

        /// <summary>
        /// Checks whether the data entity is deletable.
        /// </summary>
        /// <typeparam name="TEntity">The data entity type to be checked.</typeparam>
        /// <returns>A boolean value signifies whether the data entity is deletable.</returns>
        internal static bool IsDeletable<TEntity>()
            where TEntity : class
        {
            return IsDeletable(typeof(TEntity));
        }

        // IsTruncatable

        internal static bool IsTruncatable(Type type)
        {
            return true;
        }

        /// <summary>
        /// Checks whether the table can be truncated.
        /// </summary>
        /// <typeparam name="TEntity">The data entity type to be checked.</typeparam>
        /// <returns>A boolean value signifies whether the table is truncatable.</returns>
        internal static bool IsTruncatable<TEntity>()
            where TEntity : class
        {
            return IsTruncatable(typeof(TEntity));
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
        /// <typeparam name="TEntity">The data entity type to be checked.</typeparam>
        /// <returns>A boolean value signifies whether the data entity is inline updateable.</returns>
        internal static bool IsInlineUpdateable<TEntity>()
            where TEntity : class
        {
            return IsInlineUpdateable(typeof(TEntity));
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
        /// <typeparam name="TEntity">The data entity type to be checked.</typeparam>
        /// <returns>A boolean value signifies whether the data entity is inline insertable.</returns>
        internal static bool IsInlineInsertable<TEntity>()
            where TEntity : class
        {
            return IsInlineInsertable(typeof(TEntity));
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
        /// <typeparam name="TEntity">The data entity type to be checked.</typeparam>
        /// <returns>A boolean value signifies whether the data entity is inline mergeable.</returns>
        internal static bool IsInlineMergeable<TEntity>()
            where TEntity : class
        {
            return IsInlineMergeable(typeof(TEntity));
        }

        // IsInsertable

        internal static bool IsInsertable(Type type)
        {
            return true;
        }

        /// <summary>
        /// Checks whether the data entity is insertable.
        /// </summary>
        /// <typeparam name="TEntity">The data entity type to be checked.</typeparam>
        /// <returns>A boolean value signifies whether the data entity is insertable.</returns>
        internal static bool IsInsertable<TEntity>()
            where TEntity : class
        {
            return IsInsertable(typeof(TEntity));
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
        /// <typeparam name="TEntity">The data entity type to be checked.</typeparam>
        /// <returns>A boolean value signifies whether the data entity is mergeable.</returns>
        internal static bool IsMergeable<TEntity>()
            where TEntity : class
        {
            return IsMergeable(typeof(TEntity));
        }

        // IsQueryable
        internal static bool IsQueryable(Type type)
        {
            return true;
        }

        /// <summary>
        /// Checks whether the data entity is queryable.
        /// </summary>
        /// <typeparam name="TEntity">The data entity type to be checked.</typeparam>
        /// <returns>A boolean value signifies whether the data entity is queryable.</returns>
        internal static bool IsQueryable<TEntity>()
            where TEntity : class
        {
            return IsQueryable(typeof(TEntity));
        }

        // IsUpdateable

        internal static bool IsUpdateable(Type type)
        {
            return true;
        }

        /// <summary>
        /// Checks whether the data entity is updateable.
        /// </summary>
        /// <typeparam name="TEntity">The data entity type to be checked.</typeparam>
        /// <returns>A boolean value signifies whether the data entity is updateable.</returns>
        internal static bool IsUpdateable<TEntity>()
            where TEntity : class
        {
            return IsUpdateable(typeof(TEntity));
        }
    }
}
