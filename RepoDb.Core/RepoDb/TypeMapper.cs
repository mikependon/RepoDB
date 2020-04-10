using System;
using System.Collections.Concurrent;
using System.Data;
using System.Data.Common;
using System.Linq.Expressions;
using System.Reflection;
using RepoDb.Enumerations;
using RepoDb.Exceptions;
using RepoDb.Extensions;

namespace RepoDb
{
    /// <summary>
    /// A static class that is used to map a .NET CLR type into its equivalent <see cref="DbType"/> object.
    /// </summary>
    public static class TypeMapper
    {
        #region Privates

        private static readonly ConcurrentDictionary<int, DbType?> m_maps = new ConcurrentDictionary<int, DbType?>();

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the conversion type when converting the instance of <see cref="DbDataReader"/> object into its destination .NET CLR Types.
        /// The default value is <see cref="ConversionType.Default"/>.
        /// </summary>
        [Obsolete("Use the 'Converter.ConversionType' instead.")]
        public static ConversionType ConversionType
        {
            get { return Converter.ConversionType; }
            set { Converter.ConversionType = value; }
        }

        #endregion

        #region Methods

        #region Type Level

        /*
         * Add
         */

        /// <summary>
        /// Adds a mapping between the .NET CLR type and <see cref="DbType"/> object.
        /// </summary>
        /// <typeparam name="T">The .NET CLR type to be mapped.</typeparam>
        /// <param name="dbType">The <see cref="DbType"/> object where to map the .NET CLR type.</param>
        public static void Add<T>(DbType dbType)
        {
            Add(typeof(T), dbType);
        }

        /// <summary>
        /// Adds a mapping between the .NET CLR type and <see cref="DbType"/> object.
        /// </summary>
        /// <typeparam name="T">The .NET CLR type to be mapped.</typeparam>
        /// <param name="dbType">The <see cref="DbType"/> object where to map the .NET CLR type.</param>
        /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
        public static void Add<T>(DbType dbType,
            bool force)
        {
            Add(typeof(T), dbType, force);
        }

        /// <summary>
        /// Adds a mapping between the .NET CLR type and <see cref="DbType"/> object.
        /// </summary>
        /// <param name="type">The .NET CLR type to be mapped.</param>
        /// <param name="dbType">The <see cref="DbType"/> object where to map the .NET CLR type.</param>
        public static void Add(Type type,
            DbType dbType)
        {
            Add(type, dbType, false);
        }

        /// <summary>
        /// Adds a mapping between the .NET CLR type and <see cref="DbType"/> object.
        /// </summary>
        /// <param name="type">The .NET CLR type to be mapped.</param>
        /// <param name="dbType">The <see cref="DbType"/> object where to map the .NET CLR type.</param>
        /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
        public static void Add(Type type,
            DbType dbType,
            bool force)
        {
            var key = type.FullName.GetHashCode();
            var value = (DbType?)null;

            // Try get the cache
            if (m_maps.TryGetValue(key, out value))
            {
                if (force)
                {
                    // Update the existing one
                    m_maps.TryUpdate(key, dbType, value);
                }
                else
                {
                    // Throws an exception
                    throw new MappingExistsException($"Mapping to '{type.FullName}' already exists.");
                }
            }
            else
            {
                // Add the mapping
                m_maps.TryAdd(key, dbType);
            }
        }

        /*
         * Get
         */

        /// <summary>
        /// Gets the mapped <see cref="DbType"/> object based on the .NET CLR type.
        /// </summary>
        /// <typeparam name="T">The dynamic .NET CLR type used for mapping.</typeparam>
        /// <returns>The instance of the mapped <see cref="DbType"/> object.</returns>
        public static DbType? Get<T>()
        {
            return Get(typeof(T));
        }

        /// <summary>
        /// Gets the mapped <see cref="DbType"/> object based on the .NET CLR type.
        /// </summary>
        /// <param name="type">The .NET CLR type used for mapping.</param>
        /// <returns>The instance of the mapped <see cref="DbType"/> object.</returns>
        public static DbType? Get(Type type)
        {
            var value = (DbType?)null;
            var key = type.FullName.GetHashCode();

            // Try get the value
            m_maps.TryGetValue(key, out value);

            // Return the value
            return value;
        }

        /*
         * Remove
         */

        /// <summary>
        /// Removes the mapping of the .NET CLR type and <see cref="DbType"/> object.
        /// </summary>
        /// <typeparam name="T">The .NET CLR type mapping to be removed.</typeparam>
        public static void Remove<T>()
        {
            Remove(typeof(T));
        }

        /// <summary>
        /// Removes the mapping of the .NET CLR type and <see cref="DbType"/> object.
        /// </summary>
        /// <param name="type">The .NET CLR type mapping to be removed.</param>
        public static void Remove(Type type)
        {
            var key = type.FullName.GetHashCode();
            var value = (DbType?)null;

            // Try get the value
            m_maps.TryRemove(key, out value);
        }

        #region Obselete

        /*
         * Map
         */

        /// <summary>
        /// Adds a mapping between the .NET CLR type and <see cref="DbType"/> object.
        /// </summary>
        /// <typeparam name="T">The .NET CLR type to be mapped.</typeparam>
        /// <param name="dbType">The <see cref="DbType"/> object where to map the .NET CLR type.</param>
        [Obsolete("Use the 'Add' method instead.")]
        public static void Map<T>(DbType dbType)
        {
            Add(typeof(T), dbType);
        }

        /// <summary>
        /// Adds a mapping between the .NET CLR type and <see cref="DbType"/> object.
        /// </summary>
        /// <typeparam name="T">The .NET CLR type to be mapped.</typeparam>
        /// <param name="dbType">The <see cref="DbType"/> object where to map the .NET CLR type.</param>
        /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
        [Obsolete("Use the 'Add' method instead.")]
        public static void Map<T>(DbType dbType,
            bool force)
        {
            Add(typeof(T), dbType, force);
        }

        /// <summary>
        /// Adds a mapping between the .NET CLR type and <see cref="DbType"/> object.
        /// </summary>
        /// <param name="type">The .NET CLR type to be mapped.</param>
        /// <param name="dbType">The <see cref="DbType"/> object where to map the .NET CLR type.</param>
        [Obsolete("Use the 'Add' method instead.")]
        public static void Map(Type type,
            DbType dbType)
        {
            Add(type, dbType, false);
        }

        /// <summary>
        /// Adds a mapping between the .NET CLR type and <see cref="DbType"/> object.
        /// </summary>
        /// <param name="type">The .NET CLR type to be mapped.</param>
        /// <param name="dbType">The <see cref="DbType"/> object where to map the .NET CLR type.</param>
        /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
        [Obsolete("Use the 'Add' method instead.")]
        public static void Map(Type type,
            DbType dbType,
            bool force)
        {
            Add(type, dbType, force);
        }

        /*
         * Unmap
         */

        /// <summary>
        /// Removes the mapping of the .NET CLR type and <see cref="DbType"/> object.
        /// </summary>
        /// <typeparam name="T">The .NET CLR type mapping to be removed.</typeparam>
        [Obsolete("Use the 'Remove' method instead.")]
        public static void Unmap<T>()
        {
            Remove(typeof(T));
        }

        /// <summary>
        /// Removes the mapping of the .NET CLR type and <see cref="DbType"/> object.
        /// </summary>
        /// <param name="type">The .NET CLR type mapping to be removed.</param>
        [Obsolete("Use the 'Remove' method instead.")]
        public static void Unmap(Type type)
        {
            Remove(type);
        }

        #endregion

        #endregion

        #region PropertyLevel

        /*
         * Add
         */

        /// <summary>
        /// Adds a mapping between a class property and the database type (via expression).
        /// </summary>
        /// <typeparam name="T">The type of the entity.</typeparam>
        /// <param name="expression">The expression to be parsed.</param>
        /// <param name="dbType">The target database type.</param>
        public static void Add<T>(Expression<Func<T, object>> expression,
            DbType? dbType)
            where T : class => Add(expression, dbType, false);

        /// <summary>
        /// Adds a mapping between a class property and the database type (via expression).
        /// </summary>
        /// <typeparam name="T">The type of the entity.</typeparam>
        /// <param name="expression">The expression to be parsed.</param>
        /// <param name="dbType">The target database type.</param>
        /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
        public static void Add<T>(Expression<Func<T, object>> expression,
            DbType? dbType,
            bool force)
            where T : class => Add(ExpressionExtension.GetProperty<T>(expression), dbType, force);

        /// <summary>
        /// Adds a mapping between a class property and the database type (via property name).
        /// </summary>
        /// <typeparam name="T">The target .NET CLR type.</typeparam>
        /// <param name="propertyName">The name of the class property to be mapped.</param>
        /// <param name="dbType">The target database type.</param>
        public static void Add<T>(string propertyName,
            DbType? dbType)
            where T : class => Add<T>(propertyName, dbType, false);

        /// <summary>
        /// Adds a mapping between a class property and the database type (via property name).
        /// </summary>
        /// <typeparam name="T">The target .NET CLR type.</typeparam>
        /// <param name="propertyName">The name of the class property to be mapped.</param>
        /// <param name="dbType">The name of the class property to be mapped.</param>
        /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
        public static void Add<T>(string propertyName,
            DbType? dbType,
            bool force)
            where T : class
        {
            // Validates
            ThrowNullReferenceException(propertyName, "PropertyName");

            // Get the property
            var property = TypeExtension.GetProperty<T>(propertyName);
            if (property == null)
            {
                throw new PropertyNotFoundException($"Property '{propertyName}' is not found at type '{typeof(T).FullName}'.");
            }

            // Add to the mapping
            Add(property, dbType, force);
        }

        /// <summary>
        /// Adds a mapping between a class property and the database type (via <see cref="Field"/> object).
        /// </summary>
        /// <typeparam name="T">The target .NET CLR type.</typeparam>
        /// <param name="field">The instance of <see cref="Field"/> to be mapped.</param>
        /// <param name="dbType">The target database type.</param>
        public static void Add<T>(Field field,
            DbType? dbType)
            where T : class => Add<T>(field, dbType, false);

        /// <summary>
        /// Adds a mapping between a class property and the database type (via <see cref="Field"/> object).
        /// </summary>
        /// <typeparam name="T">The target .NET CLR type.</typeparam>
        /// <param name="field">The instance of <see cref="Field"/> to be mapped.</param>
        /// <param name="dbType">The target database type.</param>
        /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
        public static void Add<T>(Field field,
            DbType? dbType,
            bool force)
            where T : class
        {
            // Validates
            ThrowNullReferenceException(field, "Field");

            // Get the property
            var property = TypeExtension.GetProperty<T>(field.Name);
            if (property == null)
            {
                throw new PropertyNotFoundException($"Property '{field.Name}' is not found at type '{typeof(T).FullName}'.");
            }

            // Add to the mapping
            Add(property, dbType, force);
        }

        /// <summary>
        /// Adds a mapping between a <see cref="ClassProperty"/> object and the database column.
        /// </summary>
        /// <param name="classProperty">The instance of <see cref="ClassProperty"/> to be mapped.</param>
        /// <param name="dbType">The target database type.</param>
        public static void Add(ClassProperty classProperty,
            DbType? dbType) => Add(classProperty.PropertyInfo, dbType, false);

        /// <summary>
        /// Adds a mapping between a <see cref="ClassProperty"/> object and the database column.
        /// </summary>
        /// <param name="classProperty">The instance of <see cref="ClassProperty"/> to be mapped.</param>
        /// <param name="dbType">The target database type.</param>
        /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
        public static void Add(ClassProperty classProperty,
            DbType? dbType,
            bool force) => Add(classProperty?.PropertyInfo, dbType, force);

        /// <summary>
        /// Adds a mapping between a <see cref="PropertyInfo"/> object and the database column.
        /// </summary>
        /// <param name="propertyInfo">The instance of <see cref="PropertyInfo"/> to be mapped.</param>
        /// <param name="dbType">The target database type.</param>
        public static void Add(PropertyInfo propertyInfo,
            DbType? dbType) => Add(propertyInfo, dbType, false);

        /// <summary>
        /// Adds a mapping between a <see cref="PropertyInfo"/> object and the database column.
        /// </summary>
        /// <param name="propertyInfo">The instance of <see cref="PropertyInfo"/> to be mapped.</param>
        /// <param name="dbType">The target database type.</param>
        /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
        public static void Add(PropertyInfo propertyInfo,
            DbType? dbType,
            bool force)
        {
            // Validate
            ThrowNullReferenceException(propertyInfo, "PropertyInfo");
            ValidateTargetColumnName(dbType);

            // Variables
            var key = propertyInfo.GenerateCustomizedHashCode();
            var value = (DbType?)null;

            // Try get the cache
            if (m_maps.TryGetValue(key, out value))
            {
                if (force)
                {
                    // Update the existing one
                    m_maps.TryUpdate(key, dbType, value);
                }
                else
                {
                    // Throws an exception
                    throw new MappingExistsException($"A database type mapping to '{propertyInfo.DeclaringType.FullName}.{propertyInfo.Name}' already exists.");
                }
            }
            else
            {
                // Add the mapping
                m_maps.TryAdd(key, dbType);
            }
        }

        /*
         * Get
         */

        /// <summary>
        /// Gets the mapped database type of the property (via expression).
        /// </summary>
        /// <typeparam name="T">The type of the entity.</typeparam>
        /// <param name="expression">The expression to be parsed.</param>
        /// <returns>The mapped database type of the property.</returns>
        public static DbType? Get<T>(Expression<Func<T, object>> expression)
            where T : class => Get(ExpressionExtension.GetProperty<T>(expression));

        /// <summary>
        /// Gets the mapped database type of the property (via property name).
        /// </summary>
        /// <typeparam name="T">The target .NET CLR type.</typeparam>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns>The mapped database type of the property.</returns>
        public static DbType? Get<T>(string propertyName)
            where T : class => Get(TypeExtension.GetProperty<T>(propertyName));

        /// <summary>
        /// Gets the mapped database type of the property (via <see cref="Field"/> object).
        /// </summary>
        /// <typeparam name="T">The target .NET CLR type.</typeparam>
        /// <param name="field">The instance of <see cref="Field"/> object.</param>
        /// <returns>The mapped database type of the property.</returns>
        public static DbType? Get<T>(Field field)
            where T : class => Get(TypeExtension.GetProperty<T>(field.Name));

        /// <summary>
        /// Gets the mapped database type of the property via <see cref="ClassProperty"/> object.
        /// </summary>
        /// <param name="classProperty">The instance of <see cref="ClassProperty"/>.</param>
        /// <returns>The mapped database type of the property.</returns>
        public static DbType? Get(ClassProperty classProperty) =>
            Get(classProperty.PropertyInfo);

        /// <summary>
        /// Gets the mapped database type of the property via <see cref="PropertyInfo"/> object.
        /// </summary>
        /// <param name="propertyInfo">The instance of <see cref="PropertyInfo"/>.</param>
        /// <returns>The mapped database type of the property.</returns>
        public static DbType? Get(PropertyInfo propertyInfo)
        {
            // Validate
            ThrowNullReferenceException(propertyInfo, "PropertyInfo");

            // Variables
            var key = propertyInfo.GenerateCustomizedHashCode();
            var value = (DbType?)null;

            // Try get the value
            m_maps.TryGetValue(key, out value);

            // Return the value
            return value;
        }

        /*
         * Remove
         */

        /// <summary>
        /// Removes the mapping between the class property and the database column (via expression).
        /// </summary>
        /// <typeparam name="T">The type of the entity.</typeparam>
        /// <param name="expression">The expression to be parsed.</param>
        public static void Remove<T>(Expression<Func<T, object>> expression)
            where T : class => Remove(ExpressionExtension.GetProperty<T>(expression));

        /// <summary>
        /// Removes the mapping between the class property and database column (via property name).
        /// </summary>
        /// <typeparam name="T">The target .NET CLR type.</typeparam>
        /// <param name="propertyName">The name of the property.</param>
        public static void Remove<T>(string propertyName)
            where T : class => Remove(TypeExtension.GetProperty<T>(propertyName));

        /// <summary>
        /// Removes the mapping between the  class property and database column (via <see cref="Field"/> object).
        /// </summary>
        /// <typeparam name="T">The target .NET CLR type.</typeparam>
        /// <param name="field">The instance of <see cref="Field"/> object.</param>
        public static void Remove<T>(Field field)
            where T : class => Remove(TypeExtension.GetProperty<T>(field.Name));

        /// <summary>
        /// Removes the mapping between the <see cref="ClassProperty"/> object and the database column.
        /// </summary>
        /// <param name="classProperty">The instance of <see cref="ClassProperty"/>.</param>
        public static void Remove(ClassProperty classProperty) =>
            Remove(classProperty.PropertyInfo);

        /// <summary>
        /// Removes the mapping between the <see cref="PropertyInfo"/> object and the database column.
        /// </summary>
        /// <param name="propertyInfo">The instance of <see cref="PropertyInfo"/>.</param>
        public static void Remove(PropertyInfo propertyInfo)
        {
            // Validate
            ThrowNullReferenceException(propertyInfo, "PropertyInfo");

            // Variables
            var key = propertyInfo.GenerateCustomizedHashCode();
            var value = (DbType?)null;

            // Try get the value
            m_maps.TryRemove(key, out value);
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Flushes all the existing cached property mapped names.
        /// </summary>
        public static void Flush()
        {
            m_maps.Clear();
        }

        /// <summary>
        /// Validates the value of the target column name.
        /// </summary>
        /// <param name="dbType">The column name to be validated.</param>
        private static void ValidateTargetColumnName(DbType? dbType)
        {
            if (dbType == null)
            {
                throw new NullReferenceException("The target column name cannot be null or empty.");
            }
        }

        /// <summary>
        /// Validates the target object presence.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="obj">The object to be checked.</param>
        /// <param name="argument">The name of the argument.</param>
        private static void ThrowNullReferenceException<T>(T obj,
            string argument)
        {
            if (obj == null)
            {
                throw new NullReferenceException($"The argument '{argument}' cannot be null.");
            }
        }

        #endregion

        #endregion
    }
}
